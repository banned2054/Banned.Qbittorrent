namespace Banned.Qbittorrent.Models.Enums;

/// <summary>
/// 表示 uTP 与 TCP 混合流量模式。<br/>
/// Represents the uTP-TCP mixed traffic mode.
/// </summary>
public enum EnumUtpTcpMixedMode
{
    /// <summary>
    /// 优先使用 TCP。<br/>
    /// Prefer TCP connections.
    /// </summary>
    PreferTcp = 0,

    /// <summary>
    /// 与用户数量成比例（根据对端支持情况自动选择）。<br/>
    /// Peer proportional (chooses based on peer support).
    /// </summary>
    PeerProportional = 1
}
