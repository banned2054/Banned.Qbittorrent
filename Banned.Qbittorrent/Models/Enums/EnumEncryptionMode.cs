namespace Banned.Qbittorrent.Models.Enums;

/// <summary>
/// 表示 BitTorrent 传输的加密模式策略。<br/>
/// Represents the encryption mode policy for BitTorrent transfers.
/// </summary>
public enum EnumEncryptionMode
{
    /// <summary>
    /// 优先使用加密连接。<br/>
    /// Prefer encryption.
    /// </summary>
    /// <remarks>
    /// 客户端将尝试发起加密连接，但若对方不支持，则回退到明文连接。这是兼容性最好的设置。<br/>
    /// The client will attempt to initiate encrypted connections, but will fall back to plaintext if the peer doesn't support it. This is the most compatible setting.
    /// </remarks>
    Prefer = 0,

    /// <summary>
    /// 强制开启加密连接。<br/>
    /// Require encryption (Force ON).
    /// </summary>
    /// <remarks>
    /// 客户端仅与支持协议加密的 Peer 节点通信，会拒绝所有明文连接。这能提高隐私性，但可能减少可用 Peer 数量。<br/>
    /// The client will only communicate with peers that support protocol encryption and will reject all plaintext connections. This improves privacy but may reduce the number of available peers.
    /// </remarks>
    ForceOn = 1,

    /// <summary>
    /// 强制禁用加密连接。<br/>
    /// Disable encryption (Force OFF).
    /// </summary>
    /// <remarks>
    /// 客户端将仅使用明文进行通信，不使用任何协议加密。<br/>
    /// The client will communicate using plaintext only and will not use any protocol encryption.
    /// </remarks>
    ForceOff = 2
}
