using System.Text.Json;
using System.Text.Json.Serialization;

namespace Banned.Qbittorrent.Utils;

// 定义一个抽象基类处理核心逻辑
public abstract class BaseTimeSpanConverter(bool isMinutes) : JsonConverter<TimeSpan?>
{
    public override TimeSpan? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.TokenType == JsonTokenType.Null) return null;
        if (reader.TryGetInt64(out var value))
        {
            // qBittorrent 常见的“无限”值处理
            if (value is >= 8640000 or < 0) return TimeSpan.MaxValue;

            return isMinutes ? TimeSpan.FromMinutes(value) : TimeSpan.FromSeconds(value);
        }

        return null;
    }

    public override void Write(Utf8JsonWriter writer, TimeSpan? value, JsonSerializerOptions options)
    {
        if (value == null)
        {
            writer.WriteNullValue();
            return;
        }

        long output = value == TimeSpan.MaxValue
            ? 8640000
            : (long)(isMinutes ? value.Value.TotalMinutes : value.Value.TotalSeconds);

        writer.WriteNumberValue(output);
    }
}

// 派生出专门处理“秒”的类
public class SecondsTimeSpanConverter() : BaseTimeSpanConverter(false);

// 派生出专门处理“分钟”的类
public class MinutesTimeSpanConverter() : BaseTimeSpanConverter(true);
