namespace Banned.Qbittorrent.Exceptions;

public class QbittorrentBadRequestException(string message, Exception? inner = null)
    : QbittorrentException(message, 400, inner);
