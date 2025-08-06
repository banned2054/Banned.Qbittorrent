namespace Banned.Qbittorrent.Test;

public class TorrentUnitTest
{
    private QBittorrentClient _client;

    [SetUp]
    public void SetUp()
    {
        // 从 JSON 文件中读取配置
        _client = new QBittorrentClient(StaticConfig.BaseUrl, StaticConfig.Username, StaticConfig.Password);
    }

    [Test]
    public async Task TestGetAllTorrentInfo()
    {
        var data = await _client.Torrent.GetTorrentInfosAsync();
        foreach (var t in data)
        {
            Console.WriteLine($"{t.Name} {string.Join(',', t.TagList)}");
        }
    }

    [Test]
    public async Task TestDeleteTorrent()
    {
        await _client.Torrent.DeleteTorrentAsync("b963306bd91ff97492079b5510e91a111757322f");
    }

    [Test]
    public async Task TestAddTorrent()
    {
        var torrentPaths =
            new List<string>
            {
                @"D:\Downloads\Your.Forma.S01E01.1080p.BILI.WEB-DL.AAC2.0.H.264-VARYG.torrent"
            };
        const string tags = "test";
        await _client.Torrent.AddTorrentAsync(
                                              filePaths : torrentPaths,
                                              savePath : "/downloads",
                                              stopped : true,
                                              tags : tags,
                                              rename : "test-download"
                                             );
    }

    [Test]
    public async Task GetTorrents()
    {
        var trackers =
            await _client.Torrent.GetTorrentInfoAsync(hash :
                                                      "a940478142ad9d776a342f512a59e53c3304b8cee4de991c3b09e0bf214f366d");
        if (trackers.Count == 0)
        {
            Console.WriteLine("Not find");
        }
        else
        {
            Console.WriteLine(trackers[0]);
        }
    }

    [Test]
    public async Task GetTorrentTrackers()
    {
        var trackers = await _client.Torrent.GetTorrentInfoAsync(hash : "0f9323316a62a4a6b66a5568893a52c3a342501d");
    }

    [Test]
    public async Task SetTorrentLocation()
    {
        var torrent = await _client.Torrent.GetTorrentInfoAsync(hash : "436f9a9ac50e513b9b046c2f04958aafae24d175");
        foreach (var t in torrent)
        {
            Console.WriteLine($"{t.Name}\n\t{t.Hash}\n\t{t.SavePath}\n\t{t.ContentPath}");
        }
    }
}
