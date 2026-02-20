using Banned.Qbittorrent.Models.Enums;
using System.Text.Json.Serialization;

namespace Banned.Qbittorrent.Models.Torrent;

/// <summary>
/// 表示种子的 Tracker（追踪器）信息。<br/>
/// Represents tracker information for a torrent.
/// </summary>
public class TrackerInfo
{
    /// <summary>
    /// Tracker 的 URL 地址。<br/>
    /// Tracker URL.
    /// </summary>
    [JsonPropertyName("url")]
    public string Url { get; set; } = string.Empty;

    /// <summary>
    /// Tracker 的状态。<br/>
    /// Tracker status.
    /// </summary>
    [JsonPropertyName("status")]
    public EnumTrackerStatus Status { get; set; }

    /// <summary>
    /// Tracker 的层级（优先级）。<br/>
    /// Tracker tier.
    /// </summary>
    [JsonPropertyName("tier")]
    public int Tier { get; set; }

    /// <summary>
    /// 该 Tracker 提供的 Peer（用户）数量。<br/>
    /// Number of peers for this tracker.
    /// </summary>
    [JsonPropertyName("num_peers")]
    public int PeersCount { get; set; }

    /// <summary>
    /// 该 Tracker 提供的 Seed（做种者）数量。<br/>
    /// Number of seeds for this tracker.
    /// </summary>
    [JsonPropertyName("num_seeds")]
    public int SeedsCount { get; set; }

    /// <summary>
    /// 该 Tracker 提供的 Leech（下载者）数量。<br/>
    /// Number of leeches for this tracker.
    /// </summary>
    [JsonPropertyName("num_leeches")]
    public int LeechesCount { get; set; }

    /// <summary>
    /// 已通过该 Tracker 完成下载的次数。<br/>
    /// Number of times the torrent has been downloaded via this tracker.
    /// </summary>
    [JsonPropertyName("num_downloaded")]
    public int DownloadedCount { get; set; }

    /// <summary>
    /// Tracker 返回的消息（通常用于显示错误信息）。<br/>
    /// Message returned by the tracker (usually used for error messages).
    /// </summary>
    [JsonPropertyName("msg")]
    public string Message { get; set; } = string.Empty;
}
