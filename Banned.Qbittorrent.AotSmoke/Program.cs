using Banned.Qbittorrent;
using Banned.Qbittorrent.Models.Application;
using System.Net;
using System.Text;

using var httpClient = new HttpClient(new StubQbittorrentHandler());
using var client     = await QBittorrentClient.Create("http://localhost:8080", "admin", "password", httpClient);

var preferences = await client.Application.GetApplicationPreferences();
if (preferences?.Locale != "en")
    throw new InvalidOperationException("Source-generated preference deserialization failed.");

await client.Application.SetApplicationPreferences(new ApplicationPreferences { Locale = "en" });

var torrents       = await client.Torrent.GetTorrentInfos();
var logs           = await client.Log.GetLogs();
var rules          = await client.Rss.GetAllAutoDownloadingRule();
var searchStatuses = await client.Search.SearchStatus();
var mainData       = await client.Sync.GetMainData();
_ = await client.Transfer.GetTransferInfo();

if (torrents.Count         != 0 ||
    logs.Count             != 0 ||
    rules?.Count           != 0 ||
    searchStatuses?.Length != 0 ||
    mainData               == null)
    throw new InvalidOperationException("A source-generated JSON contract failed the NativeAOT smoke test.");

Console.WriteLine("NativeAOT smoke test passed.");

file sealed class StubQbittorrentHandler : HttpMessageHandler
{
    protected override Task<HttpResponseMessage> SendAsync(
        HttpRequestMessage request,
        CancellationToken  cancellationToken)
    {
        var responseBody = request.RequestUri?.AbsolutePath switch
        {
            "/api/v2/auth/login"        => "Ok.",
            "/api/v2/app/webapiVersion" => "2.15.1",
            "/api/v2/app/preferences"   => "{\"locale\":\"en\"}",
            "/api/v2/torrents/info"     => "[]",
            "/api/v2/log/main"          => "[]",
            "/api/v2/rss/rules"         => "{}",
            "/api/v2/search/status"     => "[]",
            "/api/v2/sync/maindata"     => "{}",
            "/api/v2/transfer/info"     => "{}",
            _                           => string.Empty
        };

        var response = new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content        = new StringContent(responseBody, Encoding.UTF8, "application/json"),
            RequestMessage = request
        };
        return Task.FromResult(response);
    }
}
