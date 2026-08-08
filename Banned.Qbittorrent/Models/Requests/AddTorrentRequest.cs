using Banned.Qbittorrent.Models.Application;
using Banned.Qbittorrent.Models.Enums;
using Banned.Qbittorrent.Utils;
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
    /// 获取远程 Torrent 文件时使用的 Cookie。<br/>
    /// Cookie used when retrieving a remote torrent file.
    /// </summary>
    public string? Cookie { get; set; }

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
    /// 是否以停止状态添加 Torrent。<br/>
    /// Whether to add the torrent in the stopped state.
    /// </summary>
    public bool? StoppedEnabled { get; set; }

    /// <summary>
    /// 是否启用“创建根目录”。<br/>
    /// Whether to enable "create root folder".
    /// </summary>
    public bool? RootFolderEnabled { get; set; }

    /// <summary>
    /// Torrent 内容的目录布局。Web API 2.7 及更高版本使用此参数。<br/>
    /// Directory layout for torrent content. This parameter is used by Web API 2.7 and later.
    /// </summary>
    public EnumContentLayout? ContentLayout { get; set; }

    /// <summary>
    /// Torrent 完成前使用的临时下载路径。<br/>
    /// Temporary download path used before the torrent is completed.
    /// </summary>
    public string? DownloadPath { get; set; }

    /// <summary>
    /// 是否使用临时下载路径。未设置且提供了 <see cref="DownloadPath"/> 时默认为 <see langword="true"/>。<br/>
    /// Whether to use the temporary download path. Defaults to <see langword="true"/> when
    /// <see cref="DownloadPath"/> is provided and this value is not set.
    /// </summary>
    public bool? UseDownloadPathEnabled { get; set; }

    /// <summary>
    /// 自动停止 Torrent 的条件。<br/>
    /// Condition for automatically stopping the torrent.
    /// </summary>
    public EnumTorrentAddStopCondition? StopCondition { get; set; }

    /// <summary>
    /// 是否将 Torrent 添加到队列顶部。<br/>
    /// Whether to add the torrent to the top of the queue.
    /// </summary>
    public bool? AddToTopOfQueueEnabled { get; set; }

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
    /// 非活动状态下的做种时间限制（分钟）。<br/>
    /// Inactive seeding time limit (minutes).
    /// </summary>
    public int? InactiveSeedingTimeLimit { get; set; }

    /// <summary>
    /// 达到分享限制后执行的操作。<br/>
    /// Action performed after the share limit is reached.
    /// </summary>
    public EnumTorrentShareLimitAction? ShareLimitAction { get; set; }

    /// <summary>
    /// 用于 SSL Torrent 的客户端证书（PEM 格式）。<br/>
    /// Client certificate for SSL torrents (PEM format).
    /// </summary>
    public string? SslCertificate { get; set; }

    /// <summary>
    /// 用于 SSL Torrent 的客户端私钥。<br/>
    /// Client private key for SSL torrents.
    /// </summary>
    public string? SslPrivateKey { get; set; }

    /// <summary>
    /// 用于 SSL Torrent 的 Diffie-Hellman 参数。<br/>
    /// Diffie-Hellman parameters for SSL torrents.
    /// </summary>
    public string? SslDhParameters { get; set; }

    /// <summary>
    /// 是否以强制状态添加 Torrent。<br/>
    /// Whether to add the torrent in the forced state.
    /// </summary>
    public bool? ForcedEnabled { get; set; }

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
        => ToDictionaryCore(null);

    /// <summary>
    /// 根据目标 Web API 版本将请求参数转换为字典格式。<br/>
    /// Converts request parameters to a dictionary for the target Web API version.
    /// </summary>
    /// <param name="apiVersion">目标 Web API 版本。 / Target Web API version.</param>
    /// <returns>包含所有设置参数的字典。 / A dictionary containing all set parameters.</returns>
    public Dictionary<string, string> ToDictionary(ApiVersion apiVersion) => ToDictionaryCore(apiVersion);

    private Dictionary<string, string> ToDictionaryCore(ApiVersion? apiVersion)
    {
        var parameters = new Dictionary<string, string>();

        if (Urls is { Count: > 0 })
        {
            parameters["urls"] = string.Join("\n", Urls);
        }

        if (!string.IsNullOrEmpty(SavePath)) parameters["savepath"]   = SavePath;
        if (Cookie is not null) parameters["cookie"]                  = Cookie;
        if (!string.IsNullOrEmpty(Category)) parameters["category"]   = Category;
        if (!string.IsNullOrEmpty(Tags)) parameters["tags"] = Tags;
        if (SkipCheckingEnabled.HasValue) parameters["skip_checking"] = SkipCheckingEnabled.Value.ToString().ToLower();
        var stoppedEnabled = StoppedEnabled ?? PausedEnabled;
        if (stoppedEnabled.HasValue)
        {
            parameters["paused"]  = stoppedEnabled.Value.ToString().ToLower();
            parameters["stopped"] = stoppedEnabled.Value.ToString().ToLower();
        }

        if (apiVersion is { } version && version >= ApiVersion.V2_7_0)
        {
            var contentLayout = ContentLayout;
            if (contentLayout is null && RootFolderEnabled.HasValue)
                contentLayout = RootFolderEnabled.Value ? EnumContentLayout.Original : EnumContentLayout.NoSubfolder;

            if (contentLayout is not null and not EnumContentLayout.Unknown)
                parameters["contentLayout"] = contentLayout.Value.ContentLayout2String();
        }
        else
        {
            var rootFolderEnabled = RootFolderEnabled;
            if (!rootFolderEnabled.HasValue && ContentLayout is not null and not EnumContentLayout.Unknown)
                rootFolderEnabled = ContentLayout is EnumContentLayout.Original or EnumContentLayout.Subfolder;

            if (rootFolderEnabled.HasValue)
                parameters["root_folder"] = rootFolderEnabled.Value.ToString().ToLower();
        }
        if (!string.IsNullOrEmpty(Rename)) parameters["rename"] = Rename;
        if (UploadLimit.HasValue) parameters["upLimit"] = UploadLimit.Value.ToString();
        if (DownloadLimit.HasValue) parameters["dlLimit"] = DownloadLimit.Value.ToString();
        if (RatioLimit.HasValue)
            parameters["ratioLimit"] = RatioLimit.Value.ToString(CultureInfo.InvariantCulture);
        if (SeedingTimeLimit.HasValue)
            parameters["seedingTimeLimit"] = SeedingTimeLimit.Value.ToString(CultureInfo.InvariantCulture);
        if (DownloadPath is not null) parameters["downloadPath"] = DownloadPath;
        var useDownloadPathEnabled = UseDownloadPathEnabled ?? (DownloadPath is not null ? true : null);
        if (useDownloadPathEnabled.HasValue)
            parameters["useDownloadPath"] = useDownloadPathEnabled.Value.ToString().ToLowerInvariant();
        if (StopCondition.HasValue) parameters["stopCondition"] = StopCondition.Value.ToString();
        if (AddToTopOfQueueEnabled.HasValue)
            parameters["addToTopOfQueue"] = AddToTopOfQueueEnabled.Value.ToString().ToLowerInvariant();
        if (InactiveSeedingTimeLimit.HasValue)
            parameters["inactiveSeedingTimeLimit"] =
                InactiveSeedingTimeLimit.Value.ToString(CultureInfo.InvariantCulture);
        if (ShareLimitAction.HasValue) parameters["shareLimitAction"] = ShareLimitAction.Value.ToString();
        if (SslCertificate is not null) parameters["ssl_certificate"] = SslCertificate;
        if (SslPrivateKey is not null) parameters["ssl_private_key"] = SslPrivateKey;
        if (SslDhParameters is not null) parameters["ssl_dh_params"] = SslDhParameters;
        if (ForcedEnabled.HasValue)
            parameters["forced"] = ForcedEnabled.Value.ToString().ToLowerInvariant();
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
