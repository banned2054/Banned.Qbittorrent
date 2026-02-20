using Banned.Qbittorrent.Models.Enums;
using Banned.Qbittorrent.Utils;
using System.Text.Json.Serialization;

namespace Banned.Qbittorrent.Models.Transfer;

/// <summary>
/// 表示全局传输状态信息。<br/>
/// Represents global transfer status information.
/// </summary>
public class TransferInfo
{
    /// <summary>
    /// 全局下载速度 (字节/秒)。<br/>
    /// Global download rate (bytes/s).
    /// </summary>
    [JsonPropertyName("dl_info_speed")]
    public long? DownloadSpeed { get; set; }

    /// <summary>
    /// 本次会话下载的数据量 (字节)。<br/>
    /// Data downloaded this session (bytes).
    /// </summary>
    [JsonPropertyName("dl_info_data")]
    public long? DownloadDataSession { get; set; }

    /// <summary>
    /// 全局上传速度 (字节/秒)。<br/>
    /// Global upload rate (bytes/s).
    /// </summary>
    [JsonPropertyName("up_info_speed")]
    public long? UploadSpeed { get; set; }

    /// <summary>
    /// 本次会话上传的数据量 (字节)。<br/>
    /// Data uploaded this session (bytes).
    /// </summary>
    [JsonPropertyName("up_info_data")]
    public long? UploadDataSession { get; set; }

    /// <summary>
    /// 全局下载速度限制 (字节/秒)。<br/>
    /// Download rate limit (bytes/s).
    /// </summary>
    [JsonPropertyName("dl_rate_limit")]
    public long? DownloadLimit { get; set; }

    /// <summary>
    /// 全局上传速度限制 (字节/秒)。<br/>
    /// Upload rate limit (bytes/s).
    /// </summary>
    [JsonPropertyName("up_rate_limit")]
    public long? UploadLimit { get; set; }

    /// <summary>
    /// 已连接的 DHT 节点数量。<br/>
    /// Number of connected DHT nodes.
    /// </summary>
    [JsonPropertyName("dht_nodes")]
    public int? DhtNodesCount { get; set; }

    /// <summary>
    /// 连接状态。<br/>
    /// Connection status.
    /// </summary>
    [JsonPropertyName("connection_status")]
    [JsonConverter(typeof(ConnectionStatusConverter))]
    public EnumConnectionStatus? ConnectionStatus { get; set; }

    /// <summary>
    /// 是否启用种子排队。<br/>
    /// Whether torrent queueing is enabled.
    /// </summary>
    [JsonPropertyName("queueing")]
    public bool? QueueingEnabled { get; set; }

    /// <summary>
    /// 是否启用备用速度限制（低速模式）。<br/>
    /// Whether alternative speed limits are enabled.
    /// </summary>
    [JsonPropertyName("use_alt_speed_limits")]
    public bool? AltSpeedLimitsEnabled { get; set; }

    /// <summary>
    /// 传输列表刷新间隔 (毫秒)。<br/>
    /// Transfer list refresh interval (milliseconds).
    /// </summary>
    [JsonPropertyName("refresh_interval")]
    public int? RefreshInterval { get; set; }
}
