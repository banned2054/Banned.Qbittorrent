namespace Banned.Qbittorrent.Exceptions;

/// <summary>
/// 表示登录 qBittorrent 失败
/// </summary>
public class QbittorrentLoginFailedException(string message, int statusCode)
    : QbittorrentException(message, statusCode);
