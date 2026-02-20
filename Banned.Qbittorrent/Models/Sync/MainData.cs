using Banned.Qbittorrent.Models.Torrent;
using Banned.Qbittorrent.Models.Transfer;
using System.Text.Json.Serialization;

namespace Banned.Qbittorrent.Models.Sync;

/// <summary>
/// 表示主同步数据。<br/>
/// Represents the main synchronization data.
/// </summary>
public class MainData
{
    /// <summary>
    /// 响应标识符（Response ID）。<br/>
    /// Response ID.
    /// </summary>
    [JsonPropertyName("rid")]
    public int Rid { get; set; }

    /// <summary>
    /// 是否为全量更新。<br/>
    /// Whether the response contains all data or only changes.
    /// </summary>
    [JsonPropertyName("full_update")]
    public bool? FullUpdateEnabled { get; set; }

    /// <summary>
    /// 种子信息字典，键为种子的 Hash。<br/>
    /// Dictionary of torrent information, keyed by torrent hash.
    /// </summary>
    [JsonPropertyName("torrents")]
    public Dictionary<string, TorrentInfo>? Torrents { get; set; }

    /// <summary>
    /// 自上次请求以来已删除种子的 Hash 列表。<br/>
    /// List of hashes of torrents removed since the last request.
    /// </summary>
    [JsonPropertyName("torrents_removed")]
    public string[]? TorrentsRemoved { get; set; }

    /// <summary>
    /// 分类信息字典。<br/>
    /// Categories information dictionary.
    /// </summary>
    [JsonPropertyName("categories")]
    public Dictionary<string, TorrentCategory>? Categories { get; set; }

    /// <summary>
    /// 已删除分类的名称列表。<br/>
    /// List of names of categories removed.
    /// </summary>
    [JsonPropertyName("categories_removed")]
    public string[]? CategoriesRemoved { get; set; }

    /// <summary>
    /// 标签列表。<br/>
    /// List of tags.
    /// </summary>
    [JsonPropertyName("tags")]
    public string[]? Tags { get; set; }

    /// <summary>
    /// 已删除标签的列表。<br/>
    /// List of tags removed.
    /// </summary>
    [JsonPropertyName("tags_removed")]
    public string[]? TagsRemoved { get; set; }

    /// <summary>
    /// 全局服务器状态信息。<br/>
    /// Global server state information.
    /// </summary>
    [JsonPropertyName("server_state")]
    public TransferInfo? ServerState { get; set; }
}
