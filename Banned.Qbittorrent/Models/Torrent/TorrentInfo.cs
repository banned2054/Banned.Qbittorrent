using Banned.Qbittorrent.Models.Enums;
using Banned.Qbittorrent.Utils;
using System.Text.Json.Serialization;

namespace Banned.Qbittorrent.Models.Torrent;

/// <summary>
/// 表示种子的详细信息。<br/>
/// Represents detailed information of a torrent.
/// </summary>
public class TorrentInfo
{
    /// <summary>
    /// 添加时间。<br/>
    /// Time when the torrent was added.
    /// </summary>
    [JsonPropertyName("added_on")]
    [JsonConverter(typeof(UnixTimestampConverter))]
    public DateTimeOffset? AddedOn { get; set; }

    /// <summary>
    /// 剩余需要下载的字节数。<br/>
    /// Remaining bytes to download.
    /// </summary>
    [JsonPropertyName("amount_left")]
    public long? AmountLeft { get; set; }

    /// <summary>
    /// 是否启用自动种子管理。<br/>
    /// Whether Automatic Torrent Management is enabled.
    /// </summary>
    [JsonPropertyName("auto_tmm")]
    public bool? AutoTmmEnabled { get; set; }

    /// <summary>
    /// 可用性百分比。<br/>
    /// Percentage of file segments available.
    /// </summary>
    [JsonPropertyName("availability")]
    public float? Availability { get; set; }

    /// <summary>
    /// 所属分类。<br/>
    /// Category of the torrent.
    /// </summary>
    [JsonPropertyName("category")]
    public string? Category { get; set; }

    /// <summary>
    /// 已完成的字节数。<br/>
    /// Total bytes completed.
    /// </summary>
    [JsonPropertyName("completed")]
    public long? Completed { get; set; }

    /// <summary>
    /// 完成时间。<br/>
    /// Time when the torrent was completed.
    /// </summary>
    [JsonPropertyName("completion_on")]
    [JsonConverter(typeof(UnixTimestampConverter))]
    public DateTimeOffset? CompletionOn { get; set; }

    /// <summary>
    /// 内容保存路径。<br/>
    /// Absolute path of torrent content.
    /// </summary>
    [JsonPropertyName("content_path")]
    public string? ContentPath { get; set; }

    /// <summary>
    /// 下载速度限制（字节/秒）。<br/>
    /// Download speed limit (bytes/s).
    /// </summary>
    [JsonPropertyName("dl_limit")]
    public long? DlLimit { get; set; }

    /// <summary>
    /// 当前下载速度（字节/秒）。<br/>
    /// Current download speed (bytes/s).
    /// </summary>
    [JsonPropertyName("dlspeed")]
    public long? DownloadSpeed { get; set; }

    /// <summary>
    /// 已下载的总字节数。<br/>
    /// Total bytes downloaded.
    /// </summary>
    [JsonPropertyName("downloaded")]
    public long? Downloaded { get; set; }

    /// <summary>
    /// 本次会话下载的字节数。<br/>
    /// Bytes downloaded during the current session.
    /// </summary>
    [JsonPropertyName("downloaded_session")]
    public long? DownloadedSession { get; set; }

    /// <summary>
    /// 预计剩余时间。<br/>
    /// Estimated Time of Arrival.
    /// </summary>
    [JsonPropertyName("eta")]
    [JsonConverter(typeof(SecondsTimeSpanConverter))]
    public TimeSpan? EstimatedTimeArrival { get; set; }

    /// <summary>
    /// 是否启用首尾分块优先下载。<br/>
    /// Whether first and last pieces are prioritized.
    /// </summary>
    [JsonPropertyName("f_l_piece_prio")]
    public bool? FirstLastPiecePriorityEnabled { get; set; }

    /// <summary>
    /// 是否启用强制开始。<br/>
    /// Whether the torrent is force-started.
    /// </summary>
    [JsonPropertyName("force_start")]
    public bool? ForceStartEnabled { get; set; }

    /// <summary>
    /// 种子的 Hash 值。<br/>
    /// Torrent hash.
    /// </summary>
    [JsonPropertyName("hash")]
    public string? Hash { get; set; }

    /// <summary>
    /// 是否为私有种子。<br/>
    /// Whether the torrent is private.
    /// </summary>
    [JsonPropertyName("isPrivate")]
    public bool? PrivateEnabled { get; set; }

    /// <summary>
    /// 最后活跃时间。<br/>
    /// Last activity time.
    /// </summary>
    [JsonPropertyName("last_activity")]
    [JsonConverter(typeof(UnixTimestampConverter))]
    public DateTimeOffset? LastActivity { get; set; }

    /// <summary>
    /// 磁力链接。<br/>
    /// Magnet URI of the torrent.
    /// </summary>
    [JsonPropertyName("magnet_uri")]
    public string? MagnetUri { get; set; }

    /// <summary>
    /// 最大分享率限制。<br/>
    /// Maximum sharing ratio limit.
    /// </summary>
    [JsonPropertyName("max_ratio")]
    public float? MaxRatio { get; set; }

    /// <summary>
    /// 最大做种时间限制。<br/>
    /// Maximum seeding time limit.
    /// </summary>
    [JsonPropertyName("max_seeding_time")]
    [JsonConverter(typeof(MinutesTimeSpanConverter))]
    public TimeSpan? MaxSeedingTime { get; set; }

    /// <summary>
    /// 种子名称。<br/>
    /// Name of the torrent.
    /// </summary>
    [JsonPropertyName("name")]
    public string? Name { get; set; }

    /// <summary>
    /// 完成此种子的用户数（做种人数）。<br/>
    /// Number of seeds in the swarm.
    /// </summary>
    [JsonPropertyName("num_complete")]
    public int? NumComplete { get; set; }

    /// <summary>
    /// 未完成此种子的用户数（下载人数）。<br/>
    /// Number of leeches in the swarm.
    /// </summary>
    [JsonPropertyName("num_incomplete")]
    public int? NumIncomplete { get; set; }

    /// <summary>
    /// 当前连接的下载用户数。<br/>
    /// Number of connected leeches.
    /// </summary>
    [JsonPropertyName("num_leechs")]
    public int? NumLeechs { get; set; }

    /// <summary>
    /// 当前连接的上传用户数（做种者）。<br/>
    /// Number of connected seeds.
    /// </summary>
    [JsonPropertyName("num_seeds")]
    public int? NumSeeds { get; set; }

    /// <summary>
    /// 队列优先级。<br/>
    /// Priority in the queue.
    /// </summary>
    [JsonPropertyName("priority")]
    public int? Priority { get; set; }

    /// <summary>
    /// 下载进度（0-1）。<br/>
    /// Download progress (0 to 1).
    /// </summary>
    [JsonPropertyName("progress")]
    public float? Progress { get; set; }

    /// <summary>
    /// 当前分享率。<br/>
    /// Current sharing ratio.
    /// </summary>
    [JsonPropertyName("ratio")]
    public float? Ratio { get; set; }

    /// <summary>
    /// 分享率限制。<br/>
    /// Sharing ratio limit.
    /// </summary>
    [JsonPropertyName("ratio_limit")]
    public float? RatioLimit { get; set; }

    /// <summary>
    /// 下次汇报（Announce）剩余时间。<br/>
    /// Time until next announce.
    /// </summary>
    [JsonPropertyName("reannounce")]
    [JsonConverter(typeof(SecondsTimeSpanConverter))]
    public TimeSpan? ReannounceTime { get; set; }

    /// <summary>
    /// 保存路径。<br/>
    /// Save path of the torrent.
    /// </summary>
    [JsonPropertyName("save_path")]
    public string? SavePath { get; set; }

    /// <summary>
    /// 累计做种时间。<br/>
    /// Total seeding time.
    /// </summary>
    [JsonPropertyName("seeding_time")]
    [JsonConverter(typeof(SecondsTimeSpanConverter))]
    public TimeSpan? SeedingTime { get; set; }

    /// <summary>
    /// 做种时间限制。<br/>
    /// Seeding time limit.
    /// </summary>
    [JsonPropertyName("seeding_time_limit")]
    [JsonConverter(typeof(MinutesTimeSpanConverter))]
    public TimeSpan? SeedingTimeLimit { get; set; }

    /// <summary>
    /// 最后一次见到完整种子的时间。<br/>
    /// Time when the torrent was last seen complete.
    /// </summary>
    [JsonPropertyName("seen_complete")]
    [JsonConverter(typeof(UnixTimestampConverter))]
    public DateTimeOffset? SeenComplete { get; set; }

    /// <summary>
    /// 是否启用顺序下载。<br/>
    /// Whether sequential download is enabled.
    /// </summary>
    [JsonPropertyName("seq_dl")]
    public bool? SequentialDownloadEnabled { get; set; }

    /// <summary>
    /// 种子数据大小（字节）。<br/>
    /// Size of the torrent (bytes).
    /// </summary>
    [JsonPropertyName("size")]
    public long? Size { get; set; }

    /// <summary>
    /// 种子状态。<br/>
    /// State of the torrent.
    /// </summary>
    [JsonPropertyName("state")]
    [JsonConverter(typeof(TorrentStateConverter))]
    public EnumTorrentState? State { get; set; }

    /// <summary>
    /// 是否启用超级做种模式。<br/>
    /// Whether super seeding is enabled.
    /// </summary>
    [JsonPropertyName("super_seeding")]
    public bool? SuperSeedingEnabled { get; set; }

    /// <summary>
    /// 标签列表。<br/>
    /// List of tags.
    /// </summary>
    [JsonPropertyName("tags")]
    [JsonConverter(typeof(CommaSeparatedListConverter))]
    public List<string>? TagList { get; set; }

    /// <summary>
    /// 累计活跃时间。<br/>
    /// Total active time.
    /// </summary>
    [JsonPropertyName("time_active")]
    [JsonConverter(typeof(SecondsTimeSpanConverter))]
    public TimeSpan? TimeActive { get; set; }

    /// <summary>
    /// 总大小（含额外开销）。<br/>
    /// Total size including overhead.
    /// </summary>
    [JsonPropertyName("total_size")]
    public long? TotalSize { get; set; }

    /// <summary>
    /// 当前的 Tracker 服务器。<br/>
    /// Current tracker of the torrent.
    /// </summary>
    [JsonPropertyName("tracker")]
    public string? Tracker { get; set; }

    /// <summary>
    /// 上传速度限制（字节/秒）。<br/>
    /// Upload speed limit (bytes/s).
    /// </summary>
    [JsonPropertyName("up_limit")]
    public long? UpLimit { get; set; }

    /// <summary>
    /// 累计上传字节数。<br/>
    /// Total bytes uploaded.
    /// </summary>
    [JsonPropertyName("uploaded")]
    public long? Uploaded { get; set; }

    /// <summary>
    /// 本次会话上传的字节数。<br/>
    /// Bytes uploaded during the current session.
    /// </summary>
    [JsonPropertyName("uploaded_session")]
    public long? UploadedSession { get; set; }

    /// <summary>
    /// 当前上传速度（字节/秒）。<br/>
    /// Current upload speed (bytes/s).
    /// </summary>
    [JsonPropertyName("upspeed")]
    public long? UploadSpeed { get; set; }
}
