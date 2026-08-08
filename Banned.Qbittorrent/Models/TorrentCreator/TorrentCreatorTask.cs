using System.Text.Json;
using System.Text.Json.Serialization;

namespace Banned.Qbittorrent.Models.TorrentCreator;

/// <summary>
/// 表示已提交的种子创建任务。<br/>
/// Represents a submitted torrent creation task.
/// </summary>
public class TorrentCreatorTask
{
    /// <summary>任务 ID。<br/>Task ID.</summary>
    [JsonPropertyName("taskID")]
    public string? TaskId { get; set; }

    /// <summary>官方文档尚未定义的附加字段。<br/>Additional fields not yet defined by the official documentation.</summary>
    [JsonExtensionData]
    public Dictionary<string, JsonElement>? AdditionalData { get; set; }
}
