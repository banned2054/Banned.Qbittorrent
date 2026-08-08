using Banned.Qbittorrent.Models.Application;
using Banned.Qbittorrent.Services;

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

            Assert.Multiple(() =>
            {
                Assert.That(result, Is.Not.Null);
                Assert.That(result!.Bitness, Is.EqualTo(64));
                Assert.That(result.LibtorrentVersion, Is.EqualTo("2.0.11.0"));
                Assert.That(handler.Requests, Has.Count.EqualTo(1));
                Assert.That(handler.Requests[0].Method, Is.EqualTo(HttpMethod.Get));
                Assert.That(handler.Requests[0].Uri.AbsolutePath, Is.EqualTo("/api/v2/app/buildInfo"));
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

            Assert.Multiple(() =>
            {
                Assert.That(result, Has.Count.EqualTo(1));
                Assert.That(result[0].Name, Is.EqualTo("example"));
                Assert.That(handler.Requests, Has.Count.EqualTo(1));
                Assert.That(handler.Requests[0].Method, Is.EqualTo(HttpMethod.Get));
                Assert.That(handler.Requests[0].Uri.AbsolutePath, Is.EqualTo("/api/v2/search/plugins"));
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
            var result = await new TorrentService(netService, new ApiVersion(2, 15, 1)).GetAllCategories();
            var categories = result!;

            Assert.Multiple(() =>
            {
                Assert.That(result, Is.Not.Null);
                Assert.That(categories, Contains.Key("linux"));
                Assert.That(categories["linux"].SavePath, Is.EqualTo("/downloads/linux"));
                Assert.That(handler.Requests, Has.Count.EqualTo(1));
                Assert.That(handler.Requests[0].Method, Is.EqualTo(HttpMethod.Get));
                Assert.That(handler.Requests[0].Uri.AbsolutePath, Is.EqualTo("/api/v2/torrents/categories"));
            });
        }
    }

    [Test]
    public async Task SearchOperations_UseDocumentedEndpointsAndParameters()
    {
        var responses = new Queue<string>(["{\"id\":42}", ""]);
        var (netService, httpClient, handler) = CreateNetService(_ =>
            StubHttpMessageHandler.JsonResponse(responses.Dequeue()));
        using (netService)
        using (httpClient)
        {
            var service = new SearchService(netService);

            var job = await service.StartSearch("linux", ["example"]);
            await service.DownloadTorrent("magnet:?xt=urn:btih:abc", "example");

            Assert.Multiple(() =>
            {
                Assert.That(job?.Id, Is.EqualTo(42));
                Assert.That(handler.Requests, Has.Count.EqualTo(2));
                Assert.That(handler.Requests[0].Uri.AbsolutePath, Is.EqualTo("/api/v2/search/start"));
                Assert.That(DecodeForm(handler.Requests[0]), Does.Contain("pattern=linux"));
                Assert.That(handler.Requests[1].Uri.AbsolutePath, Is.EqualTo("/api/v2/search/downloadTorrent"));
                Assert.That(DecodeForm(handler.Requests[1]), Does.Contain("pluginName=example"));
                Assert.That(DecodeForm(handler.Requests[1]), Does.Contain("torrentUrl=magnet:?xt=urn:btih:abc"));
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
        var (netService, httpClient, handler) = CreateNetService(_ =>
            StubHttpMessageHandler.JsonResponse(responses.Dequeue()));
        using (netService)
        using (httpClient)
        {
            var service = new ApplicationService(netService);

            var interfaces = await service.GetNetworkInterfaces();
            var addresses = await service.GetNetworkInterfaceAddresses("eth0");
            await service.SendTestEmail();
            var contents = await service.GetDirectoryContent("/downloads");
            var metadata = await service.GetDirectoryContentWithMetadata("/downloads");

            Assert.Multiple(() =>
            {
                Assert.That(interfaces.Single().Value, Is.EqualTo("eth0"));
                Assert.That(addresses, Is.EqualTo(new[] { "127.0.0.1", "::1" }));
                Assert.That(contents, Is.EqualTo(new[] { "/downloads/a.torrent" }));
                Assert.That(metadata.Single()["size"].GetInt32(), Is.EqualTo(123));
                Assert.That(handler.Requests.Select(request => request.Uri.AbsolutePath), Is.EqualTo(new[]
                {
                    "/api/v2/app/networkInterfaceList",
                    "/api/v2/app/networkInterfaceAddressList",
                    "/api/v2/app/sendTestEmail",
                    "/api/v2/app/getDirectoryContent",
                    "/api/v2/app/getDirectoryContent"
                }));
                Assert.That(DecodeForm(handler.Requests[1]), Does.Contain("iface=eth0"));
                Assert.That(DecodeForm(handler.Requests[3]), Does.Contain("withMetadata=false"));
                Assert.That(DecodeForm(handler.Requests[4]), Does.Contain("withMetadata=true"));
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
                "/api/v2/torrents/count"  => StubHttpMessageHandler.JsonResponse("7"),
                "/api/v2/torrents/export" => StubHttpMessageHandler.BytesResponse(torrentBytes),
                _                           => StubHttpMessageHandler.JsonResponse("")
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

            Assert.Multiple(() =>
            {
                Assert.That(count, Is.EqualTo(7));
                Assert.That(exported, Is.EqualTo(torrentBytes));
                Assert.That(handler.Requests.Select(request => request.Uri.AbsolutePath), Is.EqualTo(new[]
                {
                    "/api/v2/torrents/count",
                    "/api/v2/torrents/addWebSeeds",
                    "/api/v2/torrents/editWebSeed",
                    "/api/v2/torrents/removeWebSeeds",
                    "/api/v2/torrents/export",
                    "/api/v2/torrents/setSavePath",
                    "/api/v2/torrents/setDownloadPath",
                    "/api/v2/torrents/setTags"
                }));
                Assert.That(DecodeForm(handler.Requests[1]), Does.Contain("urls=https://example.com/seed"));
                Assert.That(DecodeForm(handler.Requests[2]), Does.Contain("origUrl=https://example.com/seed"));
                Assert.That(DecodeForm(handler.Requests[5]), Does.Contain("id=abc&path=/completed"));
                Assert.That(DecodeForm(handler.Requests[6]), Does.Contain("id=abc|def&path=/incomplete"));
                Assert.That(DecodeForm(handler.Requests[7]), Does.Contain("hashes=abc&tags=linux,iso"));
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
            Assert.ThrowsAsync<Banned.Qbittorrent.Exceptions.QbittorrentNotSupportedException>(async () =>
                await new TorrentService(netService, new ApiVersion(2, 9, 1)).GetTorrentCount());
            Assert.That(handler.Requests, Is.Empty);
        }
    }

    private static (NetService NetService, HttpClient HttpClient, StubHttpMessageHandler Handler)
        CreateNetService(string json)
        => CreateNetService(_ => StubHttpMessageHandler.JsonResponse(json));

    private static (NetService NetService, HttpClient HttpClient, StubHttpMessageHandler Handler)
        CreateNetService(
            Func<HttpRequestMessage, HttpResponseMessage> responseFactory,
            ApiVersion? apiVersion = null)
    {
        var handler = new StubHttpMessageHandler(responseFactory);
        var httpClient = new HttpClient(handler);
        var netService = new NetService("http://localhost:8080", httpClient);
        netService.SetApiVersion(apiVersion ?? new ApiVersion(2, 15, 1));
        return (netService, httpClient, handler);
    }

    private static string DecodeForm(HttpRequestSnapshot request) =>
        Uri.UnescapeDataString(request.Body ?? string.Empty);
}
