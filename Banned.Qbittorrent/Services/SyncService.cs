using Banned.Qbittorrent.Models.Sync;
using System.Text.Json;

namespace Banned.Qbittorrent.Services;

public class SyncService(NetService netService)
{
    private const string BaseUrl = "/api/v2/sync";

    public async Task<MainData?> GetMainData(int rid = 0)
    {
        var parameters = new Dictionary<string, string>
        {
            { "rid", rid.ToString() }
        };

        var response = await netService.Post($"{BaseUrl}/maindata", parameters);
        return JsonSerializer.Deserialize<MainData>(response);
    }

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
