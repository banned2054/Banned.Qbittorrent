using Banned.Qbittorrent.Models.Application;
using Banned.Qbittorrent.Utils;
using System.Text.Json;

namespace Banned.Qbittorrent.Services;

/// <summary>
/// 提供与 qBittorrent 应用程序相关的服务<br/>
/// Provides services related to qBittorrent application
/// </summary>
public class ApplicationService(NetUtils netUtils)
{
    private const string BaseUrl = "/api/v2/app";

    /// <summary>
    /// 获取 Web API 版本。<br/>
    /// Get the Web API version.
    /// </summary>
    /// <returns>
    /// Web API 版本信息。<br/>
    /// Web API version information.
    /// </returns>
    public async Task<ApiVersion> GetApiVersion()
    {
        var result = await netUtils.Get($"{BaseUrl}/webapiVersion");
        return ApiVersion.Parse(result);
    }

    /// <summary>
    /// 获取 qBittorrent 版本号。<br/>
    /// Get the qBittorrent version number.
    /// </summary>
    /// <returns>
    /// qBittorrent 版本号。<br/>
    /// qBittorrent version string.
    /// </returns>
    public async Task<string> GetVersion()
    {
        var result = await netUtils.Get($"{BaseUrl}/version");
        return result;
    }

    /// <summary>
    /// 获取构建信息。<br/>
    /// Get build information.
    /// </summary>
    /// <returns>
    /// 构建信息。<br/>
    /// Build information.
    /// </returns>
    public async Task<string> GetBuildInfo()
    {
        var result = await netUtils.Get($"{BaseUrl}/buildInfo");
        return result;
    }

    /// <summary>
    /// 关闭 qBittorrent 客户端。<br/>
    /// Shut down the qBittorrent client.
    /// </summary>
    /// <remarks>
    /// 此操作会向 qBittorrent Web API 发送关闭请求，通常需要管理员权限。<br/>
    /// This operation sends a shutdown request to the qBittorrent Web API, which usually requires administrative privileges.
    /// </remarks>
    public async Task ShutDown()
    {
        await netUtils.Post($"{BaseUrl}/shutdown");
    }

    /// <summary>
    /// 获取应用程序偏好设置。<br/>
    /// Get application preferences.
    /// </summary>
    /// <returns>
    /// 应用程序的所有可用偏好设置；字段取决于 qBittorrent.ini 的内容。<br/>
    /// All available application preference settings; fields vary depending on qBittorrent.ini.
    /// </returns>
    /// <remarks>
    /// 成功时返回 HTTP 200，并返回 JSON 对象，包含应用程序设置的键值对。<br/>
    /// On success, returns HTTP 200 with a JSON object containing key-value pairs of application settings.
    /// </remarks>
    public async Task<ApplicationPreferences?> GetApplicationPreferences()
    {
        var response = await netUtils.Get($"{BaseUrl}/preferences");
        var result   = JsonSerializer.Deserialize<ApplicationPreferences>(response);
        return result;
    }
}
