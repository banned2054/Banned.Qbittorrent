using Banned.Qbittorrent.Models.Enums;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Banned.Qbittorrent.Utils;

public class StopConditionConverter : JsonConverter<EnumStopCondition>
{
    public override EnumStopCondition Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        var value = reader.GetString();
        return StringUtils.String2StopCondition(value);
    }

    public override void Write(Utf8JsonWriter writer, EnumStopCondition value, JsonSerializerOptions options)
    {
        // 直接调用你现有的扩展方法或工具类方法
        writer.WriteStringValue(value.StopCondition2String());
    }
}
