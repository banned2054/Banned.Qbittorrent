using System.Net;
using System.Net.Http.Headers;
using Banned.Qbittorrent.Exceptions;
using Banned.Qbittorrent.Models.Application;

namespace Banned.Qbittorrent.Services;

public class NetService : IDisposable
{
    private readonly HttpClient _client;
    private readonly Uri        _baseUrl;

    private ApiVersion _apiVersion;

    public Func<Task>? EnsureLoggedInHandler { get; set; }

    public NetService(string baseUrl)
    {
        _baseUrl = new Uri(baseUrl.TrimEnd('/') + "/");
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

    public  void SetApiVersion(ApiVersion apiVersion) => _apiVersion = apiVersion;
    private Uri  CombineUrl(string        subPath)    => new(_baseUrl, subPath.TrimStart('/'));

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

    public async Task<string> PostWithFiles(string                      subPath,
                                            Dictionary<string, string>? parameters,
                                            List<string>                filePaths,
                                            CancellationToken           ct = default)
    {
        await CheckAuth(false).ConfigureAwait(false);

        return await ExecuteWithRetry(() =>
        {
            var content = new MultipartFormDataContent();
            if (parameters != null)
                foreach (var (k, v) in parameters)
                    content.Add(new StringContent(v), k);

            foreach (var filePath in filePaths)
            {
                if (!File.Exists(filePath)) throw new QbittorrentFileNotFoundException(filePath);
                content.Add(new ByteArrayContent(File.ReadAllBytes(filePath)), "torrents", Path.GetFileName(filePath));
            }

            return new HttpRequestMessage(HttpMethod.Post, CombineUrl(subPath)) { Content = content };
        }, ct).ConfigureAwait(false);
    }

    private async Task CheckAuth(bool skipAuthCheck)
    {
        if (!skipAuthCheck && EnsureLoggedInHandler != null)
        {
            await EnsureLoggedInHandler.Invoke().ConfigureAwait(false);
        }
    }

    private async Task<string> ExecuteWithRetry(Func<HttpRequestMessage> requestFactory,
                                                CancellationToken        ct         = default,
                                                int                      maxRetries = 3)
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
        var ra = response.Headers.RetryAfter;
        if (ra == null) return ComputeBackoff(attempt);
        if (ra.Delta.HasValue) return ra.Delta.Value;
        if (ra.Date.HasValue) return ra.Date.Value - DateTimeOffset.UtcNow;

        return ComputeBackoff(attempt);
    }

    private static TimeSpan ComputeBackoff(int attempt)
    {
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

    public void Dispose()
    {
        _client.Dispose();
    }
}
