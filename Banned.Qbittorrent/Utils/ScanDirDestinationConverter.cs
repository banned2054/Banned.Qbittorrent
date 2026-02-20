using Banned.Qbittorrent.Models.Application;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Banned.Qbittorrent.Utils;

/// <summary>
/// 将监控目录保存位置（数字模式或自定义字符串路径）与 <see cref="ScanDirDestination"/> 相互转换。<br/>
/// Converts between scan directory destinations (numeric mode or custom string path) and <see cref="ScanDirDestination"/>.
/// </summary>
public sealed class ScanDirDestinationConverter : JsonConverter<ScanDirDestination>
{
    /// <summary>
    /// 读取并转换 JSON 数据为 <see cref="ScanDirDestination"/>。<br/>
    /// Reads and converts the JSON data to <see cref="ScanDirDestination"/>.
    /// </summary>
    /// <param name="reader">JSON 读取器。 / The JSON reader.</param>
    /// <param name="typeToConvert">要转换的类型。 / The type to convert.</param>
    /// <param name="options">序列化选项。 / Serializer options.</param>
    /// <returns>转换后的监控目录保存位置对象。 / The converted scan directory destination object.</returns>
    public override ScanDirDestination Read(ref Utf8JsonReader    reader,
                                            Type                  typeToConvert,
                                            JsonSerializerOptions options)
    {
        var dest = new ScanDirDestination();
        switch (reader.TokenType)
        {
            // 如果是数字，映射为预设的保存模式（例如：0 表示默认保存路径）
            // If it's a number, map it to a preset save mode (e.g., 0 for default save path)
            case JsonTokenType.Number :
                dest.Mode = reader.GetInt32();
                break;

            // 如果是字符串，映射为用户定义的自定义路径
            // If it's a string, map it to a user-defined custom path
            case JsonTokenType.String :
                dest.CustomPath = reader.GetString();
                break;

            default :
                reader.Skip();
                break;
        }

        return dest;
    }

    /// <summary>
    /// 将 <see cref="ScanDirDestination"/> 写入 JSON 流。<br/>
    /// Writes the <see cref="ScanDirDestination"/> to the JSON stream.
    /// </summary>
    /// <param name="writer">JSON 写入器。 / The JSON writer.</param>
    /// <param name="value">监控目录保存位置对象。 / The scan directory destination object.</param>
    /// <param name="options">序列化选项。 / Serializer options.</param>
    public override void Write(Utf8JsonWriter writer, ScanDirDestination value, JsonSerializerOptions options)
    {
        // 根据对象状态决定写入字符串路径还是数字模式
        // Decide whether to write a string path or a numeric mode based on the object's state
        if (value.IsCustomPath)
            writer.WriteStringValue(value.CustomPath);
        else if (value.Mode.HasValue)
            writer.WriteNumberValue(value.Mode.Value);
        else
            writer.WriteNullValue();
    }
}
