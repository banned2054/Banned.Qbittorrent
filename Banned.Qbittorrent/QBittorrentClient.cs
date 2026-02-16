using Banned.Qbittorrent.Services;

namespace Banned.Qbittorrent;

public class QBittorrentClient : IDisposable
{
    public AuthenticationService Authentication { get; }
    public ApplicationService    Application    { get; }
    public LogService            Log            { get; }
    public TorrentService        Torrent        { get; }

    private readonly NetService _network;

    private QBittorrentClient(
        ApplicationService    app,
        AuthenticationService authentication,
        LogService            log,
        TorrentService        torrent,
        NetService            net)
    {
        Application    = app;
        Authentication = authentication;
        Log            = log;
        Torrent        = torrent;
        _network       = net;
    }

    public static async Task<QBittorrentClient> Create(string url, string userName, string password)
    {
        var net         = new NetService(url);
        var auth        = new AuthenticationService(net, userName, password);
        var application = new ApplicationService(net);
        var apiVersion  = await application.GetApiVersion().ConfigureAwait(false);
        var log         = new LogService(net);
        net.SetApiVersion(apiVersion);
        var torrent = new TorrentService(net, apiVersion);
        return new QBittorrentClient(application, auth, log, torrent, net);
    }

    public void Dispose()
    {
        Authentication.Dispose();
        _network.Dispose();
    }
}
