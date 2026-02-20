namespace Banned.Qbittorrent.Models.Enums;

/// <summary>
/// 表示 Tracker 的连接状态。<br/>
/// Represents the connection status of a tracker.
/// </summary>
public enum EnumTrackerStatus
{
    /// <summary>
    /// 已禁用（常用于 DHT、PeX 和 LSD）。<br/>
    /// Tracker is disabled (often used for DHT, PeX, and LSD).
    /// </summary>
    Disabled = 0,

    /// <summary>
    /// 尚未联系。<br/>
    /// Tracker has not been contacted yet.
    /// </summary>
    NotContacted = 1,

    /// <summary>
    /// 正在工作。<br/>
    /// Tracker has been contacted and is working.
    /// </summary>
    Working = 2,

    /// <summary>
    /// 正在更新。<br/>
    /// Tracker is currently updating.
    /// </summary>
    Updating = 3,

    /// <summary>
    /// 停止工作（已联系，但未收到正确回复）。<br/>
    /// Tracker has been contacted, but it is not working (or doesn't send proper replies).
    /// </summary>
    NotWorking = 4
}
