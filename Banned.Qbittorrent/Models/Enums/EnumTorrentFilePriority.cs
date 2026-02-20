namespace Banned.Qbittorrent.Models.Enums;

/// <summary>
/// 表示种子里单个文件的下载优先级。<br/>
/// Represents the download priority of an individual file within a torrent.
/// </summary>
public enum EnumTorrentFilePriority
{
    /// <summary>
    /// 不下载。<br/>
    /// Do not download.
    /// </summary>
    DoNotDownload = 0,

    /// <summary>
    /// 正常优先级。<br/>
    /// Normal priority.
    /// </summary>
    Normal = 1,

    /// <summary>
    /// 高优先级。<br/>
    /// High priority.
    /// </summary>
    High = 6,

    /// <summary>
    /// 最高优先级。<br/>
    /// Maximum priority.
    /// </summary>
    Maximum = 7
}
