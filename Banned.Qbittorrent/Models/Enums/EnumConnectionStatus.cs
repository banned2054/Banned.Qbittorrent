namespace Banned.Qbittorrent.Models.Enums;

/// <summary>
/// 表示全局连接状态。<br/>
/// Represents the global connection status.
/// </summary>
public enum EnumConnectionStatus
{
    /// <summary>未知状态。 / Unknown status.</summary>
    Unknown,

    /// <summary>已连接。 / Connected.</summary>
    Connected,

    /// <summary>处于防火墙后（可能存在连通性问题）。 / Behind a firewall (possible connectivity issues).</summary>
    Firewalled,

    /// <summary>已断开连接。 / Disconnected.</summary>
    Disconnected
}
