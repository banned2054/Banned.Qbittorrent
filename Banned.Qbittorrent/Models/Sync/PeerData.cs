using System.Text.Json.Serialization;

namespace Banned.Qbittorrent.Models.Sync;

/// <summary>
/// 表示用户节点同步数据。<br/>
/// Represents peer synchronization data.
/// </summary>
public class PeerData
{
    /// <summary>
    /// 响应标识符（Response ID）。<br/>
    /// Response ID.
    /// </summary>
    [JsonPropertyName("rid")]
    public int Rid { get; set; }

    /// <summary>
    /// 是否为全量更新。<br/>
    /// Whether it is a full update.
    /// </summary>
    [JsonPropertyName("full_update")]
    public bool? FullUpdateEnabled { get; set; }

    /// <summary>
    /// 是否启用显示标志。<br/>
    /// Whether to enable showing flags.
    /// </summary>
    [JsonPropertyName("show_flags")]
    public bool? ShowFlagsEnabled { get; set; }

    /// <summary>
    /// 包含节点更新信息的字典，键为节点标识。<br/>
    /// Dictionary containing peer update information, keyed by peer identifier.
    /// </summary>
    [JsonPropertyName("peers")]
    public Dictionary<string, PeerInfo>? Peers { get; set; }

    /// <summary>
    /// 已移除的节点标识列表。<br/>
    /// List of removed peer identifiers.
    /// </summary>
    [JsonPropertyName("peers_removed")]
    public string[]? PeersRemoved { get; set; }
}
