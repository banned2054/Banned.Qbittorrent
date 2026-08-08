using Banned.Qbittorrent.Models.Application;
using Banned.Qbittorrent.Serialization;
using System.Text.Json;

namespace Banned.Qbittorrent.Services;

/// <summary>
/// 提供与 qBittorrent 应用程序相关的服务<br/>
/// Provides services related to qBittorrent application
/// </summary>
public class ApplicationService(NetService netService)
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
    /// <param name="cancellationToken">取消请求的令牌。<br/>Token used to cancel the request.</param>
    public async Task<ApiVersion> GetApiVersion(CancellationToken cancellationToken = default)
    {
        var result = await netService.Get($"{BaseUrl}/webapiVersion", skipAuthCheck : true, ct : cancellationToken);
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
    /// <param name="cancellationToken">取消请求的令牌。<br/>Token used to cancel the request.</param>
    public async Task<string> GetVersion(CancellationToken cancellationToken = default)
    {
        var result = await netService.Get($"{BaseUrl}/version", ct : cancellationToken);
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
    /// <param name="cancellationToken">取消请求的令牌。<br/>Token used to cancel the request.</param>
    public async Task<BuildInfo?> GetBuildInfo(CancellationToken cancellationToken = default)
    {
        var response = await netService.Get($"{BaseUrl}/buildInfo", ApiVersion.V2_3_0, ct : cancellationToken);
        return QBittorrentJsonSerializer.Deserialize<BuildInfo>(response);
    }

    /// <summary>
    /// 获取 qBittorrent 进程信息。<br/>
    /// Get qBittorrent process information.
    /// </summary>
    /// <returns>
    /// qBittorrent 进程信息。<br/>
    /// qBittorrent process information.
    /// </returns>
    /// <param name="cancellationToken">取消请求的令牌。<br/>Token used to cancel the request.</param>
    public async Task<ProcessInfo?> GetProcessInfo(CancellationToken cancellationToken = default)
    {
        var response = await netService.Get($"{BaseUrl}/processInfo", ApiVersion.V2_15_1, ct : cancellationToken);
        var result   = QBittorrentJsonSerializer.Deserialize<ProcessInfo>(response);
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
    /// <param name="cancellationToken">取消请求的令牌。<br/>Token used to cancel the request.</param>
    public async Task ShutDown(CancellationToken cancellationToken = default)
    {
        await netService.Post($"{BaseUrl}/shutdown", ct : cancellationToken);
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
    /// <param name="cancellationToken">取消请求的令牌。<br/>Token used to cancel the request.</param>
    public async Task<ApplicationPreferences?> GetApplicationPreferences(CancellationToken cancellationToken = default)
    {
        var response = await netService.Get($"{BaseUrl}/preferences", ct : cancellationToken);
        var result   = QBittorrentJsonSerializer.Deserialize<ApplicationPreferences>(response);
        return result;
    }

    /// <summary>
    /// 设置应用程序偏好设置。<br/>
    /// Set application preferences.
    /// </summary>
    /// <param name="applicationPreferences">
    /// 要更新的应用程序偏好设置对象。<br/>
    /// The application preference settings to be updated.
    /// </param>
    /// <remarks>
    /// 请求会将参数序列化为 JSON，并作为 <c>json</c> 字段提交到 Web API。<br/>
    /// The request serializes the preferences into JSON and submits it to the Web API as the <c>json</c> field.
    /// </remarks>
    /// <param name="cancellationToken">取消请求的令牌。<br/>Token used to cancel the request.</param>
    public async Task SetApplicationPreferences(ApplicationPreferences applicationPreferences,
                                                CancellationToken      cancellationToken = default)
    {
        var request = QBittorrentJsonSerializer.SerializeIgnoringNulls(applicationPreferences);
        var parameters = new Dictionary<string, string>
        {
            { "json", request }
        };

        await netService.Post($"{BaseUrl}/setPreferences", parameters, ct : cancellationToken);
    }

    /// <summary>
    /// 获取默认保存路径。<br/>
    /// Get the default save path.
    /// </summary>
    /// <returns>
    /// 默认保存路径字符串。<br/>
    /// The default save path string.
    /// </returns>
    /// <param name="cancellationToken">取消请求的令牌。<br/>Token used to cancel the request.</param>
    public async Task<string> GetDefaultSavePath(CancellationToken cancellationToken = default)
    {
        var result = await netService.Get($"{BaseUrl}/defaultSavePath", ct : cancellationToken);
        return result;
    }

    /// <summary>
    /// 获取应用程序正在使用的所有 Cookie。<br/>
    /// Get all cookies used by the application.
    /// </summary>
    /// <returns>
    /// Cookie 列表。<br/>
    /// A list of cookies.
    /// </returns>
    /// <param name="cancellationToken">取消请求的令牌。<br/>Token used to cancel the request.</param>
    public async Task<List<Cookie>> GetCookies(CancellationToken cancellationToken = default)
    {
        var response = await netService.Get($"{BaseUrl}/cookies", ApiVersion.V2_11_3, ct : cancellationToken);
        var result   = QBittorrentJsonSerializer.Deserialize<List<Cookie>>(response);
        return result ?? [];
    }

    /// <summary>
    /// 设置应用程序的 Cookie。<br/>
    /// Set cookies for the application.
    /// </summary>
    /// <param name="cookies">
    /// 要设置的 Cookie 列表。<br/>
    /// The list of cookies to be set.
    /// </param>
    /// <remarks>
    /// 列表中的每个 Cookie 必须包含 name, domain, path 和 value 字段。<br/>
    /// Each cookie in the list must contain name, domain, path, and value fields.
    /// </remarks>
    /// <param name="cancellationToken">取消请求的令牌。<br/>Token used to cancel the request.</param>
    public async Task SetCookies(List<Cookie> cookies, CancellationToken cancellationToken = default)
    {
        var request = QBittorrentJsonSerializer.SerializeIgnoringNulls(cookies);
        var parameters = new Dictionary<string, string>
        {
            { "json", request }
        };
        await netService.Post($"{BaseUrl}/setCookies", parameters, ApiVersion.V2_11_3, ct : cancellationToken);
    }

    /// <summary>
    /// 获取 qBittorrent 主机上的网络接口。<br/>
    /// Gets the network interfaces on the qBittorrent host.
    /// </summary>
    /// <returns>网络接口列表。<br/>The network interface list.</returns>
    /// <param name="cancellationToken">取消请求的令牌。<br/>Token used to cancel the request.</param>
    public async Task<List<NetworkInterfaceInfo>> GetNetworkInterfaces(CancellationToken cancellationToken = default)
    {
        var response =
            await netService.Get($"{BaseUrl}/networkInterfaceList", ApiVersion.V2_3_0, ct : cancellationToken);
        return QBittorrentJsonSerializer.Deserialize<List<NetworkInterfaceInfo>>(response) ?? [];
    }

    /// <summary>
    /// 获取指定网络接口的地址；接口名为空时返回所有地址。<br/>
    /// Gets addresses for a network interface, or all addresses when the interface name is empty.
    /// </summary>
    /// <param name="interfaceName">网络接口名称。<br/>Network interface name.</param>
    /// <param name="cancellationToken">取消请求的令牌。<br/>Token used to cancel the request.</param>
    /// <returns>网络地址列表。<br/>The network address list.</returns>
    public async Task<List<string>> GetNetworkInterfaceAddresses(string            interfaceName     = "",
                                                                 CancellationToken cancellationToken = default)
    {
        var parameters = new Dictionary<string, string> { { "iface", interfaceName } };
        var response = await netService.Post($"{BaseUrl}/networkInterfaceAddressList", parameters, ApiVersion.V2_3_0,
                                             ct : cancellationToken);
        return QBittorrentJsonSerializer.Deserialize<List<string>>(response) ?? [];
    }

    /// <summary>
    /// 使用当前应用程序设置发送测试邮件。<br/>
    /// Sends a test email using the current application settings.
    /// </summary>
    /// <param name="cancellationToken">取消请求的令牌。<br/>Token used to cancel the request.</param>
    public async Task SendTestEmail(CancellationToken cancellationToken = default) =>
        await netService.Post($"{BaseUrl}/sendTestEmail", targetVersion : ApiVersion.V2_10_4, ct : cancellationToken);

    /// <summary>
    /// 获取目录中的文件和子目录路径。<br/>
    /// Gets file and subdirectory paths in a directory.
    /// </summary>
    /// <param name="directoryPath">qBittorrent 主机上的目录路径。<br/>Directory path on the qBittorrent host.</param>
    /// <param name="cancellationToken">取消请求的令牌。<br/>Token used to cancel the request.</param>
    /// <returns>目录内容路径列表。<br/>The directory content paths.</returns>
    public async Task<List<string>> GetDirectoryContent(string            directoryPath,
                                                        CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(directoryPath);
        var parameters = new Dictionary<string, string>
        {
            { "dirPath", directoryPath },
            { "withMetadata", "false" }
        };
        var response = await netService.Post($"{BaseUrl}/getDirectoryContent", parameters, ApiVersion.V2_11_0,
                                             ct : cancellationToken);
        return QBittorrentJsonSerializer.Deserialize<List<string>>(response) ?? [];
    }

    /// <summary>
    /// 获取包含元数据的目录内容。<br/>
    /// Gets directory contents including metadata.
    /// </summary>
    /// <param name="directoryPath">qBittorrent 主机上的目录路径。<br/>Directory path on the qBittorrent host.</param>
    /// <param name="cancellationToken">取消请求的令牌。<br/>Token used to cancel the request.</param>
    /// <returns>每个目录项的元数据。<br/>Metadata for each directory entry.</returns>
    public async Task<List<Dictionary<string, JsonElement>>> GetDirectoryContentWithMetadata(
        string directoryPath, CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(directoryPath);
        var parameters = new Dictionary<string, string>
        {
            { "dirPath", directoryPath },
            { "withMetadata", "true" }
        };
        var response = await netService.Post($"{BaseUrl}/getDirectoryContent", parameters, ApiVersion.V2_11_8,
                                             ct : cancellationToken);
        return QBittorrentJsonSerializer.Deserialize<List<Dictionary<string, JsonElement>>>(response) ?? [];
    }
}
