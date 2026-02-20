namespace Banned.Qbittorrent.Models.Enums;

/// <summary>
/// 表示种子分片（Piece）的当前下载状态。<br/>
/// Represents the current download state of a torrent piece.
/// </summary>
/// <remarks>
/// 用于跟踪单个分片在下载过程中的具体生命周期阶段。<br/>
/// Used to track the specific lifecycle stage of an individual piece during the download process.
/// </remarks>
public enum EnumPieceState
{
    /// <summary>
    /// 还未开始下载。<br/>
    /// Not downloaded yet.
    /// </summary>
    /// <remarks>
    /// 表示该分片尚未从任何对等点（Peer）请求，或仍在等待队列中。<br/>
    /// Indicates that the piece has not been requested from any peer yet, or is still in the waiting queue.
    /// </remarks>
    None,

    /// <summary>
    /// 下载中。<br/>
    /// Now downloading.
    /// </summary>
    /// <remarks>
    /// 表示该分片的部分数据正在从网络获取，但尚未完全接收。<br/>
    /// Indicates that parts of the piece are currently being fetched from the network but are not yet fully received.
    /// </remarks>
    Downloading,

    /// <summary>
    /// 下载完成。<br/>
    /// Already downloaded.
    /// </summary>
    /// <remarks>
    /// 表示该分片已完整下载并通过了哈希校验（Hash Check）。<br/>
    /// Indicates that the piece has been fully downloaded and has passed the hash verification.
    /// </remarks>
    Finish
}
