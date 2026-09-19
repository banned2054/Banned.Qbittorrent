using Banned.Qbittorrent.Models.Application;
using Banned.Qbittorrent.Serialization;
using System.Text.Json;

namespace Banned.Qbittorrent.Services;

/// <summary>
/// 提供 qBittorrent Web UI 客户端数据的加载和保存功能。<br/>
/// Provides loading and storing of qBittorrent Web UI client data.
/// </summary>
public class ClientDataService(NetService netService)
{
    private const string BaseUrl = "/api/v2/clientdata";

    /// <summary>
    /// 加载客户端数据。<br/>
    /// Loads client data.
    /// </summary>
    /// <param name="keys">要加载的键；为 <see langword="null"/> 时加载全部数据。<br/>
    /// Keys to load; <see langword="null"/> loads all data.</param>
    /// <param name="cancellationToken">取消请求的令牌。<br/>Token used to cancel the request.</param>
    /// <returns>客户端数据键值对。<br/>Client data key-value pairs.</returns>
    /// <remarks>此方法随 Web API v2.13.1 引入，并按 Python 客户端使用 GET。<br/>
    /// Introduced with Web API v2.13.1 and uses GET as implemented by the Python client.</remarks>
    public async Task<Dictionary<string, JsonElement>> Load(
        IEnumerable<string>? keys = null, CancellationToken cancellationToken = default)
    {
        var subPath = $"{BaseUrl}/load";
        if (keys is not null)
        {
            var keyJson = QBittorrentJsonSerializer.Serialize(keys.ToList());
            subPath += $"?keys={Uri.EscapeDataString(keyJson)}";
        }

        var response = await netService.Get(subPath, ApiVersion.V2_13_1, ct : cancellationToken);
        return QBittorrentJsonSerializer.Deserialize<Dictionary<string, JsonElement>>(response) ?? [];
    }

    /// <summary>
    /// 保存客户端数据。<br/>
    /// Stores client data.
    /// </summary>
    /// <param name="data">要保存的客户端数据。<br/>Client data to store.</param>
    /// <param name="cancellationToken">取消请求的令牌。<br/>Token used to cancel the request.</param>
    /// <remarks>此方法随 Web API v2.13.1 引入。<br/>Introduced with Web API v2.13.1.</remarks>
    public async Task Store(Dictionary<string, JsonElement>? data = null, CancellationToken cancellationToken = default)
    {
        var parameters = new Dictionary<string, string>
        {
            ["data"] = QBittorrentJsonSerializer.Serialize(data ?? [])
        };
        await netService.Post($"{BaseUrl}/store", parameters, ApiVersion.V2_13_1, ct : cancellationToken);
    }
}
