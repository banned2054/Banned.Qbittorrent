namespace Banned.Qbittorrent.Exceptions;

/// <summary>
/// 表示 qBittorrent Web API 找不到请求的资源（HTTP 404）。<br/>
/// Represents a resource-not-found response (HTTP 404) from the qBittorrent Web API.
/// </summary>
/// <param name="message">错误消息。 / Error message.</param>
/// <param name="inner">导致当前异常的内部异常。 / Exception that caused the current exception.</param>
public class QbittorrentNotFoundException(string message, Exception? inner = null)
    : QbittorrentException(message, 404, inner);
