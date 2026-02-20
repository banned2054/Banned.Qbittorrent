using System.Text.Json.Serialization;

namespace Banned.Qbittorrent.Models.Search;

/// <summary>
/// 表示一个搜索任务。<br/>
/// Represents a search job.
/// </summary>
public class SearchJob
{
    /// <summary>
    /// 搜索任务的唯一 ID。<br/>
    /// The unique ID of the search job.
    /// </summary>
    [JsonPropertyName("id")]
    public int Id { get; set; }
}
