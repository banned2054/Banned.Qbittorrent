namespace Banned.Qbittorrent.Exceptions;

public class QbittorrentForbiddenException(string message, Exception? inner = null)
    : QbittorrentException(message, 403, inner);
