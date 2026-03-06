using Banned.Qbittorrent.Exceptions;
using Banned.Qbittorrent.Models.Application;
using System.Net;
using System.Net.Http.Headers;

namespace Banned.Qbittorrent.Services;

/// <summary>
/// 提供底层的 HTTP 网络请求服务，包含自动重试、身份验证检查和版本控制。<br/>
/// Provides underlying HTTP networking services, including automatic retries, authentication checks, and version control.
/// </summary>
public class NetService : IDisposable
{
    private readonly HttpClient _client;
    private readonly Uri        _baseUrl;

    private ApiVersion _apiVersion;

    /// <summary>
    /// 获取或设置用于确保用户已登录的处理程序。<br/>
    /// Gets or sets the handler used to ensure the user is logged in.
    /// </summary>
    public Func<Task>? EnsureLoggedInHandler { get; set; }

    /// <summary>
    /// 获取或设置最大重试次数。<br/>
    /// Gets or sets the maximum number of retries.
    /// </summary>
    public int MaxRetries { get; set; } = 3;

    /// <summary>
    /// 获取或设置是否启用详细日志。<br/>
    /// Gets or sets whether to enable detailed logging.
    /// </summary>
    public bool EnableDetailedLogging { get; set; } = false;

    /// <summary>
    /// 初始化 <see cref="NetService"/> 类的新实例。<br/>
    /// Initializes a new instance of the <see cref="NetService"/> class.
    /// </summary>
    /// <param name="baseUrl">qBittorrent Web UI 的基础地址。 / The base URL of qBittorrent Web UI.</param>
    /// <param name="httpClient">可选的 HttpClient 实例。 / Optional HttpClient instance.</param>
    /// <param name="timeout">可选的请求超时时间，为 null 时默认 15 秒。 / Optional request timeout, default 15 seconds when null.</param>
    public NetService(string baseUrl, HttpClient? httpClient = null, TimeSpan? timeout = null)
    {
        _baseUrl = new Uri(baseUrl.TrimEnd('/') + "/");
        var defaultTimeout = timeout ?? TimeSpan.FromSeconds(15);

        if (httpClient != null)
        {
            _client = httpClient;
        }
        else
        {
            var cookieContainer = new CookieContainer();

            var handler = new HttpClientHandler
            {
                CookieContainer   = cookieContainer,
                AllowAutoRedirect = true,
                UseCookies        = true
            };

            _client = new HttpClient(handler) { Timeout = defaultTimeout };
        }

        _client.DefaultRequestHeaders.CacheControl = new CacheControlHeaderValue
        {
            NoCache = true,
            NoStore = true
        };
    }

    /// <summary>
    /// 设置当前客户端使用的 Web API 版本。<br/>
    /// Sets the Web API version used by the current client.
    /// </summary>
    /// <param name="apiVersion">API 版本信息。 / API version information.</param>
    public void SetApiVersion(ApiVersion apiVersion) => _apiVersion = apiVersion;

    private Uri CombineUrl(string subPath) => new(_baseUrl, subPath.TrimStart('/'));

    /// <summary>
    /// 发起异步 GET 请求。<br/>
    /// Performs an asynchronous GET request.
    /// </summary>
    /// <param name="subPath">请求的子路径。 / The sub-path of the request.</param>
    /// <param name="targetVersion">该接口要求的最低 API 版本。 / The minimum API version required by this endpoint.</param>
    /// <param name="opName">操作名称，用于异常显示。 / The operation name used for exception display.</param>
    /// <param name="skipAuthCheck">是否跳过登录状态检查。 / Whether to skip the authentication check.</param>
    /// <param name="ct">取消令牌。 / Cancellation token.</param>
    /// <returns>响应体字符串。 / The response body string.</returns>
    /// <exception cref="QbittorrentNotSupportedException">当前 API 版本低于目标版本时抛出。 / Thrown when the current API version is lower than the target version.</exception>
    public async Task<string> Get(string            subPath,
                                  ApiVersion?       targetVersion = null,
                                  string?           opName        = null,
                                  bool              skipAuthCheck = false,
                                  CancellationToken ct            = default)
    {
        if (_apiVersion < targetVersion)
            throw new QbittorrentNotSupportedException(opName ?? subPath, targetVersion.Value, _apiVersion);

        await CheckAuth(skipAuthCheck).ConfigureAwait(false);

        return await ExecuteWithRetry(
                                      () => new HttpRequestMessage(HttpMethod.Get, CombineUrl(subPath)),
                                      ct).ConfigureAwait(false);
    }

    /// <summary>
    /// 发起异步 POST 请求。<br/>
    /// Performs an asynchronous POST request.
    /// </summary>
    /// <param name="subPath">请求的子路径。 / The sub-path of the request.</param>
    /// <param name="parameters">表单参数。 / Form parameters.</param>
    /// <param name="targetVersion">该接口要求的最低 API 版本。 / The minimum API version required by this endpoint.</param>
    /// <param name="opName">操作名称，用于异常显示。 / The operation name used for exception display.</param>
    /// <param name="skipAuthCheck">是否跳过登录状态检查。 / Whether to skip the authentication check.</param>
    /// <param name="ct">取消令牌。 / Cancellation token.</param>
    /// <returns>响应体字符串。 / The response body string.</returns>
    public async Task<string> Post(string                      subPath,
                                   Dictionary<string, string>? parameters    = null,
                                   ApiVersion?                 targetVersion = null,
                                   string?                     opName        = null,
                                   bool                        skipAuthCheck = false,
                                   CancellationToken           ct            = default)
    {
        if (_apiVersion < targetVersion)
            throw new QbittorrentNotSupportedException(opName ?? subPath, targetVersion.Value, _apiVersion);

        await CheckAuth(skipAuthCheck).ConfigureAwait(false);

        return await ExecuteWithRetry(() =>
        {
            var request = new HttpRequestMessage(HttpMethod.Post, CombineUrl(subPath));

            if (parameters != null)
            {
                request.Content = new FormUrlEncodedContent(parameters);
            }

            return request;
        }, ct).ConfigureAwait(false);
    }

    /// <summary>
    /// 发起带文件的异步 POST 请求（多部分表单）。<br/>
    /// Performs an asynchronous POST request with files (multipart/form-data).
    /// </summary>
    /// <param name="subPath">请求的子路径。 / The sub-path of the request.</param>
    /// <param name="parameters">额外的表单字段。 / Additional form fields.</param>
    /// <param name="filePaths">要上传的文件路径列表。 / List of file paths to upload.</param>
    /// <param name="ct">取消令牌。 / Cancellation token.</param>
    /// <returns>响应体字符串。 / The response body string.</returns>
    /// <exception cref="QbittorrentFileNotFoundException">当指定的文件不存在时抛出。 / Thrown when a specified file does not exist.</exception>
    public async Task<string> PostWithFiles(string                      subPath,
                                            Dictionary<string, string>? parameters,
                                            List<string>                filePaths,
                                            CancellationToken           ct = default)
    {
        await CheckAuth(false).ConfigureAwait(false);

        return await ExecuteWithRetry(() =>
        {
            try
            {
                var content = new MultipartFormDataContent();
                if (parameters != null)
                    foreach (var (k, v) in parameters)
                        content.Add(new StringContent(v), k);

                foreach (var filePath in filePaths)
                {
                    if (!File.Exists(filePath)) throw new QbittorrentFileNotFoundException(filePath);
                    if (EnableDetailedLogging)
                        Console.WriteLine($"Uploading file: {filePath}");

                    // 使用 FileStream 代替 File.ReadAllBytes 以减少内存使用
                    var fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.Read, 8192,
                                                    FileOptions.Asynchronous);
                    try
                    {
                        content.Add(new StreamContent(fileStream), "torrents", Path.GetFileName(filePath));
                    }
                    catch
                    {
                        fileStream.Dispose();
                        throw;
                    }
                }

                var request = new HttpRequestMessage(HttpMethod.Post, CombineUrl(subPath)) { Content = content };
                if (EnableDetailedLogging)
                    Console.WriteLine($"Sending POST request to: {request.RequestUri}");

                return Task.FromResult(request);
            }
            catch (Exception exception)
            {
                return Task.FromException<HttpRequestMessage>(exception);
            }
        }, ct).ConfigureAwait(false);
    }

    /// <summary>
    /// 并行执行多个请求。<br/>
    /// Executes multiple requests in parallel.
    /// </summary>
    /// <param name="requests">请求信息列表。 / List of request information.</param>
    /// <param name="ct">取消令牌。 / Cancellation token.</param>
    /// <returns>响应体字符串列表，与请求顺序对应。 / List of response body strings, corresponding to the request order.</returns>
    public async Task<List<string>> ExecuteParallelRequests(
        List<(string subPath, HttpMethod method, Dictionary<string, string>? parameters)> requests,
        CancellationToken                                                                 ct = default)
    {
        var tasks = requests.Select(async request =>
        {
            if (request.method == HttpMethod.Get)
            {
                return await Get(request.subPath, skipAuthCheck : false, ct : ct);
            }

            return await Post(request.subPath, request.parameters, skipAuthCheck : false, ct : ct);
        }).ToList();

        var results = await Task.WhenAll(tasks);
        return results.ToList();
    }

    /// <summary>
    /// 检查并确保身份验证状态。<br/>
    /// Checks and ensures the authentication status.
    /// </summary>
    private async Task CheckAuth(bool skipAuthCheck)
    {
        if (!skipAuthCheck && EnsureLoggedInHandler != null)
        {
            await EnsureLoggedInHandler.Invoke().ConfigureAwait(false);
        }
    }

    /// <summary>
    /// 执行请求并包含指数退避重试机制。<br/>
    /// Executes the request with an exponential backoff retry mechanism.
    /// </summary>
    /// <param name="requestFactory">用于创建请求消息的工厂方法。 / Factory method to create the request message.</param>
    /// <param name="ct">取消令牌。 / Cancellation token.</param>
    /// <param name="maxRetries">最大重试次数。 / Maximum number of retries.</param>
    /// <returns>响应体内容。 / Response body content.</returns>
    private async Task<string> ExecuteWithRetry(Func<HttpRequestMessage> requestFactory,
                                                CancellationToken        ct         = default,
                                                int?                     maxRetries = null)
    {
        return await ExecuteWithRetry(() =>
        {
            try
            {
                return Task.FromResult(requestFactory());
            }
            catch (Exception exception)
            {
                return Task.FromException<HttpRequestMessage>(exception);
            }
        }, ct, maxRetries);
    }

    private async Task<string> ExecuteWithRetry(Func<Task<HttpRequestMessage>> requestFactory,
                                                CancellationToken              ct         = default,
                                                int?                           maxRetries = null)
    {
        Exception?           lastException    = null;
        HttpResponseMessage? lastResponse     = null;
        var                  lastBody         = string.Empty;
        var                  actualMaxRetries = maxRetries ?? MaxRetries;

        for (var attempt = 1; attempt <= actualMaxRetries; attempt++)
        {
            try
            {
                using var request = await requestFactory().ConfigureAwait(false);
                if (EnableDetailedLogging)
                    Console.WriteLine($"Attempt {attempt}/{actualMaxRetries}: Sending {request.Method} request to {request.RequestUri}");

                using var response = await _client.SendAsync(
                                                             request, HttpCompletionOption.ResponseHeadersRead, ct)
                                                  .ConfigureAwait(false);

                lastResponse = response;
                lastBody     = await response.Content.ReadAsStringAsync(ct).ConfigureAwait(false);

                if (EnableDetailedLogging)
                    Console.WriteLine($"Attempt {attempt}/{actualMaxRetries}: Received response with status code: {response.StatusCode}");

                if (response.IsSuccessStatusCode) return lastBody;

                if (!RetryableStatusCodes.Contains(response.StatusCode))
                    throw MapToException(response, lastBody);

                if (attempt == actualMaxRetries) break;

                var delay = GetDelayFromRetryAfterOrBackoff(response, attempt);
                if (EnableDetailedLogging)
                    Console.WriteLine($"Attempt {attempt}/{actualMaxRetries}: Retrying in {delay.TotalMilliseconds}ms due to status code: {response.StatusCode}");

                await Task.Delay(delay, ct).ConfigureAwait(false);
            }
            catch (TaskCanceledException ex)
            {
                lastException = ex;
                if (EnableDetailedLogging)
                    Console.WriteLine($"Attempt {attempt}/{actualMaxRetries}: Task canceled: {ex.Message}");
                if (attempt == actualMaxRetries) break;

                var delay = ComputeBackoff(attempt);
                if (EnableDetailedLogging)
                    Console.WriteLine($"Attempt {attempt}/{actualMaxRetries}: Retrying in {delay.TotalMilliseconds}ms due to cancellation");

                await Task.Delay(delay, ct).ConfigureAwait(false);
            }
            catch (HttpRequestException ex)
            {
                lastException = ex;
                if (EnableDetailedLogging)
                    Console.WriteLine($"Attempt {attempt}/{actualMaxRetries}: HTTP request failed: {ex.Message}");
                if (attempt == actualMaxRetries) break;

                var delay = ComputeBackoff(attempt);
                if (EnableDetailedLogging)
                    Console.WriteLine($"Attempt {attempt}/{actualMaxRetries}: Retrying in {delay.TotalMilliseconds}ms due to network error");

                await Task.Delay(delay, ct).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                // 捕获其他异常，不进行重试
                if (EnableDetailedLogging)
                    Console.WriteLine($"Attempt {attempt}/{actualMaxRetries}: Unexpected error: {ex.Message}");
                throw;
            }
        }

        if (lastResponse != null) throw MapToException(lastResponse, lastBody);
        throw new QbittorrentServerErrorException(
                                                  $"Network error after {actualMaxRetries} attempts: {lastException?.Message ?? "unknown error"}");
    }

    /// <summary>
    /// 将 HTTP 状态码映射为特定的业务异常。<br/>
    /// Maps HTTP status codes to specific business exceptions.
    /// </summary>
    private static Exception MapToException(HttpResponseMessage response, string body)
    {
        return response.StatusCode switch
        {
            HttpStatusCode.BadRequest          => new QbittorrentBadRequestException("Bad Request."),
            HttpStatusCode.Unauthorized        => new QbittorrentUnauthorizedException("Unauthorized"),
            HttpStatusCode.Forbidden           => new QbittorrentForbiddenException("Forbidden"),
            HttpStatusCode.NotFound            => new QbittorrentNotFoundException("Not Found"),
            HttpStatusCode.Conflict            => new QbittorrentConflictException("Conflict"),
            HttpStatusCode.InternalServerError => new QbittorrentServerErrorException("Server Error"),
            _ => new QbittorrentException($"HTTP {(int)response.StatusCode} {response.ReasonPhrase}: {body}",
                                          (int)response.StatusCode)
        };
    }

    /// <summary>
    /// 根据响应头或尝试次数计算重试延迟时间。<br/>
    /// Computes the retry delay based on response headers or attempt count.
    /// </summary>
    private static TimeSpan GetDelayFromRetryAfterOrBackoff(HttpResponseMessage response, int attempt)
    {
        var ra = response.Headers.RetryAfter;
        if (ra == null) return ComputeBackoff(attempt);
        if (ra.Delta.HasValue) return ra.Delta.Value;
        if (ra.Date.HasValue) return ra.Date.Value - DateTimeOffset.UtcNow;

        return ComputeBackoff(attempt);
    }

    /// <summary>
    /// 计算指数退避延迟时间（带抖动）。<br/>
    /// Computes exponential backoff delay (with jitter).
    /// </summary>
    private static TimeSpan ComputeBackoff(int attempt)
    {
        var baseMs = 500 * (int)Math.Pow(2, attempt - 1);
        var jitter = Random.Shared.Next(0, 250);
        return TimeSpan.FromMilliseconds(baseMs + jitter);
    }

    /// <summary>
    /// 可触发重试的状态码集合。<br/>
    /// A collection of status codes that can trigger a retry.
    /// </summary>
    private static readonly HashSet<HttpStatusCode> RetryableStatusCodes =
    [
        HttpStatusCode.InternalServerError, // 500
        HttpStatusCode.BadGateway,          // 502
        HttpStatusCode.ServiceUnavailable,  // 503
        HttpStatusCode.GatewayTimeout,      // 504
        HttpStatusCode.RequestTimeout,      // 408
        HttpStatusCode.TooManyRequests
    ];

    /// <summary>
    /// 释放 <see cref="NetService"/> 使用的资源。<br/>
    /// Releases the resources used by the <see cref="NetService"/>.
    /// </summary>
    public void Dispose()
    {
        _client.Dispose();
        GC.SuppressFinalize(this);
    }
}
