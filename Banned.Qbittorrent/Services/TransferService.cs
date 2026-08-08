using Banned.Qbittorrent.Models.Application;
using Banned.Qbittorrent.Models.Transfer;
using Banned.Qbittorrent.Serialization;
using Banned.Qbittorrent.Utils;

namespace Banned.Qbittorrent.Services;

/// <summary>
/// 提供与数据传输相关的服务，包括速度限制和 Peer 管理。<br/>
/// Provides services related to data transfer, including speed limits and peer management.
/// </summary>
public class TransferService(NetService netService)
{
    private const string BaseUrl = "/api/v2/transfer";

    /// <summary>
    /// 获取全局传输信息。<br/>
    /// Get global transfer information.
    /// </summary>
    /// <returns>
    /// 包含速度、数据量及连接状态的传输信息对象。<br/>
    /// A transfer info object containing speed, data usage, and connection status.
    /// </returns>
    /// <param name="cancellationToken">取消请求的令牌。<br/>Token used to cancel the request.</param>
    public async Task<TransferInfo> GetTransferInfo(CancellationToken cancellationToken = default)
    {
        var response = await netService.Get($"{BaseUrl}/info", ct : cancellationToken);
        return QBittorrentJsonSerializer.Deserialize<TransferInfo>(response) ?? new TransferInfo();
    }

    /// <summary>
    /// 获取当前是否启用了备用速度限制（慢速模式）。<br/>
    /// Get whether alternative speed limits (slow mode) are currently enabled.
    /// </summary>
    /// <returns>
    /// 启用返回 true，否则返回 false。<br/>
    /// Returns true if enabled, otherwise false.
    /// </returns>
    /// <param name="cancellationToken">取消请求的令牌。<br/>Token used to cancel the request.</param>
    public async Task<bool> GetIsAlternativeSpeedLimitsEnabled(CancellationToken cancellationToken = default)
    {
        var response = await netService.Get($"{BaseUrl}/speedLimitsMode", ct : cancellationToken);
        return response == "1";
    }

    /// <summary>
    /// 切换备用速度限制的状态。<br/>
    /// Toggle the state of alternative speed limits.
    /// </summary>
    /// <param name="cancellationToken">取消请求的令牌。<br/>Token used to cancel the request.</param>
    public async Task ToggleAlternativeSpeedLimits(CancellationToken cancellationToken = default)
    {
        await netService.Post($"{BaseUrl}/toggleSpeedLimitsMode", ct : cancellationToken);
    }

    /// <summary>
    /// 获取全局下载速度限制。<br/>
    /// Get global download speed limit.
    /// </summary>
    /// <returns>
    /// 下载限制（字节/秒）。<br/>
    /// Download limit (bytes/s).
    /// </returns>
    /// <param name="cancellationToken">取消请求的令牌。<br/>Token used to cancel the request.</param>
    public async Task<long> GetGlobalDownloadLimit(CancellationToken cancellationToken = default)
    {
        var result = await netService.Get($"{BaseUrl}/downloadLimit", ct : cancellationToken);
        return long.Parse(result);
    }

    /// <summary>
    /// 设置全局下载速度限制。<br/>
    /// Set global download speed limit.
    /// </summary>
    /// <param name="limit">下载限制（字节/秒）。 / Download limit (bytes/s).</param>
    /// <param name="cancellationToken">取消请求的令牌。<br/>Token used to cancel the request.</param>
    public async Task SetGlobalDownloadLimit(long limit, CancellationToken cancellationToken = default)
    {
        var parameters = new Dictionary<string, string>
        {
            { "limit", limit.ToString() }
        };
        await netService.Post($"{BaseUrl}/setDownloadLimit", parameters, ct : cancellationToken);
    }

    /// <summary>
    /// 获取全局上传速度限制。<br/>
    /// Get global upload speed limit.
    /// </summary>
    /// <returns>
    /// 上传限制（字节/秒）。<br/>
    /// Upload limit (bytes/s).
    /// </returns>
    /// <param name="cancellationToken">取消请求的令牌。<br/>Token used to cancel the request.</param>
    public async Task<long> GetGlobalUploadLimit(CancellationToken cancellationToken = default)
    {
        var result = await netService.Get($"{BaseUrl}/uploadLimit", ct : cancellationToken);
        return long.Parse(result);
    }

    /// <summary>
    /// 设置全局上传速度限制。<br/>
    /// Set global upload speed limit.
    /// </summary>
    /// <param name="limit">上传限制（字节/秒）。 / Upload limit (bytes/s).</param>
    /// <param name="cancellationToken">取消请求的令牌。<br/>Token used to cancel the request.</param>
    public async Task SetGlobalUploadLimit(long limit, CancellationToken cancellationToken = default)
    {
        var parameters = new Dictionary<string, string>
        {
            { "limit", limit.ToString() }
        };
        await netService.Post($"{BaseUrl}/setUploadLimit", parameters, ct : cancellationToken);
    }

    /// <summary>
    /// 封禁指定的 Peer。<br/>
    /// Ban specified peers.
    /// </summary>
    /// <param name="peers">要封禁的 Peer 列表（格式通常为 IP:Port）。 / List of peers to ban (usually in IP:Port format).</param>
    /// <param name="cancellationToken">取消请求的令牌。<br/>Token used to cancel the request.</param>
    public async Task BanPeers(List<string> peers, CancellationToken cancellationToken = default)
    {
        var parameters = new Dictionary<string, string>
        {
            { "peers", StringUtils.Join('|', peers) }
        };
        await netService.Post($"{BaseUrl}/banPeers", parameters, ApiVersion.V2_3_0, ct : cancellationToken);
    }
}
