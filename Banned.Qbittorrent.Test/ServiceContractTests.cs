using Banned.Qbittorrent.Exceptions;
using Banned.Qbittorrent.Models.Application;
using Banned.Qbittorrent.Models.Enums;
using Banned.Qbittorrent.Models.Requests;
using Banned.Qbittorrent.Services;
using static NUnit.Framework.Assert;

namespace Banned.Qbittorrent.Test;

[TestFixture]
public class ServiceContractTests
{
    [Test]
    public async Task GetBuildInfo_DeserializesObjectResponse()
    {
        const string json = """
            {
              "bitness": 64,
              "boost": "1.86.0",
              "libtorrent": "2.0.11.0",
              "openssl": "3.4.1",
              "qt": "6.8.2",
              "zlib": "1.3.1"
            }
            """;
        var (netService, httpClient, handler) = CreateNetService(json);
        using (netService)
        using (httpClient)
        {
            var result = await new ApplicationService(netService).GetBuildInfo();

            Multiple(() =>
            {
                That(result, Is.Not.Null);
                That(result!.Bitness, Is.EqualTo(64));
                That(result.LibtorrentVersion, Is.EqualTo("2.0.11.0"));
                That(handler.Requests, Has.Count.EqualTo(1));
                That(handler.Requests[0].Method, Is.EqualTo(HttpMethod.Get));
                That(handler.Requests[0].Uri.AbsolutePath, Is.EqualTo("/api/v2/app/buildInfo"));
            });
        }
    }

    [Test]
    public async Task GetSearchPlugins_DeserializesArrayResponse()
    {
        const string json = """
            [
              {
                "enabled": true,
                "fullName": "Example plugin",
                "name": "example",
                "supportedCategories": [],
                "url": "https://example.com/plugin.py",
                "version": "1.0"
              }
            ]
            """;
        var (netService, httpClient, handler) = CreateNetService(json);
        using (netService)
        using (httpClient)
        {
            var result = await new SearchService(netService).GetSearchPlugins();

            Multiple(() =>
            {
                That(result, Has.Count.EqualTo(1));
                That(result[0].Name, Is.EqualTo("example"));
                That(handler.Requests, Has.Count.EqualTo(1));
                That(handler.Requests[0].Method, Is.EqualTo(HttpMethod.Get));
                That(handler.Requests[0].Uri.AbsolutePath, Is.EqualTo("/api/v2/search/plugins"));
            });
        }
    }

    [Test]
    public async Task GetAllCategories_DeserializesObjectResponseByCategoryName()
    {
        const string json = """
            {
              "linux": {
                "name": "linux",
                "savePath": "/downloads/linux"
              }
            }
            """;
        var (netService, httpClient, handler) = CreateNetService(json);
        using (netService)
        using (httpClient)
        {
            var result     = await new TorrentService(netService, new ApiVersion(2, 15, 1)).GetAllCategories();
            var categories = result!;

            Multiple(() =>
            {
                That(result, Is.Not.Null);
                That(categories, Contains.Key("linux"));
                That(categories["linux"].SavePath, Is.EqualTo("/downloads/linux"));
                That(handler.Requests, Has.Count.EqualTo(1));
                That(handler.Requests[0].Method, Is.EqualTo(HttpMethod.Get));
                That(handler.Requests[0].Uri.AbsolutePath, Is.EqualTo("/api/v2/torrents/categories"));
            });
        }
    }

    [Test]
    public async Task SearchOperations_UseDocumentedEndpointsAndParameters()
    {
        var responses = new Queue<string>(["{\"id\":42}", ""]);
        var (netService, httpClient, handler) =
            CreateNetService(_ => StubHttpMessageHandler.JsonResponse(responses.Dequeue()));
        using (netService)
        using (httpClient)
        {
            var service = new SearchService(netService);

            var job = await service.StartSearch("linux", ["example"]);
            await service.DownloadTorrent("magnet:?xt=urn:btih:abc", "example");

            Multiple(() =>
            {
                That(job?.Id, Is.EqualTo(42));
                That(handler.Requests, Has.Count.EqualTo(2));
                That(handler.Requests[0].Uri.AbsolutePath, Is.EqualTo("/api/v2/search/start"));
                That(DecodeForm(handler.Requests[0]), Does.Contain("pattern=linux"));
                That(handler.Requests[1].Uri.AbsolutePath, Is.EqualTo("/api/v2/search/downloadTorrent"));
                That(DecodeForm(handler.Requests[1]), Does.Contain("pluginName=example"));
                That(DecodeForm(handler.Requests[1]), Does.Contain("torrentUrl=magnet:?xt=urn:btih:abc"));
            });
        }
    }

    [Test]
    public async Task ApplicationOperations_UseDocumentedContracts()
    {
        var responses = new Queue<string>(
        [
            "[{\"name\":\"Ethernet\",\"value\":\"eth0\"}]",
            "[\"127.0.0.1\",\"::1\"]",
            "",
            "[\"/downloads/a.torrent\"]",
            "[{\"name\":\"a.torrent\",\"size\":123}]"
        ]);
        var (netService, httpClient, handler) =
            CreateNetService(_ => StubHttpMessageHandler.JsonResponse(responses.Dequeue()));
        using (netService)
        using (httpClient)
        {
            var service = new ApplicationService(netService);

            var interfaces = await service.GetNetworkInterfaces();
            var addresses  = await service.GetNetworkInterfaceAddresses("eth0");
            await service.SendTestEmail();
            var contents = await service.GetDirectoryContent("/downloads");
            var metadata = await service.GetDirectoryContentWithMetadata("/downloads");

            Multiple(() =>
            {
                That(interfaces.Single().Value, Is.EqualTo("eth0"));
                That(addresses, Is.EqualTo(["127.0.0.1", "::1"]));
                That(contents, Is.EqualTo(["/downloads/a.torrent"]));
                That(metadata.Single()["size"].GetInt32(), Is.EqualTo(123));
                That(handler.Requests.Select(request => request.Uri.AbsolutePath), Is.EqualTo([
                    "/api/v2/app/networkInterfaceList",
                    "/api/v2/app/networkInterfaceAddressList",
                    "/api/v2/app/sendTestEmail",
                    "/api/v2/app/getDirectoryContent",
                    "/api/v2/app/getDirectoryContent"
                ]));
                That(DecodeForm(handler.Requests[1]), Does.Contain("iface=eth0"));
                That(DecodeForm(handler.Requests[3]), Does.Contain("withMetadata=false"));
                That(DecodeForm(handler.Requests[4]), Does.Contain("withMetadata=true"));
            });
        }
    }

    [Test]
    public async Task TorrentOperations_UseDocumentedContractsAndPreserveExportBytes()
    {
        byte[] torrentBytes = [0, 255, 1, 128, 42];
        var (netService, httpClient, handler) = CreateNetService(request =>
                                                                     request.RequestUri!.AbsolutePath switch
                                                                     {
                                                                         "/api/v2/torrents/count" =>
                                                                             StubHttpMessageHandler.JsonResponse("7"),
                                                                         "/api/v2/torrents/export" =>
                                                                             StubHttpMessageHandler
                                                                                .BytesResponse(torrentBytes),
                                                                         _ => StubHttpMessageHandler.JsonResponse("")
                                                                     });
        using (netService)
        using (httpClient)
        {
            var service = new TorrentService(netService, new ApiVersion(2, 15, 1));

            var count = await service.GetTorrentCount();
            await service.AddTorrentWebSeeds("abc", ["https://example.com/seed"]);
            await service.EditTorrentWebSeed("abc", "https://example.com/seed", "https://example.com/new");
            await service.RemoveTorrentWebSeeds("abc", ["https://example.com/new"]);
            var exported = await service.ExportTorrent("abc");
            await service.SetTorrentSavePath("abc", "/completed");
            await service.SetTorrentsDownloadPath(["abc", "def"], "/incomplete");
            await service.SetTorrentTags("abc", ["linux", "iso"]);

            Multiple(() =>
            {
                That(count, Is.EqualTo(7));
                That(exported, Is.EqualTo(torrentBytes));
                That(handler.Requests.Select(request => request.Uri.AbsolutePath), Is.EqualTo([
                    "/api/v2/torrents/count",
                    "/api/v2/torrents/addWebSeeds",
                    "/api/v2/torrents/editWebSeed",
                    "/api/v2/torrents/removeWebSeeds",
                    "/api/v2/torrents/export",
                    "/api/v2/torrents/setSavePath",
                    "/api/v2/torrents/setDownloadPath",
                    "/api/v2/torrents/setTags"
                ]));
                That(DecodeForm(handler.Requests[1]), Does.Contain("urls=https://example.com/seed"));
                That(DecodeForm(handler.Requests[2]), Does.Contain("origUrl=https://example.com/seed"));
                That(DecodeForm(handler.Requests[5]), Does.Contain("id=abc&path=/completed"));
                That(DecodeForm(handler.Requests[6]), Does.Contain("id=abc|def&path=/incomplete"));
                That(DecodeForm(handler.Requests[7]), Does.Contain("hashes=abc&tags=linux,iso"));
            });
        }
    }

    [Test]
    public void GetTorrentCount_RejectsUnsupportedApiVersionBeforeSendingRequest()
    {
        var (netService, httpClient, handler) = CreateNetService(
                                                                 _ => StubHttpMessageHandler.JsonResponse("0"),
                                                                 new ApiVersion(2, 9, 1));
        using (netService)
        using (httpClient)
        {
            ThrowsAsync<Banned.Qbittorrent.Exceptions.QbittorrentNotSupportedException>(async () =>
                         await new TorrentService(netService, new ApiVersion(2, 9, 1)).GetTorrentCount());
            That(handler.Requests, Is.Empty);
        }
    }

    [TestCase(2, 10, EnumTorrentFilter.Paused, "paused")]
    [TestCase(2, 11, EnumTorrentFilter.Paused, "stopped")]
    [TestCase(2, 10, EnumTorrentFilter.Resumed, "resumed")]
    [TestCase(2, 11, EnumTorrentFilter.Resumed, "running")]
    [TestCase(2, 11, EnumTorrentFilter.StalledUploading, "stalled_uploading")]
    [TestCase(2, 11, EnumTorrentFilter.Error, "errored")]
    public async Task GetTorrentInfos_UsesVersionSpecificFilterName(int               major,
                                                                    int               minor,
                                                                    EnumTorrentFilter filter,
                                                                    string            expected)
    {
        var apiVersion = new ApiVersion(major, minor);
        var (netService, httpClient, handler) =
            CreateNetService(_ => StubHttpMessageHandler.JsonResponse("[]"), apiVersion);
        using (netService)
        using (httpClient)
        {
            var service = new TorrentService(netService, apiVersion);

            await service.GetTorrentInfos(filter : filter);
            await service.GetTorrentInfos(new GetTorrentInfoListRequest { Filter = filter });

            Multiple(() =>
            {
                That(handler.Requests, Has.Count.EqualTo(2));
                That(DecodeForm(handler.Requests[0]), Does.Contain($"filter={expected}"));
                That(DecodeForm(handler.Requests[1]), Does.Contain($"filter={expected}"));
            });
        }
    }

    [TestCase(true, "Original")]
    [TestCase(false, "NoSubfolder")]
    public async Task AddTorrent_ConvertsRootFolderToContentLayoutForWebApi27(bool enabled, string expected)
    {
        var (netService, httpClient, handler) =
            CreateNetService(_ => StubHttpMessageHandler.JsonResponse("Ok."), ApiVersion.V2_7_0);
        using (netService)
        using (httpClient)
        {
            await new TorrentService(netService, ApiVersion.V2_7_0).AddTorrent(new AddTorrentRequest
            {
                Urls              = ["magnet:?xt=urn:btih:abc"],
                RootFolderEnabled = enabled
            });

            var form = DecodeForm(handler.Requests.Single());
            Multiple(() =>
            {
                That(form, Does.Contain($"contentLayout={expected}"));
                That(form, Does.Not.Contain("root_folder="));
            });
        }
    }

    [TestCase(EnumContentLayout.Original, "true")]
    [TestCase(EnumContentLayout.Subfolder, "true")]
    [TestCase(EnumContentLayout.NoSubfolder, "false")]
    public async Task AddTorrent_ConvertsContentLayoutToRootFolderBeforeWebApi27(
        EnumContentLayout layout, string expected)
    {
        var apiVersion = ApiVersion.V2_6_0;
        var (netService, httpClient, handler) =
            CreateNetService(_ => StubHttpMessageHandler.JsonResponse("Ok."), apiVersion);
        using (netService)
        using (httpClient)
        {
            await new TorrentService(netService, apiVersion).AddTorrent(new AddTorrentRequest
            {
                Urls          = ["magnet:?xt=urn:btih:abc"],
                ContentLayout = layout
            });

            var form = DecodeForm(handler.Requests.Single());
            Multiple(() =>
            {
                That(form, Does.Contain($"root_folder={expected}"));
                That(form, Does.Not.Contain("contentLayout="));
            });
        }
    }

    [Test]
    public async Task AddTorrent_SerializesExtendedParametersOnLatestWebApi()
    {
        var (netService, httpClient, handler) =
            CreateNetService(_ => StubHttpMessageHandler.JsonResponse("Ok."), ApiVersion.V2_15_1);
        using (netService)
        using (httpClient)
        {
            await new TorrentService(netService, ApiVersion.V2_15_1)
               .AddTorrent(urls : ["magnet:?xt=urn:btih:abc"],
                           tags : "linux,iso",
                           stopped : true,
                           ratioLimit : 1.25f,
                           seedingTimeLimit : 120,
                           cookie : "session=abc",
                           contentLayout : EnumContentLayout.Subfolder,
                           downloadPath : "/incomplete",
                           useDownloadPath : false,
                           stopCondition : EnumTorrentAddStopCondition.FilesChecked,
                           addToTopOfQueue : true,
                           inactiveSeedingTimeLimit : 30,
                           shareLimitAction : EnumTorrentShareLimitAction.RemoveWithContent,
                           sslCertificate : "certificate",
                           sslPrivateKey : "private-key",
                           sslDhParameters : "dh-parameters",
                           forced : true);

            var form = DecodeForm(handler.Requests.Single());
            Multiple(() =>
            {
                That(form, Does.Contain("cookie=session=abc"));
                That(form, Does.Contain("tags=linux,iso"));
                That(form, Does.Contain("paused=true"));
                That(form, Does.Contain("stopped=true"));
                That(form, Does.Contain("contentLayout=Subfolder"));
                That(form, Does.Contain("ratioLimit=1.25"));
                That(form, Does.Contain("seedingTimeLimit=120"));
                That(form, Does.Contain("downloadPath=/incomplete"));
                That(form, Does.Contain("useDownloadPath=false"));
                That(form, Does.Contain("stopCondition=FilesChecked"));
                That(form, Does.Contain("addToTopOfQueue=true"));
                That(form, Does.Contain("inactiveSeedingTimeLimit=30"));
                That(form, Does.Contain("shareLimitAction=RemoveWithContent"));
                That(form, Does.Contain("ssl_certificate=certificate"));
                That(form, Does.Contain("ssl_private_key=private-key"));
                That(form, Does.Contain("ssl_dh_params=dh-parameters"));
                That(form, Does.Contain("forced=true"));
            });
        }
    }

    [TestCase(null, "true")]
    [TestCase(false, "false")]
    public async Task AddTorrent_DefaultsUseDownloadPathOnlyWhenNotExplicitlySet(bool? useDownloadPath,
        string                                                                         expected)
    {
        var (netService, httpClient, handler) =
            CreateNetService(_ => StubHttpMessageHandler.JsonResponse("Ok."), ApiVersion.V2_8_4);
        using (netService)
        using (httpClient)
        {
            await new TorrentService(netService, ApiVersion.V2_8_4).AddTorrent(new AddTorrentRequest
            {
                Urls                   = ["magnet:?xt=urn:btih:abc"],
                DownloadPath           = "/incomplete",
                UseDownloadPathEnabled = useDownloadPath
            });

            That(DecodeForm(handler.Requests.Single()), Does.Contain($"useDownloadPath={expected}"));
        }
    }

    [Test]
    public void AddTorrentRequest_ToDictionaryWithoutVersionSerializesExtendedParameters()
    {
        var parameters = new AddTorrentRequest
        {
            Tags          = "tag",
            DownloadPath  = "/incomplete",
            ForcedEnabled = true
        }.ToDictionary();

        Multiple(() =>
        {
            That(parameters["tags"], Is.EqualTo("tag"));
            That(parameters["downloadPath"], Is.EqualTo("/incomplete"));
            That(parameters["useDownloadPath"], Is.EqualTo("true"));
            That(parameters["forced"], Is.EqualTo("true"));
        });
    }

    [TestCase("tags")]
    [TestCase("ratioLimit")]
    [TestCase("seedingTimeLimit")]
    [TestCase("downloadPath")]
    [TestCase("useDownloadPath")]
    [TestCase("stopCondition")]
    [TestCase("addToTopOfQueue")]
    [TestCase("inactiveSeedingTimeLimit")]
    [TestCase("shareLimitAction")]
    [TestCase("ssl_certificate")]
    [TestCase("ssl_private_key")]
    [TestCase("ssl_dh_params")]
    [TestCase("forced")]
    public async Task AddTorrent_SendsExtendedParametersToOlderWebApiVersions(string parameter)
    {
        var apiVersion = new ApiVersion(2, 0);
        var request    = CreateAddTorrentRequestWithParameter(parameter);

        var (netService, httpClient, handler) =
            CreateNetService(_ => StubHttpMessageHandler.JsonResponse("Ok."), apiVersion);
        using (netService)
        using (httpClient)
        {
            await new TorrentService(netService, apiVersion).AddTorrent(request);

            Multiple(() =>
            {
                That(handler.Requests, Has.Count.EqualTo(1));
                That(DecodeForm(handler.Requests.Single()), Does.Contain($"{parameter}="));
            });
        }
    }

    [TestCase("setShareLimits", 2, 0, 0)]
    [TestCase("editTracker", 2, 1, 1)]
    [TestCase("renameFile", 2, 3, 0)]
    [TestCase("renameFolder", 2, 6, 0)]
    [TestCase("banPeers", 2, 2, 0)]
    public void VersionedOperations_RejectUnsupportedApiVersionBeforeSendingRequest(
        string operation, int major, int minor, int patch)
    {
        var apiVersion = new ApiVersion(major, minor, patch);
        var (netService, httpClient, handler) =
            CreateNetService(_ => StubHttpMessageHandler.JsonResponse(""), apiVersion);
        using (netService)
        using (httpClient)
        {
            var torrent  = new TorrentService(netService, apiVersion);
            var transfer = new TransferService(netService);
            Func<Task> action = operation switch
            {
                "setShareLimits" => async () => await torrent.SetTorrentShareLimit("abc", ratioLimit : 1),
                "editTracker"    => async () => await torrent.EditTorrentTracker("abc", "old", "new"),
                "renameFile"     => async () => await torrent.RenameTorrentFile("abc", 0, "new"),
                "renameFolder"   => async () => await torrent.RenameTorrentFolder("abc", "old", "new"),
                "banPeers"       => async () => await transfer.BanPeers(["127.0.0.1:6881"]),
                _                => throw new ArgumentOutOfRangeException(nameof(operation), operation, null)
            };

            ThrowsAsync<QbittorrentNotSupportedException>(action);
            That(handler.Requests, Is.Empty);
        }
    }

    [Test]
    public async Task TorrentCreatorOperations_UseDocumentedContractsAndPreserveTorrentBytes()
    {
        byte[] torrentBytes = [0, 255, 10, 128];
        var (netService, httpClient, handler) = CreateNetService(request =>
                                                                     request.RequestUri!.AbsolutePath switch
                                                                     {
                                                                         "/api/v2/torrentcreator/addTask" =>
                                                                             StubHttpMessageHandler.JsonResponse(
                                                                                  "{\"taskID\":\"task-1\",\"futureField\":\"preserved\"}"),
                                                                         "/api/v2/torrentcreator/status" =>
                                                                             StubHttpMessageHandler.JsonResponse(
                                                                                  "[{\"taskID\":\"task-1\",\"status\":\"Finished\",\"progress\":1,\"futureField\":42}]"),
                                                                         "/api/v2/torrentcreator/torrentFile" =>
                                                                             StubHttpMessageHandler
                                                                                .BytesResponse(torrentBytes),
                                                                         _ => StubHttpMessageHandler.JsonResponse("")
                                                                     });
        using (netService)
        using (httpClient)
        {
            var service = new TorrentCreatorService(netService);
            var task = await service.AddTask(new CreateTorrentTaskRequest
            {
                SourcePath          = "/data/source",
                TorrentFilePath     = "/data/result.torrent",
                Format              = EnumTorrentCreatorFormat.Hybrid,
                StartSeeding        = true,
                IsPrivate           = false,
                OptimizeAlignment   = true,
                PaddedFileSizeLimit = 1024,
                PieceSize           = 16384,
                Comment             = "release",
                Trackers            = ["https://tracker.example/announce"],
                UrlSeeds            = ["https://seed.example/file"]
            });
            var statuses    = await service.GetStatuses(task!.TaskId);
            var torrentFile = await service.GetTorrentFile(task.TaskId!);
            await service.DeleteTask(task.TaskId!);

            Multiple(() =>
            {
                That(task.TaskId, Is.EqualTo("task-1"));
                That(task.AdditionalData, Contains.Key("futureField"));
                That(statuses.Single().Status, Is.EqualTo(EnumTorrentCreatorTaskStatus.Finished));
                That(statuses.Single().Progress, Is.EqualTo(1));
                That(statuses.Single().AdditionalData, Contains.Key("futureField"));
                That(torrentFile, Is.EqualTo(torrentBytes));
                That(handler.Requests.Select(request => request.Uri.AbsolutePath), Is.EqualTo([
                    "/api/v2/torrentcreator/addTask",
                    "/api/v2/torrentcreator/status",
                    "/api/v2/torrentcreator/torrentFile",
                    "/api/v2/torrentcreator/deleteTask"
                ]));
                That(DecodeForm(handler.Requests[0]), Does.Contain("sourcePath=/data/source"));
                That(DecodeForm(handler.Requests[0]), Does.Contain("format=hybrid"));
                That(DecodeForm(handler.Requests[0]), Does.Contain("startSeeding=true"));
                That(DecodeForm(handler.Requests[0]), Does.Contain("private=false"));
                That(DecodeForm(handler.Requests[0]), Does.Contain("trackers=https://tracker.example/announce"));
                That(DecodeForm(handler.Requests[1]), Is.EqualTo("taskID=task-1"));
                That(DecodeForm(handler.Requests[2]), Is.EqualTo("taskID=task-1"));
                That(DecodeForm(handler.Requests[3]), Is.EqualTo("taskID=task-1"));
            });
        }
    }

    [Test]
    public async Task GetSearchCategories_UsesEndpointBeforeItsRemovalVersion()
    {
        const string json = """
            [
              {
                "id": "all",
                "name": "All categories"
              }
            ]
            """;
        var (netService, httpClient, handler) =
            CreateNetService(_ => StubHttpMessageHandler.JsonResponse(json), ApiVersion.V2_5_1);
        using (netService)
        using (httpClient)
        {
            var categories = await new SearchService(netService).GetSearchCategories("enabled");

            Multiple(() =>
            {
                That(categories.Single().Id, Is.EqualTo("all"));
                That(categories.Single().Name, Is.EqualTo("All categories"));
                That(handler.Requests, Has.Count.EqualTo(1));
                That(handler.Requests[0].Method, Is.EqualTo(HttpMethod.Post));
                That(handler.Requests[0].Uri.AbsolutePath, Is.EqualTo("/api/v2/search/categories"));
                That(DecodeForm(handler.Requests[0]), Is.EqualTo("pluginName=enabled"));
            });
        }
    }

    [Test]
    public void GetSearchCategories_RejectsRemovedEndpointBeforeSendingRequest()
    {
        var (netService, httpClient, handler) =
            CreateNetService(_ => StubHttpMessageHandler.JsonResponse("[]"), ApiVersion.V2_6_0);
        using (netService)
        using (httpClient)
        {
            ThrowsAsync<QbittorrentEndpointRemovedException>(async () => await new SearchService(netService)
                                                                .GetSearchCategories());
            That(handler.Requests, Is.Empty);
        }
    }

    private static (NetService NetService, HttpClient HttpClient, StubHttpMessageHandler Handler)
        CreateNetService(string json) => CreateNetService(_ => StubHttpMessageHandler.JsonResponse(json));

    private static (NetService NetService, HttpClient HttpClient, StubHttpMessageHandler Handler)
        CreateNetService(Func<HttpRequestMessage, HttpResponseMessage> responseFactory, ApiVersion? apiVersion = null)
    {
        var handler    = new StubHttpMessageHandler(responseFactory);
        var httpClient = new HttpClient(handler);
        var netService = new NetService("http://localhost:8080", httpClient);
        netService.SetApiVersion(apiVersion ?? new ApiVersion(2, 15, 1));
        return (netService, httpClient, handler);
    }

    private static string DecodeForm(HttpRequestSnapshot request) =>
        Uri.UnescapeDataString(request.Body ?? string.Empty);

    private static AddTorrentRequest CreateAddTorrentRequestWithParameter(string parameter)
    {
        var request = new AddTorrentRequest { Urls = ["magnet:?xt=urn:btih:abc"] };
        switch (parameter)
        {
            case "tags" :
                request.Tags = "tag";
                break;
            case "ratioLimit" :
                request.RatioLimit = 1;
                break;
            case "seedingTimeLimit" :
                request.SeedingTimeLimit = 1;
                break;
            case "downloadPath" :
                request.DownloadPath           = "/incomplete";
                request.UseDownloadPathEnabled = false;
                break;
            case "useDownloadPath" :
                request.UseDownloadPathEnabled = true;
                break;
            case "stopCondition" :
                request.StopCondition = EnumTorrentAddStopCondition.MetadataReceived;
                break;
            case "addToTopOfQueue" :
                request.AddToTopOfQueueEnabled = true;
                break;
            case "inactiveSeedingTimeLimit" :
                request.InactiveSeedingTimeLimit = 1;
                break;
            case "shareLimitAction" :
                request.ShareLimitAction = EnumTorrentShareLimitAction.Stop;
                break;
            case "ssl_certificate" :
                request.SslCertificate = "certificate";
                break;
            case "ssl_private_key" :
                request.SslPrivateKey = "private-key";
                break;
            case "ssl_dh_params" :
                request.SslDhParameters = "dh-parameters";
                break;
            case "forced" :
                request.ForcedEnabled = true;
                break;
            default :
                throw new ArgumentOutOfRangeException(nameof(parameter), parameter, null);
        }

        return request;
    }
}
