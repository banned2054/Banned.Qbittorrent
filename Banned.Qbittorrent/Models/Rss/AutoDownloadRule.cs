using Banned.Qbittorrent.Models.Enums;
using Banned.Qbittorrent.Utils;
using System.Text.Json.Serialization;

namespace Banned.Qbittorrent.Models.Rss;

/// <summary>
/// 表示 RSS 自动下载规则。<br/>
/// Represents an RSS auto-download rule.
/// </summary>
public class AutoDownloadRule
{
    /// <summary>
    /// 是否启用该规则。<br/>
    /// Whether the rule is enabled.
    /// </summary>
    [JsonPropertyName("enabled")]
    public bool? Enabled { get; set; }

    /// <summary>
    /// 种子名称必须包含的字符串。<br/>
    /// The string the torrent name must contain.
    /// </summary>
    [JsonPropertyName("mustContain")]
    public string? MustContain { get; set; }

    /// <summary>
    /// 种子名称不能包含的字符串。<br/>
    /// The string the torrent name must not contain.
    /// </summary>
    [JsonPropertyName("mustNotContain")]
    public string? MustNotContain { get; set; }

    /// <summary>
    /// 在 "mustContain" 和 "mustNotContain" 中启用正则表达式模式。<br/>
    /// Enable regular expression mode in "mustContain" and "mustNotContain".
    /// </summary>
    [JsonPropertyName("useRegex")]
    public bool? UseRegex { get; set; }

    /// <summary>
    /// 集数过滤器定义 (例如: 1x01;1x02)。<br/>
    /// Episode filter definition (e.g., 1x01;1x02).
    /// </summary>
    [JsonPropertyName("episodeFilter")]
    public string? EpisodeFilter { get; set; }

    /// <summary>
    /// 启用智能集数过滤器 (防止重复下载同一集)。<br/>
    /// Enable smart episode filter (prevents downloading the same episode twice).
    /// </summary>
    [JsonPropertyName("smartFilter")]
    public bool? SmartFilter { get; set; }

    /// <summary>
    /// 智能过滤器已匹配的集数 ID 列表。<br/>
    /// List of episode IDs already matched by the smart filter.
    /// </summary>
    [JsonPropertyName("previouslyMatchedEpisodes")]
    public List<string>? PreviouslyMatchedEpisodes { get; set; }

    /// <summary>
    /// 该规则应用到的订阅源 URL 列表。<br/>
    /// List of feed URLs to which the rule applies.
    /// </summary>
    [JsonPropertyName("affectedFeeds")]
    public List<string>? AffectedFeeds { get; set; }

    /// <summary>
    /// 忽略后续规则匹配的天数。<br/>
    /// Number of days to ignore subsequent rule matches.
    /// </summary>
    [JsonPropertyName("ignoreDays")]
    public int? IgnoreDays { get; set; }

    /// <summary>
    /// 规则最后一次匹配的时间 (字符串格式)。<br/>
    /// Time when the rule last matched (string format).
    /// </summary>
    [JsonPropertyName("lastMatch")]
    public string? LastMatch { get; set; }

    /// <summary>
    /// 以暂停模式添加匹配的种子。<br/>
    /// Add matched torrents in paused mode.
    /// </summary>
    [JsonPropertyName("addPaused")]
    public bool? AddInPausedMode { get; set; }

    /// <summary>
    /// 为种子分配的分类。<br/>
    /// Category assigned to the torrent.
    /// </summary>
    [JsonPropertyName("assignedCategory")]
    public string? AssignedCategory { get; set; }

    /// <summary>
    /// 种子保存的目录路径。<br/>
    /// Directory path where the torrent is saved.
    /// </summary>
    [JsonPropertyName("savePath")]
    public string? SavePath { get; set; }

    /// <summary>
    /// 停止条件。 / Stop condition.
    /// </summary>
    [JsonPropertyName("stopCondition")]
    [JsonConverter(typeof(StopConditionConverter))]
    public EnumStopCondition StopCondition { get; set; }

    /// <summary>
    /// 内容布局。 / Content layout.
    /// </summary>
    [JsonPropertyName("contentLayout")]
    [JsonConverter(typeof(ContentLayoutConverter))]
    public EnumContentLayout ContentLayout { get; set; }
}
