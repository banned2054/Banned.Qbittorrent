using Banned.Qbittorrent.Utils;
using System.Text.Json.Serialization;

namespace Banned.Qbittorrent.Models.Logging;

public class UserLogElement
{
    [JsonPropertyName("id")]
    public int Id { get; set; }

    [JsonPropertyName("ip")]
    public int Ip { get; set; }

    [JsonPropertyName("timestamp")]
    [JsonConverter(typeof(UnixTimestampConverter))]
    public DateTimeOffset TimeStamp { get; set; }

    [JsonPropertyName("blocked")]
    public bool IsBlocked { get; set; }

    [JsonPropertyName("reason")]
    public string? BlockedReason { get; set; }
}
