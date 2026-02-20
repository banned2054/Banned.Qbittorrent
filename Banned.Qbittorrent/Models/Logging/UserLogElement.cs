using Banned.Qbittorrent.Utils;
using System.Text.Json.Serialization;

namespace Banned.Qbittorrent.Models.Logging;

/// <summary>
/// 表示用户日志条目（通常与封锁信息相关）。<br/>
/// Represents a user log entry (typically related to blocking information).
/// </summary>
public class UserLogElement
{
    /// <summary>
    /// 日志条目的唯一 ID。<br/>
    /// The unique ID of the log entry.
    /// </summary>
    [JsonPropertyName("id")]
    public int Id { get; set; }

    /// <summary>
    /// 相关的 IP 地址。<br/>
    /// The associated IP address.
    /// </summary>
    [JsonPropertyName("ip")]
    public string Ip { get; set; } = string.Empty;

    /// <summary>
    /// 日志发生的时间戳。<br/>
    /// The timestamp of the log entry.
    /// </summary>
    [JsonPropertyName("timestamp")]
    [JsonConverter(typeof(UnixTimestampConverter))]
    public DateTimeOffset TimeStamp { get; set; }

    /// <summary>
    /// 是否已被封锁。<br/>
    /// Whether the user has been blocked.
    /// </summary>
    [JsonPropertyName("blocked")]
    public bool IsBlocked { get; set; }

    /// <summary>
    /// 封锁的原因。<br/>
    /// The reason for the block.
    /// </summary>
    [JsonPropertyName("reason")]
    public string? BlockedReason { get; set; }
}
