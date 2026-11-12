using Banned.Qbittorrent.Exceptions;
using Banned.Qbittorrent.Models;
using Banned.Qbittorrent.Models.Application;
using Banned.Qbittorrent.Models.Enums;
using Banned.Qbittorrent.Models.Requests;
using Banned.Qbittorrent.Services;
using System.Net;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using static NUnit.Framework.Assert;

namespace Banned.Qbittorrent.Test;

[TestFixture]
public class StreamingJsonTests
{
    [TestCase(false)]
    [TestCase(true)]
    public async Task TorrentLists_ReadChunkedUtf8WithoutBufferingAndKeepLegacyContracts(bool useRequestObject)
    {
        const string entry = """
            {"hash":"abc","name":"下载🧪 \"name\"","state":"pausedUP","tags":" old , new ","future":{"enabled":true}}
            """;
        var       json       = "[" + string.Join(",", Enumerable.Repeat(entry, 1024)) + "]";
        using var stream     = new ChunkedReadStream(Encoding.UTF8.GetBytes(json), 37);
        var       handler    = new StubHttpMessageHandler(_ => StreamingResponse(stream));
        using var httpClient = new HttpClient(handler);
        using var netService = new NetService("http://localhost:8080", httpClient);
        var       apiVersion = ApiVersion.Parse("2.1.1");
        netService.SetApiVersion(apiVersion);
        var torrent = new TorrentService(netService, apiVersion);

        var result = useRequestObject
            ? await torrent.GetTorrentInfos(new GetTorrentInfoListRequest())
            : await torrent.GetTorrentInfos();

        Multiple(() =>
        {
            That(result, Has.Count.EqualTo(1024));
            That(result[0].Name, Is.EqualTo("下载🧪 \"name\""));
            That(result[0].State, Is.EqualTo(EnumTorrentState.StoppedUpload));
            That(result[0].TagList, Is.EqualTo(new[] { "old", "new" }));
            That(result[0].Error, Is.Null);
            That(handler.Requests.Single().Method, Is.EqualTo(HttpMethod.Post));
            That(handler.Requests.Single().Uri.AbsolutePath, Is.EqualTo("/api/v2/torrents/info"));
            That(handler.Requests.Single().Body, Is.EqualTo(useRequestObject ? "filter=all" : string.Empty));
            That(stream.Disposed, Is.True);
        });
    }

    [Test]
    public async Task MainData_ReadsBomAndPartialUpdatesWithUnknownFields()
    {
        const string json = """
            {"rid":17,"torrents":{"abc":{"name":"动画🧪","state":"stoppedUP"}},"future":[1,2]}
            """;
        using var stream = new ChunkedReadStream([.. Encoding.UTF8.GetPreamble(), .. Encoding.UTF8.GetBytes(json)], 1);
        var       handler = new StubHttpMessageHandler(_ => StreamingResponse(stream));
        using var httpClient = new HttpClient(handler);
        using var netService = new NetService("http://localhost:8080", httpClient);

        var result = await new SyncService(netService).GetMainData(16);

        Multiple(() =>
        {
            That(result!.Rid, Is.EqualTo(17));
            That(result.FullUpdateEnabled, Is.Null);
            That(result.Torrents!["abc"].Name, Is.EqualTo("动画🧪"));
            That(result.Torrents["abc"].State, Is.EqualTo(EnumTorrentState.StoppedUpload));
            That(result.Torrents["abc"].Error, Is.Null);
            That(result.TorrentsRemoved, Is.Null);
            That(handler.Requests.Single().Body, Is.EqualTo("rid=16"));
            That(stream.Disposed, Is.True);
        });
    }

    [Test]
    public async Task PeerData_ReadsChunkedUpdatesAndPreservesRequestParameters()
    {
        const string json = """
            {"rid":9,"peers":{"peer":{"client":"旧客户端🧪","future":true}},"peers_removed":["old"]}
            """;
        using var stream     = new ChunkedReadStream(Encoding.UTF8.GetBytes(json), 1);
        var       handler    = new StubHttpMessageHandler(_ => StreamingResponse(stream));
        using var httpClient = new HttpClient(handler);
        using var netService = new NetService("http://localhost:8080", httpClient);

        var result = await new SyncService(netService).GetPeerData(8, "abc");

        Multiple(() =>
        {
            That(result!.Rid, Is.EqualTo(9));
            That(result.Peers!["peer"].Client, Is.EqualTo("旧客户端🧪"));
            That(result.FullUpdateEnabled, Is.Null);
            That(result.PeersRemoved, Is.EqualTo(new[] { "old" }));
            That(handler.Requests.Single().Body, Is.EqualTo("rid=8&hash=abc"));
            That(stream.Disposed, Is.True);
        });
    }

    [TestCase(0)]
    [TestCase(1)]
    [TestCase(2)]
    [TestCase(3)]
    public async Task NullResponses_KeepExistingEmptyListAndNullResults(int endpoint)
    {
        using var stream     = new ChunkedReadStream("null"u8.ToArray(), 1);
        var       handler    = new StubHttpMessageHandler(_ => StreamingResponse(stream));
        using var httpClient = new HttpClient(handler);
        using var netService = new NetService("http://localhost:8080", httpClient);

        var result = await ReadEndpoint(netService, endpoint);

        That(result, endpoint < 2 ? Is.Empty : Is.Null);
        That(stream.Disposed, Is.True);
    }

    [TestCase("utf-8", false, true)]
    [TestCase("utf-8", true, true)]
    [TestCase("utf-16", false, true)]
    [TestCase("utf-16", true, true)]
    [TestCase("utf-16", true, false)]
    [TestCase("utf-16BE", true, false)]
    [TestCase("utf-32", true, false)]
    [TestCase("iso-8859-1", false, true)]
    public async Task Encodings_MatchExistingBufferedDecoding(string charset, bool includeBom, bool declareCharset)
    {
        const string json     = """[{"name":"café"}]""";
        var          encoding = Encoding.GetEncoding(charset);
        byte[]       bytes    = [.. includeBom ? encoding.GetPreamble() : [], .. encoding.GetBytes(json)];
        var handler = new StubHttpMessageHandler(_ =>
        {
            var content = new ByteArrayContent(bytes);
            content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
            if (declareCharset) content.Headers.ContentType.CharSet = $"\"{charset}\"";
            return new HttpResponseMessage(HttpStatusCode.OK) { Content = content };
        });
        using var httpClient = new HttpClient(handler);
        using var netService = new NetService("http://localhost:8080", httpClient);
        using var buffered   = JsonDocument.Parse(await netService.Post("/api/v2/torrents/info"));

        var result = await new TorrentService(netService, default).GetTorrentInfos();

        That(result.Single().Name, Is.EqualTo(buffered.RootElement[0].GetProperty("name").GetString()));
    }

    [TestCase(0)]
    [TestCase(1)]
    [TestCase(2)]
    [TestCase(3)]
    public async Task CancellationDuringBodyRead_PropagatesAndDisposesResponseWithoutReplay(int endpoint)
    {
        using var stream       = new ChunkedReadStream("null"u8.ToArray(), 4) { BlockAfterPrefix = true };
        var       handler      = new StubHttpMessageHandler(_ => StreamingResponse(stream));
        using var httpClient   = new HttpClient(handler);
        using var netService   = new NetService("http://localhost:8080", httpClient);
        using var cancellation = new CancellationTokenSource();

        var request = ReadEndpoint(netService, endpoint, cancellation.Token);
        await stream.ReadBlocked.Task.WaitAsync(TimeSpan.FromSeconds(5));
        cancellation.Cancel();
        CatchAsync<OperationCanceledException>(async () => await request);

        That(handler.Requests, Has.Count.EqualTo(1));
        That(stream.Disposed, Is.True);
    }

    [Test]
    public async Task InvalidUtf8_UsesTheSameReplacementFallbackAsBufferedDecoding()
    {
        byte[] bytes = [.. Encoding.UTF8.GetBytes("[{\"name\":\""), 0xC3, 0x28, .. Encoding.UTF8.GetBytes("\"}]")];
        var handler = new StubHttpMessageHandler(_ => new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new ByteArrayContent(bytes)
        });
        using var httpClient = new HttpClient(handler);
        using var netService = new NetService("http://localhost:8080", httpClient);
        using var buffered   = JsonDocument.Parse(await netService.Post("/api/v2/torrents/info"));

        var result = await new TorrentService(netService, default).GetTorrentInfos();

        That(result.Single().Name, Is.EqualTo(buffered.RootElement[0].GetProperty("name").GetString()));
    }

    [Test]
    public async Task DetailedDiagnostics_KeepTheOriginalResponsePreviewAndDecodedLength()
    {
        const string json = """{"rid":5,"torrents":{}}""";
        var handler = new StubHttpMessageHandler(request => StubHttpMessageHandler.JsonResponse(
                                                      request.RequestUri!.AbsolutePath switch
                                                      {
                                                          "/api/v2/auth/login"        => "Ok.",
                                                          "/api/v2/app/webapiVersion" => "2.1.1",
                                                          "/api/v2/app/version"       => "4.1.0",
                                                          _                           => json
                                                      }));
        using var httpClient  = new HttpClient(handler);
        var       diagnostics = new List<string>();
        using var client = await QBittorrentClient.Create("http://localhost:8080", "user", "password",
                                                          new QBittorrentClientOptions
                                                          {
                                                              HttpClient = httpClient, DiagnosticSink = diagnostics.Add
                                                          });

        var result = await client.Sync.GetMainData();

        That(result!.Rid, Is.EqualTo(5));
        That(diagnostics, Has.Some.Contains($"bodyLength={json.Length} bodyPreview='{json}'"));
    }

    [TestCase(0)]
    [TestCase(1)]
    [TestCase(2)]
    [TestCase(3)]
    public void MalformedJson_IsNotRetriedAndDisposesResponse(int endpoint)
    {
        using var stream     = new ChunkedReadStream("{\"rid\":"u8.ToArray(), 1);
        var       handler    = new StubHttpMessageHandler(_ => StreamingResponse(stream));
        using var httpClient = new HttpClient(handler);
        using var netService = new NetService("http://localhost:8080", httpClient);

        CatchAsync<JsonException>(async () => await ReadEndpoint(netService, endpoint));

        That(handler.Requests, Has.Count.EqualTo(1));
        That(stream.Disposed, Is.True);
    }

    [TestCase(HttpStatusCode.Unauthorized)]
    [TestCase(HttpStatusCode.ServiceUnavailable)]
    public async Task RecoverableErrors_KeepAuthenticationAndHttpRetryBehavior(HttpStatusCode status)
    {
        using var stream        = new ChunkedReadStream("{\"rid\":5}"u8.ToArray(), 1);
        using var failedContent = new StringContent("not JSON");
        var       calls         = 0;
        var handler = new StubHttpMessageHandler(_ =>
        {
            if (++calls > 1) return StreamingResponse(stream);
            var response = new HttpResponseMessage(status) { Content = failedContent };
            response.Headers.RetryAfter = new RetryConditionHeaderValue(TimeSpan.Zero);
            return response;
        });
        using var httpClient = new HttpClient(handler);
        using var netService = new NetService("http://localhost:8080", httpClient) { MaxRetries = 2 };
        var       authCalls  = new List<bool>();
        netService.EnsureLoggedInAsyncHandler = (force, _) =>
        {
            authCalls.Add(force);
            return Task.CompletedTask;
        };

        var result = await new SyncService(netService).GetMainData(4);

        That(result!.Rid, Is.EqualTo(5));
        That(handler.Requests.Select(request => request.Body), Is.EqualTo(new[] { "rid=4", "rid=4" }));
        That(authCalls, Is.EqualTo(status == HttpStatusCode.Unauthorized ? new[] { false, true } : new[] { false }));
        That(stream.Disposed, Is.True);
    }

    [TestCase(HttpStatusCode.BadRequest, typeof(QbittorrentBadRequestException))]
    [TestCase(HttpStatusCode.Unauthorized, typeof(QbittorrentUnauthorizedException))]
    [TestCase(HttpStatusCode.Forbidden, typeof(QbittorrentForbiddenException))]
    [TestCase(HttpStatusCode.NotFound, typeof(QbittorrentNotFoundException))]
    [TestCase(HttpStatusCode.Conflict, typeof(QbittorrentConflictException))]
    [TestCase(HttpStatusCode.InternalServerError, typeof(QbittorrentServerErrorException))]
    public void ErrorResponses_KeepExistingExceptionMapping(HttpStatusCode status, Type exceptionType)
    {
        var handler = new StubHttpMessageHandler(_ => new HttpResponseMessage(status)
        {
            Content = new StringContent("not JSON")
        });
        using var httpClient = new HttpClient(handler);
        using var netService = new NetService("http://localhost:8080", httpClient) { MaxRetries = 1 };

        var exception = CatchAsync<QbittorrentException>(async () => await new SyncService(netService).GetMainData());

        That(exception, Is.TypeOf(exceptionType));
        That(handler.Requests, Has.Count.EqualTo(1));
    }

    [Test]
    public void BodyTransportFailure_KeepsPostReplayPolicyAndDisposesResponse()
    {
        using var stream     = new ChunkedReadStream("null"u8.ToArray(), 4) { FailAfterPrefix = true };
        var       handler    = new StubHttpMessageHandler(_ => StreamingResponse(stream));
        using var httpClient = new HttpClient(handler);
        using var netService = new NetService("http://localhost:8080", httpClient);

        CatchAsync<QbittorrentException>(async () => await new SyncService(netService).GetMainData());

        That(handler.Requests, Has.Count.EqualTo(1));
        That(stream.Disposed, Is.True);
    }

    private static async Task<object?> ReadEndpoint(NetService        netService, int endpoint,
                                                    CancellationToken cancellationToken = default) =>
        endpoint switch
        {
            0 => await new TorrentService(netService, default).GetTorrentInfos(cancellationToken : cancellationToken),
            1 => await new TorrentService(netService, default).GetTorrentInfos(new GetTorrentInfoListRequest(),
                                                                               cancellationToken),
            2 => await new SyncService(netService).GetMainData(cancellationToken : cancellationToken),
            3 => await new SyncService(netService).GetPeerData(cancellationToken : cancellationToken),
            _ => throw new ArgumentOutOfRangeException(nameof(endpoint))
        };

    private static HttpResponseMessage StreamingResponse(Stream stream) => new(HttpStatusCode.OK)
    {
        Content = new StreamingOnlyContent(stream)
    };

    // Buffering this content fails, so success verifies that the HTTP body is actually read as a stream.
    private sealed class StreamingOnlyContent(Stream stream) : HttpContent
    {
        protected override Task<Stream> CreateContentReadStreamAsync() => Task.FromResult(stream);

        protected override Task<Stream> CreateContentReadStreamAsync(CancellationToken cancellationToken) =>
            Task.FromResult(stream);

        protected override Task SerializeToStreamAsync(Stream target, TransportContext? context) =>
            throw new InvalidOperationException("The JSON response must not be buffered.");

        protected override bool TryComputeLength(out long length)
        {
            length = 0;
            return false;
        }
    }

    private sealed class ChunkedReadStream(byte[] bytes, int chunkSize) : Stream
    {
        private int _position;

        public bool BlockAfterPrefix { get; init; }
        public bool FailAfterPrefix  { get; init; }
        public bool Disposed         { get; private set; }

        public TaskCompletionSource ReadBlocked { get; } = new(TaskCreationOptions.RunContinuationsAsynchronously);

        public override bool CanRead  => !Disposed;
        public override bool CanSeek  => false;
        public override bool CanWrite => false;
        public override long Length   => throw new NotSupportedException();

        public override long Position
        {
            get => throw new NotSupportedException();
            set => throw new NotSupportedException();
        }

        public override async ValueTask<int> ReadAsync(Memory<byte>      buffer,
                                                       CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();
            if (_position >= 4 && BlockAfterPrefix)
            {
                ReadBlocked.TrySetResult();
                await Task.Delay(Timeout.InfiniteTimeSpan, cancellationToken);
            }

            if (_position >= 4 && FailAfterPrefix) throw new IOException("The connection was interrupted.");
            var count = Math.Min(buffer.Length, Math.Min(chunkSize, bytes.Length - _position));
            bytes.AsMemory(_position, count).CopyTo(buffer);
            _position += count;
            return count;
        }

        protected override void Dispose(bool disposing)
        {
            Disposed = true;
            base.Dispose(disposing);
        }

        public override void Flush()                                              => throw new NotSupportedException();
        public override int  Read(byte[]    buffer, int        offset, int count) => throw new NotSupportedException();
        public override long Seek(long      offset, SeekOrigin origin) => throw new NotSupportedException();
        public override void SetLength(long value)                         => throw new NotSupportedException();
        public override void Write(byte[]   buffer, int offset, int count) => throw new NotSupportedException();
    }
}
