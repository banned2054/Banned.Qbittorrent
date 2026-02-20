namespace Banned.Qbittorrent.Models.Enums;

/// <summary>
/// 定义监控目录（Scan Directory）中发现新种子时的操作。<br/>
/// Defines the actions to be taken when a new torrent is found in a monitored directory.
/// </summary>
/// <remarks>
/// 用于配置 qBittorrent 如何处理自动监控文件夹中检测到的 .torrent 文件。<br/>
/// Used to configure how qBittorrent handles .torrent files detected in automatically monitored folders.
/// </remarks>
public enum EnumScanDirAction
{
    /// <summary>
    /// 禁用监控。<br/>
    /// Scan directory disabled.
    /// </summary>
    /// <remarks>
    /// 客户端将忽略该目录，不会自动添加其中的种子文件。<br/>
    /// The client will ignore this directory and will not automatically add torrent files found within it.
    /// </remarks>
    Disable = -1,

    /// <summary>
    /// 下载到默认保存路径。<br/>
    /// Download to the default save path.
    /// </summary>
    /// <remarks>
    /// 自动添加的种子将统一保存到全局设置中定义的默认下载目录。<br/>
    /// Automatically added torrents will be saved to the default download directory defined in the global settings.
    /// </remarks>
    DefaultSavePath = 0,

    /// <summary>
    /// 根据分类切换保存路径。<br/>
    /// Switch save path based on category.
    /// </summary>
    /// <remarks>
    /// 根据种子所属的分类（通常由子目录名称决定）自动分配保存路径。<br/>
    /// Automatically assigns the save path based on the category the torrent belongs to (typically determined by subdirectory names).
    /// </remarks>
    SwitchBasedOnCategory = 1
}
