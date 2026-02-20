using Banned.Qbittorrent.Models.Application;
using Banned.Qbittorrent.Models.Search;
using Banned.Qbittorrent.Utils;
using System.Text.Json;

namespace Banned.Qbittorrent.Services;

public class SearchService(NetService netService)
{
    private const string BaseUrl = "/api/v2/search";

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

    public async Task StopSearch(int id)
    {
        var parameters = new Dictionary<string, string>
        {
            { "id", id.ToString() }
        };
        await netService.Post($"{BaseUrl}/stop", parameters, ApiVersion.V2_1_1);
    }

    public async Task StopSearch(SearchJob job) => await StopSearch(job.Id);

    public async Task<SearchStatus[]?> SearchStatus(int? id = null)
    {
        var url = $"{BaseUrl}/status";

        if (id.HasValue) url += $"?id={id.Value}";

        var response = await netService.Get(url, ApiVersion.V2_1_1);
        return JsonSerializer.Deserialize<SearchStatus[]>(response);
    }

    public async Task GetSearchResults(int id, int limit = 0, int? offset = null)
    {
        var url      = $"{BaseUrl}/results?id={id}&limit={limit}&offset={offset}";
        var response = await netService.Get(url, ApiVersion.V2_1_1);
    }
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
    public async Task DeleteSearchResults(int id)
    {
        var parameters = new Dictionary<string, string>
        {
            { "id", id.ToString() }
        };
        await netService.Post($"{BaseUrl}/delete", parameters, ApiVersion.V2_1_1);
    }

    public async Task DeleteSearchResults(SearchJob job) => await DeleteSearchResults(job.Id);

    public async Task<SearchPlugins?> GetSearchPlugins()
    {
        var response = await netService.Get($"{BaseUrl}/plugins", ApiVersion.V2_1_1);
        return JsonSerializer.Deserialize<SearchPlugins>(response);
    }

    public async Task InstallSearchPlugin(string[] sources)
    {
        var parameters = new Dictionary<string, string>
        {
            { "sources", StringUtils.Join('|', sources) }
        };
        await netService.Post($"{BaseUrl}/installPlugin", parameters, ApiVersion.V2_1_1);
    }

    public async Task InstallSearchPlugin(string source) => await InstallSearchPlugin([source]);

    public async Task UninstallSearchPlugin(string[] names)
    {
        var parameters = new Dictionary<string, string>
        {
            { "names", StringUtils.Join('|', names) }
        };
        await netService.Post($"{BaseUrl}/uninstallPlugin", parameters, ApiVersion.V2_1_1);
    }

    public async Task UninstallSearchPlugin(string name) => await UninstallSearchPlugin([name]);

    public async Task EnableSearchPlugin(string[] names, bool enable)
    {
        var parameters = new Dictionary<string, string>
        {
            { "names", StringUtils.Join('|', names) },
            { "enable", enable.ToString().ToLower() }
        };
        await netService.Post($"{BaseUrl}/enablePlugin", parameters, ApiVersion.V2_1_1);
    }

    public async Task UpdateSearchPlugins() =>
        await netService.Post($"{BaseUrl}/updatePlugins", null, ApiVersion.V2_1_1);
}
