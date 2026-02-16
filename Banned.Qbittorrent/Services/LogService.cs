using Banned.Qbittorrent.Models.Log;
using System.Text.Json;

namespace Banned.Qbittorrent.Services;

public class LogService(NetService netService)
{
    private const string BaseUrl = "/api/v2/log";

    public async Task<List<LogElement>> GetLogs(bool normal   = true,
                                                bool info     = true,
                                                bool warning  = true,
                                                bool critical = true,
                                                int  lastId   = -1)
    {
        var url = $"{BaseUrl}/main?"      +
                  $"normal={normal}&"     +
                  $"info={info}&"         +
                  $"warning={warning}&"   +
                  $"critical={critical}&" +
                  $"last_known_id={lastId}";

        var response = await netService.Get(url);
        return JsonSerializer.Deserialize<List<LogElement>>(response) ?? [];
    }

    public async Task<List<UserLogElement>> GetUserLogs(int lastId = -1)
    {
        var url      = $"{BaseUrl}/peer?last_known_id={lastId}";
        var response = await netService.Get(url);
        return JsonSerializer.Deserialize<List<UserLogElement>>(response) ?? [];
    }

    public async Task<List<LogElement>> GetNormalLog(int lastId = -1) =>
        await GetLogs(normal : true, info : false, warning : false, critical : false, lastId);

    public async Task<List<LogElement>> GetInfoLog(int lastId = -1) =>
        await GetLogs(normal : false, info : true, warning : false, critical : false, lastId);

    public async Task<List<LogElement>> GetWarningLog(int lastId = -1) =>
        await GetLogs(normal : false, info : false, warning : true, critical : false, lastId);

    public async Task<List<LogElement>> GetCriticalLog(int lastId = -1) =>
        await GetLogs(normal : false, info : false, warning : false, critical : true, lastId);
}
