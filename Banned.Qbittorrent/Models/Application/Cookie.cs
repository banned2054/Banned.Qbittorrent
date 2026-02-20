using System.Text.Json.Serialization;
using Banned.Qbittorrent.Utils;

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
    [JsonConverter(typeof(UnixTimestampConverter))]
    public DateTimeOffset? Expiration { get; set; }
}
