namespace Banned.Qbittorrent.Exceptions;

public class QbittorrentUnauthorizedException(string message, Exception? inner = null)
    : QbittorrentException(message, 401, inner);
