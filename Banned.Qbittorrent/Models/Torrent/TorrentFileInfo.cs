using Banned.Qbittorrent.Models.Enums;
using System.Text.Json.Serialization;

namespace Banned.Qbittorrent.Models.Torrent;

public class TorrentFileInfo
{
    [JsonPropertyName("index")]
    public int? Index { get; set; }

    [JsonPropertyName("name")]
    public string Name { get; set; } = string.Empty;

    [JsonPropertyName("size")]
    public long Size { get; set; }

    [JsonPropertyName("progress")]
    public double Progress { get; set; }

    [JsonPropertyName("priority")]
    public EnumTorrentFilePriority Priority { get; set; }

    [JsonPropertyName("is_seed")]
    public bool IsSeed { get; set; }

    [JsonPropertyName("piece_range")]
    public int[] PieceRange { get; set; } = [];

    [JsonPropertyName("availability")]
    public double? Availability { get; set; }
}
