using System.Text.Json.Serialization;

namespace Banned.Qbittorrent.Models.Search;

/// <summary>
/// 表示搜索任务的状态信息。<br/>
/// Represents the status information of a search job.
/// </summary>
public class SearchStatus
{
    /// <summary>
    /// 搜索任务的唯一 ID。<br/>
    /// The unique ID of the search job.
    /// </summary>
    [JsonPropertyName("id")]
    public int Id { get; set; }

    /// <summary>
    /// 当前搜索任务的状态字符串。<br/>
    /// The status string of the current search job.
    /// </summary>
    [JsonPropertyName("status")]
    public string Status { get; set; } = null!;

    /// <summary>
    /// 目前已搜索到的结果总数。<br/>
    /// The total number of results found so far.
    /// </summary>
    [JsonPropertyName("total")]
    public int Count { get; set; }
}
