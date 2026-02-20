using Banned.Qbittorrent.Models.Enums;
using Banned.Qbittorrent.Utils;

namespace Banned.Qbittorrent.Models.Requests;

/// <summary>
/// 获取种子信息列表的请求参数模型。<br/>
/// Request model for getting the torrent information list.
/// </summary>
public class GetTorrentInfoListRequest
{
    /// <summary>
    /// 种子过滤器。<br/>
    /// Torrent filter.
    /// </summary>
    public EnumTorrentFilter Filter { get; set; }

    /// <summary>
    /// 过滤特定类别的种子。<br/>
    /// Filter torrents by a specific category.
    /// </summary>
    public string? Category { get; set; }

    /// <summary>
    /// 过滤特定标签的种子。<br/>
    /// Filter torrents by a specific tag.
    /// </summary>
    public string? Tag { get; set; }

    /// <summary>
    /// 排序字段（例如：name, size, priority 等）。<br/>
    /// Field to sort by (e.g., name, size, priority).
    /// </summary>
    public string? Sort { get; set; }

    /// <summary>
    /// 是否启用反向排序。<br/>
    /// Whether to enable reverse sorting.
    /// </summary>
    public bool ReverseEnabled { get; set; }

    /// <summary>
    /// 限制返回的结果数量。<br/>
    /// Limit the number of results returned.
    /// </summary>
    public int Limit { get; set; }

    /// <summary>
    /// 结果偏移量（用于分页）。<br/>
    /// Result offset (used for pagination).
    /// </summary>
    public int Offset { get; set; }

    /// <summary>
    /// 种子 Hash 列表，用于获取特定种子的信息。<br/>
    /// List of torrent hashes to get info for specific torrents.
    /// </summary>
    public List<string>? HashList { get; set; } = [];

    /// <summary>
    /// 将请求参数转换为查询字符串。<br/>
    /// Converts request parameters to a query string.
    /// </summary>
    /// <returns>查询字符串。 / The query string.</returns>
    public override string ToString()
    {
        var parameters = new List<string>();

        if (Filter != EnumTorrentFilter.All)
            parameters.Add($"filter={Uri.EscapeDataString(Filter.TorrentFilter2String())}");
        if (!string.IsNullOrEmpty(Category)) parameters.Add($"category={Uri.EscapeDataString(Category)}");
        if (!string.IsNullOrEmpty(Tag)) parameters.Add($"tag={Uri.EscapeDataString(Tag)}");
        if (!string.IsNullOrEmpty(Sort)) parameters.Add($"sort={Uri.EscapeDataString(Sort)}");
        if (ReverseEnabled) parameters.Add($"reverse={ReverseEnabled.ToString().ToLower()}");
        if (Limit  > 0) parameters.Add($"limit={Limit}");
        if (Offset > 0) parameters.Add($"offset={Offset}");
        if (HashList == null || !HashList.Any()) return string.Join("&", parameters);
        var hashes = string.Join("|", HashList.Select(Uri.EscapeDataString));
        parameters.Add($"hashes={hashes}");
        return string.Join("&", parameters);
    }
}
