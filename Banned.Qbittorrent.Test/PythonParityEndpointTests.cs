using Banned.Qbittorrent.Exceptions;
using Banned.Qbittorrent.Models.Application;
using Banned.Qbittorrent.Models.Requests;
using Banned.Qbittorrent.Services;
using System.Text.Json;
using static NUnit.Framework.Assert;

namespace Banned.Qbittorrent.Test;

[TestFixture]
public class PythonParityEndpointTests
{
    [Test]
    public void AddTorrentRequest_SendsPythonCompatibilityAndExtendedFields()
    {
        var parameters = new AddTorrentRequest
        {
            SkipCheckingEnabled = true,
            FilePriorities      = [0, 1, 7],
            Downloader          = "aria2"
        }.ToDictionary();

        Multiple(() =>
        {
            That(parameters["skip_checking"], Is.EqualTo("true"));
            That(parameters["seedMode"], Is.EqualTo("true"));
            That(parameters["filePriorities"], Is.EqualTo("0,1,7"));
            That(parameters["downloader"], Is.EqualTo("aria2"));
        });
    }

    [Test]
    public async Task Reannounce_SendsOptionalTrackerUrlsAsPipeSeparatedValues()
    {
        var (netService, httpClient, handler) = CreateNetService(_ => StubHttpMessageHandler.JsonResponse(""));
        using (netService)
        using (httpClient)
        {
            await new TorrentService(netService, ApiVersion.V2_16_2)
               .ReannounceTorrents(["abc", "def"], ["https://one", "https://two"]);

            That(handler.Requests.Single().Method, Is.EqualTo(HttpMethod.Post));
            That(handler.Requests.Single().Uri.AbsolutePath, Is.EqualTo("/api/v2/torrents/reannounce"));
            That(DecodeForm(handler.Requests.Single()), Is.EqualTo("hashes=abc|def&urls=https://one|https://two"));
        }
    }

    [Test]
    public async Task PieceAvailability_UsesPostAndDeserializesCounts()
    {
        var (netService, httpClient, handler) =
            CreateNetService(_ => StubHttpMessageHandler.JsonResponse("[0,2,1]"));
        using (netService)
        using (httpClient)
        {
            var result = await new TorrentService(netService, ApiVersion.V2_16_2)
               .GetTorrentPieceAvailability("abc");

            Multiple(() =>
            {
                That(result, Is.EqualTo([0, 2, 1]));
                That(handler.Requests.Single().Method, Is.EqualTo(HttpMethod.Post));
                That(DecodeForm(handler.Requests.Single()), Is.EqualTo("hash=abc"));
            });
        }
    }

    [Test]
    public async Task SslParameters_UsePythonEndpointNamesAndPost()
    {
        var (netService, httpClient, handler) =
            CreateNetService(_ => StubHttpMessageHandler.JsonResponse(
                                                                      """{"ssl_certificate":"cert","ssl_private_key":"key","ssl_dh_params":"dh"}"""));
        using (netService)
        using (httpClient)
        {
            var service = new TorrentService(netService, ApiVersion.V2_16_2);
            var result  = await service.GetTorrentSslParameters("abc");

            await service.SetTorrentSslParameters("abc", "cert", "key", "dh");

            Multiple(() =>
            {
                That(result?.Certificate, Is.EqualTo("cert"));
                That(result?.PrivateKey, Is.EqualTo("key"));
                That(result?.DhParameters, Is.EqualTo("dh"));
                That(handler.Requests[0].Method, Is.EqualTo(HttpMethod.Post));
                That(handler.Requests[0].Uri.AbsolutePath, Is.EqualTo("/api/v2/torrents/SSLParameters"));
                That(handler.Requests[1].Uri.AbsolutePath, Is.EqualTo("/api/v2/torrents/setSSLParameters"));
                That(DecodeForm(handler.Requests[1]),
                     Is.EqualTo("hash=abc&ssl_certificate=cert&ssl_private_key=key&ssl_dh_params=dh"));
            });
        }
    }

    [Test]
    public async Task MetadataEndpoints_UsePythonMethodsAndVersions()
    {
        var (netService, httpClient, handler) =
            CreateNetService(request => request.RequestUri!.AbsolutePath.EndsWith("saveMetadata")
                                 ? StubHttpMessageHandler.BytesResponse([1, 2, 3])
                                 : StubHttpMessageHandler.JsonResponse("""{"name":"example"}"""));
        using (netService)
        using (httpClient)
        {
            var service = new TorrentService(netService, ApiVersion.V2_16_2);
            var fetched = await service.FetchTorrentMetadata("magnet:?xt=urn:btih:abc", "aria2");
            var saved   = await service.SaveTorrentMetadata("magnet:?xt=urn:btih:abc");

            Multiple(() =>
            {
                That(fetched["name"].GetString(), Is.EqualTo("example"));
                That(saved, Is.EqualTo(new byte[] { 1, 2, 3 }));
                That(handler.Requests[0].Method, Is.EqualTo(HttpMethod.Post));
                That(DecodeForm(handler.Requests[0]), Is.EqualTo("source=magnet:?xt=urn:btih:abc&downloader=aria2"));
                That(handler.Requests[1].Method, Is.EqualTo(HttpMethod.Post));
            });
        }
    }

    [Test]
    public async Task ApiKeyEndpointsUseTheirIndividualVersionBoundaries()
    {
        var (netService, httpClient, handler) =
            CreateNetService(_ => StubHttpMessageHandler.JsonResponse("""{"apiKey":"rotated"}"""));
        using (netService)
        using (httpClient)
        {
            var application = new ApplicationService(netService);
            var apiKey      = await application.RotateApiKey();
            await application.DeleteApiKey();

            Multiple(() =>
            {
                That(apiKey, Is.EqualTo("rotated"));
                That(handler.Requests[0].Uri.AbsolutePath, Is.EqualTo("/api/v2/app/rotateAPIKey"));
                That(handler.Requests[1].Uri.AbsolutePath, Is.EqualTo("/api/v2/app/deleteAPIKey"));
            });
        }
    }

    [Test]
    public async Task ClientDataLoadUsesGetAndStoreUsesPost()
    {
        var (netService, httpClient, handler) =
            CreateNetService(_ => StubHttpMessageHandler.JsonResponse("""{"theme":"dark"}"""));
        using (netService)
        using (httpClient)
        {
            var service = new ClientDataService(netService);
            var loaded  = await service.Load(["theme"]);

            using var document = JsonDocument.Parse("""{"theme":"light"}""");
            await service.Store(new Dictionary<string, JsonElement>
            {
                ["theme"] = document.RootElement.GetProperty("theme").Clone()
            });

            Multiple(() =>
            {
                That(loaded["theme"].GetString(), Is.EqualTo("dark"));
                That(handler.Requests[0].Method, Is.EqualTo(HttpMethod.Get));
                That(Uri.UnescapeDataString(handler.Requests[0].Uri.Query),
                     Is.EqualTo("?keys=[\"theme\"]"));
                That(handler.Requests[1].Method, Is.EqualTo(HttpMethod.Post));
                That(DecodeForm(handler.Requests[1]), Is.EqualTo("data={\"theme\":\"light\"}"));
            });
        }
    }

    [Test]
    public void VersionBoundariesRejectUnsupportedPythonEndpointsBeforeSending()
    {
        var (netService, httpClient, handler) =
            CreateNetService(_ => StubHttpMessageHandler.JsonResponse(""), new ApiVersion(2, 15));
        using (netService)
        using (httpClient)
        {
            ThrowsAsync<QbittorrentNotSupportedException>(async () =>
                                                              await new TorrentService(netService,
                                                                           new ApiVersion(2, 15))
                                                                 .GetTorrentPieceAvailability("abc"));
            That(handler.Requests, Is.Empty);
        }
    }

    private static (NetService NetService, HttpClient HttpClient, StubHttpMessageHandler Handler)
        CreateNetService(Func<HttpRequestMessage, HttpResponseMessage> responseFactory, ApiVersion? apiVersion = null)
    {
        var handler    = new StubHttpMessageHandler(responseFactory);
        var httpClient = new HttpClient(handler);
        var netService = new NetService("http://localhost:8080", httpClient);
        netService.SetApiVersion(apiVersion ?? ApiVersion.V2_16_2);
        return (netService, httpClient, handler);
    }

    private static string DecodeForm(HttpRequestSnapshot request) =>
        Uri.UnescapeDataString(request.Body ?? string.Empty);
}
