using Banned.Qbittorrent.Exceptions;
using Banned.Qbittorrent.Models.Application;
using System.Net;
using System.Net.Http.Headers;

namespace Banned.Qbittorrent.Utils;

public class NetUtils
{
    private readonly HttpClient _client;
    private readonly Uri        _baseUrl;

    private ApiVersion _apiVersion;

    private readonly string _userName;
    private readonly string _password;

    private readonly SemaphoreSlim  _loginLock = new(1, 1);
    private volatile bool           _isLoggedIn;
    private          DateTimeOffset _loginExpiry = DateTimeOffset.MinValue;

    public NetUtils(string baseUrl, string userName, string password)
    {
        _baseUrl    = new Uri(baseUrl.TrimEnd('/') + "/");
        _userName   = userName;
        _password   = password;
        _apiVersion = new ApiVersion(2);
        var cookieContainer = new CookieContainer();

        var handler = new HttpClientHandler
        {
            CookieContainer   = cookieContainer,
            AllowAutoRedirect = true,
            UseCookies        = true
        };

        _client = new HttpClient(handler)
        {
            Timeout = TimeSpan.FromSeconds(15)
        };

        _client.DefaultRequestHeaders.CacheControl = new CacheControlHeaderValue
        {
            NoCache = true,
            NoStore = true
        };
    }

    public void SetApiVersion(ApiVersion apiVersion)
    {
        _apiVersion = apiVersion;
    }

    /// <summary>
    /// 拼接 baseUrl 和 subPath，确保不会出现多余或缺少 "/"
    /// </summary>
    private Uri CombineUrl(string subPath)
    {
        return new Uri(_baseUrl, subPath.TrimStart('/'));
    }

    /// <summary>
    /// 通用 GET 请求（异步）
    /// </summary>
    public async Task<string> Get(string            subPath,
                                  ApiVersion?       targetVersion = null,
                                  string?           opName        = null,
                                  CancellationToken ct            = default)
    {
        if (targetVersion != null && _apiVersion < targetVersion)
            throw new QbittorrentNotSupportedException(opName ?? subPath, targetVersion, _apiVersion);

        await EnsureLoggedIn().ConfigureAwait(false);

        return await ExecuteWithRetry(
                                           () => new HttpRequestMessage(HttpMethod.Get, CombineUrl(subPath)),
                                           ct).ConfigureAwait(false);
    }

    /// <summary>
    /// 通用 POST 请求（异步）
    /// </summary>
    public async Task<string> Post(string                      subPath,
                                   Dictionary<string, string>? parameters    = null,
                                   ApiVersion?                 targetVersion = null, string? opName = null,
                                   CancellationToken           ct            = default)
    {
        if (targetVersion != null && _apiVersion < targetVersion)
            throw new QbittorrentNotSupportedException(opName ?? subPath, targetVersion, _apiVersion);

        await EnsureLoggedIn().ConfigureAwait(false);

        return await ExecuteWithRetry(
                                           () =>
                                           {
                                               if (parameters != null)
                                                   return new HttpRequestMessage(HttpMethod.Post, CombineUrl(subPath))
                                                   {
                                                       Content = new FormUrlEncodedContent(parameters)
                                                   };
                                               return null;
                                           },
                                           ct).ConfigureAwait(false);
    }

    /// <summary>
    /// 通用 POST 文件上传（异步）
    /// </summary>
    public async Task<string> PostWithFiles(string                      subPath,
                                            Dictionary<string, string>? parameters,
                                            List<string>                filePaths,
                                            CancellationToken           ct = default)
    {
        await EnsureLoggedIn().ConfigureAwait(false);

        return await ExecuteWithRetry(() =>
        {
            var content = new MultipartFormDataContent();
            if (parameters != null)
                foreach (var (k, v) in parameters)
                    content.Add(new StringContent(v), k);

            foreach (var filePath in filePaths)
            {
                if (!File.Exists(filePath)) throw new QbittorrentFileNotFoundException(filePath);
                // 注意：每次重试都重新读取文件，保证内容新鲜、未释放
                content.Add(new ByteArrayContent(File.ReadAllBytes(filePath)), "torrents", Path.GetFileName(filePath));
            }

            return new HttpRequestMessage(HttpMethod.Post, CombineUrl(subPath)) { Content = content };
        }, ct).ConfigureAwait(false);
    }

    private async Task EnsureLoggedIn()
    {
        if (_isLoggedIn && DateTimeOffset.UtcNow < _loginExpiry) return;

        await _loginLock.WaitAsync().ConfigureAwait(false);
        try
        {
            if (_isLoggedIn && DateTimeOffset.UtcNow < _loginExpiry) return;
            await Login().ConfigureAwait(false);
            _isLoggedIn  = true;
            _loginExpiry = DateTimeOffset.UtcNow.AddHours(8); // TODO: 若能解析 Set-Cookie 中的过期时间则用真实值
        }
        finally
        {
            _loginLock.Release();
        }
    }

    private async Task Login()
    {
        var content = new FormUrlEncodedContent(new Dictionary<string, string>
        {
            { "username", _userName },
            { "password", _password }
        });

        var response = await _client.PostAsync(CombineUrl("api/v2/auth/login"), content);
        var body     = await response.Content.ReadAsStringAsync();
        if (!response.IsSuccessStatusCode)
        {
            throw new QbittorrentLoginFailedException(
                                                      $"Login failed: HTTP {(int)response.StatusCode} {response.ReasonPhrase}, Body: {body}",
                                                      (int)response.StatusCode);
        }
    }

    /// <summary>
    /// 自动重试逻辑（异步）
    /// </summary>
    private async Task<string> ExecuteWithRetry(Func<HttpRequestMessage?> requestFactory,
                                                     CancellationToken         ct         = default,
                                                     int                       maxRetries = 3)
    {
        Exception?           lastException = null;
        HttpResponseMessage? lastResponse  = null;
        var                  lastBody      = string.Empty;

        for (var attempt = 1; attempt <= maxRetries; attempt++)
        {
            try
            {
                using var request = requestFactory();
                using var response = await _client.SendAsync(
                                                             request, HttpCompletionOption.ResponseHeadersRead, ct)
                                                  .ConfigureAwait(false);

                lastResponse = response;
                lastBody     = await response.Content.ReadAsStringAsync(ct).ConfigureAwait(false);

                if (response.IsSuccessStatusCode) return lastBody;

                if (!RetryableStatusCodes.Contains(response.StatusCode))
                    throw MapToException(response, lastBody);

                if (attempt == maxRetries) break;
                await Task.Delay(GetDelayFromRetryAfterOrBackoff(response, attempt), ct).ConfigureAwait(false);
            }
            catch (TaskCanceledException ex)
            {
                lastException = ex;
                if (attempt == maxRetries) break;
                await Task.Delay(ComputeBackoff(attempt), ct).ConfigureAwait(false);
            }
            catch (HttpRequestException ex)
            {
                lastException = ex;
                if (attempt == maxRetries) break;
                await Task.Delay(ComputeBackoff(attempt), ct).ConfigureAwait(false);
            }
        }

        if (lastResponse != null) throw MapToException(lastResponse, lastBody);
        throw new QbittorrentServerErrorException(
                                                  $"Network error after {maxRetries} attempts: {lastException?.Message ?? "unknown error"}");
    }

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

    private static TimeSpan GetDelayFromRetryAfterOrBackoff(HttpResponseMessage response, int attempt)
    {
        // 尊重 Retry-After（秒或日期）
        var ra = response.Headers.RetryAfter;
        if (ra == null) return ComputeBackoff(attempt);
        if (ra.Delta.HasValue) return ra.Delta.Value;
        if (ra.Date.HasValue) return ra.Date.Value - DateTimeOffset.UtcNow;

        return ComputeBackoff(attempt);
    }

    private static TimeSpan ComputeBackoff(int attempt)
    {
        // 指数退避 + 抖动：0.5s, 1s, 2s... + [0,250)ms
        var baseMs = 500 * (int)Math.Pow(2, attempt - 1);
        var jitter = Random.Shared.Next(0, 250);
        return TimeSpan.FromMilliseconds(baseMs + jitter);
    }

    private static readonly HashSet<HttpStatusCode> RetryableStatusCodes =
    [
        HttpStatusCode.InternalServerError, // 500
        HttpStatusCode.BadGateway,          // 502
        HttpStatusCode.ServiceUnavailable,  // 503
        HttpStatusCode.GatewayTimeout,      // 504
        HttpStatusCode.RequestTimeout,      // 408
        HttpStatusCode.TooManyRequests
    ];
}
