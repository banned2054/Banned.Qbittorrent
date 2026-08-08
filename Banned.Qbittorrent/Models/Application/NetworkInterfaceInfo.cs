using System.Text.Json.Serialization;

namespace Banned.Qbittorrent.Models.Application;

/// <summary>
/// 表示 qBittorrent 主机上的网络接口选项。<br/>
/// Represents a network interface option on the qBittorrent host.
/// </summary>
public class NetworkInterfaceInfo
{
    /// <summary>接口的显示名称。<br/>Display name of the interface.</summary>
    [JsonPropertyName("name")]
    public string? Name { get; set; }

    /// <summary>接口的配置值。<br/>Configuration value of the interface.</summary>
    [JsonPropertyName("value")]
    public string? Value { get; set; }
}
