using Banned.Qbittorrent.Models.Sync;
using System.Text.Json;

namespace Banned.Qbittorrent.Services;

/// <summary>
/// 提供与数据同步相关的服务，用于获取服务器的增量更新。<br/>
/// Provides services related to data synchronization for fetching incremental updates from the server.
/// </summary>
public class SyncService(NetService netService)
{
    private const string BaseUrl = "/api/v2/sync";

    /// <summary>
    /// 获取主数据更新。<br/>
    /// Get main data dynamic response.
    /// </summary>
    /// <param name="rid">
    /// 响应 ID。如果是第一次请求，请设为 0。服务器将只返回自该 rid 以来发生变化的数据。<br/>
    /// Response ID. If this is the first request, set it to 0. The server will only return data changed since this rid.
    /// </param>
    /// <returns>
    /// 包含种子信息、传输状态、分类及标签的主数据更新对象。<br/>
    /// A main data update object containing torrents, transfer state, categories, and tags.
    /// </returns>
    public async Task<MainData?> GetMainData(int rid = 0)
    {
        var parameters = new Dictionary<string, string>
        {
            { "rid", rid.ToString() }
        };

        var response = await netService.Post($"{BaseUrl}/maindata", parameters);
        return JsonSerializer.Deserialize<MainData>(response);
    }

    /// <summary>
    /// 获取特定种子的 Peer（连接用户）增量更新数据。<br/>
    /// Get peer (connected user) incremental update data for a specific torrent.
    /// </summary>
    /// <param name="rid">响应 ID。 / Response ID.</param>
    /// <param name="hash">种子的 Hash 值。 / Torrent hash.</param>
    /// <returns>
    /// Peer 增量更新数据对象。<br/>
    /// A peer incremental update data object.
    /// </returns>
    public async Task<PeerData?> GetPeerData(int rid = 0, string hash = "")
    {
        var parameters = new Dictionary<string, string>
        {
            { "rid", rid.ToString() },
            { "hash", hash }
        };

        var response = await netService.Post($"{BaseUrl}/torrentPeers", parameters);
        return JsonSerializer.Deserialize<PeerData>(response);
    }
}
