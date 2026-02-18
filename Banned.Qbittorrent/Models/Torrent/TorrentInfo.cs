using Banned.Qbittorrent.Models.Enums;
using System.Text.Json.Serialization;

namespace Banned.Qbittorrent.Models.Torrent;

/// <summary>
/// 表示种子的详细信息。<br/>
/// Represents detailed information of a torrent.
/// </summary>
public class TorrentInfo
{
    /// <summary>添加时间（Unix 时间戳）。 / Time when the torrent was added (Unix timestamp).</summary>
    [JsonPropertyName("added_on")]
    public long? AddedOnUnix { get; set; }

    /// <summary>添加时间。 / Time when the torrent was added.</summary>
    [JsonIgnore]
    public DateTimeOffset? AddedOn =>
        AddedOnUnix == null ? null : DateTimeOffset.FromUnixTimeSeconds(AddedOnUnix.Value);

    /// <summary>剩余需要下载的字节数。 / Remaining bytes to download.</summary>
    [JsonPropertyName("amount_left")]
    public long? AmountLeft { get; set; }

    /// <summary>是否启用了自动种子管理。 / Whether Automatic Torrent Management is enabled.</summary>
    [JsonPropertyName("auto_tmm")]
    public bool? AutoTmm { get; set; }

    /// <summary>可用性百分比。 / Percentage of file segments available.</summary>
    [JsonPropertyName("availability")]
    public float? Availability { get; set; }

    /// <summary>所属分类。 / Category of the torrent.</summary>
    [JsonPropertyName("category")]
    public string? Category { get; set; }

    /// <summary>已完成的字节数。 / Total bytes completed.</summary>
    [JsonPropertyName("completed")]
    public long? Completed { get; set; }

    /// <summary>完成时间（Unix 时间戳）。 / Time when the torrent was completed (Unix timestamp).</summary>
    [JsonPropertyName("completion_on")]
    public long? CompletionOnUnix { get; set; }

    /// <summary>完成时间。 / Time when the torrent was completed.</summary>
    [JsonIgnore]
    public DateTimeOffset? CompletionOn =>
        CompletionOnUnix == null ? null : DateTimeOffset.FromUnixTimeSeconds(CompletionOnUnix.Value);

    /// <summary>内容保存路径。 / Absolute path of torrent content.</summary>
    [JsonPropertyName("content_path")]
    public string? ContentPath { get; set; }

    /// <summary>下载速度限制（字节/秒）。 / Download speed limit (bytes/s).</summary>
    [JsonPropertyName("dl_limit")]
    public long? DlLimit { get; set; }

    /// <summary>当前下载速度（字节/秒）。 / Current download speed (bytes/s).</summary>
    [JsonPropertyName("dlspeed")]
    public long? DownloadSpeed { get; set; }

    /// <summary>已下载的总字节数。 / Total bytes downloaded.</summary>
    [JsonPropertyName("downloaded")]
    public long? Downloaded { get; set; }

    /// <summary>本次会话下载的字节数。 / Bytes downloaded during the current session.</summary>
    [JsonPropertyName("downloaded_session")]
    public long? DownloadedSession { get; set; }

    /// <summary>预计剩余时间（秒）。 / Estimated Time of Arrival (seconds).</summary>
    [JsonPropertyName("eta")]
    public int? EtaSeconds { get; set; }

    /// <summary>预计剩余时间。 / Estimated Time of Arrival.</summary>
    [JsonIgnore]
    public TimeSpan? Eta => EtaSeconds == null ? null : TimeSpan.FromSeconds(EtaSeconds.Value);

    /// <summary>是否优先下载第一块和最后一块。 / Whether first and last pieces are prioritized.</summary>
    [JsonPropertyName("f_l_piece_prio")]
    public bool? FirstLastPiecePriority { get; set; }

    /// <summary>是否强制开始。 / Whether the torrent is force-started.</summary>
    [JsonPropertyName("force_start")]
    public bool? ForceStart { get; set; }

    /// <summary>种子的 Hash 值。 / Torrent hash.</summary>
    [JsonPropertyName("hash")]
    public string? Hash { get; set; }

    /// <summary>是否为私有种子。 / Whether the torrent is private.</summary>
    [JsonPropertyName("isPrivate")]
    public bool? IsPrivate { get; set; }

    /// <summary>最后活跃时间（Unix 时间戳）。 / Last activity time (Unix timestamp).</summary>
    [JsonPropertyName("last_activity")]
    public long? LastActivityUnix { get; set; }

    /// <summary>最后活跃时间。 / Last activity time.</summary>
    [JsonIgnore]
    public DateTimeOffset? LastActivity =>
        LastActivityUnix == null ? null : DateTimeOffset.FromUnixTimeSeconds(LastActivityUnix.Value);

    /// <summary>磁力链接。 / Magnet URI of the torrent.</summary>
    [JsonPropertyName("magnet_uri")]
    public string? MagnetUri { get; set; }

    /// <summary>最大分享率限制。 / Maximum sharing ratio limit.</summary>
    [JsonPropertyName("max_ratio")]
    public float? MaxRatio { get; set; }

    /// <summary>最大做种时间限制（分钟）。 / Maximum seeding time limit (minutes).</summary>
    [JsonPropertyName("max_seeding_time")]
    public int? MaxSeedingTimeSeconds { get; set; }

    /// <summary>最大做种时间限制。 / Maximum seeding time limit.</summary>
    [JsonIgnore]
    public TimeSpan? MaxSeedingTime =>
        MaxSeedingTimeSeconds == null ? null : TimeSpan.FromMinutes(MaxSeedingTimeSeconds.Value);

    /// <summary>种子名称。 / Name of the torrent.</summary>
    [JsonPropertyName("name")]
    public string? Name { get; set; }

    /// <summary>完成此种子的用户数。 / Number of seeds in the swarm.</summary>
    [JsonPropertyName("num_complete")]
    public int? NumComplete { get; set; }

    /// <summary>未完成此种子的用户数。 / Number of leeches in the swarm.</summary>
    [JsonPropertyName("num_incomplete")]
    public int? NumIncomplete { get; set; }

    /// <summary>当前连接的下载用户数。 / Number of connected leeches.</summary>
    [JsonPropertyName("num_leechs")]
    public int? NumLeechs { get; set; }

    /// <summary>当前连接的上传用户数（做种者）。 / Number of connected seeds.</summary>
    [JsonPropertyName("num_seeds")]
    public int? NumSeeds { get; set; }

    /// <summary>队列优先级。 / Priority in the queue.</summary>
    [JsonPropertyName("priority")]
    public int? Priority { get; set; }

    /// <summary>下载进度（0-1）。 / Download progress (0 to 1).</summary>
    [JsonPropertyName("progress")]
    public float? Progress { get; set; }

    /// <summary>当前分享率。 / Current sharing ratio.</summary>
    [JsonPropertyName("ratio")]
    public float? Ratio { get; set; }

    /// <summary>分享率限制。 / Sharing ratio limit.</summary>
    [JsonPropertyName("ratio_limit")]
    public float? RatioLimit { get; set; }

    /// <summary>下次汇报（Announce）剩余时间（秒）。 / Time until next announce (seconds).</summary>
    [JsonPropertyName("reannounce")]
    public int? ReannounceSeconds { get; set; }

    /// <summary>下次汇报剩余时间。 / Time until next announce.</summary>
    [JsonIgnore]
    public TimeSpan? ReannounceTime =>
        ReannounceSeconds == null ? null : TimeSpan.FromSeconds(ReannounceSeconds.Value);

    /// <summary>保存路径。 / Save path of the torrent.</summary>
    [JsonPropertyName("save_path")]
    public string? SavePath { get; set; }

    /// <summary>累计做种时间（秒）。 / Total seeding time (seconds).</summary>
    [JsonPropertyName("seeding_time")]
    public int? SeedingTimeSeconds { get; set; }

    /// <summary>累计做种时间。 / Total seeding time.</summary>
    [JsonIgnore]
    public TimeSpan? SeedingTime => SeedingTimeSeconds == null ? null : TimeSpan.FromSeconds(SeedingTimeSeconds.Value);

    /// <summary>做种时间限制（分钟）。 / Seeding time limit (minutes).</summary>
    [JsonPropertyName("seeding_time_limit")]
    public int? SeedingTimeLimitSeconds { get; set; }

    /// <summary>做种时间限制。 / Seeding time limit.</summary>
    [JsonIgnore]
    public TimeSpan? SeedingTimeLimit =>
        SeedingTimeLimitSeconds == null ? null : TimeSpan.FromMinutes(SeedingTimeLimitSeconds.Value);

    /// <summary>最后一次见到完整种子的时间（Unix 时间戳）。 / Time when the torrent was last seen complete (Unix timestamp).</summary>
    [JsonPropertyName("seen_complete")]
    public long? SeenCompleteUnix { get; set; }

    /// <summary>最后一次见到完整种子的时间。 / Time when the torrent was last seen complete.</summary>
    [JsonIgnore]
    public DateTimeOffset? SeenComplete =>
        SeenCompleteUnix == null ? null : DateTimeOffset.FromUnixTimeSeconds(SeenCompleteUnix.Value);

    /// <summary>是否开启顺序下载。 / Whether sequential download is enabled.</summary>
    [JsonPropertyName("seq_dl")]
    public bool? SeqDl { get; set; }

    /// <summary>种子数据大小（字节）。 / Size of the torrent (bytes).</summary>
    [JsonPropertyName("size")]
    public long? Size { get; set; }

    /// <summary>种子状态（原始字符串）。 / Raw state string of the torrent.</summary>
    [JsonPropertyName("state")]
    public string? StateStr { get; set; }

    /// <summary>种子状态枚举。 / Torrent state enum.</summary>
    [JsonIgnore]
    public EnumTorrentState? State => string.IsNullOrWhiteSpace(StateStr)
        ? null
        : EnumTorrentStateExtensions.FromTorrentStateString(StateStr);

    /// <summary>是否开启超级做种模式。 / Whether super seeding is enabled.</summary>
    [JsonPropertyName("super_seeding")]
    public bool? SuperSeeding { get; set; }

    /// <summary>标签列表（原始逗号分隔字符串）。 / Raw comma-separated tags string.</summary>
    [JsonPropertyName("tags")]
    public string? TagListStr { get; set; }

    /// <summary>解析后的标签列表。 / Parsed list of tags.</summary>
    [JsonIgnore]
    public List<string> TagList =>
        string.IsNullOrWhiteSpace(TagListStr)
            ? []
            : TagListStr.Split(",", StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries).ToList();

    /// <summary>累计活跃时间（秒）。 / Total active time (seconds).</summary>
    [JsonPropertyName("time_active")]
    public int? TimeActiveSeconds { get; set; }

    /// <summary>累计活跃时间。 / Total active time.</summary>
    [JsonIgnore]
    public TimeSpan? TimeActive => TimeActiveSeconds == null ? null : TimeSpan.FromSeconds(TimeActiveSeconds.Value);

    /// <summary>总大小（含额外开销）。 / Total size including overhead.</summary>
    [JsonPropertyName("total_size")]
    public long? TotalSize { get; set; }

    /// <summary>当前的 Tracker 服务器。 / Current tracker of the torrent.</summary>
    [JsonPropertyName("tracker")]
    public string? Tracker { get; set; }

    /// <summary>上传速度限制（字节/秒）。 / Upload speed limit (bytes/s).</summary>
    [JsonPropertyName("up_limit")]
    public long? UpLimit { get; set; }

    /// <summary>累计上传字节数。 / Total bytes uploaded.</summary>
    [JsonPropertyName("uploaded")]
    public long? Uploaded { get; set; }

    /// <summary>本次会话上传的字节数。 / Bytes uploaded during the current session.</summary>
    [JsonPropertyName("uploaded_session")]
    public long? UploadedSession { get; set; }

    /// <summary>当前上传速度（字节/秒）。 / Current upload speed (bytes/s).</summary>
    [JsonPropertyName("upspeed")]
    public long? UploadSpeed { get; set; }
}
