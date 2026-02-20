using System.Text.Json;
using System.Text.Json.Serialization;

namespace Banned.Qbittorrent.Utils;

public class CommaSeparatedListConverter : JsonConverter<List<string>>
{
    public override List<string> Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        // 处理 API 返回的是 null 或非字符串的情况
        if (reader.TokenType != JsonTokenType.String) return [];

        var value = reader.GetString();
        if (string.IsNullOrWhiteSpace(value)) return [];

        // 逻辑与你之前的一致：拆分、去空格、去空值
        return value.Split(',', StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries)
                    .ToList();
    }

    public override void Write(Utf8JsonWriter writer, List<string> value, JsonSerializerOptions options)
    {
        if (value == null || value.Count == 0)
        {
            writer.WriteStringValue(string.Empty);
            return;
        }

        // 序列化回字符串，方便传参给 API
        writer.WriteStringValue(string.Join(",", value));
    }
}
