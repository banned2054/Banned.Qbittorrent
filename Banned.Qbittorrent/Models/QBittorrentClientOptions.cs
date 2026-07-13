using Banned.Qbittorrent.Models.Enums;

namespace Banned.Qbittorrent.Models;

/// <summary>
/// 配置 <see cref="QBittorrentClient"/> 的网络与重试行为。<br/>
/// Configures networking and retry behavior for <see cref="QBittorrentClient"/>.
/// </summary>
public sealed class QBittorrentClientOptions
{
    /// <summary>
    /// 获取调用者管理的 HTTP 客户端。设置后，库不会修改或释放该实例。<br/>
    /// Gets the caller-managed HTTP client. When set, the library neither modifies nor disposes it.
    /// </summary>
    public HttpClient? HttpClient { get; init; }

    /// <summary>获取每个请求的最大尝试次数。 / Gets the maximum number of attempts per request.</summary>
    public int MaxRetries { get; init; } = 3;

    /// <summary>获取请求总超时时间。 / Gets the overall request timeout.</summary>
    public TimeSpan Timeout { get; init; } = TimeSpan.FromSeconds(15);

    /// <summary>获取 TCP 连接建立超时时间。 / Gets the TCP connection establishment timeout.</summary>
    public TimeSpan ConnectTimeout { get; init; } = TimeSpan.FromSeconds(5);

    /// <summary>获取是否在网络故障异常中包含额外诊断信息。 / Gets whether network failure exceptions include additional diagnostics.</summary>
    public bool EnableDetailedLogging { get; init; }

    /// <summary>
    /// 获取可选的实时诊断接收器。该回调可能被并发调用，且回调异常不会影响客户端请求。<br/>
    /// Gets the optional real-time diagnostic sink. It may be invoked concurrently, and callback exceptions do not affect client requests.
    /// </summary>
    public Action<string>? DiagnosticSink { get; init; }

    /// <summary>获取默认 HTTP 客户端使用的地址族策略。 / Gets the address-family policy used by the default HTTP client.</summary>
    public AddressFamilyPreference AddressFamilyPreference { get; init; } = AddressFamilyPreference.System;

    /// <summary>获取系统策略连接失败后是否允许自动升级为 IPv4 优先。 / Gets whether a failed system-policy connection may automatically upgrade to IPv4-first.</summary>
    public bool EnableAutomaticIPv4Fallback { get; init; } = true;
}
