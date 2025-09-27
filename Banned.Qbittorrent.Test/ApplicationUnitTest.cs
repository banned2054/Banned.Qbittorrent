using DotNetEnv;

namespace Banned.Qbittorrent.Test;

public class ApplicationUnitTest
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
    public async Task TestGetApiVersion()
    {
        var version = await _client.Application.GetApiVersion();
        Console.WriteLine(version);
        Assert.Pass();
    }

    [Test]
    public async Task TestGetVersion()
    {
        var version = await _client.Application.GetVersion();
        Console.WriteLine(version);
        Assert.Pass();
    }

    [Test]
    public async Task TestGetBuildInfo()
    {
        var buildInfo = await _client.Application.GetBuildInfo();
        Console.WriteLine(buildInfo);
        Assert.Pass();
    }
}
