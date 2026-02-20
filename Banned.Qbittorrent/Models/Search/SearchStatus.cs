using System.Text.Json.Serialization;

namespace Banned.Qbittorrent.Models.Search;

public class SearchStatus
{
    [JsonPropertyName("id")]
    public int Id { get; set; }

    [JsonPropertyName("status")]
    public string Status { get; set; } = null!;

    [JsonPropertyName("total")]
    public int TotalResults { get; set; }
}
