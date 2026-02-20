using System.Text.Json.Serialization;

namespace Banned.Qbittorrent.Models.Search;

public class SearchPlugins
{
    [JsonPropertyName("enabled")]
    public bool? Enabled { get; set; }

    [JsonPropertyName("fullName")]
    public string? FullName { get; set; }

    [JsonPropertyName("name")]
    public string? Name { get; set; }

    [JsonPropertyName("supportedCategories")]
    public SearchCategory[]? SupportedCategories { get; set; }

    [JsonPropertyName("url")]
    public string? Url { get; set; }

    [JsonPropertyName("version")]
    public string? Version { get; set; }
}
