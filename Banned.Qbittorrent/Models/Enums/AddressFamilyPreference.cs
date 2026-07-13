namespace Banned.Qbittorrent.Models.Enums;

/// <summary>
/// 指定默认 HTTP 连接使用的网络地址族顺序。<br/>
/// Specifies the network address-family order used by default HTTP connections.
/// </summary>
public enum AddressFamilyPreference
{
    /// <summary>使用操作系统和 .NET 的默认连接策略。 / Uses the operating system and .NET default connection policy.</summary>
    System,

    /// <summary>先尝试 IPv4，失败后再尝试 IPv6。 / Tries IPv4 before falling back to IPv6.</summary>
    PreferIPv4,

    /// <summary>先尝试 IPv6，失败后再尝试 IPv4。 / Tries IPv6 before falling back to IPv4.</summary>
    PreferIPv6
}
