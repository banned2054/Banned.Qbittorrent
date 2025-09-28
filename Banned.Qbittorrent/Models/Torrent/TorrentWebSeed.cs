using System.Text.Json.Serialization;

namespace Banned.Qbittorrent.Models.Torrent;

public class TorrentWebSeed
{
    [JsonPropertyName("url")]
    public string Url { get; set; } = null!;
}
