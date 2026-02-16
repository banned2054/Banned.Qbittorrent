namespace Banned.Qbittorrent.Exceptions;

public class QbittorrentNotFoundException(string message, Exception? inner = null)
    : QbittorrentException(message, 404, inner);
