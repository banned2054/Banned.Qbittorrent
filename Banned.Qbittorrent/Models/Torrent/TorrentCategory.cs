using System.Text.Json.Serialization;

namespace Banned.Qbittorrent.Models.Torrent;

/// <summary>
/// 表示种子分类信息。<br/>
/// Represents torrent category information.
/// </summary>
public class TorrentCategory
{
    /// <summary>
    /// 分类名称。<br/>
    /// Category name.
    /// </summary>
    [JsonPropertyName("name")]
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// 该分类对应的保存路径。<br/>
    /// The save path associated with this category.
    /// </summary>
    [JsonPropertyName("savePath")]
    public string SavePath { get; set; } = string.Empty;
}
