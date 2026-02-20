using Banned.Qbittorrent.Models.Enums;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Banned.Qbittorrent.Utils;

/// <summary>
/// 将 qBittorrent 种子状态字符串与 <see cref="EnumTorrentState"/> 枚举相互转换。<br/>
/// Converts between qBittorrent torrent state strings and the <see cref="EnumTorrentState"/> enum.
/// </summary>
public class TorrentStateConverter : JsonConverter<EnumTorrentState>
{
    /// <summary>
    /// 读取并转换 JSON 字符串为 <see cref="EnumTorrentState"/>。<br/>
    /// Reads and converts the JSON string to <see cref="EnumTorrentState"/>.
    /// </summary>
    /// <param name="reader">JSON 读取器。 / The JSON reader.</param>
    /// <param name="typeToConvert">要转换的类型。 / The type to convert.</param>
    /// <param name="options">序列化选项。 / Serializer options.</param>
    /// <returns>转换后的种子状态枚举；若为空则返回 Unknown。 / The converted torrent state enum; returns Unknown if empty.</returns>
    public override EnumTorrentState Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        var value = reader.GetString();
        // 处理空字符串并调用工具类解析
        // Handle empty strings and call utility class for parsing
        return string.IsNullOrWhiteSpace(value)
            ? EnumTorrentState.Unknown
            : StringUtils.String2TorrentState(value);
    }

    /// <summary>
    /// 将 <see cref="EnumTorrentState"/> 枚举转换为 API 识别的字符串并写入 JSON。<br/>
    /// Converts the <see cref="EnumTorrentState"/> enum to an API-recognized string and writes it to JSON.
    /// </summary>
    /// <param name="writer">JSON 写入器。 / The JSON writer.</param>
    /// <param name="value">种子状态枚举值。 / The torrent state enum value.</param>
    /// <param name="options">序列化选项。 / Serializer options.</param>
    /// <remarks>
    /// 默认使用 V5 版本的状态字符串（如 "stoppedUP" 而非 "pausedUP"）。<br/>
    /// Defaults to V5 state strings (e.g., "stoppedUP" instead of "pausedUP").
    /// </remarks>
    public override void Write(Utf8JsonWriter writer, EnumTorrentState value, JsonSerializerOptions options)
    {
        // 序列化时默认采用最新的 V5 规范
        // Default to the latest V5 specification during serialization
        writer.WriteStringValue(value.TorrentState2StringV5());
    }
}
