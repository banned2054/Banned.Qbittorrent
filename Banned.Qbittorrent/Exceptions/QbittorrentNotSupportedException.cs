using Banned.Qbittorrent.Models.Application;

namespace Banned.Qbittorrent.Exceptions;

/// <summary>
/// 表示服务器的 Web API 版本不支持请求的端点。<br/>
/// Represents an endpoint unsupported by the server's Web API version.
/// </summary>
/// <param name="endpoint">请求的 API 端点。 / Requested API endpoint.</param>
/// <param name="required">端点要求的最低 Web API 版本。 / Minimum Web API version required by the endpoint.</param>
/// <param name="current">服务器当前的 Web API 版本。 / Current Web API version of the server.</param>
public class QbittorrentNotSupportedException(string endpoint, ApiVersion required, ApiVersion current)
    : QbittorrentException($"The endpoint '{endpoint}' requires WebAPI >= {required}, but server is {current}.")
{
    /// <summary>请求的 API 端点。</summary>
    public string Endpoint { get; } = endpoint;

    /// <summary>需要的最低 WebAPI 版本。</summary>
    public ApiVersion RequiredVersion { get; } = required;

    /// <summary>服务器当前的 WebAPI 版本。</summary>
    public ApiVersion CurrentVersion { get; } = current;
}
