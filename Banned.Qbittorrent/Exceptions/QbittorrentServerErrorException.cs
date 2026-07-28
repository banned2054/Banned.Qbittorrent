namespace Banned.Qbittorrent.Exceptions;

/// <summary>
/// 表示 qBittorrent Web API 返回了服务器错误（HTTP 500）。<br/>
/// Represents a server error response (HTTP 500) from the qBittorrent Web API.
/// </summary>
/// <param name="message">错误消息。 / Error message.</param>
/// <param name="inner">导致当前异常的内部异常。 / Exception that caused the current exception.</param>
public class QbittorrentServerErrorException(string message, Exception? inner = null)
    : QbittorrentException(message, 500, inner);
