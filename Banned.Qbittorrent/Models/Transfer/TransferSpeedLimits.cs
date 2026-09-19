using System.Text.Json.Serialization;

namespace Banned.Qbittorrent.Models.Transfer;

/// <summary>
/// 表示全局与备用速度限制。<br/>
/// Represents the global and alternative speed limits.
/// </summary>
/// <remarks>
/// 对应 <c>transfer/getSpeedLimits</c>（qBittorrent v5.3.0 / Web API v2.16.0 引入），
/// 一次请求即可取得以前需要四次请求才能获得的四个限制值。<br/>
/// Corresponds to <c>transfer/getSpeedLimits</c>, introduced with qBittorrent v5.3.0
/// (Web API v2.16.0), which returns in one request the four limits that previously
/// required four separate requests.
/// </remarks>
public class TransferSpeedLimits
{
    /// <summary>
    /// 全局上传速度限制 (字节/秒)。<br/>
    /// Global upload speed limit (bytes/s).
    /// </summary>
    [JsonPropertyName("up_limit")]
    public long? UploadLimit { get; set; }

    /// <summary>
    /// 全局下载速度限制 (字节/秒)。<br/>
    /// Global download speed limit (bytes/s).
    /// </summary>
    [JsonPropertyName("dl_limit")]
    public long? DownloadLimit { get; set; }

    /// <summary>
    /// 备用上传速度限制 (字节/秒)。<br/>
    /// Alternative upload speed limit (bytes/s).
    /// </summary>
    [JsonPropertyName("alt_up_limit")]
    public long? AlternativeUploadLimit { get; set; }

    /// <summary>
    /// 备用下载速度限制 (字节/秒)。<br/>
    /// Alternative download speed limit (bytes/s).
    /// </summary>
    [JsonPropertyName("alt_dl_limit")]
    public long? AlternativeDownloadLimit { get; set; }
}
