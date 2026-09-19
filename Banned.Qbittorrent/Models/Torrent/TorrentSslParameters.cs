using System.Text.Json.Serialization;

namespace Banned.Qbittorrent.Models.Torrent;

/// <summary>
/// 指定 Torrent 的 SSL 参数。<br/>
/// SSL parameters configured for a torrent.
/// </summary>
public sealed class TorrentSslParameters
{
    /// <summary>客户端证书。<br/>Client certificate.</summary>
    [JsonPropertyName("ssl_certificate")]
    public string? Certificate { get; set; }

    /// <summary>客户端私钥。<br/>Client private key.</summary>
    [JsonPropertyName("ssl_private_key")]
    public string? PrivateKey { get; set; }

    /// <summary>Diffie-Hellman 参数。<br/>Diffie-Hellman parameters.</summary>
    [JsonPropertyName("ssl_dh_params")]
    public string? DhParameters { get; set; }
}
