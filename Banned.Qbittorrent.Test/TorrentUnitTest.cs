using DotNetEnv;

namespace Banned.Qbittorrent.Test;

public class TorrentUnitTest
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
    public async Task TestGetAllTorrentInfo()
    {
        var data = await _client.Torrent.GetTorrentInfos();
        foreach (var t in data)
        {
            Console.WriteLine($"{t.Name} {string.Join(',', t.TagList)}");
        }
    }

    [Test]
    public async Task TestAddTorrent()
    {
        var torrentPaths = new List<string>
        {
            @"D:\Downloads\Your.Forma.S01E01.1080p.BILI.WEB-DL.AAC2.0.H.264-VARYG.torrent"
        };
        const string tags = "test";
        await _client.Torrent.AddTorrent(
                                         filePaths : torrentPaths,
                                         savePath : "/downloads",
                                         stopped : true,
                                         tags : tags,
                                         rename : "test-download"
                                        );
    }

    [Test]
    public async Task TestDeleteTorrent()
    {
        await _client.Torrent.DeleteTorrent("b963306bd91ff97492079b5510e91a111757322f");
    }


    [Test]
    public async Task GetTorrents()
    {
        var tracker =
            await _client.Torrent.GetTorrentInfo("a940478142ad9d776a342f512a59e53c3304b8cee4de991c3b09e0bf214f366d");
        if (tracker == null)
        {
            Console.WriteLine("Not find");
        }
        else
        {
            Console.WriteLine(tracker);
        }
    }

    [Test]
    public async Task GetTorrentTrackers()
    {
        var trackers = await _client.Torrent.GetTorrentInfo("0f9323316a62a4a6b66a5568893a52c3a342501d");
    }

    [Test]
    public async Task SetTorrentLocation()
    {
    }

    [Test]
    public async Task TestGetPieceStates()
    {
        var result =
            await _client.Torrent.GetTorrentPiecesStates("d866173fb508d762d177719bcac230a416e80220");
        foreach (var pState in result)
        {
            Console.WriteLine(pState.ToString());
        }
    }

    [Test]
    public async Task TestGetTorrentWebSeed()
    {
        var result =
            await _client.Torrent.GetTorrentWebSeeds("d866173fb508d762d177719bcac230a416e80220");
        foreach (var seed in result)
        {
            Console.WriteLine(seed.Url);
        }
    }

    [Test]
    public async Task TestResumeTorrent()
    {
        await _client.Torrent.ResumeTorrent("4f1043e4910b4d1a2a20e2c1674d0fed670a13f2");
    }

    [Test]
    public async Task TestGetTorrentDownloadLimit()
    {
        await _client.Torrent.GetTorrentDownloadLimit("d1fb4312d161b6d1aa39963d7a8eccd41a3599f3");
    }
}
