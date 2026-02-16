using System.Text.Json.Serialization;

namespace Banned.Qbittorrent.Models.Application;

public class Cookie
{
    [JsonPropertyName("name")]
    public string Name { get; set; }

    [JsonPropertyName("domain")]
    public string Domain { get; set; }

    [JsonPropertyName("path")]
    public string Path { get; set; }

    [JsonPropertyName("value")]
    public string Value { get; set; }

    [JsonPropertyName("expirationDate")]
    public long ExpirationDateUnix { get; set; }

    [JsonIgnore]
    public DateTimeOffset? Expiration
    {
        get => ExpirationDateUnix <= 0 ? null : DateTimeOffset.FromUnixTimeSeconds(ExpirationDateUnix);
        set => ExpirationDateUnix = (long)(value?.ToUnixTimeSeconds() ?? 0);
    }
}
