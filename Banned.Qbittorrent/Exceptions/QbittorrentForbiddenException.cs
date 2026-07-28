namespace Banned.Qbittorrent.Exceptions;

/// <summary>
/// 表示 qBittorrent Web API 拒绝了请求（HTTP 403）。<br/>
/// Represents a forbidden response (HTTP 403) from the qBittorrent Web API.
/// </summary>
/// <param name="message">错误消息。 / Error message.</param>
/// <param name="inner">导致当前异常的内部异常。 / Exception that caused the current exception.</param>
public class QbittorrentForbiddenException(string message, Exception? inner = null)
    : QbittorrentException(message, 403, inner);
