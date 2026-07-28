namespace Banned.Qbittorrent.Exceptions;

/// <summary>
/// 表示 qBittorrent Web API 要求身份验证（HTTP 401）。<br/>
/// Represents an authentication-required response (HTTP 401) from the qBittorrent Web API.
/// </summary>
/// <param name="message">错误消息。 / Error message.</param>
/// <param name="inner">导致当前异常的内部异常。 / Exception that caused the current exception.</param>
public class QbittorrentUnauthorizedException(string message, Exception? inner = null)
    : QbittorrentException(message, 401, inner);
