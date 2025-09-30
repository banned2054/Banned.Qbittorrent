using System.Text.Json.Serialization;

namespace Banned.Qbittorrent.Models.Torrent;

public class TorrentCategory
{
    [JsonPropertyName("name")]
    public string Name { get; set; }

    [JsonPropertyName("savePath")]
    public string SavePath { get; set; }
}
