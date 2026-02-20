using Banned.Qbittorrent.Models.Enums;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Banned.Qbittorrent.Utils;

/// <summary>
/// 将代理类型（数字或字符串名）与 <see cref="EnumProxyType"/> 枚举相互转换。<br/>
/// Converts between proxy types (numeric or string name) and the <see cref="EnumProxyType"/> enum.
/// </summary>
public sealed class ProxyTypeJsonConverter : JsonConverter<EnumProxyType?>
{
    /// <summary>
    /// 读取并转换 JSON 数据为 <see cref="EnumProxyType"/>。<br/>
    /// Reads and converts the JSON data to <see cref="EnumProxyType"/>.
    /// </summary>
    /// <param name="reader">JSON 读取器。 / The JSON reader.</param>
    /// <param name="typeToConvert">要转换的类型。 / The type to convert.</param>
    /// <param name="options">序列化选项。 / Serializer options.</param>
    /// <returns>转换后的代理类型枚举；若无法识别则返回 null。 / The converted proxy type enum; returns null if unrecognized.</returns>
    public override EnumProxyType? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        switch (reader.TokenType)
        {
            // 处理数值格式（如 0, 1, 2...）
            // Handle numeric format (e.g., 0, 1, 2...)
            case JsonTokenType.Number :
                return (EnumProxyType)reader.GetInt32();

            // 处理字符串格式（如 "HTTP", "SOCKS5"...）
            // Handle string format (e.g., "HTTP", "SOCKS5"...)
            case JsonTokenType.String :
            {
                var s = reader.GetString();
                if (Enum.TryParse<EnumProxyType>(s, true, out var e))
                    return e;
                break;
            }
        }

        return null;
    }

    /// <summary>
    /// 将 <see cref="EnumProxyType"/> 枚举转换为数值并写入 JSON。<br/>
    /// Converts the <see cref="EnumProxyType"/> enum to a numeric value and writes it to JSON.
    /// </summary>
    /// <param name="writer">JSON 写入器。 / The JSON writer.</param>
    /// <param name="value">代理类型枚举值。 / The proxy type enum value.</param>
    /// <param name="options">序列化选项。 / Serializer options.</param>
    public override void Write(Utf8JsonWriter writer, EnumProxyType? value, JsonSerializerOptions options)
    {
        // 序列化为整数值，符合 API 对配置项的预期
        // Serialize as an integer value, as expected by the API for configuration items
        writer.WriteNumberValue((int)(value ?? 0));
    }
}
