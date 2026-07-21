using Banned.Qbittorrent.Models.Application;
using Banned.Qbittorrent.Models.Enums;
using Banned.Qbittorrent.Models.Logging;
using Banned.Qbittorrent.Models.Rss;
using Banned.Qbittorrent.Models.Search;
using Banned.Qbittorrent.Models.Sync;
using Banned.Qbittorrent.Models.Torrent;
using Banned.Qbittorrent.Models.Transfer;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.Json.Serialization.Metadata;

namespace Banned.Qbittorrent.Serialization;

[JsonSerializable(typeof(ProcessInfo))]
[JsonSerializable(typeof(ApplicationPreferences))]
[JsonSerializable(typeof(List<Cookie>))]
[JsonSerializable(typeof(TransferInfo))]
[JsonSerializable(typeof(List<TorrentInfo>))]
[JsonSerializable(typeof(TorrentProperties))]
[JsonSerializable(typeof(List<TrackerInfo>))]
[JsonSerializable(typeof(List<TorrentWebSeed>))]
[JsonSerializable(typeof(List<TorrentFileInfo>))]
[JsonSerializable(typeof(List<EnumPieceState>))]
[JsonSerializable(typeof(List<string>))]
[JsonSerializable(typeof(Dictionary<string, long>))]
[JsonSerializable(typeof(List<TorrentCategory>))]
[JsonSerializable(typeof(MainData))]
[JsonSerializable(typeof(PeerData))]
[JsonSerializable(typeof(SearchJob))]
[JsonSerializable(typeof(SearchStatus[]))]
[JsonSerializable(typeof(SearchResult))]
[JsonSerializable(typeof(SearchPlugins))]
[JsonSerializable(typeof(Dictionary<string, JsonElement>))]
[JsonSerializable(typeof(AutoDownloadRule))]
[JsonSerializable(typeof(Dictionary<string, AutoDownloadRule>))]
[JsonSerializable(typeof(Dictionary<string, string[]>))]
[JsonSerializable(typeof(List<LogElement>))]
[JsonSerializable(typeof(List<UserLogElement>))]
internal sealed partial class QBittorrentJsonContext : JsonSerializerContext;

internal static class QBittorrentJsonSerializer
{
    private static readonly QBittorrentJsonContext IgnoreNullContext = new(
        new JsonSerializerOptions { DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull });

    public static T? Deserialize<T>(string json) =>
        JsonSerializer.Deserialize(json, GetTypeInfo<T>(QBittorrentJsonContext.Default));

    public static string Serialize<T>(T value) =>
        JsonSerializer.Serialize(value, GetTypeInfo<T>(QBittorrentJsonContext.Default));

    public static string SerializeIgnoringNulls<T>(T value) =>
        JsonSerializer.Serialize(value, GetTypeInfo<T>(IgnoreNullContext));

    private static JsonTypeInfo<T> GetTypeInfo<T>(JsonSerializerContext context) =>
        context.GetTypeInfo(typeof(T)) as JsonTypeInfo<T>
        ?? throw new InvalidOperationException($"No JSON metadata was generated for {typeof(T)}.");
}
