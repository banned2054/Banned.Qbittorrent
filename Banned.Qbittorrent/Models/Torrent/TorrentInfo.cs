using Banned.Qbittorrent.Models.Enums;
using System.Text.Json.Serialization;

namespace Banned.Qbittorrent.Models.Torrent;

public class TorrentInfo
{
    [JsonPropertyName("added_on")]
    public long AddedOnUnix { get; set; }

    [JsonIgnore]
    public DateTimeOffset AddedOn => DateTimeOffset.FromUnixTimeSeconds(AddedOnUnix);

    [JsonPropertyName("amount_left")]
    public long AmountLeft { get; set; }

    [JsonPropertyName("auto_tmm")]
    public bool AutoTmm { get; set; }

    [JsonPropertyName("availability")]
    public float Availability { get; set; }

    [JsonPropertyName("category")]
    public string? Category { get; set; }

    [JsonPropertyName("completed")]
    public long Completed { get; set; }

    [JsonPropertyName("completion_on")]
    public long CompletionOnUnix { get; set; }

    [JsonIgnore]
    public DateTimeOffset CompletionOn => DateTimeOffset.FromUnixTimeSeconds(CompletionOnUnix);

    [JsonPropertyName("content_path")]
    public string? ContentPath { get; set; }

    [JsonPropertyName("dl_limit")]
    public long DlLimit { get; set; }

    [JsonPropertyName("dlspeed")]
    public long DownloadSpeed { get; set; }

    [JsonPropertyName("downloaded")]
    public long Downloaded { get; set; }

    [JsonPropertyName("downloaded_session")]
    public long DownloadedSession { get; set; }

    [JsonPropertyName("eta")]
    public int EtaSeconds { get; set; }

    [JsonIgnore]
    public TimeSpan Eta => TimeSpan.FromSeconds(EtaSeconds);

    [JsonPropertyName("f_l_piece_prio")]
    public bool FirstLastPiecePriority { get; set; }

    [JsonPropertyName("force_start")]
    public bool ForceStart { get; set; }

    [JsonPropertyName("hash")]
    public string? Hash { get; set; }

    [JsonPropertyName("isPrivate")]
    public bool? IsPrivate { get; set; }

    [JsonPropertyName("last_activity")]
    public long LastActivityUnix { get; set; }

    [JsonIgnore]
    public DateTimeOffset LastActivity => DateTimeOffset.FromUnixTimeSeconds(LastActivityUnix);

    [JsonPropertyName("magnet_uri")]
    public string? MagnetUri { get; set; }

    [JsonPropertyName("max_ratio")]
    public float MaxRatio { get; set; }

    [JsonPropertyName("max_seeding_time")]
    public int MaxSeedingTimeSeconds { get; set; }

    [JsonIgnore]
    public TimeSpan MaxSeedingTime => TimeSpan.FromSeconds(MaxSeedingTimeSeconds);

    [JsonPropertyName("name")]
    public string? Name { get; set; }

    [JsonPropertyName("num_complete")]
    public int NumComplete { get; set; }

    [JsonPropertyName("num_incomplete")]
    public int NumIncomplete { get; set; }

    [JsonPropertyName("num_leechs")]
    public int NumLeechs { get; set; }

    [JsonPropertyName("num_seeds")]
    public int NumSeeds { get; set; }

    [JsonPropertyName("priority")]
    public int Priority { get; set; }

    [JsonPropertyName("progress")]
    public float Progress { get; set; }

    [JsonPropertyName("ratio")]
    public float Ratio { get; set; }

    [JsonPropertyName("ratio_limit")]
    public float RatioLimit { get; set; }

    [JsonPropertyName("reannounce")]
    public int? ReannounceSeconds { get; set; }

    [JsonIgnore]
    public TimeSpan? ReannounceTime =>
        ReannounceSeconds == null ? null : TimeSpan.FromSeconds(ReannounceSeconds.Value);

    [JsonPropertyName("save_path")]
    public string? SavePath { get; set; }

    [JsonPropertyName("seeding_time")]
    public int SeedingTimeSeconds { get; set; }

    [JsonIgnore]
    public TimeSpan SeedingTime => TimeSpan.FromSeconds(SeedingTimeSeconds);

    [JsonPropertyName("seeding_time_limit")]
    public int SeedingTimeLimitSeconds { get; set; }

    [JsonIgnore]
    public TimeSpan SeedingTimeLimit => TimeSpan.FromSeconds(SeedingTimeLimitSeconds);

    [JsonPropertyName("seen_complete")]
    public long SeenCompleteUnix { get; set; }

    [JsonIgnore]
    public DateTimeOffset SeenComplete => DateTimeOffset.FromUnixTimeSeconds(SeenCompleteUnix);

    [JsonPropertyName("seq_dl")]
    public bool SeqDl { get; set; }

    [JsonPropertyName("size")]
    public long Size { get; set; }

    [JsonPropertyName("state")]
    public string StateStr { get; set; } = null!;

    [JsonIgnore]
    public EnumTorrentState State => EnumTorrentStateExtensions.FromTorrentStateString(StateStr);

    [JsonPropertyName("super_seeding")]
    public bool SuperSeeding { get; set; }

    [JsonPropertyName("tags")]
    public string? TagListStr { get; set; }

    [JsonIgnore]
    public List<string> TagList =>
        string.IsNullOrWhiteSpace(TagListStr)
            ? []
            : TagListStr.Split(",", StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries).ToList();

    [JsonPropertyName("time_active")]

    public int TimeActiveSeconds { get; set; }

    [JsonIgnore]
    public TimeSpan TimeActive => TimeSpan.FromSeconds(TimeActiveSeconds);

    [JsonPropertyName("total_size")]
    public long TotalSize { get; set; }

    [JsonPropertyName("tracker")]
    public string? Tracker { get; set; }

    [JsonPropertyName("up_limit")]
    public long UpLimit { get; set; }

    [JsonPropertyName("uploaded")]
    public long Uploaded { get; set; }

    [JsonPropertyName("uploaded_session")]
    public long UploadedSession { get; set; }

    [JsonPropertyName("upspeed")]
    public long UploadSpeed { get; set; }
}
