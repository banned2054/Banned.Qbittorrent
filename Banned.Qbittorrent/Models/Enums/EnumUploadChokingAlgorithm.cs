namespace Banned.Qbittorrent.Models.Enums;

/// <summary>
/// 表示上传阻塞算法。<br/>
/// Represents the upload choking algorithm.
/// </summary>
public enum EnumUploadChokingAlgorithm
{
    /// <summary>
    /// 轮询（Round Robin）。<br/>
    /// Round-robin algorithm.
    /// </summary>
    RoundRobin = 0,

    /// <summary>
    /// 最快上传（优先考虑上传速度最快的用户）。<br/>
    /// Fastest upload (prioritize peers with the fastest upload speeds).
    /// </summary>
    FastestUpload = 1,

    /// <summary>
    /// 反吸血（优先考虑分享率高的用户）。<br/>
    /// Anti-leech (prioritize peers who share more).
    /// </summary>
    AntiLeech = 2
}
