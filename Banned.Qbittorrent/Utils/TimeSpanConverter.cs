using System.Text.Json;
using System.Text.Json.Serialization;

namespace Banned.Qbittorrent.Utils;

/// <summary>
/// 处理时间跨度转换的抽象基类，支持秒或分钟与 <see cref="TimeSpan"/> 的相互转换。<br/>
/// Abstract base class for TimeSpan conversion, supporting mapping between seconds or minutes and <see cref="TimeSpan"/>.
/// </summary>
/// <param name="isMinutes">是否以分钟为单位。 / Whether the unit is minutes.</param>
public abstract class BaseTimeSpanConverter(bool isMinutes) : JsonConverter<TimeSpan?>
{
    /// <summary>
    /// 读取并解析 JSON 数值为 <see cref="TimeSpan"/>。<br/>
    /// Reads and parses the JSON numeric value into <see cref="TimeSpan"/>.
    /// </summary>
    /// <param name="reader">JSON 读取器。 / The JSON reader.</param>
    /// <param name="typeToConvert">要转换的类型。 / The type to convert.</param>
    /// <param name="options">序列化选项。 / Serializer options.</param>
    /// <returns>转换后的时间跨度；若为特殊数值则返回 <see cref="TimeSpan.MaxValue"/>。 / The converted TimeSpan; returns <see cref="TimeSpan.MaxValue"/> for special values.</returns>
    public override TimeSpan? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.TokenType == JsonTokenType.Null) return null;
        if (reader.TryGetInt64(out var value))
        {
            // qBittorrent 常见的“无限”值处理（如 -1 或 8640000）
            // Handling common "unlimited" values in qBittorrent (e.g., -1 or 8640000)
            if (value is >= 8640000 or < 0) return TimeSpan.MaxValue;

            return isMinutes ? TimeSpan.FromMinutes(value) : TimeSpan.FromSeconds(value);
        }

        return null;
    }

    /// <summary>
    /// 将 <see cref="TimeSpan"/> 转换为数值并写入 JSON。<br/>
    /// Converts <see cref="TimeSpan"/> to a numeric value and writes it to JSON.
    /// </summary>
    /// <param name="writer">JSON 写入器。 / The JSON writer.</param>
    /// <param name="value">时间跨度对象。 / The TimeSpan object.</param>
    /// <param name="options">序列化选项。 / Serializer options.</param>
    public override void Write(Utf8JsonWriter writer, TimeSpan? value, JsonSerializerOptions options)
    {
        if (value == null)
        {
            writer.WriteNullValue();
            return;
        }

        // 将 MaxValue 映射回 API 识别的“无限”常数
        // Map MaxValue back to the "unlimited" constant recognized by the API
        long output = value == TimeSpan.MaxValue
            ? 8640000
            : (long)(isMinutes ? value.Value.TotalMinutes : value.Value.TotalSeconds);

        writer.WriteNumberValue(output);
    }
}

/// <summary>
/// 将以“秒”为单位的数值与 <see cref="TimeSpan"/> 相互转换。<br/>
/// Converts between numeric values in "seconds" and <see cref="TimeSpan"/>.
/// </summary>
public class SecondsTimeSpanConverter() : BaseTimeSpanConverter(false);

/// <summary>
/// 将以“分钟”为单位的数值与 <see cref="TimeSpan"/> 相互转换。<br/>
/// Converts between numeric values in "minutes" and <see cref="TimeSpan"/>.
/// </summary>
public class MinutesTimeSpanConverter() : BaseTimeSpanConverter(true);
