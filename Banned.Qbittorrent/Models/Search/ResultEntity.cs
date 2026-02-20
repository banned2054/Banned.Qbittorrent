using System.Text.Json.Serialization;

namespace Banned.Qbittorrent.Models.Search;

public class ResultEntity
{
    [JsonPropertyName("descrLink")]
    public string? DescriptionLink { get; set; }

    [JsonPropertyName("fileName")]
    public string? FileName { get; set; }

    [JsonPropertyName("fileSize")]
    public long? FileSize { get; set; }

    [JsonPropertyName("fileUrl")]
    public string? FileUrl { get; set; }

    [JsonPropertyName("nbLeechers")]
    public int? NumberOfLeechers { get; set; }

    [JsonPropertyName("nbSeeders")]
    public int? NumberOfSeeders { get; set; }

    [JsonPropertyName("siteUrl")]
    public string? SiteUrl { get; set; }
}
