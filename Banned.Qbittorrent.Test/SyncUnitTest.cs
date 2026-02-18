using DotNetEnv;

namespace Banned.Qbittorrent.Test;

internal class SyncUnitTest
{
    private QBittorrentClient _client;

    public readonly string Hash = "f1d24aa14594ca77117c3761ab951d75b9e22f72";

    [SetUp]
    public async Task SetUp()
    {
        Env.TraversePath().Load();
        var url  = Environment.GetEnvironmentVariable("QBT_URL");
        var user = Environment.GetEnvironmentVariable("QBT_USER");
        var pass = Environment.GetEnvironmentVariable("QBT_PASS");
        _client = await QBittorrentClient.Create(url, user, pass);
    }


    [Test]
    public async Task TestSyncMain()
    {
        var data = await _client.Sync.GetMainData(0);
        Console.WriteLine(data.Rid);
    }

    [Test]
    public async Task TestSyncPeers()
    {
        var data = await _client.Sync.GetPeerData(0, Hash);
        Console.WriteLine(data.Rid);
    }
}
