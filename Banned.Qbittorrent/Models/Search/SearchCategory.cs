using System.Text.Json.Serialization;

namespace Banned.Qbittorrent.Models.Search;

/// <summary>
/// 表示搜索类别。<br/>
/// Represents a search category.
/// </summary>
public class SearchCategory
{
    /// <summary>
    /// 搜索类别的唯一标识符。<br/>
    /// The unique identifier of the search category.
    /// </summary>
    [JsonPropertyName("id")]
    public string? Id { get; init; }

    /// <summary>
    /// 搜索类别的显示名称。<br/>
    /// The display name of the search category.
    /// </summary>
    [JsonPropertyName("name")]
    public string? Name { get; init; }
}
