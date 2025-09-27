namespace Banned.Qbittorrent.Exceptions;

public class QbittorrentServerErrorException(string message, Exception? inner = null)
    : QbittorrentException(message, 500, inner);
