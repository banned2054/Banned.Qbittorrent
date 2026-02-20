using Banned.Qbittorrent.Models.Enums;
using Banned.Qbittorrent.Utils;
using System.Text.Json.Serialization;

namespace Banned.Qbittorrent.Models.Logging;

/// <summary>
/// 表示一条日志条目。<br/>
/// Represents a log entry.
/// </summary>
public class LogElement
{
    /// <summary>
    /// 日志条目的唯一 ID。<br/>
    /// The unique ID of the log entry.
    /// </summary>
    [JsonPropertyName("id")]
    public int Id { get; set; }

    /// <summary>
    /// 日志消息内容。<br/>
    /// The content of the log message.
    /// </summary>
    [JsonPropertyName("message")]
    public string Message { get; set; } = string.Empty;

    /// <summary>
    /// 日志发生的时间戳。<br/>
    /// The timestamp of the log entry.
    /// </summary>
    [JsonPropertyName("timestamp")]
    [JsonConverter(typeof(UnixTimestampConverter))]
    public DateTimeOffset TimeStamp { get; set; }

    /// <summary>
    /// 日志的类型（如：普通、警告、错误等）。<br/>
    /// The type of the log entry (e.g., Normal, Warning, Critical).
    /// </summary>
    [JsonPropertyName("type")]
    public EnumLogType Type { get; set; }
}
