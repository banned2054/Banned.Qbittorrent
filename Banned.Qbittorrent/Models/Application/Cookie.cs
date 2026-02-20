using Banned.Qbittorrent.Utils;
using System.Text.Json.Serialization;

namespace Banned.Qbittorrent.Models.Application;

/// <summary>
/// 表示 qBittorrent 应用程序使用的 Cookie 信息。<br/>
/// Represents cookie information used by the qBittorrent application.
/// </summary>
public class Cookie
{
    /// <summary>
    /// Cookie 的名称。<br/>
    /// The name of the cookie.
    /// </summary>
    [JsonPropertyName("name")]
    public string Name { get; set; }

    /// <summary>
    /// Cookie 所属的域名。<br/>
    /// The domain of the cookie.
    /// </summary>
    [JsonPropertyName("domain")]
    public string Domain { get; set; }

    /// <summary>
    /// Cookie 的有效路径。<br/>
    /// The path of the cookie.
    /// </summary>
    [JsonPropertyName("path")]
    public string Path { get; set; }

    /// <summary>
    /// Cookie 的值。<br/>
    /// The value of the cookie.
    /// </summary>
    [JsonPropertyName("value")]
    public string Value { get; set; }

    /// <summary>
    /// Cookie 的过期时间。<br/>
    /// The expiration date of the cookie.
    /// </summary>
    /// <remarks>
    /// 该字段在 JSON 中以 Unix 时间戳格式存储，并由 <see cref="UnixTimestampConverter"/> 进行转换。<br/>
    /// This field is stored as a Unix timestamp in JSON and converted by <see cref="UnixTimestampConverter"/>.
    /// </remarks>
    [JsonPropertyName("expirationDate")]
    [JsonConverter(typeof(UnixTimestampConverter))]
    public DateTimeOffset? Expiration { get; set; }
}
