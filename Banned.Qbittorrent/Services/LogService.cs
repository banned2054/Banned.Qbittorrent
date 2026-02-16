using Banned.Qbittorrent.Models.Logging;
using System.Text.Json;

namespace Banned.Qbittorrent.Services;

/// <summary>
/// 提供与 qBittorrent 日志相关的服务。<br/>
/// Provides services related to qBittorrent logs.
/// </summary>
public class LogService(NetService netService)
{
    private const string BaseUrl = "/api/v2/log";

    /// <summary>
    /// 获取主日志。<br/>
    /// Get main logs.
    /// </summary>
    /// <param name="normal">是否包含普通级别日志。 / Include normal messages.</param>
    /// <param name="info">是否包含信息级别日志。 / Include info messages.</param>
    /// <param name="warning">是否包含警告级别日志。 / Include warning messages.</param>
    /// <param name="critical">是否包含严重错误级别日志。 / Include critical messages.</param>
    /// <param name="lastId">返回 ID 大于此值的日志（用于增量获取）。 / Returns logs with ID greater than this value.</param>
    /// <returns>
    /// 日志列表。<br/>
    /// A list of log elements.
    /// </returns>
    public async Task<List<LogElement>> GetLogs(bool normal   = true,
                                                bool info     = true,
                                                bool warning  = true,
                                                bool critical = true,
                                                int  lastId   = -1)
    {
        var url = $"{BaseUrl}/main?"                           +
                  $"normal={normal.ToString().ToLower()}&"     +
                  $"info={info.ToString().ToLower()}&"         +
                  $"warning={warning.ToString().ToLower()}&"   +
                  $"critical={critical.ToString().ToLower()}&" +
                  $"last_known_id={lastId}";

        var response = await netService.Get(url);
        return JsonSerializer.Deserialize<List<LogElement>>(response) ?? [];
    }

    /// <summary>
    /// 获取与用户（Peer）相关的日志。<br/>
    /// Get peer-related logs.
    /// </summary>
    /// <param name="lastId">返回 ID 大于此值的日志。 / Returns logs with ID greater than this value.</param>
    /// <returns>
    /// 用户日志列表。<br/>
    /// A list of user log elements.
    /// </returns>
    public async Task<List<UserLogElement>> GetUserLogs(int lastId = -1)
    {
        var url      = $"{BaseUrl}/peer?last_known_id={lastId}";
        var response = await netService.Get(url);
        return JsonSerializer.Deserialize<List<UserLogElement>>(response) ?? [];
    }

    /// <summary>
    /// 仅获取普通级别日志。<br/>
    /// Get only normal logs.
    /// </summary>
    /// <param name="lastId">上一个已知日志的 ID。 / The last known log ID.</param>
    /// <returns>普通日志列表。 / A list of normal log elements.</returns>
    public async Task<List<LogElement>> GetNormalLog(int lastId = -1) =>
        await GetLogs(normal : true, info : false, warning : false, critical : false, lastId);

    /// <summary>
    /// 仅获取信息级别日志。<br/>
    /// Get only info logs.
    /// </summary>
    /// <param name="lastId">上一个已知日志的 ID。 / The last known log ID.</param>
    /// <returns>信息日志列表。 / A list of info log elements.</returns>
    public async Task<List<LogElement>> GetInfoLog(int lastId = -1) =>
        await GetLogs(normal : false, info : true, warning : false, critical : false, lastId);

    /// <summary>
    /// 仅获取警告级别日志。<br/>
    /// Get only warning logs.
    /// </summary>
    /// <param name="lastId">上一个已知日志的 ID。 / The last known log ID.</param>
    /// <returns>警告日志列表。 / A list of warning log elements.</returns>
    public async Task<List<LogElement>> GetWarningLog(int lastId = -1) =>
        await GetLogs(normal : false, info : false, warning : true, critical : false, lastId);

    /// <summary>
    /// 仅获取严重错误级别日志。<br/>
    /// Get only critical logs.
    /// </summary>
    /// <param name="lastId">上一个已知日志的 ID。 / The last known log ID.</param>
    /// <returns>严重错误日志列表。 / A list of critical log elements.</returns>
    public async Task<List<LogElement>> GetCriticalLog(int lastId = -1) =>
        await GetLogs(normal : false, info : false, warning : false, critical : true, lastId);
}
