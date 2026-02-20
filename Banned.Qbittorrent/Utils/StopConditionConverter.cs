using Banned.Qbittorrent.Models.Enums;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Banned.Qbittorrent.Utils;

/// <summary>
/// 将 qBittorrent 停止条件字符串与 <see cref="EnumStopCondition"/> 枚举相互转换。<br/>
/// Converts between qBittorrent stop condition strings and the <see cref="EnumStopCondition"/> enum.
/// </summary>
public class StopConditionConverter : JsonConverter<EnumStopCondition>
{
    /// <summary>
    /// 读取并转换 JSON 字符串为 <see cref="EnumStopCondition"/>。<br/>
    /// Reads and converts the JSON string to <see cref="EnumStopCondition"/>.
    /// </summary>
    /// <param name="reader">JSON 读取器。 / The JSON reader.</param>
    /// <param name="typeToConvert">要转换的类型。 / The type to convert.</param>
    /// <param name="options">序列化选项。 / Serializer options.</param>
    /// <returns>转换后的停止条件枚举。 / The converted stop condition enum.</returns>
    public override EnumStopCondition Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        var value = reader.GetString();
        // 使用工具类将 API 返回的条件字符串解析为枚举
        // Use utility class to parse the condition string from API into an enum
        return StringUtils.String2StopCondition(value);
    }

    /// <summary>
    /// 将 <see cref="EnumStopCondition"/> 枚举转换为 API 识别的字符串并写入 JSON。<br/>
    /// Converts the <see cref="EnumStopCondition"/> enum to an API-recognized string and writes it to JSON.
    /// </summary>
    /// <param name="writer">JSON 写入器。 / The JSON writer.</param>
    /// <param name="value">停止条件枚举值。 / The stop condition enum value.</param>
    /// <param name="options">序列化选项。 / Serializer options.</param>
    public override void Write(Utf8JsonWriter writer, EnumStopCondition value, JsonSerializerOptions options)
    {
        // 直接调用扩展方法或工具类方法进行序列化输出
        // Directly call extension or utility methods for serialization output
        writer.WriteStringValue(value.StopCondition2String());
    }
}
