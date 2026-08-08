using System.Text.Json.Serialization;

namespace Banned.Qbittorrent.Models.Application;

/// <summary>
/// 表示 qBittorrent 及其主要依赖项的构建信息。<br/>
/// Represents qBittorrent build information and its primary dependencies.
/// </summary>
public class BuildInfo
{
    /// <summary>进程位数。<br/>Process bitness.</summary>
    [JsonPropertyName("bitness")]
    public int? Bitness { get; set; }

    /// <summary>Boost 版本。<br/>Boost version.</summary>
    [JsonPropertyName("boost")]
    public string? BoostVersion { get; set; }

    /// <summary>libtorrent 版本。<br/>libtorrent version.</summary>
    [JsonPropertyName("libtorrent")]
    public string? LibtorrentVersion { get; set; }

    /// <summary>OpenSSL 版本。<br/>OpenSSL version.</summary>
    [JsonPropertyName("openssl")]
    public string? OpenSslVersion { get; set; }

    /// <summary>Qt 版本。<br/>Qt version.</summary>
    [JsonPropertyName("qt")]
    public string? QtVersion { get; set; }

    /// <summary>zlib 版本。<br/>zlib version.</summary>
    [JsonPropertyName("zlib")]
    public string? ZlibVersion { get; set; }
}
