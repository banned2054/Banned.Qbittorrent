using Banned.Qbittorrent.Exceptions;
using Banned.Qbittorrent.Models.Application;
using Banned.Qbittorrent.Models.Enums;
using Banned.Qbittorrent.Models.Requests;
using Banned.Qbittorrent.Models.Transfer;
using Banned.Qbittorrent.Services;
using static NUnit.Framework.Assert;

namespace Banned.Qbittorrent.Test;

/// <summary>
/// 覆盖 Web API v2.16.x 新增端点的契约测试。<br/>
/// Contract tests for the endpoints added by Web API v2.16.x.
/// </summary>
[TestFixture]
public class WebApi216EndpointTests
{
    private static readonly ApiVersion Supported = new(2, 16, 2);

    [Test]
    public async Task GetFreeSpaceAtPath_ReturnsBytesForRequestedPath()
    {
        var (netService, httpClient, handler) =
            CreateNetService(_ => StubHttpMessageHandler.JsonResponse("1099511627776"));
        using (netService)
        using (httpClient)
        {
            var freeSpace = await new ApplicationService(netService).GetFreeSpaceAtPath("/downloads/movies");

            Multiple(() =>
            {
                That(freeSpace, Is.EqualTo(1099511627776L));
                That(handler.Requests, Has.Count.EqualTo(1));
                That(handler.Requests[0].Method, Is.EqualTo(HttpMethod.Post));
                That(handler.Requests[0].Uri.AbsolutePath, Is.EqualTo("/api/v2/app/getFreeSpaceAtPath"));
                That(DecodeForm(handler.Requests[0]), Is.EqualTo("path=/downloads/movies"));
            });
        }
    }

    [Test]
    public async Task GetFreeSpaceAtPath_ReturnsMinusOneWhenServerCannotDetermineSpace()
    {
        var (netService, httpClient, handler) =
            CreateNetService(_ => StubHttpMessageHandler.JsonResponse("-1"));
        using (netService)
        using (httpClient)
        {
            var freeSpace = await new ApplicationService(netService).GetFreeSpaceAtPath("/missing/path");

            That(freeSpace, Is.EqualTo(-1L));
        }
    }

    [Test]
    public void GetFreeSpaceAtPath_RejectsEmptyPathBeforeSendingRequest()
    {
        var (netService, httpClient, handler) =
            CreateNetService(_ => StubHttpMessageHandler.JsonResponse("0"));
        using (netService)
        using (httpClient)
        {
            ThrowsAsync<ArgumentException>(async () => await new ApplicationService(netService)
                                              .GetFreeSpaceAtPath("  "));
            That(handler.Requests, Is.Empty);
        }
    }

    [Test]
    public async Task CloneAutoDownloadingRule_UsesSourceAndCloneNames()
    {
        var (netService, httpClient, handler) = CreateNetService(_ => StubHttpMessageHandler.JsonResponse(""));
        using (netService)
        using (httpClient)
        {
            await new RssService(netService).CloneAutoDownloadingRule("original-rule", "cloned-rule");

            Multiple(() =>
            {
                That(handler.Requests, Has.Count.EqualTo(1));
                That(handler.Requests[0].Method, Is.EqualTo(HttpMethod.Post));
                That(handler.Requests[0].Uri.AbsolutePath, Is.EqualTo("/api/v2/rss/cloneRule"));
                That(DecodeForm(handler.Requests[0]), Is.EqualTo("sourceName=original-rule&cloneName=cloned-rule"));
            });
        }
    }

    [Test]
    public async Task GetSpeedLimits_DeserializesAllFourLimitsFromSingleRequest()
    {
        const string json = """
            {
              "dl_limit": 1024000,
              "up_limit": 2048000,
              "alt_dl_limit": 512000,
              "alt_up_limit": 256000,
              "futureField": "preserved"
            }
            """;
        var (netService, httpClient, handler) = CreateNetService(_ => StubHttpMessageHandler.JsonResponse(json));
        using (netService)
        using (httpClient)
        {
            var limits = await new TransferService(netService).GetSpeedLimits();

            Multiple(() =>
            {
                That(limits, Is.TypeOf<TransferSpeedLimits>());
                That(limits.DownloadLimit, Is.EqualTo(1024000L));
                That(limits.UploadLimit, Is.EqualTo(2048000L));
                That(limits.AlternativeDownloadLimit, Is.EqualTo(512000L));
                That(limits.AlternativeUploadLimit, Is.EqualTo(256000L));
                That(handler.Requests, Has.Count.EqualTo(1));
                That(handler.Requests[0].Method, Is.EqualTo(HttpMethod.Get));
                That(handler.Requests[0].Uri.AbsolutePath, Is.EqualTo("/api/v2/transfer/getSpeedLimits"));
            });
        }
    }

    [Test]
    public async Task SetSpeedLimits_SendsAllFourRequiredLimits()
    {
        var (netService, httpClient, handler) = CreateNetService(_ => StubHttpMessageHandler.JsonResponse(""));
        using (netService)
        using (httpClient)
        {
            await new TransferService(netService).SetSpeedLimits(2048000, 1024000, 256000, 512000);

            Multiple(() =>
            {
                That(handler.Requests, Has.Count.EqualTo(1));
                That(handler.Requests[0].Method, Is.EqualTo(HttpMethod.Post));
                That(handler.Requests[0].Uri.AbsolutePath, Is.EqualTo("/api/v2/transfer/setSpeedLimits"));
                That(DecodeForm(handler.Requests[0]),
                     Is.EqualTo("up_limit=2048000&dl_limit=1024000&alt_up_limit=256000&alt_dl_limit=512000"));
            });
        }
    }

    [TestCase("pauseSession")]
    [TestCase("resumeSession")]
    public async Task SessionControls_PostToDocumentedEndpoints(string endpoint)
    {
        var (netService, httpClient, handler) = CreateNetService(_ => StubHttpMessageHandler.JsonResponse(""));
        using (netService)
        using (httpClient)
        {
            var transfer = new TransferService(netService);
            if (endpoint == "pauseSession")
                await transfer.PauseSession();
            else
                await transfer.ResumeSession();

            Multiple(() =>
            {
                That(handler.Requests, Has.Count.EqualTo(1));
                That(handler.Requests[0].Method, Is.EqualTo(HttpMethod.Post));
                That(handler.Requests[0].Uri.AbsolutePath, Is.EqualTo($"/api/v2/transfer/{endpoint}"));
                That(handler.Requests[0].Body, Is.Null.Or.Empty);
            });
        }
    }

    [TestCase("pauseSession")]
    [TestCase("resumeSession")]
    public void SessionControls_RejectWebApiBefore2162BeforeSendingRequest(string endpoint)
    {
        var (netService, httpClient, handler) =
            CreateNetService(_ => StubHttpMessageHandler.JsonResponse(""), ApiVersion.V2_16_0);
        using (netService)
        using (httpClient)
        {
            var transfer = new TransferService(netService);
            Func<Task> action = endpoint == "pauseSession"
                ? async () => await transfer.PauseSession()
                : async () => await transfer.ResumeSession();

            ThrowsAsync<QbittorrentNotSupportedException>(action);
            That(handler.Requests, Is.Empty);
        }
    }

    [Test]
    public async Task DownloadFile_ReturnsServerSidePathForFileIndex()
    {
        var (netService, httpClient, handler) =
            CreateNetService(_ => StubHttpMessageHandler.JsonResponse("/downloads/linux/ubuntu.iso"));
        using (netService)
        using (httpClient)
        {
            var path = await new TorrentService(netService, Supported).DownloadFile("abc123", 2);

            Multiple(() =>
            {
                That(path, Is.EqualTo("/downloads/linux/ubuntu.iso"));
                That(handler.Requests, Has.Count.EqualTo(1));
                That(handler.Requests[0].Method, Is.EqualTo(HttpMethod.Post));
                That(handler.Requests[0].Uri.AbsolutePath, Is.EqualTo("/api/v2/torrents/downloadFile"));
                That(DecodeForm(handler.Requests[0]), Is.EqualTo("hash=abc123&file=2"));
            });
        }
    }

    [Test]
    public async Task DownloadFile_AcceptsFilePathInsteadOfIndex()
    {
        var (netService, httpClient, handler) = CreateNetService(_ => StubHttpMessageHandler.JsonResponse("path"));
        using (netService)
        using (httpClient)
        {
            await new TorrentService(netService, Supported).DownloadFile("abc123", "sub dir/movie.mkv");

            That(DecodeForm(handler.Requests.Single()), Is.EqualTo("hash=abc123&file=sub+dir/movie.mkv"));
        }
    }

    [Test]
    public void DownloadFile_RejectsWebApiBefore2160BeforeSendingRequest()
    {
        var (netService, httpClient, handler) =
            CreateNetService(_ => StubHttpMessageHandler.JsonResponse("path"), new ApiVersion(2, 15, 4));
        using (netService)
        using (httpClient)
        {
            ThrowsAsync<QbittorrentNotSupportedException>(async () => await new TorrentService(netService,
                                                                   new ApiVersion(2, 15, 4)).DownloadFile("abc", 0));
            That(handler.Requests, Is.Empty);
        }
    }

    [Test]
    public async Task AddTorrent_SendsShareLimitsMode()
    {
        var (netService, httpClient, handler) = CreateNetService(_ => StubHttpMessageHandler.JsonResponse("Ok."),
                                                                 Supported);
        using (netService)
        using (httpClient)
        {
            await new TorrentService(netService, Supported).AddTorrent(urls : ["magnet:?xt=urn:btih:abc"],
                                                                       shareLimitAction :
                                                                       EnumTorrentShareLimitAction.Stop,
                                                                       shareLimitsMode :
                                                                       EnumTorrentShareLimitsMode.MatchAny);

            var form = DecodeForm(handler.Requests.Single());
            Multiple(() =>
            {
                That(form, Does.Contain("shareLimitAction=Stop"));
                That(form, Does.Contain("shareLimitsMode=MatchAny"));
            });
        }
    }

    [Test]
    public void AddTorrentRequest_ToDictionaryConvertsShareLimitsModeToApiString()
    {
        var parameters = new AddTorrentRequest { ShareLimitsMode = EnumTorrentShareLimitsMode.MatchAll }.ToDictionary();

        That(parameters["shareLimitsMode"], Is.EqualTo("MatchAll"));
    }

    [Test]
    public async Task SetTorrentShareLimit_SendsShareLimitActionAndMode()
    {
        var (netService, httpClient, handler) = CreateNetService(_ => StubHttpMessageHandler.JsonResponse(""),
                                                                 Supported);
        using (netService)
        using (httpClient)
        {
            await new TorrentService(netService, Supported).SetTorrentShareLimit("abc",
                     shareLimitAction : EnumTorrentShareLimitAction.RemoveWithContent,
                     shareLimitsMode : EnumTorrentShareLimitsMode.Default);

            var form = DecodeForm(handler.Requests.Single());
            Multiple(() =>
            {
                That(handler.Requests[0].Uri.AbsolutePath, Is.EqualTo("/api/v2/torrents/setShareLimits"));
                That(form, Does.Contain("hashes=abc"));
                That(form, Does.Contain("shareLimitAction=RemoveWithContent"));
                That(form, Does.Contain("shareLimitsMode=Default"));
            });
        }
    }

    [Test]
    public void SetTorrentShareLimit_RejectsCallWithoutAnyShareLimitSetting()
    {
        var (netService, httpClient, handler) = CreateNetService(_ => StubHttpMessageHandler.JsonResponse(""),
                                                                 Supported);
        using (netService)
        using (httpClient)
        {
            ThrowsAsync<ArgumentException>(async () => await new TorrentService(netService, Supported)
                                              .SetTorrentShareLimit("abc"));
            That(handler.Requests, Is.Empty);
        }
    }

    [Test]
    public async Task SetTorrentsShareLimit_ForwardsShareLimitSettingsForAllHashes()
    {
        var (netService, httpClient, handler) = CreateNetService(_ => StubHttpMessageHandler.JsonResponse(""),
                                                                 Supported);
        using (netService)
        using (httpClient)
        {
            await new TorrentService(netService, Supported)
               .SetTorrentsShareLimit(["abc", "def"], shareLimitsMode : EnumTorrentShareLimitsMode.MatchAny);

            var form = DecodeForm(handler.Requests.Single());
            Multiple(() =>
            {
                That(form, Does.Contain("hashes=abc|def"));
                That(form, Does.Contain("shareLimitsMode=MatchAny"));
            });
        }
    }

    [Test]
    public async Task SetAllTorrentsShareLimit_ForwardsShareLimitSettingsForAllTorrents()
    {
        var (netService, httpClient, handler) = CreateNetService(_ => StubHttpMessageHandler.JsonResponse(""),
                                                                 Supported);
        using (netService)
        using (httpClient)
        {
            await new TorrentService(netService, Supported)
               .SetAllTorrentsShareLimit(shareLimitAction : EnumTorrentShareLimitAction.Stop,
                                         shareLimitsMode : EnumTorrentShareLimitsMode.MatchAll);

            var form = DecodeForm(handler.Requests.Single());
            Multiple(() =>
            {
                That(form, Does.Contain("hashes=all"));
                That(form, Does.Contain("shareLimitAction=Stop"));
                That(form, Does.Contain("shareLimitsMode=MatchAll"));
            });
        }
    }

    [Test]
    public async Task CreateTorrentTask_SendsIgnoreDotfilesWhenRequested()
    {
        var (netService, httpClient, handler) =
            CreateNetService(_ => StubHttpMessageHandler.JsonResponse("{\"taskID\":\"task-1\"}"));
        using (netService)
        using (httpClient)
        {
            await new TorrentCreatorService(netService).AddTask(new CreateTorrentTaskRequest
            {
                SourcePath     = "/data/source",
                IgnoreDotfiles = false
            });

            That(DecodeForm(handler.Requests.Single()), Does.Contain("ignoreDotfiles=false"));
        }
    }

    [Test]
    public void CreateTorrentTask_OmitsIgnoreDotfilesWhenNotSpecified()
    {
        var parameters = new CreateTorrentTaskRequest { SourcePath = "/data/source" }.ToDictionary();

        That(parameters, Does.Not.ContainKey("ignoreDotfiles"));
    }

    private static (NetService NetService, HttpClient HttpClient, StubHttpMessageHandler Handler)
        CreateNetService(Func<HttpRequestMessage, HttpResponseMessage> responseFactory, ApiVersion? apiVersion = null)
    {
        var handler    = new StubHttpMessageHandler(responseFactory);
        var httpClient = new HttpClient(handler);
        var netService = new NetService("http://localhost:8080", httpClient);
        netService.SetApiVersion(apiVersion ?? Supported);
        return (netService, httpClient, handler);
    }

    private static string DecodeForm(HttpRequestSnapshot request) =>
        Uri.UnescapeDataString(request.Body ?? string.Empty);
}
