namespace Banned.Qbittorrent.Models.Enums;

/// <summary>
/// 表示 RSS 规则的停止条件。<br/>
/// Represents the stop condition for RSS rules.
/// </summary>
public enum EnumStopCondition
{
    /// <summary>未知或未设置。 / Unknown or not set.</summary>
    Unknown,

    /// <summary>从不停止。 / Never stop.</summary>
    Never,

    /// <summary>一旦获取到元数据就停止。 / Stop once metadata is received.</summary>
    MetadataReceived,

    /// <summary>一旦种子被添加就停止。 / Stop once the torrent is added.</summary>
    TorrentAdded
}
