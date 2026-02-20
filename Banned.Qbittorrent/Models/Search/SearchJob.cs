using System.Text.Json.Serialization;

namespace Banned.Qbittorrent.Models.Search;

public class SearchJob
{
    [JsonPropertyName("id")]
    public int Id { get; set; }
}
