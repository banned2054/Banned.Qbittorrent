namespace Banned.Qbittorrent.Models.Enums;

/// <summary>
/// 添加 Torrent 时自动停止的条件。<br/>
/// Condition for automatically stopping a torrent while adding it.
/// </summary>
public enum EnumTorrentAddStopCondition
{
    /// <summary>收到 Torrent 元数据后停止。 / Stop after torrent metadata is received.</summary>
    MetadataReceived,

    /// <summary>文件校验完成后停止。 / Stop after the files have been checked.</summary>
    FilesChecked
}
