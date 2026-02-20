using System.Text.Json.Serialization;

namespace Banned.Qbittorrent.Models.Search;

/// <summary>
/// 表示搜索插件的信息。<br/>
/// Represents information about a search plugin.
/// </summary>
public class SearchPlugins
{
    /// <summary>
    /// 指示插件是否已启用。<br/>
    /// Indicates whether the plugin is enabled.
    /// </summary>
    [JsonPropertyName("enabled")]
    public bool? IsEnabled { get; set; }

    /// <summary>
    /// 插件的全名。<br/>
    /// The full name of the plugin.
    /// </summary>
    [JsonPropertyName("fullName")]
    public string? FullName { get; set; }

    /// <summary>
    /// 插件的简短名称。<br/>
    /// The short name of the plugin.
    /// </summary>
    [JsonPropertyName("name")]
    public string? Name { get; set; }

    /// <summary>
    /// 插件支持的搜索类别列表。<br/>
    /// List of search categories supported by the plugin.
    /// </summary>
    [JsonPropertyName("supportedCategories")]
    public SearchCategory[]? SupportedCategories { get; set; }

    /// <summary>
    /// 插件来源的 URL。<br/>
    /// The URL of the plugin source.
    /// </summary>
    [JsonPropertyName("url")]
    public string? Url { get; set; }

    /// <summary>
    /// 插件的版本号。<br/>
    /// The version of the plugin.
    /// </summary>
    [JsonPropertyName("version")]
    public string? Version { get; set; }
}
