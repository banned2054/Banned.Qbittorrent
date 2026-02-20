using System.Text.Json;
using System.Text.Json.Serialization;

namespace Banned.Qbittorrent.Utils;

/// <summary>
/// 将 Unix 时间戳（秒）与 <see cref="DateTimeOffset"/> 相互转换。<br/>
/// Converts between Unix timestamps (seconds) and <see cref="DateTimeOffset"/>.
/// </summary>
public class UnixTimestampConverter : JsonConverter<DateTimeOffset?>
{
    /// <summary>
    /// 读取并转换 Unix 时间戳为 <see cref="DateTimeOffset"/>。<br/>
    /// Reads and converts a Unix timestamp to <see cref="DateTimeOffset"/>.
    /// </summary>
    /// <param name="reader">JSON 读取器。 / The JSON reader.</param>
    /// <param name="typeToConvert">要转换的类型。 / The type to convert.</param>
    /// <param name="options">序列化选项。 / Serializer options.</param>
    /// <returns>转换后的时间偏移对象；若为 null 则返回 null。 / The converted DateTimeOffset; returns null if the value is null.</returns>
    public override DateTimeOffset? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.TokenType == JsonTokenType.Null) return null;

        if (reader.TryGetInt64(out var unixTime))
        {
            // qBittorrent API 通常返回秒级时间戳
            // qBittorrent API usually returns timestamps in seconds
            return DateTimeOffset.FromUnixTimeSeconds(unixTime);
        }

        return null;
    }

    /// <summary>
    /// 将 <see cref="DateTimeOffset"/> 转换为 Unix 时间戳并写入 JSON。<br/>
    /// Converts <see cref="DateTimeOffset"/> to a Unix timestamp and writes it to JSON.
    /// </summary>
    /// <param name="writer">JSON 写入器。 / The JSON writer.</param>
    /// <param name="value">时间偏移对象。 / The DateTimeOffset object.</param>
    /// <param name="options">序列化选项。 / Serializer options.</param>
    public override void Write(Utf8JsonWriter writer, DateTimeOffset? value, JsonSerializerOptions options)
    {
        if (value.HasValue)
        {
            // 序列化为秒级 Unix 时间戳
            // Serialize to a Unix timestamp in seconds
            writer.WriteNumberValue(value.Value.ToUnixTimeSeconds());
        }
        else
        {
            writer.WriteNullValue();
        }
    }
}
