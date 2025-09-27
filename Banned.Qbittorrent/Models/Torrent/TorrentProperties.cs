using System.Text.Json.Serialization;

namespace Banned.Qbittorrent.Models.Torrent;

public class TorrentProperties
{
    [JsonPropertyName("addition_date")]
    public long AdditionDate { get; set; }

    [JsonPropertyName("comment")]
    public string Comment { get; set; } = string.Empty;

    [JsonPropertyName("completion_date")]
    public long CompletionDate { get; set; }

    [JsonPropertyName("created_by")]
    public string CreatedBy { get; set; } = string.Empty;

    [JsonPropertyName("creation_date")]
    public long CreationDate { get; set; }

    [JsonPropertyName("dl_limit")]
    public long DownloadLimit { get; set; }

    [JsonPropertyName("dl_speed")]
    public long DownloadSpeed { get; set; }

    [JsonPropertyName("dl_speed_avg")]
    public long DownloadSpeedAvg { get; set; }

    [JsonPropertyName("eta")]
    public long Eta { get; set; }

    [JsonPropertyName("isPrivate")]
    public bool IsPrivate { get; set; }

    [JsonPropertyName("last_seen")]
    public long LastSeen { get; set; }

    [JsonPropertyName("nb_connections")]
    public int Connections { get; set; }

    [JsonPropertyName("nb_connections_limit")]
    public int ConnectionsLimit { get; set; }

    [JsonPropertyName("peers")]
    public int Peers { get; set; }

    [JsonPropertyName("peers_total")]
    public int PeersTotal { get; set; }

    [JsonPropertyName("piece_size")]
    public long PieceSize { get; set; }

    [JsonPropertyName("pieces_have")]
    public int PiecesHave { get; set; }

    [JsonPropertyName("pieces_num")]
    public int PiecesNum { get; set; }

    [JsonPropertyName("reannounce")]
    public int ReannounceInterval { get; set; }

    [JsonPropertyName("save_path")]
    public string SavePath { get; set; } = string.Empty;

    [JsonPropertyName("seeding_time")]
    public long SeedingTime { get; set; }

    [JsonPropertyName("seeds")]
    public int Seeds { get; set; }

    [JsonPropertyName("seeds_total")]
    public int SeedsTotal { get; set; }

    [JsonPropertyName("share_ratio")]
    public double ShareRatio { get; set; }

    [JsonPropertyName("time_elapsed")]
    public long TimeElapsed { get; set; }

    [JsonPropertyName("total_downloaded")]
    public long TotalDownloaded { get; set; }

    [JsonPropertyName("total_downloaded_session")]
    public long TotalDownloadedSession { get; set; }

    [JsonPropertyName("total_size")]
    public long TotalSize { get; set; }

    [JsonPropertyName("total_uploaded")]
    public long TotalUploaded { get; set; }

    [JsonPropertyName("total_uploaded_session")]
    public long TotalUploadedSession { get; set; }

    [JsonPropertyName("total_wasted")]
    public long TotalWasted { get; set; }

    [JsonPropertyName("up_limit")]
    public long UploadLimit { get; set; }

    [JsonPropertyName("up_speed")]
    public long UploadSpeed { get; set; }

    [JsonPropertyName("up_speed_avg")]
    public long UploadSpeedAvg { get; set; }
}
