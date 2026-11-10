using Banned.Qbittorrent.Models.Application;
using Banned.Qbittorrent.Services;
using static NUnit.Framework.Assert;

namespace Banned.Qbittorrent.Test;

[TestFixture]
public class LegacyCategoryCompatibilityTests
{
    [TestCase("2.1.1")]
    [TestCase("2.8.3")]
    [TestCase("2.16.1")]
    public async Task OriginalCategoryCalls_KeepLegacyFormsOnOlderServers(string version)
    {
        var apiVersion = ApiVersion.Parse(version);
        var handler = new StubHttpMessageHandler(_ => StubHttpMessageHandler.JsonResponse(""));
        using var httpClient = new HttpClient(handler);
        using var netService = new NetService("http://localhost:8080", httpClient);
        netService.SetApiVersion(apiVersion);
        var torrent = new TorrentService(netService, apiVersion);
        using var cancellation = new CancellationTokenSource();

        await torrent.CreateCategory("movies", "", null, false, cancellation.Token);
        await torrent.EditCategory("movies", "", null, false, cancellation.Token);
        await torrent.CreateCategory("movies", "/movies", cancellationToken : cancellation.Token);
        await torrent.EditCategory("movies", "/movies", downloadPath : "/incomplete",
                                   downloadPathEnable : true, cancellationToken : cancellation.Token);

        That(handler.Requests.Select(request => request.Uri.AbsolutePath), Is.EqualTo(new[]
        {
            "/api/v2/torrents/createCategory", "/api/v2/torrents/editCategory",
            "/api/v2/torrents/createCategory", "/api/v2/torrents/editCategory"
        }));
        That(handler.Requests.Select(request => Uri.UnescapeDataString(request.Body!)), Is.EqualTo(new[]
        {
            "category=movies&savePath=&downloadPathEnabled=False",
            "category=movies&savePath=&downloadPathEnabled=False",
            "category=movies&savePath=/movies",
            "category=movies&savePath=/movies&downloadPath=/incomplete&downloadPathEnabled=True"
        }));
    }

    [Test]
    public async Task OlderSearchResults_AllowMissingEngineFullName()
    {
        const string json = """{"status":"Running","results":[{"engineName":"example"}],"total":1}""";
        var handler = new StubHttpMessageHandler(_ => StubHttpMessageHandler.JsonResponse(json));
        using var httpClient = new HttpClient(handler);
        using var netService = new NetService("http://localhost:8080", httpClient);
        netService.SetApiVersion(new ApiVersion(2, 8, 3));

        var result = await new SearchService(netService).GetSearchResults(1);

        Multiple(() =>
        {
            That(result!.Status!.Status, Is.EqualTo("Running"));
            That(result.Results![0].EngineName, Is.EqualTo("example"));
            That(result.Results[0].EngineFullName, Is.Null);
        });
    }
}
