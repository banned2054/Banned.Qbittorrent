using Banned.Qbittorrent.Models.Torrent;
using Banned.Qbittorrent.Models.Transfer;
using System.Text.Json.Serialization;

namespace Banned.Qbittorrent.Models.Sync;

/// <summary>
/// 表示同步的主数据。 / Represents the main data from sync.
/// </summary>
public class MainData
{
    /// <summary>响应 ID。 / Response ID.</summary>
    [JsonPropertyName("rid")]
    public int Rid { get; set; }

    /// <summary>是否为全量更新。 / Whether the response contains all data or only changes.</summary>
    [JsonPropertyName("full_update")]
    public bool? IsFullUpdate { get; set; }

    /// <summary>
    /// 种子信息字典。键为种子的 Hash。 / Dictionary of torrent information. Key is torrent Hash.
    /// </summary>
    [JsonPropertyName("torrents")]
    public Dictionary<string, TorrentInfo>? Torrents { get; set; }

    /// <summary>
    /// 已删除种子的 Hash 列表。 / List of hashes of torrents removed since last request.
    /// </summary>
    [JsonPropertyName("torrents_removed")]
    public List<string>? TorrentsRemoved { get; set; }

    /// <summary>
    /// 分类信息。 / Categories information.
    /// </summary>
    [JsonPropertyName("categories")]
    public Dictionary<string, TorrentCategory>? Categories { get; set; }

    /// <summary>
    /// 已删除分类的名称列表。 / List of names of categories removed.
    /// </summary>
    [JsonPropertyName("categories_removed")]
    public List<string>? CategoriesRemoved { get; set; }

    /// <summary>
    /// 标签列表。 / List of tags.
    /// </summary>
    [JsonPropertyName("tags")]
    public List<string>? Tags { get; set; }

    /// <summary>
    /// 已删除标签的列表。 / List of tags removed.
    /// </summary>
    [JsonPropertyName("tags_removed")]
    public List<string>? TagsRemoved { get; set; }

    /// <summary>
    /// 服务器状态信息。 / Global server state information.
    /// </summary>
    [JsonPropertyName("server_state")]
    public TransferInfo? ServerState { get; set; }
}
