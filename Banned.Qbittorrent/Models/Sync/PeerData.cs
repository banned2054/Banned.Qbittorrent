using System.Text.Json.Serialization;

namespace Banned.Qbittorrent.Models.Sync;

public class PeerData
{
    [JsonPropertyName("rid")]
    public int Rid { get; set; }

    [JsonPropertyName("full_update")]
    public bool? IsFullUpdate { get; set; }

    [JsonPropertyName("show_flags")]
    public bool? ShowFlags { get; set; }

    [JsonPropertyName("peers")]
    public Dictionary<string, PeerInfo>? Peers { get; set; }

    [JsonPropertyName("peers_removed")]
    public List<string>? PeersRemoved { get; set; }
}
