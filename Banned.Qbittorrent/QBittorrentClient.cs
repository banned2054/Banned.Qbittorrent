using Banned.Qbittorrent.Services;

namespace Banned.Qbittorrent;

/// <summary>
/// qBittorrent 客户端主入口，集成所有 Web API 服务模块。<br/>
/// The main entry point for the qBittorrent client, integrating all Web API service modules.
/// </summary>
public class QBittorrentClient : IDisposable
{
    /// <summary>
    /// 获取应用程序相关服务（版本、偏好设置、Cookie 等）。<br/>
    /// Gets services related to the application (version, preferences, cookies, etc.).
    /// </summary>
    public ApplicationService Application { get; }

    /// <summary>
    /// 获取身份验证服务（登录、登出、保活检查）。<br/>
    /// Gets the authentication service (login, logout, keep-alive checks).
    /// </summary>
    public AuthenticationService Authentication { get; }

    /// <summary>
    /// 获取日志相关服务。<br/>
    /// Gets services related to logs.
    /// </summary>
    public LogService Log { get; }

    /// <summary>
    /// 获取 RSS 订阅相关服务。<br/>
    /// Gets services related to RSS feeds.
    /// </summary>
    public RssService Rss { get; }

    /// <summary>
    /// 获取搜索相关服务。<br/>
    /// Gets services related to search.
    /// </summary>
    public SearchService Search { get; }

    /// <summary>
    /// 获取同步相关服务（主数据、种子状态增量更新）。<br/>
    /// Gets services related to synchronization (main data, torrent status incremental updates).
    /// </summary>
    public SyncService Sync { get; }

    /// <summary>
    /// 获取种子管理相关服务（添加、删除、暂停、标签等）。<br/>
    /// Gets services related to torrent management (add, delete, pause, tags, etc.).
    /// </summary>
    public TorrentService Torrent { get; }

    /// <summary>
    /// 获取传输状态相关服务（全局速度限制、总传输量）。<br/>
    /// Gets services related to transfer status (global speed limits, total transfer data).
    /// </summary>
    public TransferService Transfer { get; }

    private readonly NetService _network;

    /// <summary>
    /// 私有构造函数，通过静态工厂方法 <see cref="Create"/> 进行初始化。<br/>
    /// Private constructor, initialized via the static factory method <see cref="Create"/>.
    /// </summary>
    private QBittorrentClient(
        ApplicationService    app,
        AuthenticationService authentication,
        LogService            log,
        RssService            rss,
        SearchService         search,
        SyncService           sync,
        TorrentService        torrent,
        TransferService       transfer,
        NetService            net)
    {
        Application    = app;
        Authentication = authentication;
        Log            = log;
        Rss            = rss;
        Search         = search;
        Sync           = sync;
        Torrent        = torrent;
        Transfer       = transfer;
        _network       = net;
    }

    /// <summary>
    /// 创建并初始化一个新的 <see cref="QBittorrentClient"/> 实例。<br/>
    /// Creates and initializes a new <see cref="QBittorrentClient"/> instance.
    /// </summary>
    /// <param name="url">qBittorrent Web UI 地址（例如：http://localhost:8080）。 / qBittorrent Web UI URL.</param>
    /// <param name="userName">用户名。 / Username.</param>
    /// <param name="password">密码。 / Password.</param>
    /// <param name="httpClient">自定义的 HttpClient 实例，为 null 时使用默认实例。 / Custom HttpClient instance, uses default instance when null.</param>
    /// <param name="maxRetries">最大重试次数。 / Maximum number of retries.</param>
    /// <param name="timeout">请求超时时间，为 null 时默认 15 秒。 / Request timeout, default 15 seconds when null.</param>
    /// <param name="enableDetailedLogging">是否启用详细日志。 / Whether to enable detailed logging.</param>
    /// <returns>已完成 API 版本协商的客户端实例。 / A client instance with API version negotiation completed.</returns>
    /// <remarks>
    /// 此方法会自动调用 <c>GetApiVersion</c> 并将其配置到网络服务中，以确保后续请求的版本兼容性。<br/>
    /// This method automatically calls <c>GetApiVersion</c> and configures it in the network service to ensure version compatibility for subsequent requests.
    /// </remarks>
    public static async Task<QBittorrentClient> Create(string      url, string userName, string password,
                                                       HttpClient? httpClient = null, int? maxRetries = null,
                                                       TimeSpan?   timeout = null, bool? enableDetailedLogging = null)
    {
        var net = new NetService(url, httpClient, timeout);
        if (maxRetries.HasValue)
            net.MaxRetries = maxRetries.Value;
        if (enableDetailedLogging.HasValue)
            net.EnableDetailedLogging = enableDetailedLogging.Value;

        var application = new ApplicationService(net);
        var auth        = new AuthenticationService(net, userName, password);
        var apiVersion  = await application.GetApiVersion().ConfigureAwait(false);
        net.SetApiVersion(apiVersion);
        var log      = new LogService(net);
        var rss      = new RssService(net);
        var search   = new SearchService(net);
        var sync     = new SyncService(net);
        var torrent  = new TorrentService(net, apiVersion);
        var transfer = new TransferService(net);

        return new QBittorrentClient(application, auth, log, rss, search, sync, torrent, transfer, net);
    }

    /// <summary>
    /// 释放 <see cref="QBittorrentClient"/> 及其内部服务使用的资源。<br/>
    /// Releases the resources used by the <see cref="QBittorrentClient"/> and its internal services.
    /// </summary>
    public void Dispose()
    {
        Authentication.Dispose();
        _network.Dispose();
        GC.SuppressFinalize(this);
    }
}
