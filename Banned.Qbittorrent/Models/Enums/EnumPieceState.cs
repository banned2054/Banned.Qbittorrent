namespace Banned.Qbittorrent.Models.Enums;

public enum EnumPieceState
{
    /// <summary>
    /// 还未开始下载<br/>
    /// Not downloaded yet
    /// </summary>
    None,

    /// <summary>
    /// 下载中<br/>
    /// Now downloading
    /// </summary>
    Downloading,

    /// <summary>
    /// 下载完成<br/>
    /// Already downloaded
    /// </summary>
    Finish
}
