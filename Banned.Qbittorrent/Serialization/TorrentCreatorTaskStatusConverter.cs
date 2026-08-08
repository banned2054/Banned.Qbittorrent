using Banned.Qbittorrent.Models.Enums;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Banned.Qbittorrent.Serialization;

/// <summary>
/// 转换种子创建任务状态字符串。<br/>
/// Converts torrent creation task status strings.
/// </summary>
public class TorrentCreatorTaskStatusConverter : JsonConverter<EnumTorrentCreatorTaskStatus>
{
    /// <inheritdoc />
    public override EnumTorrentCreatorTaskStatus Read(ref Utf8JsonReader    reader,
                                                      Type                  typeToConvert,
                                                      JsonSerializerOptions options)
    {
        var value = reader.GetString();
        return value?.ToLowerInvariant() switch
        {
            "failed"   => EnumTorrentCreatorTaskStatus.Failed,
            "queued"   => EnumTorrentCreatorTaskStatus.Queued,
            "running"  => EnumTorrentCreatorTaskStatus.Running,
            "finished" => EnumTorrentCreatorTaskStatus.Finished,
            _          => EnumTorrentCreatorTaskStatus.Unknown
        };
    }

    /// <inheritdoc />
    public override void Write(Utf8JsonWriter writer, EnumTorrentCreatorTaskStatus value, JsonSerializerOptions options)
    {
        writer.WriteStringValue(value switch
        {
            EnumTorrentCreatorTaskStatus.Failed   => "Failed",
            EnumTorrentCreatorTaskStatus.Queued   => "Queued",
            EnumTorrentCreatorTaskStatus.Running  => "Running",
            EnumTorrentCreatorTaskStatus.Finished => "Finished",
            _                                     => "Unknown"
        });
    }
}
