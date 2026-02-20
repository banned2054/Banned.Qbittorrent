namespace Banned.Qbittorrent.Models.Enums;

/// <summary>
/// 表示种子的详细运行状态。<br/>
/// Represents the detailed running state of a torrent.
/// </summary>
public enum EnumTorrentState
{
    /// <summary>
    /// 发生错误。<br/>
    /// An error occurred.
    /// </summary>
    Error,

    /// <summary>
    /// 文件丢失。<br/>
    /// Files are missing.
    /// </summary>
    MissingFiles,

    /// <summary>
    /// 正在上传。<br/>
    /// Torrent is being uploaded.
    /// </summary>
    Uploading,

    /// <summary>
    /// 已停止上传（已完成并暂停）。<br/>
    /// Uploading has stopped (completed and paused).
    /// </summary>
    StoppedUpload,

    /// <summary>
    /// 队列中等待上传。<br/>
    /// Queued for upload.
    /// </summary>
    QueuedUpload,

    /// <summary>
    /// 等待上传（无上传连接）。<br/>
    /// Uploading is stalled (no upload connections).
    /// </summary>
    StalledUpload,

    /// <summary>
    /// 正在校验已上传的数据。<br/>
    /// Checking uploaded data.
    /// </summary>
    CheckingUpload,

    /// <summary>
    /// 强制上传。<br/>
    /// Forced upload.
    /// </summary>
    ForcedUpload,

    /// <summary>
    /// 正在分配磁盘空间。<br/>
    /// Allocating disk space.
    /// </summary>
    Allocating,

    /// <summary>
    /// 正在下载。<br/>
    /// Torrent is being downloaded.
    /// </summary>
    Downloading,

    /// <summary>
    /// 正在下载元数据。<br/>
    /// Fetching torrent metadata.
    /// </summary>
    MetaDownload,

    /// <summary>
    /// 已停止下载（未完成并暂停）。<br/>
    /// Downloading has stopped (incomplete and paused).
    /// </summary>
    StoppedDownload,

    /// <summary>
    /// 队列中等待下载。<br/>
    /// Queued for download.
    /// </summary>
    QueuedDownload,

    /// <summary>
    /// 等待下载（无下载连接）。<br/>
    /// Downloading is stalled (no download connections).
    /// </summary>
    StalledDownload,

    /// <summary>
    /// 正在校验已下载的数据。<br/>
    /// Checking downloaded data.
    /// </summary>
    CheckingDownload,

    /// <summary>
    /// 强制下载。<br/>
    /// Forced download.
    /// </summary>
    ForcedDownload,

    /// <summary>
    /// 正在校验恢复数据。<br/>
    /// Checking resume data.
    /// </summary>
    CheckingResumeData,

    /// <summary>
    /// 正在移动文件。<br/>
    /// Torrent is being moved.
    /// </summary>
    Moving,

    /// <summary>
    /// 未知状态。<br/>
    /// Unknown status.
    /// </summary>
    Unknown,
}
