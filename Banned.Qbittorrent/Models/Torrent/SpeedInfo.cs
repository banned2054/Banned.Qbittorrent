namespace Banned.Qbittorrent.Models.Torrent;

/// <summary>
/// 表示种子的速度信息。<br/>
/// Represents speed information for a torrent.
/// </summary>
public class SpeedInfo
{
    /// <summary>
    /// 种子的唯一 Hash 值。<br/>
    /// The unique hash of the torrent.
    /// </summary>
    public string Hash { get; set; } = string.Empty;

    /// <summary>
    /// 传输速度（字节/秒）。<br/>
    /// Transmission speed (bytes/s).
    /// </summary>
    public long Speed { get; set; }
}
