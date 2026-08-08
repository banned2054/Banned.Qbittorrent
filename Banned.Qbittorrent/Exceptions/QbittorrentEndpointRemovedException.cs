using Banned.Qbittorrent.Models.Application;

namespace Banned.Qbittorrent.Exceptions;

/// <summary>
/// 表示请求的端点已从服务器的 Web API 版本中移除。<br/>
/// Represents an endpoint removed from the server's Web API version.
/// </summary>
/// <param name="endpoint">请求的 API 端点。 / Requested API endpoint.</param>
/// <param name="removed">移除端点的 Web API 版本。 / Web API version that removed the endpoint.</param>
/// <param name="current">服务器当前的 Web API 版本。 / Current Web API version of the server.</param>
public class QbittorrentEndpointRemovedException(string endpoint, ApiVersion removed, ApiVersion current)
    : QbittorrentException($"The endpoint '{endpoint}' was removed in WebAPI {removed}, but server is {current}.")
{
    /// <summary>请求的 API 端点。<br/>Requested API endpoint.</summary>
    public string Endpoint { get; } = endpoint;

    /// <summary>移除端点的 Web API 版本。<br/>Web API version that removed the endpoint.</summary>
    public ApiVersion RemovedVersion { get; } = removed;

    /// <summary>服务器当前的 Web API 版本。<br/>Current Web API version of the server.</summary>
    public ApiVersion CurrentVersion { get; } = current;
}
