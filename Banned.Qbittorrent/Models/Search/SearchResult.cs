using System.Text.Json.Serialization;

namespace Banned.Qbittorrent.Models.Search;

/// <summary>
/// 表示搜索任务的结果汇总。<br/>
/// Represents the summary of search job results.
/// </summary>
public class SearchResult
{
    /// <summary>
    /// 符合搜索条件的实体列表。<br/>
    /// List of entities matching the search criteria.
    /// </summary>
    [JsonPropertyName("results")]
    public ResultEntity[]? Results { get; set; }

    /// <summary>
    /// 当前搜索任务的状态。<br/>
    /// The status of the current search job.
    /// </summary>
    [JsonPropertyName("status")]
    public SearchStatus? Status { get; set; }

    /// <summary>
    /// 搜索到的结果总数。<br/>
    /// The total number of search results found.
    /// </summary>
    [JsonPropertyName("total")]
    public int? Count { get; set; }
}
