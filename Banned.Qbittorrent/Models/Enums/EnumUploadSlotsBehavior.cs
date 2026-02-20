namespace Banned.Qbittorrent.Models.Enums;

/// <summary>
/// 表示上传槽位的分配行为。<br/>
/// Represents the behavior for upload slots allocation.
/// </summary>
public enum EnumUploadSlotsBehavior
{
    /// <summary>
    /// 固定数量。<br/>
    /// Fixed number of upload slots.
    /// </summary>
    Fixed = 0,

    /// <summary>
    /// 自动分配。<br/>
    /// Automatically manage the number of upload slots.
    /// </summary>
    Auto = 1
}
