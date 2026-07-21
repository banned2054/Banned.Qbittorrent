using Banned.Qbittorrent.Serialization;
using System.Text.Json.Serialization;

namespace Banned.Qbittorrent.Models.Application;

/// <summary>
/// 表示 qBittorrent 进程信息。<br/>
/// Represents qBittorrent process information.
/// </summary>
public sealed class ProcessInfo
{
    /// <summary>
    /// qBittorrent 进程启动时间，Unix 时间戳，单位秒。<br/>
    /// The qBittorrent process launch time as a Unix timestamp in seconds.
    /// </summary>
    [JsonPropertyName("launch_time")]
    [JsonConverter(typeof(UnixTimestampConverter))]
    public DateTimeOffset? LaunchTime { get; init; }
}
