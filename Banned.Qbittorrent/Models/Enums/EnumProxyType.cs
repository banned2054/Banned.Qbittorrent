namespace Banned.Qbittorrent.Models.Enums;

public enum EnumProxyType
{
    Disabled                    = 0,
    HttpWithoutAuthentication   = 1,
    Socks5WithoutAuthentication = 2,
    HttpWithAuthentication      = 3,
    Socks5WithAuthentication    = 4,
    Socks4WithAuthentication    = 5,
}
