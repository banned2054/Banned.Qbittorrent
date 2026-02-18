using DotNetEnv;

namespace Banned.Qbittorrent.Test;

internal class RssUnitTest
{
    private QBittorrentClient _client;

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
    public async Task TestGetAllAutoDownloadingRules()
    {
        var version = await _client.Rss.GetAllAutoDownloadingRule();
        Console.WriteLine(version);
        Assert.Pass();
    }
}
