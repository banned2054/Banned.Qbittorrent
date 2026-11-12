using Banned.Qbittorrent.Models.Application;
using Banned.Qbittorrent.Models.Enums;
using Banned.Qbittorrent.Models.Logging;
using Banned.Qbittorrent.Models.Rss;
using Banned.Qbittorrent.Models.Search;
using Banned.Qbittorrent.Models.Sync;
using Banned.Qbittorrent.Models.Torrent;
using Banned.Qbittorrent.Models.TorrentCreator;
using Banned.Qbittorrent.Models.Transfer;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.Json.Serialization.Metadata;

namespace Banned.Qbittorrent.Serialization;

[JsonSerializable(typeof(ProcessInfo))]
[JsonSerializable(typeof(BuildInfo))]
[JsonSerializable(typeof(List<NetworkInterfaceInfo>))]
[JsonSerializable(typeof(ApplicationPreferences))]
[JsonSerializable(typeof(List<Cookie>))]
[JsonSerializable(typeof(TransferInfo))]
[JsonSerializable(typeof(TransferSpeedLimits))]
[JsonSerializable(typeof(List<TorrentInfo>))]
[JsonSerializable(typeof(TorrentProperties))]
[JsonSerializable(typeof(List<TrackerInfo>))]
[JsonSerializable(typeof(List<TorrentWebSeed>))]
[JsonSerializable(typeof(List<TorrentFileInfo>))]
[JsonSerializable(typeof(TorrentSslParameters))]
[JsonSerializable(typeof(TorrentCreatorTask))]
[JsonSerializable(typeof(List<TorrentCreatorTaskStatus>))]
[JsonSerializable(typeof(List<EnumPieceState>))]
[JsonSerializable(typeof(List<string>))]
[JsonSerializable(typeof(List<int>))]
[JsonSerializable(typeof(Dictionary<string, long>))]
[JsonSerializable(typeof(Dictionary<string, string>))]
[JsonSerializable(typeof(Dictionary<string, TorrentCategory>))]
[JsonSerializable(typeof(MainData))]
[JsonSerializable(typeof(PeerData))]
[JsonSerializable(typeof(SearchJob))]
[JsonSerializable(typeof(SearchStatus[]))]
[JsonSerializable(typeof(SearchResult))]
[JsonSerializable(typeof(List<SearchCategory>))]
[JsonSerializable(typeof(List<SearchPlugins>))]
[JsonSerializable(typeof(Dictionary<string, JsonElement>))]
[JsonSerializable(typeof(List<Dictionary<string, JsonElement>>))]
[JsonSerializable(typeof(AutoDownloadRule))]
[JsonSerializable(typeof(Dictionary<string, AutoDownloadRule>))]
[JsonSerializable(typeof(Dictionary<string, string[]>))]
[JsonSerializable(typeof(List<LogElement>))]
[JsonSerializable(typeof(List<UserLogElement>))]
internal sealed partial class QBittorrentJsonContext : JsonSerializerContext;

internal static class QBittorrentJsonSerializer
{
    private static readonly QBittorrentJsonContext IgnoreNullContext =
        new(new JsonSerializerOptions { DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull });

    public static T? Deserialize<T>(string json) =>
        JsonSerializer.Deserialize(json, GetTypeInfo<T>(QBittorrentJsonContext.Default));

    public static async Task<T?> DeserializeAsync<T>(HttpContent content, CancellationToken cancellationToken)
    {
        // Keep ReadAsStringAsync's charset handling for uncommon encodings and invalid charset headers.
        var charset = content.Headers.ContentType?.CharSet;
        if (charset != null && !string.Equals(charset.Trim('"'), "utf-8", StringComparison.OrdinalIgnoreCase))
            return Deserialize<T>(await content.ReadAsStringAsync(cancellationToken).ConfigureAwait(false));

        try
        {
            var stream = await content.ReadAsStreamAsync(cancellationToken).ConfigureAwait(false);
            var prefix = new byte[4];
            var count = await stream.ReadAtLeastAsync(prefix, prefix.Length, false, cancellationToken)
                                    .ConfigureAwait(false);
            using var replay = new PrefixReadStream(stream, prefix.AsMemory(0, count));

            // ReadAsStringAsync detects UTF-16/UTF-32 BOMs when no charset was supplied.
            if (charset == null && count >= 2 &&
                ((prefix[0] == 0xFF && prefix[1] == 0xFE) || (prefix[0] == 0xFE && prefix[1] == 0xFF)))
            {
                using var bufferedContent = new StreamContent(replay);
                return Deserialize<T>(await bufferedContent.ReadAsStringAsync(cancellationToken).ConfigureAwait(false));
            }

            // Preserve ReadAsStringAsync's replacement fallback for invalid UTF-8, without a full body string.
            using var utf8 = Encoding.CreateTranscodingStream(replay, Encoding.UTF8, Encoding.UTF8, true);
            return await JsonSerializer.DeserializeAsync(utf8, GetTypeInfo<T>(QBittorrentJsonContext.Default),
                                                         cancellationToken).ConfigureAwait(false);
        }
        catch (IOException exception)
        {
            // Match HttpContent's buffered-read errors so the existing transport retry policy still applies.
            throw new HttpRequestException("Error while reading JSON response content.", exception);
        }
    }

    public static string Serialize<T>(T value) =>
        JsonSerializer.Serialize(value, GetTypeInfo<T>(QBittorrentJsonContext.Default));

    public static string SerializeIgnoringNulls<T>(T value) =>
        JsonSerializer.Serialize(value, GetTypeInfo<T>(IgnoreNullContext));

    private static JsonTypeInfo<T> GetTypeInfo<T>(JsonSerializerContext context) =>
        context.GetTypeInfo(typeof(T)) as JsonTypeInfo<T>
     ?? throw new InvalidOperationException($"No JSON metadata was generated for {typeof(T)}.");

    // The HttpResponseMessage owns the underlying stream; only replay the bytes used to inspect its BOM.
    private sealed class PrefixReadStream(Stream stream, ReadOnlyMemory<byte> prefix) : Stream
    {
        private ReadOnlyMemory<byte> _prefix = prefix;

        public override bool CanRead  => stream.CanRead;
        public override bool CanSeek  => false;
        public override bool CanWrite => false;
        public override long Length   => throw new NotSupportedException();

        public override long Position
        {
            get => throw new NotSupportedException();
            set => throw new NotSupportedException();
        }

        public override int Read(byte[] buffer, int offset, int count)
        {
            var copied = CopyPrefix(buffer.AsMemory(offset, count));
            return copied != 0 ? copied : stream.Read(buffer, offset, count);
        }

        public override ValueTask<int> ReadAsync(Memory<byte> buffer, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();
            var copied = CopyPrefix(buffer);
            return copied != 0 ? ValueTask.FromResult(copied) : stream.ReadAsync(buffer, cancellationToken);
        }

        public override Task<int> ReadAsync(byte[]            buffer, int offset, int count,
                                            CancellationToken cancellationToken) =>
            ReadAsync(buffer.AsMemory(offset, count), cancellationToken).AsTask();

        private int CopyPrefix(Memory<byte> buffer)
        {
            var count = Math.Min(buffer.Length, _prefix.Length);
            _prefix[..count].CopyTo(buffer);
            _prefix = _prefix[count..];
            return count;
        }

        public override void Flush()
        {
            throw new NotSupportedException();
        }

        public override long Seek(long offset, SeekOrigin origin) => throw new NotSupportedException();

        public override void SetLength(long value)
        {
            throw new NotSupportedException();
        }

        public override void Write(byte[] buffer, int offset, int count)
        {
            throw new NotSupportedException();
        }
    }
}
