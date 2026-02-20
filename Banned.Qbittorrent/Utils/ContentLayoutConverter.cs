using Banned.Qbittorrent.Models.Enums;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Banned.Qbittorrent.Utils;

/// <summary>
/// 将 qBittorrent 内容布局字符串与 <see cref="EnumContentLayout"/> 枚举相互转换。<br/>
/// Converts between qBittorrent content layout strings and the <see cref="EnumContentLayout"/> enum.
/// </summary>
public class ContentLayoutConverter : JsonConverter<EnumContentLayout>
{
    /// <summary>
    /// 读取并转换 JSON 字符串为 <see cref="EnumContentLayout"/>。<br/>
    /// Reads and converts the JSON string to <see cref="EnumContentLayout"/>.
    /// </summary>
    /// <param name="reader">JSON 读取器。 / The JSON reader.</param>
    /// <param name="typeToConvert">要转换的类型。 / The type to convert.</param>
    /// <param name="options">序列化选项。 / Serializer options.</param>
    /// <returns>转换后的内容布局枚举。 / The converted content layout enum.</returns>
    public override EnumContentLayout Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        var value = reader.GetString();
        // 使用工具类将 API 字符串解析为内部枚举
        // Use utility class to parse API string into internal enum
        return StringUtils.String2ContentLayout(value);
    }

    /// <summary>
    /// 将 <see cref="EnumContentLayout"/> 枚举转换为 API 识别的字符串并写入 JSON。<br/>
    /// Converts the <see cref="EnumContentLayout"/> enum to an API-recognized string and writes it to JSON.
    /// </summary>
    /// <param name="writer">JSON 写入器。 / The JSON writer.</param>
    /// <param name="value">内容布局枚举值。 / The content layout enum value.</param>
    /// <param name="options">序列化选项。 / Serializer options.</param>
    public override void Write(Utf8JsonWriter writer, EnumContentLayout value, JsonSerializerOptions options)
    {
        // 直接调用扩展方法或工具类方法进行序列化
        // Directly call extension or utility methods for serialization
        writer.WriteStringValue(value.ContentLayout2String());
    }
}
