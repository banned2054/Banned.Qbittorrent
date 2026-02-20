using System.Text.Json.Serialization;

namespace Banned.Qbittorrent.Models.Search;

public class SearchResult
{
    [JsonPropertyName("results")]
    public ResultEntity[]? Results { get; set; }

    [JsonPropertyName("status")]
    public SearchStatus? Status { get; set; }

    [JsonPropertyName("total")]
    public int? Total { get; set; }
}
