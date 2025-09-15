using Banned.Qbittorrent.Exceptions;
using System.Net;
using System.Net.Http.Headers;

namespace Banned.Qbittorrent.Utils;

public class NetUtils
{
    private readonly HttpClient      _client;
    private readonly CookieContainer _cookieContainer;
    private readonly Uri             _baseUrl;

    private readonly string _userName;
    private readonly string _password;

    public NetUtils(string baseUrl, string userName, string password)
    {
        _baseUrl         = new Uri(baseUrl.TrimEnd('/') + "/");
        _userName        = userName;
        _password        = password;
        _cookieContainer = new CookieContainer();

        var handler = new HttpClientHandler
        {
            CookieContainer   = _cookieContainer,
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
    public async Task<string> GetAsync(string subPath)
    {
        await EnsureLoggedInAsync();

        var request = new HttpRequestMessage(HttpMethod.Get, CombineUrl(subPath));
        request.Headers.ConnectionClose = true;

        return await ExecuteWithRetryAsync(() => _client.SendAsync(request));
    }

    /// <summary>
    /// 通用 POST 请求（异步）
    /// </summary>
    public async Task<string> PostAsync(string subPath, Dictionary<string, string> parameters)
    {
        await EnsureLoggedInAsync();

        var request = new HttpRequestMessage(HttpMethod.Post, CombineUrl(subPath))
        {
            Content = new FormUrlEncodedContent(parameters)
        };
        request.Headers.ConnectionClose = true;

        return await ExecuteWithRetryAsync(() => _client.SendAsync(request));
    }

    /// <summary>
    /// 通用 POST 文件上传（异步）
    /// </summary>
    public async Task<string> PostWithFilesAsync(string                     subPath,
                                                 Dictionary<string, string> parameters,
                                                 List<string>               filePaths)
    {
        await EnsureLoggedInAsync();

        using var content = new MultipartFormDataContent();
        foreach (var param in parameters)
        {
            content.Add(new StringContent(param.Value), param.Key);
        }

        foreach (var filePath in filePaths)
        {
            if (File.Exists(filePath))
            {
                content.Add(new ByteArrayContent(await File.ReadAllBytesAsync(filePath)), "torrents",
                            Path.GetFileName(filePath));
            }
            else
            {
                throw new QbittorrentFileNotFoundException(filePath);
            }
        }

        var request = new HttpRequestMessage(HttpMethod.Post, CombineUrl(subPath)) { Content = content };
        request.Headers.ConnectionClose = true;

        return await ExecuteWithRetryAsync(() => _client.SendAsync(request));
    }

    private async Task EnsureLoggedInAsync()
    {
        if (_cookieContainer.GetCookies(_baseUrl).Count > 0) return;
        await LoginAsync();
    }

    private async Task LoginAsync()
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
    private static async Task<string> ExecuteWithRetryAsync(Func<Task<HttpResponseMessage>> action,
                                                            int                             maxRetries = 3)
    {
        Exception?           lastException = null;
        HttpResponseMessage? lastResponse  = null;

        var lastBody = string.Empty;

        for (var attempt = 1; attempt <= maxRetries; attempt++)
        {
            try
            {
                using var response = await action();
                lastResponse = response;

                // 先把 body 读出来，避免 Dispose 后拿不到
                lastBody = await response.Content.ReadAsStringAsync();

                if (response.IsSuccessStatusCode)
                    return lastBody;

                if (!RetryableStatusCodes.Contains(response.StatusCode)) throw MapToException(response, lastBody);
                // 可重试状态码：到达最大次数则结束，否则等待后重试
                if (attempt == maxRetries) break;
                await Task.Delay(GetDelayFromRetryAfterOrBackoff(response, attempt));
            }
            catch (TaskCanceledException ex) // 包括超时
            {
                lastException = ex;
                if (attempt == maxRetries) break;
                await Task.Delay(ComputeBackoff(attempt));
            }
            catch (HttpRequestException ex) // 连接中断、DNS 问题等
            {
                lastException = ex;
                if (attempt == maxRetries) break;
                await Task.Delay(ComputeBackoff(attempt));
            }
        }

        // 到这里说明用尽重试：有响应就按响应映射异常；否则按网络异常抛出
        if (lastResponse != null)
            throw MapToException(lastResponse, lastBody);

        throw new QbittorrentServerErrorException(
                                                  $"Network error after {maxRetries} attempts: {lastException?.Message ?? "unknown error"}");
    }

    private static Exception MapToException(HttpResponseMessage response, string body)
    {
        return response.StatusCode switch
        {
            HttpStatusCode.Unauthorized        => new QbittorrentUnauthorizedException("Unauthorized: 请检查用户名/密码。"),
            HttpStatusCode.Forbidden           => new QbittorrentForbiddenException("Forbidden: 没有权限或登录态已过期。"),
            HttpStatusCode.NotFound            => new QbittorrentNotFoundException($"Not Found: {body}"),
            HttpStatusCode.Conflict            => new QbittorrentConflictException("Conflict: 种子已存在或状态冲突。"),
            HttpStatusCode.InternalServerError => new QbittorrentServerErrorException($"Server Error: {body}"),
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
