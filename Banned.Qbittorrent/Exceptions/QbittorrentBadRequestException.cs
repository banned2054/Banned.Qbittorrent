namespace Banned.Qbittorrent.Exceptions;

/// <summary>
/// 表示 qBittorrent Web API 返回了错误请求（HTTP 400）。<br/>
/// Represents a bad request response (HTTP 400) from the qBittorrent Web API.
/// </summary>
/// <param name="message">错误消息。 / Error message.</param>
/// <param name="inner">导致当前异常的内部异常。 / Exception that caused the current exception.</param>
public class QbittorrentBadRequestException(string message, Exception? inner = null)
    : QbittorrentException(message, 400, inner);
