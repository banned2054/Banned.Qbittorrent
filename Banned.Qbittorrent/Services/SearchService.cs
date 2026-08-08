using Banned.Qbittorrent.Models.Application;
using Banned.Qbittorrent.Models.Search;
using Banned.Qbittorrent.Serialization;
using Banned.Qbittorrent.Utils;

namespace Banned.Qbittorrent.Services;

/// <summary>
/// 提供与 qBittorrent 搜索相关的服务。<br/>
/// Provides services related to qBittorrent search.
/// </summary>
public class SearchService(NetService netService)
{
    private const string BaseUrl = "/api/v2/search";

    private static readonly ApiVersionRange SearchCategoriesVersionRange = new(ApiVersion.V2_1_1, ApiVersion.V2_6_0);

    /// <summary>
    /// 开始一个新的搜索作业。<br/>
    /// Start a new search job.
    /// </summary>
    /// <param name="pattern">搜索关键词。Search pattern.</param>
    /// <param name="plugins">要使用的插件列表（"all" 表示所有插件）。List of plugins to use (or "all").</param>
    /// <param name="category">要搜索的类别（"all" 表示所有类别）。Category to search in (or "all").</param>
    /// <param name="cancellationToken">取消请求的令牌。<br/>Token used to cancel the request.</param>
    /// <returns>包含搜索作业 ID 的对象。An object containing the search job ID.</returns>
    public async Task<SearchJob?> StartSearch(string   pattern,
                                              string[] plugins,
                                              string   category = "all", CancellationToken cancellationToken = default)
    {
        var parameters = new Dictionary<string, string>
        {
            { "pattern", pattern },
            { "plugins", StringUtils.Join('|', plugins) },
            { "category", category },
        };
        var result = await netService.Post($"{BaseUrl}/start", parameters, ApiVersion.V2_1_1,
                                           ct : cancellationToken);
        return QBittorrentJsonSerializer.Deserialize<SearchJob>(result);
    }

    /// <summary>
    /// 停止指定的搜索作业。<br/>
    /// Stop the specific search job.
    /// </summary>
    /// <param name="id">搜索作业的 ID。Search job ID.</param>
    /// <param name="cancellationToken">取消请求的令牌。<br/>Token used to cancel the request.</param>
    public async Task StopSearch(int id, CancellationToken cancellationToken = default)
    {
        var parameters = new Dictionary<string, string>
        {
            { "id", id.ToString() }
        };
        await netService.Post($"{BaseUrl}/stop", parameters, ApiVersion.V2_1_1, ct : cancellationToken);
    }

    /// <summary>
    /// 停止指定的搜索作业。<br/>
    /// Stop the specific search job.
    /// </summary>
    /// <param name="job">搜索作业对象。Search job object.</param>
    /// <param name="cancellationToken">取消请求的令牌。<br/>Token used to cancel the request.</param>
    public async Task StopSearch(SearchJob job, CancellationToken cancellationToken = default) =>
        await StopSearch(job.Id, cancellationToken);

    /// <summary>
    /// 获取搜索作业的状态。<br/>
    /// Get the status of search jobs.
    /// </summary>
    /// <param name="id">可选的搜索作业 ID。如果不指定，则返回所有搜索作业的状态。<br/>Optional search job ID. If not specified, returns status of all jobs.</param>
    /// <param name="cancellationToken">取消请求的令牌。<br/>Token used to cancel the request.</param>
    /// <returns>搜索状态列表。A list of search statuses.</returns>
    public async Task<SearchStatus[]?> SearchStatus(int? id = null, CancellationToken cancellationToken = default)
    {
        var url = $"{BaseUrl}/status";

        if (id.HasValue) url += $"?id={id.Value}";

        var response = await netService.Get(url, ApiVersion.V2_1_1, ct : cancellationToken);
        return QBittorrentJsonSerializer.Deserialize<SearchStatus[]>(response);
    }

    /// <summary>
    /// 获取特定搜索作业的结果。<br/>
    /// Get results of a specific search job.
    /// </summary>
    /// <param name="id">搜索作业 ID。Search job ID.</param>
    /// <param name="limit">返回结果的最大数量。Max number of results to return.</param>
    /// <param name="offset">结果偏移量。Result offset.</param>
    /// <param name="cancellationToken">取消请求的令牌。<br/>Token used to cancel the request.</param>
    /// <returns>包含搜索结果及状态的对象。An object containing search results and status.</returns>
    public async Task<SearchResult?> GetSearchResults(int id,
                                                      int limit  = 0,
                                                      int offset = 0, CancellationToken cancellationToken = default)
    {
        var parameters = new Dictionary<string, string>
        {
            { "id", id.ToString() },
            { "limit", limit.ToString() },
            { "offset", offset.ToString() }
        };
        var response =
            await netService.Post($"{BaseUrl}/results", parameters, ApiVersion.V2_1_1, ct : cancellationToken);
        return QBittorrentJsonSerializer.Deserialize<SearchResult>(response);
    }

    /// <summary>
    /// 删除搜索作业。<br/>
    /// Delete search job.
    /// </summary>
    /// <param name="id">搜索作业 ID。Search job ID.</param>
    /// <param name="cancellationToken">取消请求的令牌。<br/>Token used to cancel the request.</param>
    public async Task DeleteSearchResults(int id, CancellationToken cancellationToken = default)
    {
        var parameters = new Dictionary<string, string>
        {
            { "id", id.ToString() }
        };
        await netService.Post($"{BaseUrl}/delete", parameters, ApiVersion.V2_1_1, ct : cancellationToken);
    }

    /// <summary>
    /// 删除搜索作业。<br/>
    /// Delete search job.
    /// </summary>
    /// <param name="job">搜索作业对象。Search job object.</param>
    /// <param name="cancellationToken">取消请求的令牌。<br/>Token used to cancel the request.</param>
    public async Task DeleteSearchResults(SearchJob job, CancellationToken cancellationToken = default) =>
        await DeleteSearchResults(job.Id, cancellationToken);

    /// <summary>
    /// 获取搜索类别。此端点仅存在于 Web API 2.1.1 至 2.6.0 之前。<br/>
    /// Gets search categories. This endpoint is available from Web API 2.1.1 until, but excluding, 2.6.0.
    /// </summary>
    /// <param name="pluginName">可选的插件筛选器，支持 "all" 和 "enabled"。<br/>Optional plugin filter; "all" and "enabled" are supported.</param>
    /// <param name="cancellationToken">取消请求的令牌。<br/>Token used to cancel the request.</param>
    /// <returns>搜索类别列表。A list of search categories.</returns>
    public async Task<List<SearchCategory>> GetSearchCategories(string?           pluginName        = null,
                                                                CancellationToken cancellationToken = default)
    {
        var parameters = pluginName == null ? null : new Dictionary<string, string> { { "pluginName", pluginName } };

        var response = await netService.Post($"{BaseUrl}/categories", parameters, SearchCategoriesVersionRange,
                                             ct : cancellationToken);
        return QBittorrentJsonSerializer.Deserialize<List<SearchCategory>>(response) ?? [];
    }

    /// <summary>
    /// 获取所有已安装的搜索插件。<br/>
    /// Get all installed search plugins.
    /// </summary>
    /// <param name="cancellationToken">取消请求的令牌。<br/>Token used to cancel the request.</param>
    /// <returns>搜索插件列表。A list of search plugins.</returns>
    public async Task<List<SearchPlugins>> GetSearchPlugins(CancellationToken cancellationToken = default)
    {
        var response = await netService.Get($"{BaseUrl}/plugins", ApiVersion.V2_1_1, ct : cancellationToken);
        return QBittorrentJsonSerializer.Deserialize<List<SearchPlugins>>(response) ?? [];
    }

    /// <summary>
    /// 安装搜索插件。<br/>
    /// Install search plugins.
    /// </summary>
    /// <param name="sources">插件源列表（可以是本地路径或 URL）。List of plugin sources (local paths or URLs).</param>
    /// <param name="cancellationToken">取消请求的令牌。<br/>Token used to cancel the request.</param>
    public async Task InstallSearchPlugin(string[] sources, CancellationToken cancellationToken = default)
    {
        var parameters = new Dictionary<string, string>
        {
            { "sources", StringUtils.Join('|', sources) }
        };
        await netService.Post($"{BaseUrl}/installPlugin", parameters, ApiVersion.V2_1_1, ct : cancellationToken);
    }

    /// <summary>
    /// 安装搜索插件。<br/>
    /// Install search plugin.
    /// </summary>
    /// <param name="source">插件源（可以是本地路径或 URL）。Plugin source (local path or URL).</param>
    /// <param name="cancellationToken">取消请求的令牌。<br/>Token used to cancel the request.</param>
    public async Task InstallSearchPlugin(string source, CancellationToken cancellationToken = default) =>
        await InstallSearchPlugin([source], cancellationToken);

    /// <summary>
    /// 卸载搜索插件。<br/>
    /// Uninstall search plugins.
    /// </summary>
    /// <param name="names">要卸载的插件名称列表。List of plugin names to uninstall.</param>
    /// <param name="cancellationToken">取消请求的令牌。<br/>Token used to cancel the request.</param>
    public async Task UninstallSearchPlugin(string[] names, CancellationToken cancellationToken = default)
    {
        var parameters = new Dictionary<string, string>
        {
            { "names", StringUtils.Join('|', names) }
        };
        await netService.Post($"{BaseUrl}/uninstallPlugin", parameters, ApiVersion.V2_1_1, ct : cancellationToken);
    }

    /// <summary>
    /// 卸载搜索插件。<br/>
    /// Uninstall search plugin.
    /// </summary>
    /// <param name="name">要卸载的插件名称。Plugin name to uninstall.</param>
    /// <param name="cancellationToken">取消请求的令牌。<br/>Token used to cancel the request.</param>
    public async Task UninstallSearchPlugin(string name, CancellationToken cancellationToken = default) =>
        await UninstallSearchPlugin([name], cancellationToken);

    /// <summary>
    /// 启用或禁用搜索插件。<br/>
    /// Enable or disable search plugins.
    /// </summary>
    /// <param name="names">插件名称列表。List of plugin names.</param>
    /// <param name="enable">是否启用。Whether to enable.</param>
    /// <param name="cancellationToken">取消请求的令牌。<br/>Token used to cancel the request.</param>
    public async Task EnableSearchPlugin(string[] names, bool enable, CancellationToken cancellationToken = default)
    {
        var parameters = new Dictionary<string, string>
        {
            { "names", StringUtils.Join('|', names) },
            { "enable", enable.ToString().ToLower() }
        };
        await netService.Post($"{BaseUrl}/enablePlugin", parameters, ApiVersion.V2_1_1, ct : cancellationToken);
    }

    /// <summary>
    /// 更新搜索插件。<br/>
    /// Update search plugins.
    /// </summary>
    /// <param name="cancellationToken">取消请求的令牌。<br/>Token used to cancel the request.</param>
    public async Task UpdateSearchPlugins(CancellationToken cancellationToken = default) =>
        await netService.Post($"{BaseUrl}/updatePlugins", null, ApiVersion.V2_1_1, ct : cancellationToken);

    /// <summary>
    /// 通过搜索插件下载种子文件或磁力链接。<br/>
    /// Downloads a torrent file or magnet link through a search plugin.
    /// </summary>
    /// <param name="torrentUrl">种子文件 URL 或磁力链接。<br/>Torrent file URL or magnet link.</param>
    /// <param name="pluginName">搜索插件名称。<br/>Search plugin name.</param>
    /// <param name="cancellationToken">取消请求的令牌。<br/>Token used to cancel the request.</param>
    public async Task DownloadTorrent(string torrentUrl,
                                      string pluginName, CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(torrentUrl);
        ArgumentException.ThrowIfNullOrWhiteSpace(pluginName);

        var parameters = new Dictionary<string, string>
        {
            { "torrentUrl", torrentUrl },
            { "pluginName", pluginName }
        };
        await netService.Post($"{BaseUrl}/downloadTorrent", parameters, ApiVersion.V2_11_0, ct : cancellationToken);
    }
}
