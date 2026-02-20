namespace Banned.Qbittorrent.Models.Enums;

/// <summary>
/// 表示种子列表的过滤类别。<br/>
/// Represents the filter category for the torrent list.
/// </summary>
public enum EnumTorrentFilter
{
    /// <summary>
    /// 所有种子。<br/>
    /// All torrents.
    /// </summary>
    All,

    /// <summary>
    /// 正在下载。<br/>
    /// Torrents currently downloading.
    /// </summary>
    Downloading,

    /// <summary>
    /// 正在做种。<br/>
    /// Torrents currently seeding.
    /// </summary>
    Seeding,

    /// <summary>
    /// 已完成。<br/>
    /// Completed torrents.
    /// </summary>
    Completed,

    /// <summary>
    /// 已暂停。<br/>
    /// Paused torrents.
    /// </summary>
    Paused,

    /// <summary>
    /// 活跃的（正在上传或下载）。<br/>
    /// Active torrents (currently uploading or downloading).
    /// </summary>
    Active,

    /// <summary>
    /// 不活跃的（未在上传或下载）。<br/>
    /// Inactive torrents (not currently uploading or downloading).
    /// </summary>
    Inactive,

    /// <summary>
    /// 已恢复（对应 Web API 中的 resumed 状态）。<br/>
    /// Resumed torrents.
    /// </summary>
    Resumed,

    /// <summary>
    /// 等待中（未开始传输）。<br/>
    /// Stalled torrents.
    /// </summary>
    Stalled,

    /// <summary>
    /// 等待上传中。<br/>
    /// Stalled uploading torrents.
    /// </summary>
    StalledUploading,

    /// <summary>
    /// 等待下载中。<br/>
    /// Stalled downloading torrents.
    /// </summary>
    StalledDownloading,

    /// <summary>
    /// 错误的。<br/>
    /// Torrents with errors.
    /// </summary>
    Error
}
