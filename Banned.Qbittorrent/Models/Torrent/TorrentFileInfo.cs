using Banned.Qbittorrent.Models.Enums;
using System.Text.Json.Serialization;

namespace Banned.Qbittorrent.Models.Torrent;

/// <summary>
/// 表示种子内单个文件的详细信息。<br/>
/// Represents detailed information about an individual file within a torrent.
/// </summary>
public class TorrentFileInfo
{
    /// <summary>
    /// 文件索引。<br/>
    /// File index.
    /// </summary>
    [JsonPropertyName("index")]
    public int? Index { get; set; }

    /// <summary>
    /// 文件名称（包含相对于根目录的路径）。<br/>
    /// File name (including path relative to root).
    /// </summary>
    [JsonPropertyName("name")]
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// 文件大小（字节）。<br/>
    /// File size (bytes).
    /// </summary>
    [JsonPropertyName("size")]
    public long Size { get; set; }

    /// <summary>
    /// 文件下载进度。<br/>
    /// File download progress.
    /// </summary>
    [JsonPropertyName("progress")]
    public double Progress { get; set; }

    /// <summary>
    /// 文件下载优先级。<br/>
    /// File download priority.
    /// </summary>
    [JsonPropertyName("priority")]
    public EnumTorrentFilePriority Priority { get; set; }

    /// <summary>
    /// 是否已启用做种状态（文件是否已完整且可供做种）。<br/>
    /// Whether the file is enabled as seed (file is complete and available for seeding).
    /// </summary>
    [JsonPropertyName("is_seed")]
    public bool SeedEnabled { get; set; }

    /// <summary>
    /// 该文件包含的分块范围（起始和结束分块索引）。<br/>
    /// Range of pieces for this file (start and end piece index).
    /// </summary>
    [JsonPropertyName("piece_range")]
    public int[] PieceRange { get; set; } = [];

    /// <summary>
    /// 文件的可用性百分比。<br/>
    /// File availability percentage.
    /// </summary>
    [JsonPropertyName("availability")]
    public double? Availability { get; set; }
}
