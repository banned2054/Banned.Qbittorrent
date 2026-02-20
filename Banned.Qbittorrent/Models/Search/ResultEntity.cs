using System.Text.Json.Serialization;

namespace Banned.Qbittorrent.Models.Search;

/// <summary>
/// 表示搜索结果实体。<br/>
/// Represents a search result entity.
/// </summary>
public class ResultEntity
{
    /// <summary>
    /// 描述页面的链接。<br/>
    /// Link to the description page.
    /// </summary>
    [JsonPropertyName("descrLink")]
    public string? DescriptionLink { get; set; }

    /// <summary>
    /// 文件名称。<br/>
    /// File name.
    /// </summary>
    [JsonPropertyName("fileName")]
    public string? FileName { get; set; }

    /// <summary>
    /// 文件大小（字节）。<br/>
    /// File size (in bytes).
    /// </summary>
    [JsonPropertyName("fileSize")]
    public long? FileSize { get; set; }

    /// <summary>
    /// 文件的下载链接或磁力链接。<br/>
    /// Download URL or magnet link of the file.
    /// </summary>
    [JsonPropertyName("fileUrl")]
    public string? FileUrl { get; set; }

    /// <summary>
    /// 吸血者（下载中）的数量。<br/>
    /// Number of leechers.
    /// </summary>
    [JsonPropertyName("nbLeechers")]
    public int? NumberOfLeechers { get; set; }

    /// <summary>
    /// 做种者的数量。<br/>
    /// Number of seeders.
    /// </summary>
    [JsonPropertyName("nbSeeders")]
    public int? NumberOfSeeders { get; set; }

    /// <summary>
    /// 来源网站的 URL。<br/>
    /// URL of the source site.
    /// </summary>
    [JsonPropertyName("siteUrl")]
    public string? SiteUrl { get; set; }
}
