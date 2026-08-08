using Banned.Qbittorrent.Exceptions;
using Banned.Qbittorrent.Models;
using System.Net;
using System.Net.Http.Headers;
using System.Text;

namespace Banned.Qbittorrent.Utils;

/// <summary>
/// 提供网络选项和请求重放判断相关的无状态方法。<br/>
/// Provides stateless helpers for network options and request replay decisions.
/// </summary>
internal static class NetUtils
{
    internal static void ValidateOptions(QBittorrentClientOptions options)
    {
        if (options.MaxRetries < 1)
            throw new ArgumentOutOfRangeException(nameof(options), "MaxRetries must be at least 1.");
        if (options.Timeout != Timeout.InfiniteTimeSpan && options.Timeout <= TimeSpan.Zero)
            throw new ArgumentOutOfRangeException(nameof(options), "Timeout must be positive or infinite.");
        if (options.ConnectTimeout != Timeout.InfiniteTimeSpan && options.ConnectTimeout <= TimeSpan.Zero)
            throw new ArgumentOutOfRangeException(nameof(options), "ConnectTimeout must be positive or infinite.");
    }

    internal static bool IsConnectionEstablishmentFailure(Exception exception)
    {
        if (exception is HttpRequestException httpRequestException)
        {
            return httpRequestException.HttpRequestError is
                HttpRequestError.NameResolutionError or
                HttpRequestError.ConnectionError or
                HttpRequestError.SecureConnectionError;
        }

        if (exception is not TaskCanceledException)
            return false;

        return EnumerateExceptionChain(exception).Any(current => current is TimeoutException &&
                                                                 current.Message
                                                                        .Contains("connection could not be established",
                                                                                  StringComparison.OrdinalIgnoreCase) &&
                                                                 current.Message.Contains("ConnectTimeout",
                                                                          StringComparison.OrdinalIgnoreCase));
    }

    internal static bool IsSafeToReplayAfterConnectionFailure(
        HttpMethod? method,
        bool        isAuthenticationEndpoint)
    {
        return method == HttpMethod.Get     ||
               method == HttpMethod.Head    ||
               method == HttpMethod.Options ||
               method == HttpMethod.Trace   ||
               (method == HttpMethod.Post && isAuthenticationEndpoint);
    }

    internal static QbittorrentException MapToException(HttpResponseMessage response, string body)
    {
        return response.StatusCode switch
        {
            HttpStatusCode.BadRequest          => new QbittorrentBadRequestException("Bad Request."),
            HttpStatusCode.Unauthorized        => new QbittorrentUnauthorizedException("Unauthorized"),
            HttpStatusCode.Forbidden           => new QbittorrentForbiddenException("Forbidden"),
            HttpStatusCode.NotFound            => new QbittorrentNotFoundException("Not Found"),
            HttpStatusCode.Conflict            => new QbittorrentConflictException("Conflict"),
            HttpStatusCode.InternalServerError => new QbittorrentServerErrorException("Server Error"),
            _ => new QbittorrentException(
                                          $"HTTP {(int)response.StatusCode} {response.ReasonPhrase}: {body}",
                                          (int)response.StatusCode)
        };
    }

    internal static TimeSpan GetRetryDelay(HttpResponseMessage response, int attempt)
    {
        var retryAfter = response.Headers.RetryAfter;
        if (retryAfter?.Delta.HasValue == true) return retryAfter.Delta.Value;
        if (retryAfter?.Date.HasValue  == true) return retryAfter.Date.Value - DateTimeOffset.UtcNow;
        return ComputeBackoff(attempt);
    }

    internal static TimeSpan ComputeBackoff(int attempt)
    {
        var baseMs = 500 * (int)Math.Pow(2, attempt - 1);
        var jitter = Random.Shared.Next(0, 250);
        return TimeSpan.FromMilliseconds(Math.Min(2000, baseMs + jitter));
    }

    internal static bool IsRetryableStatusCode(HttpStatusCode statusCode)
    {
        return statusCode is
            HttpStatusCode.InternalServerError or
            HttpStatusCode.BadGateway or
            HttpStatusCode.ServiceUnavailable or
            HttpStatusCode.GatewayTimeout or
            HttpStatusCode.RequestTimeout or
            HttpStatusCode.TooManyRequests;
    }

    internal static bool IsAuthenticationFailure(HttpStatusCode statusCode) =>
        statusCode is HttpStatusCode.Unauthorized or HttpStatusCode.Forbidden;

    internal static bool IsAuthenticationEndpoint(string subPath) =>
        subPath.TrimStart('/').StartsWith("api/v2/auth/", StringComparison.OrdinalIgnoreCase);

    internal static string DescribeRequest(HttpRequestMessage request)
    {
        var contentLength = request.Content?.Headers.ContentLength;
        var contentType   = request.Content?.Headers.ContentType?.ToString() ?? "<none>";
        return
            $"method={request.Method} uri={RedactUri(request.RequestUri)} version={request.Version} versionPolicy={request.VersionPolicy} " +
            $"contentType={contentType} contentLength={(contentLength.HasValue ? contentLength.Value.ToString() : "<unknown>")} " +
            $"headers=[{SanitizeHeaders(request.Headers)}] contentHeaders=[{SanitizeHeaders(request.Content?.Headers)}]";
    }

    internal static string RedactUri(Uri? uri)
    {
        if (uri == null) return "<null>";
        var path = string.IsNullOrEmpty(uri.AbsolutePath) ? "/" : uri.AbsolutePath;
        return uri.Query.Length > 0
            ? $"<redacted-origin>{path}?<redacted-query>"
            : $"<redacted-origin>{path}";
    }

    internal static string DescribeEndpoint(Uri uri) =>
        $"scheme={uri.Scheme} port={uri.Port} isDefaultPort={uri.IsDefaultPort} " +
        $"isLoopback={uri.IsLoopback} hostType={Uri.CheckHostName(uri.Host)} hostLength={uri.Host.Length}";

    internal static string DescribeResponse(HttpResponseMessage response, string body)
    {
        var bodyPreview = body.Length > 256 ? body[..256] + "<truncated>" : body;
        bodyPreview = bodyPreview.Replace("\r", "\\r").Replace("\n", "\\n");
        return
            $"status={(int)response.StatusCode}({response.StatusCode}) reason={response.ReasonPhrase ?? "<null>"} " +
            $"version={response.Version} headers=[{SanitizeHeaders(response.Headers)}] "                            +
            $"contentHeaders=[{SanitizeHeaders(response.Content.Headers)}] bodyLength={body.Length} bodyPreview='{bodyPreview}'";
    }

    internal static string DescribeException(Exception? exception)
    {
        if (exception == null) return "<none>";

        var builder = new StringBuilder();
        for (var current = exception; current != null; current = current.InnerException)
        {
            if (builder.Length > 0) builder.Append(" -> ");
            builder.Append(current.GetType().FullName)
                   .Append("(hresult=0x")
                   .Append(current.HResult.ToString("X8"))
                   .Append(", message='")
                   .Append(current.Message.Replace("\r", "\\r").Replace("\n", "\\n"))
                   .Append("')");
        }

        return builder.ToString();
    }

    private static string SanitizeHeaders(HttpHeaders? headers)
    {
        if (headers == null) return "<none>";

        var values = headers.Select(header =>
        {
            var value = IsSensitiveHeader(header.Key)
                ? "<redacted>"
                : string.Join("|", header.Value.Select(item => item.Length > 128 ? item[..128] + "<truncated>" : item));
            return $"{header.Key}={value}";
        });
        return string.Join("; ", values);
    }

    private static bool IsSensitiveHeader(string headerName) =>
        headerName.Equals("Authorization", StringComparison.OrdinalIgnoreCase) ||
        headerName.Equals("Cookie", StringComparison.OrdinalIgnoreCase)        ||
        headerName.Equals("Set-Cookie", StringComparison.OrdinalIgnoreCase);

    private static IEnumerable<Exception> EnumerateExceptionChain(Exception exception)
    {
        for (Exception? current = exception; current != null; current = current.InnerException)
            yield return current;
    }
}
