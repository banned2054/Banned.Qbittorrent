using System.Text.Json.Serialization;

namespace Banned.Qbittorrent.Models.Sync;

public class PeerInfo
{
    [JsonPropertyName("client")]
    public string? Client { get; set; }

    [JsonPropertyName("connection")]
    public string? Connection { get; set; }

    [JsonPropertyName("country")]
    public string? Country { get; set; }

    [JsonPropertyName("country_code")]
    public string? CountryCode { get; set; }

    [JsonPropertyName("dl_speed")]
    public long? DownloadSpeed { get; set; }

    [JsonPropertyName("up_speed")]
    public long? UploadSpeed { get; set; }

    [JsonPropertyName("progress")]
    public double? Progress { get; set; }

    [JsonPropertyName("flags")]
    public string? Flags { get; set; }

    [JsonPropertyName("flags_desc")]
    public string? FlagsDescription { get; set; }

    [JsonPropertyName("relevance")]
    public double? Relevance { get; set; }

    [JsonPropertyName("files")]
    public string? Files { get; set; }
}
