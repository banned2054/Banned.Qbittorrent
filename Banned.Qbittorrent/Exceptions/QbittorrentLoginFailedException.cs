namespace Banned.Qbittorrent.Exceptions;

/// <summary>
/// 表示登录 qBittorrent 失败。<br/>
/// Represents a failed qBittorrent login attempt.
/// </summary>
/// <param name="message">错误消息。 / Error message.</param>
/// <param name="statusCode">登录响应的 HTTP 状态码。 / HTTP status code of the login response.</param>
public class QbittorrentLoginFailedException(string message, int statusCode)
    : QbittorrentException(message, statusCode);
