using System.Text.Json.Serialization;

namespace Banned.Qbittorrent.Models.Search;

public class SearchCategory
{
    [JsonPropertyName("id")]
    public string? Id { get; init; }

    [JsonPropertyName("name")]
    public string? Name { get; init; }
}
