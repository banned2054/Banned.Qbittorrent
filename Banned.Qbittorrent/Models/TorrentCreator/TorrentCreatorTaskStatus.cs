using Banned.Qbittorrent.Models.Enums;
using Banned.Qbittorrent.Serialization;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Banned.Qbittorrent.Models.TorrentCreator;

/// <summary>
/// 表示种子创建任务的当前状态。<br/>
/// Represents the current state of a torrent creation task.
/// </summary>
public class TorrentCreatorTaskStatus
{
    /// <summary>任务 ID。<br/>Task ID.</summary>
    [JsonPropertyName("taskID")]
    public string? TaskId { get; set; }

    /// <summary>任务状态。<br/>Task status.</summary>
    [JsonPropertyName("status")]
    [JsonConverter(typeof(TorrentCreatorTaskStatusConverter))]
    public EnumTorrentCreatorTaskStatus Status { get; set; }

    /// <summary>任务进度。<br/>Task progress.</summary>
    [JsonPropertyName("progress")]
    public double? Progress { get; set; }

    /// <summary>官方文档尚未定义的附加字段。<br/>Additional fields not yet defined by the official documentation.</summary>
    [JsonExtensionData]
    public Dictionary<string, JsonElement>? AdditionalData { get; set; }
}
