using System.Text.Json;
using System.Text.Json.Serialization;

namespace Banned.Qbittorrent.Models.Application;

public sealed class ScanDirDestinationConverter : JsonConverter<ScanDirDestination>
{
    public override ScanDirDestination Read(ref Utf8JsonReader    reader,
                                            Type                  typeToConvert,
                                            JsonSerializerOptions options)
    {
        var dest = new ScanDirDestination();
        switch (reader.TokenType)
        {
            case JsonTokenType.Number :
                dest.Mode = reader.GetInt32();
                break;
            case JsonTokenType.String :
                dest.CustomPath = reader.GetString();
                break;
            default :
                reader.Skip();
                break;
        }

        return dest;
    }

    public override void Write(Utf8JsonWriter writer, ScanDirDestination value, JsonSerializerOptions options)
    {
        if (value.IsCustomPath)
            writer.WriteStringValue(value.CustomPath);
        else if (value.Mode.HasValue)
            writer.WriteNumberValue(value.Mode.Value);
        else
            writer.WriteNullValue();
    }
}
