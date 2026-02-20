using Banned.Qbittorrent.Models.Enums;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Banned.Qbittorrent.Utils;

public class TorrentStateConverter : JsonConverter<EnumTorrentState>
{
    public override EnumTorrentState Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        var value = reader.GetString();
        return string.IsNullOrWhiteSpace(value)
            ? EnumTorrentState.Unknown
            : StringUtils.String2TorrentState(value);
    }

    public override void Write(Utf8JsonWriter writer, EnumTorrentState value, JsonSerializerOptions options)
    {
        writer.WriteStringValue(value.TorrentState2StringV5());
    }
}
