namespace Banned.Qbittorrent.Models.Enums;

/// <summary>
/// 表示搜索任务的状态。<br/>
/// Represents the status of a search job.
/// </summary>
public enum EnumSearchStatus
{
    /// <summary>
    /// 正在运行。<br/>
    /// The search job is currently running.
    /// </summary>
    Running,

    /// <summary>
    /// 已停止。<br/>
    /// The search job has been stopped.
    /// </summary>
    Stopped,

    /// <summary>
    /// 未知状态。<br/>
    /// The search job status is unknown.
    /// </summary>
    Unknown,
}
