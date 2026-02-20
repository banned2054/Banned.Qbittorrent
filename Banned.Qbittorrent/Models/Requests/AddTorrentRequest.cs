using System.Globalization;

namespace Banned.Qbittorrent.Models.Requests;

/// <summary>
/// 添加种子请求的参数模型。<br/>
/// Request model for adding torrents.
/// </summary>
public class AddTorrentRequest
{
    /// <summary>
    /// 本地种子文件路径列表。<br/>
    /// List of local torrent file paths.
    /// </summary>
    public List<string>? FilePaths { get; set; }

    /// <summary>
    /// 种子下载链接（Magnet 或 HTTP）。<br/>
    /// Torrent download URLs (Magnet or HTTP).
    /// </summary>
    public List<string>? Urls { get; set; }

    /// <summary>
    /// 下载保存路径。<br/>
    /// Download save path.
    /// </summary>
    public string? SavePath { get; set; } = "/download";

    /// <summary>
    /// 种子所属类别。<br/>
    /// Category for the torrent.
    /// </summary>
    public string? Category { get; set; }

    /// <summary>
    /// 种子标签（多个标签用逗号分隔）。<br/>
    /// Torrent tags (multiple tags separated by comma).
    /// </summary>
    public string? Tags { get; set; }

    /// <summary>
    /// 是否启用“跳过哈希校验”。<br/>
    /// Whether to enable "skip hash check".
    /// </summary>
    public bool? SkipCheckingEnabled { get; set; }

    /// <summary>
    /// 是否启用“添加后暂停”。<br/>
    /// Whether to enable "pause after adding".
    /// </summary>
    public bool? PausedEnabled { get; set; }

    /// <summary>
    /// 是否启用“创建根目录”。<br/>
    /// Whether to enable "create root folder".
    /// </summary>
    public bool? RootFolderEnabled { get; set; }

    /// <summary>
    /// 重命名种子。<br/>
    /// Rename the torrent.
    /// </summary>
    public string? Rename { get; set; }

    /// <summary>
    /// 上传限制（字节/秒）。<br/>
    /// Upload speed limit (bytes/second).
    /// </summary>
    public int? UploadLimit { get; set; }

    /// <summary>
    /// 下载限制（字节/秒）。<br/>
    /// Download speed limit (bytes/second).
    /// </summary>
    public int? DownloadLimit { get; set; }

    /// <summary>
    /// 分享率限制。<br/>
    /// Share ratio limit.
    /// </summary>
    public float? RatioLimit { get; set; }

    /// <summary>
    /// 做种时间限制（分钟）。<br/>
    /// Seeding time limit (minutes).
    /// </summary>
    public int? SeedingTimeLimit { get; set; }

    /// <summary>
    /// 是否启用自动种子管理（TMM）。<br/>
    /// Whether to enable Automatic Torrent Management (TMM).
    /// </summary>
    public bool? AutoTmmEnabled { get; set; }

    /// <summary>
    /// 是否启用顺序下载。<br/>
    /// Whether to enable sequential download.
    /// </summary>
    public bool? SequentialDownloadEnabled { get; set; }

    /// <summary>
    /// 是否启用首尾分块优先下载。<br/>
    /// Whether to enable first and last piece priority.
    /// </summary>
    public bool? FirstLastPiecePriorityEnabled { get; set; }

    /// <summary>
    /// 将请求参数转换为字典格式。<br/>
    /// Converts request parameters to a dictionary.
    /// </summary>
    /// <returns>包含所有设置参数的字典。 / A dictionary containing all set parameters.</returns>
    public Dictionary<string, string> ToDictionary()
    {
        var parameters = new Dictionary<string, string>();

        if (Urls is { Count: > 0 })
        {
            parameters["urls"] = string.Join("\n", Urls);
        }

        if (!string.IsNullOrEmpty(SavePath)) parameters["savepath"]   = SavePath;
        if (!string.IsNullOrEmpty(Category)) parameters["category"]   = Category;
        if (!string.IsNullOrEmpty(Tags)) parameters["tags"]           = Tags;
        if (SkipCheckingEnabled.HasValue) parameters["skip_checking"] = SkipCheckingEnabled.Value.ToString().ToLower();
        if (PausedEnabled.HasValue)
        {
            parameters["paused"]  = PausedEnabled.Value.ToString().ToLower();
            parameters["stopped"] = PausedEnabled.Value.ToString().ToLower();
        }

        if (RootFolderEnabled.HasValue) parameters["root_folder"] = RootFolderEnabled.Value.ToString().ToLower();
        if (!string.IsNullOrEmpty(Rename)) parameters["rename"] = Rename;
        if (UploadLimit.HasValue) parameters["upLimit"] = UploadLimit.Value.ToString();
        if (DownloadLimit.HasValue) parameters["dlLimit"] = DownloadLimit.Value.ToString();
        if (RatioLimit.HasValue) parameters["ratioLimit"] = RatioLimit.Value.ToString(CultureInfo.InvariantCulture);
        if (SeedingTimeLimit.HasValue) parameters["seedingTimeLimit"] = SeedingTimeLimit.Value.ToString();
        if (AutoTmmEnabled.HasValue) parameters["autoTMM"] = AutoTmmEnabled.Value.ToString().ToLower();
        if (SequentialDownloadEnabled.HasValue)
            parameters["sequentialDownload"] = SequentialDownloadEnabled.Value.ToString().ToLower();
        if (FirstLastPiecePriorityEnabled.HasValue)
            parameters["firstLastPiecePrio"] = FirstLastPiecePriorityEnabled.Value.ToString().ToLower();

        return parameters;
    }

    /// <inheritdoc/>
    public override string ToString()
    {
        return string.Join("&", ToDictionary().Select(p => $"{p.Key}={Uri.EscapeDataString(p.Value)}"));
    }
}
