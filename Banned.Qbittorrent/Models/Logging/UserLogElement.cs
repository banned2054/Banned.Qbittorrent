using System.Text.Json.Serialization;

namespace Banned.Qbittorrent.Models.Logging;

public class UserLogElement
{
    [JsonPropertyName("id")]
    public int Id { get; set; }

    [JsonPropertyName("ip")]
    public int Ip { get; set; }

    [JsonPropertyName("timestamp")]
    public long TimeStampUnix { get; set; }

    [JsonIgnore]
    public DateTimeOffset TimeStamp => DateTimeOffset.FromUnixTimeSeconds(TimeStampUnix);

    [JsonPropertyName("blocked")]
    public bool IsBlocked { get; set; }

    [JsonPropertyName("reason")]
    public string? BlockedReason { get; set; }
}
