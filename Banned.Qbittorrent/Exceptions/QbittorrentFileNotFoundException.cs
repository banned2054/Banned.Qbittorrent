namespace Banned.Qbittorrent.Exceptions;

/// <summary>
/// 表示客户端在本地找不到要上传的文件。<br/>
/// Represents a local file that the client could not find for upload.
/// </summary>
/// <param name="filePath">未找到的本地文件路径。 / Path of the local file that was not found.</param>
public class QbittorrentFileNotFoundException(string filePath)
    : QbittorrentException($"File not found: {filePath}", 400)
{
    /// <summary>获取未找到的本地文件路径。<br/>Gets the path of the local file that was not found.</summary>
    public string FilePath { get; } = filePath;
}
