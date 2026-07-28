namespace Banned.Qbittorrent.Exceptions;

/// <summary>
/// 表示 qBittorrent Web API 返回了冲突响应（HTTP 409）。<br/>
/// Represents a conflict response (HTTP 409) from the qBittorrent Web API.
/// </summary>
/// <param name="message">错误消息。 / Error message.</param>
/// <param name="inner">导致当前异常的内部异常。 / Exception that caused the current exception.</param>
public class QbittorrentConflictException(string message, Exception? inner = null)
    : QbittorrentException(message, 409, inner);
