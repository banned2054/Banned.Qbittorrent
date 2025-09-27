namespace Banned.Qbittorrent.Exceptions;

public class QbittorrentConflictException(string message, Exception? inner = null)
    : QbittorrentException(message, 409, inner);
