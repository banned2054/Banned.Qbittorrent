namespace Banned.Qbittorrent.Models.Enums;

/// <summary>
/// Torrent 分享限制的组合判定模式。<br/>
/// How multiple share limits are combined once they are reached.
/// </summary>
/// <remarks>
/// Add Torrent 和 share-limit 操作中的该参数由 Web API v2.16.0 引入。<br/>
/// This parameter was introduced for Add Torrent and share-limit operations with Web API v2.16.0.
/// </remarks>
public enum EnumTorrentShareLimitsMode
{
    /// <summary>使用 qBittorrent 默认行为。 / Use the qBittorrent default behavior.</summary>
    Default,

    /// <summary>任一限制达到即触发。 / Trigger when any share limit is reached.</summary>
    MatchAny,

    /// <summary>所有限制均达到才触发。 / Trigger only when all share limits are reached.</summary>
    MatchAll
}
