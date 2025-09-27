using Banned.Qbittorrent.Services;
using Banned.Qbittorrent.Utils;

namespace Banned.Qbittorrent;

public class QBittorrentClient
{
    public ApplicationService Application;
    public TorrentService     Torrent;

    // 私有构造：只做字段赋值，不执行任何异步/网络操作
    private QBittorrentClient(ApplicationService app, TorrentService torrent)
    {
        Application = app;
        Torrent     = torrent;
    }

    // ✅ 推荐入口：异步工厂，避免在构造函数里 .Result
    public static async Task<QBittorrentClient> Create(string url, string userName, string password)
    {
        var netUtils    = new NetUtils(url, userName, password);
        var application = new ApplicationService(netUtils);
        var apiVersion  = await application.GetApiVersion().ConfigureAwait(false); // 不捕获同步上下文
        netUtils.SetApiVersion(apiVersion);
        var torrent = new TorrentService(netUtils, apiVersion);
        return new QBittorrentClient(application, torrent);
    }
}
