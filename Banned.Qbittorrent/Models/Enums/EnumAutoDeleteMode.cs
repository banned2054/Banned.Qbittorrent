namespace Banned.Qbittorrent.Models.Enums;

/// <summary>
/// 表示种子文件在添加到下载列表后的自动删除模式。<br/>
/// Represents the auto-delete mode for torrent files after they are added.
/// </summary>
public enum EnumAutoDeleteMode
{
    /// <summary>
    /// 从不删除。<br/>
    /// Never delete.
    /// </summary>
    /// <remarks>
    /// 即使种子已成功添加，原始 .torrent 文件也会保留在监控目录中。<br/>
    /// The original .torrent file remains in the monitored directory even after the torrent is successfully added.
    /// </remarks>
    Never = 0,

    /// <summary>
    /// 成功添加后删除。<br/>
    /// Delete if successfully added.
    /// </summary>
    /// <remarks>
    /// 仅当 qBittorrent 成功加载并开始下载该种子后，才删除原始文件。<br/>
    /// The original file is deleted only after qBittorrent has successfully loaded and started the download.
    /// </remarks>
    IfAdded = 1,

    /// <summary>
    /// 总是删除。<br/>
    /// Always delete.
    /// </summary>
    /// <remarks>
    /// 无论添加是否成功（例如重复种子），都会尝试从监控目录中移除该文件。<br/>
    /// Attempt to remove the file from the monitored directory regardless of whether the addition was successful (e.g., duplicate torrents).
    /// </remarks>
    Always = 2
}
