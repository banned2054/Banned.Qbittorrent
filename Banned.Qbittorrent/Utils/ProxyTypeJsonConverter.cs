using Banned.Qbittorrent.Models.Enums;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Banned.Qbittorrent.Utils;

public sealed class ProxyTypeJsonConverter : JsonConverter<EnumProxyType?>
{
    public override EnumProxyType? Read(
        ref Utf8JsonReader    reader,
        Type                  typeToConvert,
        JsonSerializerOptions options)
    {
        switch (reader.TokenType)
        {
            case JsonTokenType.Number :
                return (EnumProxyType)reader.GetInt32();
            case JsonTokenType.String :
            {
                var s = reader.GetString();
                if (Enum.TryParse<EnumProxyType>(s, true, out var e))
                    return e;
                break;
            }
        }

        return null;
    }

    public override void Write(
        Utf8JsonWriter        writer,
        EnumProxyType?        value,
        JsonSerializerOptions options)
    {
        writer.WriteNumberValue((int)(value ?? 0));
    }
}
