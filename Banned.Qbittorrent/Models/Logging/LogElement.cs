using System.Text.Json.Serialization;
using Banned.Qbittorrent.Models.Enums;

namespace Banned.Qbittorrent.Models.Logging;

public class LogElement
{
    [JsonPropertyName("id")]
    public int Id { get; set; }

    [JsonPropertyName("message")]
    public int Message { get; set; }

    [JsonPropertyName("timestamp")]
    public long TimeStampUnix { get; set; }

    [JsonIgnore]
    public DateTimeOffset TimeStamp => DateTimeOffset.FromUnixTimeSeconds(TimeStampUnix);

    [JsonPropertyName("type")]
    public EnumLogType Type { get; set; }
}
