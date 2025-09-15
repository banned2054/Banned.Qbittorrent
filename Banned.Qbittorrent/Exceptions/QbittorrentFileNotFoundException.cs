namespace Banned.Qbittorrent.Exceptions;

/// <summary>
/// 表示客户端在本地找不到要上传的文件
/// </summary>
public class QbittorrentFileNotFoundException(string filePath)
    : QbittorrentException($"File not found: {filePath}", 400)
{
    public string FilePath { get; } = filePath;
}
