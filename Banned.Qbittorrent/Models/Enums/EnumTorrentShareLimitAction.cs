namespace Banned.Qbittorrent.Models.Enums;

/// <summary>
/// Torrent 达到分享限制后执行的操作。<br/>
/// Action performed after a torrent reaches its share limit.
/// </summary>
public enum EnumTorrentShareLimitAction
{
    /// <summary>停止 Torrent。 / Stop the torrent.</summary>
    Stop,

    /// <summary>移除 Torrent。 / Remove the torrent.</summary>
    Remove,

    /// <summary>移除 Torrent 及其内容。 / Remove the torrent and its content.</summary>
    RemoveWithContent,

    /// <summary>启用超级做种。 / Enable super seeding.</summary>
    EnableSuperSeeding
}
