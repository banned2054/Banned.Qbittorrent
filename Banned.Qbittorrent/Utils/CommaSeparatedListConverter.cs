using System.Text.Json;
using System.Text.Json.Serialization;

namespace Banned.Qbittorrent.Utils;

/// <summary>
/// 将逗号分隔的字符串（如 "tag1,tag2"）与 <see cref="List{T}"/> 相互转换。<br/>
/// Converts between a comma-separated string (e.g., "tag1,tag2") and a <see cref="List{T}"/>.
/// </summary>
public class CommaSeparatedListConverter : JsonConverter<List<string>>
{
    /// <summary>
    /// 将 JSON 字符串读取并拆分为字符串列表。<br/>
    /// Reads and splits the JSON string into a list of strings.
    /// </summary>
    /// <param name="reader">JSON 读取器。 / The JSON reader.</param>
    /// <param name="typeToConvert">要转换的类型。 / The type to convert.</param>
    /// <param name="options">序列化选项。 / Serializer options.</param>
    /// <returns>拆分后的字符串列表；若值为空则返回空列表。 / A list of split strings; returns an empty list if the value is null or empty.</returns>
    public override List<string> Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        // 处理 API 返回的是 null 或非字符串的情况
        // Handle cases where the API returns null or a non-string value
        if (reader.TokenType != JsonTokenType.String) return [];

        var value = reader.GetString();
        if (string.IsNullOrWhiteSpace(value)) return [];

        // 逻辑：拆分、去空格、去空值
        // Logic: Split, trim entries, and remove empty entries
        return value.Split(',', StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries)
                    .ToList();
    }

    /// <summary>
    /// 将字符串列表合并为逗号分隔的字符串并写入 JSON。<br/>
    /// Joins the list of strings into a comma-separated string and writes it to JSON.
    /// </summary>
    /// <param name="writer">JSON 写入器。 / The JSON writer.</param>
    /// <param name="value">要写入的字符串列表。 / The list of strings to be written.</param>
    /// <param name="options">序列化选项。 / Serializer options.</param>
    public override void Write(Utf8JsonWriter writer, List<string> value, JsonSerializerOptions options)
    {
        if (value == null || value.Count == 0)
        {
            writer.WriteStringValue(string.Empty);
            return;
        }

        // 序列化回字符串，方便传参给 API
        // Serialize back to a string, making it easy to pass as a parameter to the API
        writer.WriteStringValue(string.Join(",", value));
    }
}
