namespace Banned.Qbittorrent.Exceptions;

public class QbittorrentException(string message, int? statusCode = null, Exception? inner = null)
    : Exception(message, inner)
{
    public int? StatusCode { get; } = statusCode;
}
