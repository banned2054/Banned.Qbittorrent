using Banned.Qbittorrent.Models.Enums;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Banned.Qbittorrent.Utils;

public class ConnectionStatusConverter : JsonConverter<EnumConnectionStatus>
{
    public override EnumConnectionStatus Read(ref Utf8JsonReader    reader, Type typeToConvert,
                                              JsonSerializerOptions options)
    {
        var value = reader.GetString();
        return StringUtils.String2ConnectionStatus(value);
    }

    public override void Write(Utf8JsonWriter writer, EnumConnectionStatus value, JsonSerializerOptions options)
    {
        writer.WriteStringValue(value.ToString().ToLower());
    }
}
