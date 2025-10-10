using Banned.Qbittorrent.Services;
using Banned.Qbittorrent.Utils;

namespace Banned.Qbittorrent;

public class QBittorrentClient
{
    public ApplicationService Application;
    public TorrentService     Torrent;

    private QBittorrentClient(ApplicationService app, TorrentService torrent)
    {
        Application = app;
        Torrent     = torrent;
    }

    public static async Task<QBittorrentClient> Create(string url, string userName, string password)
    {
        var netUtils    = new NetUtils(url, userName, password);
        var application = new ApplicationService(netUtils);
        var apiVersion  = await application.GetApiVersion().ConfigureAwait(false);
        netUtils.SetApiVersion(apiVersion);
        var torrent = new TorrentService(netUtils, apiVersion);
        return new QBittorrentClient(application, torrent);
    }
}
