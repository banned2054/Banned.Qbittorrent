using System.Text.Json.Serialization;

namespace Banned.Qbittorrent.Models.Sync;

/// <summary>
/// 表示用户节点的详细信息。<br/>
/// Represents detailed information about a peer.
/// </summary>
public class PeerInfo
{
    /// <summary>
    /// 客户端名称。<br/>
    /// Client name.
    /// </summary>
    [JsonPropertyName("client")]
    public string? Client { get; set; }

    /// <summary>
    /// 连接类型（如：uTP, TCP）。<br/>
    /// Connection type (e.g., uTP, TCP).
    /// </summary>
    [JsonPropertyName("connection")]
    public string? Connection { get; set; }

    /// <summary>
    /// 国家名称。<br/>
    /// Country name.
    /// </summary>
    [JsonPropertyName("country")]
    public string? Country { get; set; }

    /// <summary>
    /// 国家代码。<br/>
    /// Country code.
    /// </summary>
    [JsonPropertyName("country_code")]
    public string? CountryCode { get; set; }

    /// <summary>
    /// 下载速度（字节/秒）。<br/>
    /// Download speed (bytes/s).
    /// </summary>
    [JsonPropertyName("dl_speed")]
    public long? DownloadSpeed { get; set; }

    /// <summary>
    /// 上传速度（字节/秒）。<br/>
    /// Upload speed (bytes/s).
    /// </summary>
    [JsonPropertyName("up_speed")]
    public long? UploadSpeed { get; set; }

    /// <summary>
    /// 进度百分比。<br/>
    /// Progress percentage.
    /// </summary>
    [JsonPropertyName("progress")]
    public double? Progress { get; set; }

    /// <summary>
    /// 节点标志。<br/>
    /// Peer flags.
    /// </summary>
    [JsonPropertyName("flags")]
    public string? Flags { get; set; }

    /// <summary>
    /// 节点标志的描述。<br/>
    /// Description of peer flags.
    /// </summary>
    [JsonPropertyName("flags_desc")]
    public string? FlagsDescription { get; set; }

    /// <summary>
    /// 相关性（节点拥有本端缺失数据的比例）。<br/>
    /// Relevance (percentage of files this peer has that the local client does not).
    /// </summary>
    [JsonPropertyName("relevance")]
    public double? Relevance { get; set; }

    /// <summary>
    /// 节点正在下载的文件列表。<br/>
    /// List of files the peer is downloading.
    /// </summary>
    [JsonPropertyName("files")]
    public string? Files { get; set; }
}
