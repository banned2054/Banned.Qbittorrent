namespace Banned.Qbittorrent.Models.Enums;

/// <summary>
/// 定义达到分享率限制后的操作。<br/>
/// Defines the actions to be taken when the ratio limit is reached.
/// </summary>
/// <remarks>
/// 当种子达到预设的分享率或分享时间限制时，客户端将执行此枚举指定的行为。<br/>
/// When a torrent reaches the preset ratio or seeding time limit, the client will execute the behavior specified by this enum.
/// </remarks>
public enum EnumMaxRatioAction
{
    /// <summary>
    /// 暂停种子。<br/>
    /// Pause the torrent.
    /// </summary>
    /// <remarks>
    /// 种子将停止传输并进入暂停状态，但保留在列表中且不删除任何数据。<br/>
    /// The torrent will stop transferring and enter a paused state, but remains in the list without deleting any data.
    /// </remarks>
    Pause = 0,

    /// <summary>
    /// 移除种子。<br/>
    /// Remove the torrent.
    /// </summary>
    /// <remarks>
    /// 从 qBittorrent 列表中删除该任务，但保留硬盘上的本地文件。<br/>
    /// Deletes the task from the qBittorrent list, but keeps the local files on the disk.
    /// </remarks>
    Remove = 1,

    /// <summary>
    /// 移除种子并删除文件。<br/>
    /// Remove the torrent and delete its files.
    /// </summary>
    /// <remarks>
    /// 从列表中删除该任务，并永久删除与其关联的所有本地数据文件。<br/>
    /// Deletes the task from the list and permanently deletes all associated local data files.
    /// </remarks>
    RemoveAndDeleteFiles = 2
}
