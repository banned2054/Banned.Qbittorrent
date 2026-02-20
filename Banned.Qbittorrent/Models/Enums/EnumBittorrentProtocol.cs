namespace Banned.Qbittorrent.Models.Enums;

/// <summary>
/// 表示 qBittorrent 使用的 BitTorrent 传输协议。<br/>
/// Represents the BitTorrent transfer protocol used by qBittorrent.
/// </summary>
public enum EnumBittorrentProtocol
{
    /// <summary>
    /// 同时使用 TCP 和 uTP 协议。<br/>
    /// Use both TCP and uTP protocols.
    /// </summary>
    /// <remarks>
    /// 这是默认推荐设置，可以兼顾兼容性与性能。<br/>
    /// This is the recommended default setting for a balance between compatibility and performance.
    /// </remarks>
    Both = 0,

    /// <summary>
    /// 仅使用 TCP 协议。<br/>
    /// Use TCP protocol only.
    /// </summary>
    /// <remarks>
    /// 标准传输控制协议，在某些网络环境下比 uTP 更稳定。<br/>
    /// Standard Transmission Control Protocol, which may be more stable than uTP in certain network environments.
    /// </remarks>
    Tcp = 1,

    /// <summary>
    /// 仅使用 uTP 协议。<br/>
    /// Use uTP protocol only.
    /// </summary>
    /// <remarks>
    /// 基于 UDP 的微传输协议，旨在减少对网络延迟的影响。<br/>
    /// Micro Transport Protocol based on UDP, designed to reduce the impact on network latency.
    /// </remarks>
    UTp = 2
}
