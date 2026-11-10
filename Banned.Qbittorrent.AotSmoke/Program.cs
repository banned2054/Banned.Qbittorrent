using Banned.Qbittorrent;
using Banned.Qbittorrent.Models.Application;
using System.Net;
using System.Text;

using var httpClient = new HttpClient(new StubQbittorrentHandler());
using var client     = await QBittorrentClient.Create("http://localhost:8080", "admin", "password", httpClient);

var preferences = await client.Application.GetApplicationPreferences();
if (preferences?.Locale != "en" || preferences.ConfirmRemoveAllTags != true ||
    preferences.ConfirmRemoveTrackerFromAllTorrents != false)
    throw new InvalidOperationException("Source-generated preference deserialization failed.");

await client.Application.SetApplicationPreferences(new ApplicationPreferences { Locale = "en" });

var torrents       = await client.Torrent.GetTorrentInfos();
var logs           = await client.Log.GetLogs();
var rules          = await client.Rss.GetAllAutoDownloadingRule();
var searchStatuses = await client.Search.SearchStatus();
var searchResults  = await client.Search.GetSearchResults(1);
var mainData       = await client.Sync.GetMainData();
var creatorTasks   = await client.TorrentCreator.GetStatuses();
_ = await client.Transfer.GetTransferInfo();
var speedLimits = await client.Transfer.GetSpeedLimits();

if (torrents.Count                       != 0    ||
    logs.Count                           != 0    ||
    rules?.Count                         != 0    ||
    searchStatuses?.Length               != 0    ||
    creatorTasks.Count                   != 0    ||
    speedLimits.UploadLimit              != 2048 ||
    speedLimits.DownloadLimit            != 1024 ||
    speedLimits.AlternativeUploadLimit   != 256  ||
    speedLimits.AlternativeDownloadLimit != 512  ||
    mainData?.Torrents?["abc"]?.Error     != "Disk error" ||
    searchResults?.Status?.Status        != "Stopped" ||
    searchResults.Results?[0].EngineFullName != "Example Engine")
    throw new InvalidOperationException("A source-generated JSON contract failed the NativeAOT smoke test.");

Console.WriteLine("NativeAOT smoke test passed.");

file sealed class StubQbittorrentHandler : HttpMessageHandler
{
    protected override Task<HttpResponseMessage> SendAsync(
        HttpRequestMessage request, CancellationToken cancellationToken)
    {
        var responseBody = request.RequestUri?.AbsolutePath switch
        {
            "/api/v2/auth/login"            => "Ok.",
            "/api/v2/app/webapiVersion"     => "2.17.0",
            "/api/v2/app/version"           => "5.4.0alpha1",
            "/api/v2/app/preferences"       => "{\"locale\":\"en\",\"confirm_remove_all_tags\":true,\"confirm_remove_tracker_from_all_torrents\":false}",
            "/api/v2/torrents/info"         => "[]",
            "/api/v2/log/main"              => "[]",
            "/api/v2/rss/rules"             => "{}",
            "/api/v2/search/status"         => "[]",
            "/api/v2/search/results"        => "{\"status\":\"Stopped\",\"results\":[{\"engineName\":\"example\",\"engineFullName\":\"Example Engine\"}],\"total\":1}",
            "/api/v2/torrentcreator/status" => "[]",
            "/api/v2/sync/maindata"         => "{\"torrents\":{\"abc\":{\"error\":\"Disk error\"}}}",
            "/api/v2/transfer/info"         => "{}",
            "/api/v2/transfer/getSpeedLimits" =>
                "{\"up_limit\":2048,\"dl_limit\":1024,\"alt_up_limit\":256,\"alt_dl_limit\":512}",
            _ => string.Empty
        };

        var response = new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content        = new StringContent(responseBody, Encoding.UTF8, "application/json"),
            RequestMessage = request
        };
        return Task.FromResult(response);
    }
}
