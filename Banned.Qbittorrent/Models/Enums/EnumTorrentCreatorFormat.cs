namespace Banned.Qbittorrent.Models.Enums;

/// <summary>
/// 表示创建种子时使用的 BitTorrent 格式。<br/>
/// Represents the BitTorrent format used when creating a torrent.
/// </summary>
public enum EnumTorrentCreatorFormat
{
    /// <summary>BitTorrent v1 格式。<br/>BitTorrent v1 format.</summary>
    V1,

    /// <summary>BitTorrent v2 格式。<br/>BitTorrent v2 format.</summary>
    V2,

    /// <summary>同时兼容 v1 和 v2 的混合格式。<br/>Hybrid format compatible with v1 and v2.</summary>
    Hybrid
}
