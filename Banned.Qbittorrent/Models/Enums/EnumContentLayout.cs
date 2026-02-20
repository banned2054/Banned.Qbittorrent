namespace Banned.Qbittorrent.Models.Enums;

/// <summary>
/// 表示种子内容的目录结构布局。<br/>
/// Represents the directory layout for torrent content.
/// </summary>
public enum EnumContentLayout
{
    /// <summary>
    /// 未知布局。<br/>
    /// Unknown layout.
    /// </summary>
    Unknown,

    /// <summary>
    /// 原始布局。<br/>
    /// Original layout.
    /// </summary>
    /// <remarks>
    /// 使用种子文件定义的原始目录结构。<br/>
    /// Use the original directory structure defined in the torrent file.
    /// </remarks>
    Original,

    /// <summary>
    /// 创建子文件夹。<br/>
    /// Create subfolder.
    /// </summary>
    /// <remarks>
    /// 无论种子是否包含根目录，始终为该种子创建一个同名子文件夹。<br/>
    /// Always create a subfolder with the same name as the torrent, regardless of whether the torrent contains a root directory.
    /// </remarks>
    Subfolder,

    /// <summary>
    /// 不创建子文件夹。<br/>
    /// Don't create subfolder.
    /// </summary>
    /// <remarks>
    /// 直接将所有文件平铺在保存路径下，不创建额外的根目录。<br/>
    /// Place all files directly in the save path without creating an additional root directory.
    /// </remarks>
    NoSubfolder
}
