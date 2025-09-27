using Banned.Qbittorrent.Models.Enums;
using System.Text.Json.Serialization;

namespace Banned.Qbittorrent.Models.Torrent;

public class TrackerInfo
{
    [JsonPropertyName("url")]
    public string Url { get; set; } = string.Empty;

    [JsonPropertyName("status")]
    public EnumTrackerStatus Status { get; set; }

    [JsonPropertyName("tier")]
    public int Tier { get; set; }

    [JsonPropertyName("num_peers")]
    public int NumPeers { get; set; }

    [JsonPropertyName("num_seeds")]
    public int NumSeeds { get; set; }

    [JsonPropertyName("num_leeches")]
    public int NumLeeches { get; set; }

    [JsonPropertyName("num_downloaded")]
    public int NumDownloaded { get; set; }

    [JsonPropertyName("msg")]
    public string Message { get; set; } = string.Empty;
}
