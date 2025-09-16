namespace Banned.Qbittorrent.Test;

public class ApplicationUnitTest
{
    private QBittorrentClient _client;

    [SetUp]
    public async Task SetUp()
    {
        // 从 JSON 文件中读取配置
        _client = await QBittorrentClient.CreateAsync(StaticConfig.BaseUrl,
                                                      StaticConfig.Username,
                                                      StaticConfig.Password);
    }

    [Test]
    public async Task TestGetApiVersion()
    {
        var version = await _client.Application.GetApiVersionAsync();
        Console.WriteLine(version);
        Assert.Pass();
    }

    [Test]
    public async Task TestGetBuildInfo()
    {
        var buildInfo = await _client.Application.GetBuildInfoAsync();
        Console.WriteLine(buildInfo);
        Assert.Pass();
    }
}
