using System.Text.Json.Serialization;

namespace Banned.Qbittorrent.Models.Torrent;

/// <summary>
/// 表示种子的 Web 种子（HTTP 源）信息。<br/>
/// Represents information about a torrent's web seed (HTTP source).
/// </summary>
public class TorrentWebSeed
{
    /// <summary>
    /// Web 种子的 URL 地址。<br/>
    /// The URL of the web seed.
    /// </summary>
    [JsonPropertyName("url")]
    public string Url { get; set; } = null!;
}
