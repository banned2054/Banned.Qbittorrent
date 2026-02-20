using Banned.Qbittorrent.Models.Application;
using Banned.Qbittorrent.Models.Search;
using Banned.Qbittorrent.Utils;
using System.Text.Json;

namespace Banned.Qbittorrent.Services;

/// <summary>
/// 提供与 qBittorrent 搜索相关的服务。<br/>
/// Provides services related to qBittorrent search.
/// </summary>
public class SearchService(NetService netService)
{
    private const string BaseUrl = "/api/v2/search";

    /// <summary>
    /// 开始一个新的搜索作业。<br/>
    /// Start a new search job.
    /// </summary>
    /// <param name="pattern">搜索关键词。Search pattern.</param>
    /// <param name="plugins">要使用的插件列表（"all" 表示所有插件）。List of plugins to use (or "all").</param>
    /// <param name="category">要搜索的类别（"all" 表示所有类别）。Category to search in (or "all").</param>
    /// <returns>包含搜索作业 ID 的对象。An object containing the search job ID.</returns>
    public async Task<SearchJob?> StartSearch(string pattern, string[] plugins, string category = "all")
    {
        var parameters = new Dictionary<string, string>
        {
            { "pattern", pattern },
            { "plugins", StringUtils.Join('|', plugins) },
            { "category", category },
        };
        var result = await netService.Post(BaseUrl, parameters, ApiVersion.V2_1_1);
        return JsonSerializer.Deserialize<SearchJob>(result);
    }

    /// <summary>
    /// 停止指定的搜索作业。<br/>
    /// Stop the specific search job.
    /// </summary>
    /// <param name="id">搜索作业的 ID。Search job ID.</param>
    public async Task StopSearch(int id)
    {
        var parameters = new Dictionary<string, string>
        {
            { "id", id.ToString() }
        };
        await netService.Post($"{BaseUrl}/stop", parameters, ApiVersion.V2_1_1);
    }

    /// <summary>
    /// 停止指定的搜索作业。<br/>
    /// Stop the specific search job.
    /// </summary>
    /// <param name="job">搜索作业对象。Search job object.</param>
    public async Task StopSearch(SearchJob job) => await StopSearch(job.Id);

    /// <summary>
    /// 获取搜索作业的状态。<br/>
    /// Get the status of search jobs.
    /// </summary>
    /// <param name="id">可选的搜索作业 ID。如果不指定，则返回所有搜索作业的状态。<br/>Optional search job ID. If not specified, returns status of all jobs.</param>
    /// <returns>搜索状态列表。A list of search statuses.</returns>
    public async Task<SearchStatus[]?> SearchStatus(int? id = null)
    {
        var url = $"{BaseUrl}/status";

        if (id.HasValue) url += $"?id={id.Value}";

        var response = await netService.Get(url, ApiVersion.V2_1_1);
        return JsonSerializer.Deserialize<SearchStatus[]>(response);
    }

    /// <summary>
    /// 获取特定搜索作业的结果。<br/>
    /// Get results of a specific search job.
    /// </summary>
    /// <param name="id">搜索作业 ID。Search job ID.</param>
    /// <param name="limit">返回结果的最大数量。Max number of results to return.</param>
    /// <param name="offset">结果偏移量。Result offset.</param>
    /// <returns>包含搜索结果及状态的对象。An object containing search results and status.</returns>
    public async Task<SearchResult?> GetSearchResults(int id, int limit = 0, int offset = 0)
    {
        var parameters = new Dictionary<string, string>
        {
            { "id", id.ToString() },
            { "limit", limit.ToString() },
            { "offset", offset.ToString() }
        };
        var response = await netService.Post($"{BaseUrl}/results", parameters, ApiVersion.V2_1_1);
        return JsonSerializer.Deserialize<SearchResult>(response);
    }

    /// <summary>
    /// 删除搜索作业。<br/>
    /// Delete search job.
    /// </summary>
    /// <param name="id">搜索作业 ID。Search job ID.</param>
    public async Task DeleteSearchResults(int id)
    {
        var parameters = new Dictionary<string, string>
        {
            { "id", id.ToString() }
        };
        await netService.Post($"{BaseUrl}/delete", parameters, ApiVersion.V2_1_1);
    }

    /// <summary>
    /// 删除搜索作业。<br/>
    /// Delete search job.
    /// </summary>
    /// <param name="job">搜索作业对象。Search job object.</param>
    public async Task DeleteSearchResults(SearchJob job) => await DeleteSearchResults(job.Id);

    /// <summary>
    /// 获取所有已安装的搜索插件。<br/>
    /// Get all installed search plugins.
    /// </summary>
    /// <returns>搜索插件列表。A list of search plugins.</returns>
    public async Task<SearchPlugins?> GetSearchPlugins()
    {
        var response = await netService.Get($"{BaseUrl}/plugins", ApiVersion.V2_1_1);
        return JsonSerializer.Deserialize<SearchPlugins>(response);
    }

    /// <summary>
    /// 安装搜索插件。<br/>
    /// Install search plugins.
    /// </summary>
    /// <param name="sources">插件源列表（可以是本地路径或 URL）。List of plugin sources (local paths or URLs).</param>
    public async Task InstallSearchPlugin(string[] sources)
    {
        var parameters = new Dictionary<string, string>
        {
            { "sources", StringUtils.Join('|', sources) }
        };
        await netService.Post($"{BaseUrl}/installPlugin", parameters, ApiVersion.V2_1_1);
    }

    /// <summary>
    /// 安装搜索插件。<br/>
    /// Install search plugin.
    /// </summary>
    /// <param name="source">插件源（可以是本地路径或 URL）。Plugin source (local path or URL).</param>
    public async Task InstallSearchPlugin(string source) => await InstallSearchPlugin([source]);

    /// <summary>
    /// 卸载搜索插件。<br/>
    /// Uninstall search plugins.
    /// </summary>
    /// <param name="names">要卸载的插件名称列表。List of plugin names to uninstall.</param>
    public async Task UninstallSearchPlugin(string[] names)
    {
        var parameters = new Dictionary<string, string>
        {
            { "names", StringUtils.Join('|', names) }
        };
        await netService.Post($"{BaseUrl}/uninstallPlugin", parameters, ApiVersion.V2_1_1);
    }

    /// <summary>
    /// 卸载搜索插件。<br/>
    /// Uninstall search plugin.
    /// </summary>
    /// <param name="name">要卸载的插件名称。Plugin name to uninstall.</param>
    public async Task UninstallSearchPlugin(string name) => await UninstallSearchPlugin([name]);

    /// <summary>
    /// 启用或禁用搜索插件。<br/>
    /// Enable or disable search plugins.
    /// </summary>
    /// <param name="names">插件名称列表。List of plugin names.</param>
    /// <param name="enable">是否启用。Whether to enable.</param>
    public async Task EnableSearchPlugin(string[] names, bool enable)
    {
        var parameters = new Dictionary<string, string>
        {
            { "names", StringUtils.Join('|', names) },
            { "enable", enable.ToString().ToLower() }
        };
        await netService.Post($"{BaseUrl}/enablePlugin", parameters, ApiVersion.V2_1_1);
    }

    /// <summary>
    /// 更新搜索插件。<br/>
    /// Update search plugins.
    /// </summary>
    public async Task UpdateSearchPlugins() =>
        await netService.Post($"{BaseUrl}/updatePlugins", null, ApiVersion.V2_1_1);
}
