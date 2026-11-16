using Banned.Qbittorrent.Exceptions;
using Banned.Qbittorrent.Models.Application;
using Banned.Qbittorrent.Models.Enums;
using Banned.Qbittorrent.Services;
using System.Net;
using static NUnit.Framework.Assert;

namespace Banned.Qbittorrent.Test;

[TestFixture]
public class FilePriorityCompatibilityTests
{
    [TestCase("v4.1.0", "2.0.0", false)]
    [TestCase("v4.1.4", "2.1.1", false)]
    [TestCase("v4.1.5", "2.2.0", true)]
    [TestCase("v5.3.0", "2.16.2", true)]
    [TestCase("v4.1.4", "2.17.0", true)]
    [TestCase("v4.1.5", "2.1.1", false)]
    public async Task SetFilesPriority_UsesNegotiatedApiVersion(
        string applicationVersion, string apiVersion, bool batchSupported)
    {
        var handler = new StubHttpMessageHandler(request => request.RequestUri!.AbsolutePath switch
        {
            "/api/v2/auth/login" => StubHttpMessageHandler.JsonResponse("Ok."),
            "/api/v2/app/webapiVersion" => StubHttpMessageHandler.JsonResponse(apiVersion),
            "/api/v2/app/version" => StubHttpMessageHandler.JsonResponse(applicationVersion),
            "/api/v2/torrents/filePrio" => StubHttpMessageHandler.JsonResponse(""),
            _ => throw new InvalidOperationException($"Unexpected request: {request.RequestUri}")
        });
        using var httpClient = new HttpClient(handler);
        using var client     = await QBittorrentClient.Create("http://localhost:8080", "user", "password", httpClient);
        handler.Requests.Clear();

        await client.Torrent.SetFilesPriority("abc123", [2, 5, 9], EnumTorrentFilePriority.High);

        AssertPriorityRequests(handler, batchSupported ? ["2|5|9"] : ["2", "5", "9"]);
    }

    [TestCase("2.0.1", false)]
    [TestCase("2.1.1", false)]
    [TestCase("2.2.0", true)]
    [TestCase("2.17.0", true)]
    public async Task SetFilesPriority_DirectServiceUsesApiVersion(
        string version, bool batchSupported)
    {
        var       apiVersion = ApiVersion.Parse(version);
        var       handler    = new StubHttpMessageHandler(_ => StubHttpMessageHandler.JsonResponse(""));
        using var httpClient = new HttpClient(handler);
        using var netService = new NetService("http://localhost:8080", httpClient);
        netService.SetApiVersion(apiVersion);

        await new TorrentService(netService, apiVersion, ApplicationVersion.V4_3_3)
           .SetFilesPriority("abc123", [2, 5, 9], EnumTorrentFilePriority.High);

        AssertPriorityRequests(handler, batchSupported ? ["2|5|9"] : ["2", "5", "9"]);
    }

    [TestCase("2.1.1")]
    [TestCase("2.2.0")]
    public async Task SetFilePriority_SendsOneRequestWithSingularId(string version)
    {
        var       apiVersion = ApiVersion.Parse(version);
        var       handler    = new StubHttpMessageHandler(_ => StubHttpMessageHandler.JsonResponse(""));
        using var httpClient = new HttpClient(handler);
        using var netService = new NetService("http://localhost:8080", httpClient);
        netService.SetApiVersion(apiVersion);

        await new TorrentService(netService, apiVersion, ApplicationVersion.V4_3_3)
           .SetFilePriority("abc123", 5, EnumTorrentFilePriority.High);

        AssertPriorityRequests(handler, ["5"]);
    }

    [TestCase(null)]
    [TestCase(new int[] { })]
    [TestCase(new[] { 2, -1 })]
    public void SetFilesPriority_ValidatesAllIndexesBeforeSendingRequests(int[]? indexes)
    {
        var       handler    = new StubHttpMessageHandler(_ => StubHttpMessageHandler.JsonResponse(""));
        using var httpClient = new HttpClient(handler);
        using var netService = new NetService("http://localhost:8080", httpClient);
        netService.SetApiVersion(ApiVersion.V2_1_1);

        ThrowsAsync<ArgumentException>(async () =>
                                           await new TorrentService(netService, ApiVersion.V2_1_1,
                                                                    ApplicationVersion.V4_3_3)
                                              .SetFilesPriority("abc123", indexes?.ToList()!,
                                                                EnumTorrentFilePriority.High));

        That(handler.Requests, Is.Empty);
    }

    [Test]
    public void SetFilesPriority_OldServerStopsAfterAnError()
    {
        var requestCount = 0;
        var handler = new StubHttpMessageHandler(_ => ++requestCount == 2
                                                     ? new HttpResponseMessage(HttpStatusCode.BadRequest)
                                                         { Content = new StringContent("invalid file") }
                                                     : StubHttpMessageHandler.JsonResponse(""));
        using var httpClient = new HttpClient(handler);
        using var netService = new NetService("http://localhost:8080", httpClient);
        netService.SetApiVersion(ApiVersion.V2_1_1);

        ThrowsAsync<QbittorrentBadRequestException>(async () =>
                                                        await new TorrentService(netService, ApiVersion.V2_1_1,
                                                                     ApplicationVersion.V4_3_3)
                                                           .SetFilesPriority("abc123", [2, 5, 9],
                                                                             EnumTorrentFilePriority.High));

        AssertPriorityRequests(handler, ["2", "5"]);
    }

    [Test]
    public void SetFilesPriority_OldServerStopsWhenCanceled()
    {
        using var cancellation = new CancellationTokenSource();
        var handler = new StubHttpMessageHandler(_ =>
        {
            cancellation.Cancel();
            return StubHttpMessageHandler.JsonResponse("");
        });
        using var httpClient = new HttpClient(handler);
        using var netService = new NetService("http://localhost:8080", httpClient);
        netService.SetApiVersion(ApiVersion.V2_1_1);

        CatchAsync<OperationCanceledException>(async () =>
                                                   await new TorrentService(netService, ApiVersion.V2_1_1,
                                                                            ApplicationVersion.V4_3_3)
                                                      .SetFilesPriority("abc123", [2, 5, 9],
                                                                        EnumTorrentFilePriority.High,
                                                                        cancellation.Token));

        AssertPriorityRequests(handler, ["2"]);
    }

    [Test]
    public void SetFilesPriority_OldServerDoesNotSendRequestsWhenAlreadyCanceled()
    {
        var       handler    = new StubHttpMessageHandler(_ => StubHttpMessageHandler.JsonResponse(""));
        using var httpClient = new HttpClient(handler);
        using var netService = new NetService("http://localhost:8080", httpClient);
        netService.SetApiVersion(ApiVersion.V2_1_1);
        using var cancellation = new CancellationTokenSource();
        cancellation.Cancel();

        CatchAsync<OperationCanceledException>(async () =>
                                                   await new TorrentService(netService, ApiVersion.V2_1_1,
                                                                            ApplicationVersion.V4_3_3)
                                                      .SetFilesPriority("abc123", [2, 5, 9],
                                                                        EnumTorrentFilePriority.High,
                                                                        cancellation.Token));

        That(handler.Requests, Is.Empty);
    }

    private static void AssertPriorityRequests(StubHttpMessageHandler handler, string[] expectedIds)
    {
        Multiple(() =>
        {
            That(handler.Requests, Has.Count.EqualTo(expectedIds.Length));
            That(handler.Requests.Select(request => request.Method), Is.All.EqualTo(HttpMethod.Post));
            That(handler.Requests.Select(request => request.Uri.AbsolutePath),
                 Is.All.EqualTo("/api/v2/torrents/filePrio"));
            That(handler.Requests.Select(request => Uri.UnescapeDataString(request.Body!)),
                 Is.EqualTo(expectedIds.Select(id => $"hash=abc123&id={id}&priority=6")));
        });
    }
}
