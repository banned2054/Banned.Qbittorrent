namespace Banned.Qbittorrent.Models.Enums;

/// <summary>
/// 定义 qBittorrent 使用的网络代理类型。<br/>
/// Defines the types of network proxies used by qBittorrent.
/// </summary>
/// <remarks>
/// 用于配置客户端如何通过中间服务器连接到 Tracker 或 Peer。<br/>
/// Used to configure how the client connects to trackers or peers through an intermediary server.
/// </remarks>
public enum EnumProxyType
{
    /// <summary>
    /// 禁用代理。<br/>
    /// Proxy disabled.
    /// </summary>
    /// <remarks>
    /// 客户端将直接连接到互联网，不使用任何代理服务器。<br/>
    /// The client will connect directly to the internet without using any proxy server.
    /// </remarks>
    Disabled = 0,

    /// <summary>
    /// 不带身份验证的 HTTP 代理。<br/>
    /// HTTP proxy without authentication.
    /// </summary>
    /// <remarks>
    /// 使用标准的 HTTP 协议代理，无需用户名和密码。<br/>
    /// Uses standard HTTP protocol proxy without requiring a username and password.
    /// </remarks>
    HttpWithoutAuthentication = 1,

    /// <summary>
    /// 不带身份验证的 SOCKS5 代理。<br/>
    /// SOCKS5 proxy without authentication.
    /// </summary>
    /// <remarks>
    /// 使用 SOCKS5 协议代理，支持 UDP 转发，无需身份验证。<br/>
    /// Uses SOCKS5 protocol proxy supporting UDP forwarding without authentication.
    /// </remarks>
    Socks5WithoutAuthentication = 2,

    /// <summary>
    /// 带身份验证的 HTTP 代理。<br/>
    /// HTTP proxy with authentication.
    /// </summary>
    /// <remarks>
    /// 使用 HTTP 协议代理，并需要提供凭据（用户名/密码）进行验证。<br/>
    /// Uses HTTP protocol proxy and requires credentials (username/password) for verification.
    /// </remarks>
    HttpWithAuthentication = 3,

    /// <summary>
    /// 带身份验证的 SOCKS5 代理。<br/>
    /// SOCKS5 proxy with authentication.
    /// </summary>
    /// <remarks>
    /// 使用 SOCKS5 协议代理，并需要提供凭据进行验证，安全性更高。<br/>
    /// Uses SOCKS5 protocol proxy and requires credentials for verification, providing better security.
    /// </remarks>
    Socks5WithAuthentication = 4,

    /// <summary>
    /// 带身份验证的 SOCKS4 代理。<br/>
    /// SOCKS4 proxy with authentication.
    /// </summary>
    /// <remarks>
    /// 使用较旧的 SOCKS4 协议代理，通常不支持 UDP，需要身份验证。<br/>
    /// Uses the older SOCKS4 protocol proxy, typically without UDP support, requiring authentication.
    /// </remarks>
    Socks4WithAuthentication = 5,
}
