using Banned.Qbittorrent.Models.Enums;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Banned.Qbittorrent.Utils;

/// <summary>
/// 将 qBittorrent 状态字符串与 <see cref="EnumConnectionStatus"/> 枚举相互转换。<br/>
/// Converts between qBittorrent status strings and the <see cref="EnumConnectionStatus"/> enum.
/// </summary>
public class ConnectionStatusConverter : JsonConverter<EnumConnectionStatus>
{
    /// <summary>
    /// 读取并转换 JSON 字符串为 <see cref="EnumConnectionStatus"/>。<br/>
    /// Reads and converts the JSON string to <see cref="EnumConnectionStatus"/>.
    /// </summary>
    /// <param name="reader">JSON 读取器。 / The JSON reader.</param>
    /// <param name="typeToConvert">要转换的类型。 / The type to convert.</param>
    /// <param name="options">序列化选项。 / Serializer options.</param>
    /// <returns>转换后的连接状态枚举。 / The converted connection status enum.</returns>
    public override EnumConnectionStatus Read(ref Utf8JsonReader    reader, Type typeToConvert,
                                              JsonSerializerOptions options)
    {
        var value = reader.GetString();
        // 使用工具类进行字符串到枚举的解析
        // Use utility class for string-to-enum parsing
        return StringUtils.String2ConnectionStatus(value);
    }

    /// <summary>
    /// 将 <see cref="EnumConnectionStatus"/> 枚举转换为小写字符串并写入 JSON。<br/>
    /// Converts the <see cref="EnumConnectionStatus"/> enum to a lowercase string and writes it to JSON.
    /// </summary>
    /// <param name="writer">JSON 写入器。 / The JSON writer.</param>
    /// <param name="value">连接状态枚举值。 / The connection status enum value.</param>
    /// <param name="options">序列化选项。 / Serializer options.</param>
    public override void Write(Utf8JsonWriter writer, EnumConnectionStatus value, JsonSerializerOptions options)
    {
        // 确保输出为小写以符合 API 规范
        // Ensure the output is lowercase to comply with API specifications
        writer.WriteStringValue(value.ToString().ToLower());
    }
}
