using Banned.Qbittorrent.Utils;
using System.Text.Json.Serialization;

namespace Banned.Qbittorrent.Models.Torrent;

/// <summary>
/// 表示种子的通用属性。<br/>
/// Represents the general properties of a torrent.
/// </summary>
public class TorrentProperties
{
    /// <summary>
    /// 种子添加日期。<br/>
    /// The date when the torrent was added.
    /// </summary>
    [JsonPropertyName("addition_date")]
    [JsonConverter(typeof(UnixTimestampConverter))]
    public DateTimeOffset AdditionDate { get; set; }

    /// <summary>
    /// 种子注释。<br/>
    /// Torrent comment.
    /// </summary>
    [JsonPropertyName("comment")]
    public string Comment { get; set; } = string.Empty;

    /// <summary>
    /// 种子完成日期。<br/>
    /// The date when the torrent was completed.
    /// </summary>
    [JsonPropertyName("completion_date")]
    [JsonConverter(typeof(UnixTimestampConverter))]
    public DateTimeOffset CompletionDate { get; set; }

    /// <summary>
    /// 种子创建者。<br/>
    /// Torrent creator.
    /// </summary>
    [JsonPropertyName("created_by")]
    public string CreatedBy { get; set; } = string.Empty;

    /// <summary>
    /// 种子创建日期。<br/>
    /// The date when the torrent was created.
    /// </summary>
    [JsonPropertyName("creation_date")]
    [JsonConverter(typeof(UnixTimestampConverter))]
    public DateTimeOffset CreationDate { get; set; }

    /// <summary>
    /// 下载速度限制（字节/秒）。<br/>
    /// Download speed limit (bytes/s).
    /// </summary>
    [JsonPropertyName("dl_limit")]
    public long DownloadLimit { get; set; }

    /// <summary>
    /// 当前下载速度（字节/秒）。<br/>
    /// Current download speed (bytes/s).
    /// </summary>
    [JsonPropertyName("dl_speed")]
    public long DownloadSpeed { get; set; }

    /// <summary>
    /// 平均下载速度（字节/秒）。<br/>
    /// Average download speed (bytes/s).
    /// </summary>
    [JsonPropertyName("dl_speed_avg")]
    public long DownloadSpeedAverage { get; set; }

    /// <summary>
    /// 预计剩余时间。<br/>
    /// Estimated Time of Arrival.
    /// </summary>
    [JsonPropertyName("eta")]
    [JsonConverter(typeof(SecondsTimeSpanConverter))]
    public TimeSpan EstimatedTimeArrival { get; set; }

    /// <summary>
    /// 是否启用私有模式。<br/>
    /// Whether private mode is enabled.
    /// </summary>
    [JsonPropertyName("isPrivate")]
    public bool PrivateEnabled { get; set; }

    /// <summary>
    /// 最后一次看到完整种子的时间。<br/>
    /// The last time the torrent was seen complete.
    /// </summary>
    [JsonPropertyName("last_seen")]
    [JsonConverter(typeof(UnixTimestampConverter))]
    public DateTimeOffset LastSeen { get; set; }

    /// <summary>
    /// 当前连接数。<br/>
    /// Number of current connections.
    /// </summary>
    [JsonPropertyName("nb_connections")]
    public int Connections { get; set; }

    /// <summary>
    /// 最大连接数限制。<br/>
    /// Maximum connections limit.
    /// </summary>
    [JsonPropertyName("nb_connections_limit")]
    public int ConnectionsLimit { get; set; }

    /// <summary>
    /// 当前连接的下载用户数。<br/>
    /// Number of connected peers (leeches).
    /// </summary>
    [JsonPropertyName("peers")]
    public int Peers { get; set; }

    /// <summary>
    /// 所有的下载用户总数。<br/>
    /// Total number of peers (leeches) in the swarm.
    /// </summary>
    [JsonPropertyName("peers_total")]
    public int PeersCount { get; set; }

    /// <summary>
    /// 分块大小（字节）。<br/>
    /// Size of each piece (bytes).
    /// </summary>
    [JsonPropertyName("piece_size")]
    public long PieceSize { get; set; }

    /// <summary>
    /// 已拥有的分块数量。<br/>
    /// Number of pieces already owned.
    /// </summary>
    [JsonPropertyName("pieces_have")]
    public int PiecesHave { get; set; }

    /// <summary>
    /// 分块总数。<br/>
    /// Total number of pieces.
    /// </summary>
    [JsonPropertyName("pieces_num")]
    public int PiecesNum { get; set; }

    /// <summary>
    /// 下次汇报（Announce）的时间间隔。<br/>
    /// Time interval until next announce.
    /// </summary>
    [JsonPropertyName("reannounce")]
    public int ReannounceInterval { get; set; }

    /// <summary>
    /// 保存路径。<br/>
    /// Save path of the torrent.
    /// </summary>
    [JsonPropertyName("save_path")]
    public string SavePath { get; set; } = string.Empty;

    /// <summary>
    /// 累计做种时间。<br/>
    /// Total seeding time.
    /// </summary>
    [JsonPropertyName("seeding_time")]
    [JsonConverter(typeof(SecondsTimeSpanConverter))]
    public TimeSpan SeedingTime { get; set; }

    /// <summary>
    /// 当前连接的做种者数量。<br/>
    /// Number of connected seeds.
    /// </summary>
    [JsonPropertyName("seeds")]
    public int Seeds { get; set; }

    /// <summary>
    /// 所有的做种者总数。<br/>
    /// Total number of seeds in the swarm.
    /// </summary>
    [JsonPropertyName("seeds_total")]
    public int SeedsCount { get; set; }

    /// <summary>
    /// 当前分享率。<br/>
    /// Current share ratio.
    /// </summary>
    [JsonPropertyName("share_ratio")]
    public double ShareRatio { get; set; }

    /// <summary>
    /// 累计耗时。<br/>
    /// Total time elapsed.
    /// </summary>
    [JsonPropertyName("time_elapsed")]
    [JsonConverter(typeof(SecondsTimeSpanConverter))]
    public TimeSpan TimeElapsed { get; set; }

    /// <summary>
    /// 累计下载字节数。<br/>
    /// Total bytes downloaded.
    /// </summary>
    [JsonPropertyName("total_downloaded")]
    public long TotalDownloaded { get; set; }

    /// <summary>
    /// 本次会话下载的字节数。<br/>
    /// Total bytes downloaded in current session.
    /// </summary>
    [JsonPropertyName("total_downloaded_session")]
    public long TotalDownloadedSession { get; set; }

    /// <summary>
    /// 种子总大小（字节）。<br/>
    /// Total size of the torrent (bytes).
    /// </summary>
    [JsonPropertyName("total_size")]
    public long TotalSize { get; set; }

    /// <summary>
    /// 累计上传字节数。<br/>
    /// Total bytes uploaded.
    /// </summary>
    [JsonPropertyName("total_uploaded")]
    public long TotalUploaded { get; set; }

    /// <summary>
    /// 本次会话上传的字节数。<br/>
    /// Total bytes uploaded in current session.
    /// </summary>
    [JsonPropertyName("total_uploaded_session")]
    public long TotalUploadedSession { get; set; }

    /// <summary>
    /// 累计浪费的字节数。<br/>
    /// Total bytes wasted.
    /// </summary>
    [JsonPropertyName("total_wasted")]
    public long TotalWasted { get; set; }

    /// <summary>
    /// 上传速度限制（字节/秒）。<br/>
    /// Upload speed limit (bytes/s).
    /// </summary>
    [JsonPropertyName("up_limit")]
    public long UploadLimit { get; set; }

    /// <summary>
    /// 当前上传速度（字节/秒）。<br/>
    /// Current upload speed (bytes/s).
    /// </summary>
    [JsonPropertyName("up_speed")]
    public long UploadSpeed { get; set; }

    /// <summary>
    /// 平均上传速度（字节/秒）。<br/>
    /// Average upload speed (bytes/s).
    /// </summary>
    [JsonPropertyName("up_speed_avg")]
    public long UploadSpeedAverage { get; set; }
}
