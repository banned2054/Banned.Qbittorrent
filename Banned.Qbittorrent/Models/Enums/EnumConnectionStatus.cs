namespace Banned.Qbittorrent.Models.Enums;

/// <summary>
/// 表示 qBittorrent 客户端的全局网络连接状态。<br/>
/// Represents the global network connection status of the qBittorrent client.
/// </summary>
public enum EnumConnectionStatus
{
    /// <summary>
    /// 未知状态。<br/>
    /// Unknown status.
    /// </summary>
    Unknown,

    /// <summary>
    /// 已连接。<br/>
    /// Connected.
    /// </summary>
    /// <remarks>
    /// 表示客户端已成功连接到 Peer 节点且网络通畅。<br/>
    /// Indicates that the client is successfully connected to peers and the network is clear.
    /// </remarks>
    Connected,

    /// <summary>
    /// 处于防火墙后。<br/>
    /// Firewalled.
    /// </summary>
    /// <remarks>
    /// 表示可能存在连通性问题，通常是由于端口转发未正确配置导致的。<br/>
    /// Indicates possible connectivity issues, usually caused by improperly configured port forwarding.
    /// </remarks>
    Firewalled,

    /// <summary>
    /// 已断开连接。<br/>
    /// Disconnected.
    /// </summary>
    /// <remarks>
    /// 客户端当前未连接到任何网络。<br/>
    /// The client is currently not connected to any network.
    /// </remarks>
    Disconnected
}
