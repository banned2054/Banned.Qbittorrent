using Banned.Qbittorrent.Models.Application;

namespace Banned.Qbittorrent.Exceptions;

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
