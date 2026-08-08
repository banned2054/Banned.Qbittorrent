namespace Banned.Qbittorrent.Models.Enums;

/// <summary>
/// 表示种子创建任务的状态。<br/>
/// Represents the status of a torrent creation task.
/// </summary>
public enum EnumTorrentCreatorTaskStatus
{
    /// <summary>未知状态。<br/>Unknown status.</summary>
    Unknown,

    /// <summary>任务失败。<br/>The task failed.</summary>
    Failed,

    /// <summary>任务正在排队。<br/>The task is queued.</summary>
    Queued,

    /// <summary>任务正在运行。<br/>The task is running.</summary>
    Running,

    /// <summary>任务已完成。<br/>The task finished.</summary>
    Finished
}
