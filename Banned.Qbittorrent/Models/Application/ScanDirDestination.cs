using Banned.Qbittorrent.Utils;
using System.Text.Json.Serialization;

namespace Banned.Qbittorrent.Models.Application;

/// <summary>
/// 表示监控目录（Scan Directory）的下载保存目的地。<br/>
/// Represents the download destination for a monitored directory.
/// </summary>
/// <remarks>
/// 该类由 <see cref="ScanDirDestinationConverter"/> 进行序列化处理，以兼容 qBittorrent 混合类型的 Web API。<br/>
/// This class is serialized by <see cref="ScanDirDestinationConverter"/> to be compatible with qBittorrent's hybrid-type Web API.
/// </remarks>
[JsonConverter(typeof(ScanDirDestinationConverter))]
public sealed class ScanDirDestination
{
    /// <summary>
    /// 保存模式。<br/>
    /// The save mode.
    /// </summary>
    /// <remarks>
    /// 通常为数值：0 表示“保存到默认位置”，1 表示“保存到当前监控目录”。<br/>
    /// Usually numeric: 0 for "Save to default location", 1 for "Save to the same directory".
    /// </remarks>
    public int? Mode { get; set; }

    /// <summary>
    /// 自定义保存路径。<br/>
    /// The custom save path.
    /// </summary>
    /// <remarks>
    /// 当此值不为空时，下载内容将保存到此指定的绝对路径。<br/>
    /// When this value is not null, downloads will be saved to this specific absolute path.
    /// </remarks>
    public string? CustomPath { get; set; }

    /// <summary>
    /// 获取一个值，该值指示是否使用了自定义保存路径。<br/>
    /// Gets a value indicating whether a custom save path is used.
    /// </summary>
    public bool IsCustomPath => CustomPath != null;

    /// <summary>
    /// 返回表示当前对象的字符串。<br/>
    /// Returns a string that represents the current object.
    /// </summary>
    /// <returns>
    /// 如果是自定义路径则返回路径内容，否则返回模式数值。<br/>
    /// The custom path string if it exists; otherwise, the string representation of the mode.
    /// </returns>
    public override string ToString() => IsCustomPath ? CustomPath! : Mode?.ToString() ?? "null";
}
