using Banned.Qbittorrent.Models.Enums;
using Banned.Qbittorrent.Serialization;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Banned.Qbittorrent.Models.Application;

/// <summary>
/// 表示 qBittorrent 的应用程序首选项/设置。<br/>
/// Represents qBittorrent application preferences/settings.
/// </summary>
public sealed class ApplicationPreferences
{
    #region Basic Settings(基础设置)

    /// <summary>界面语言区域设置。<br/>User interface locale.</summary>
    [JsonPropertyName("locale")]
    public string? Locale { get; set; }

    /// <summary>是否启用创建子文件夹。<br/>Whether to create subfolder.</summary>
    [JsonPropertyName("create_subfolder_enabled")]
    public bool? CreateSubfolderEnabled { get; set; }

    /// <summary>添加种子时是否默认为停止/暂停状态。<br/>Whether to start torrents in paused/stopped state by default.</summary>
    [JsonPropertyName("start_paused_enabled")]
    public bool? StartPausedEnabled { get; set; }

    /// <summary>添加种子文件后的处理方式。<br/>How torrent files are handled after being added.</summary>
    [JsonPropertyName("auto_delete_mode")]
    public EnumAutoDeleteMode? AutoDeleteMode { get; set; }

    /// <summary>预分配所有磁盘空间。<br/>Preallocate disk space for all files.</summary>
    [JsonPropertyName("preallocate_all")]
    public bool? PreallocateAll { get; set; }

    /// <summary>未完成文件使用扩展名 (!qB)。<br/>Append .!qB extension to incomplete files.</summary>
    [JsonPropertyName("incomplete_files_ext")]
    public bool? IncompleteFilesExt { get; set; }

    #endregion

    #region Torrent Management Mode (自动管理模式)

    /// <summary>是否默认启用自动种子管理。<br/>Whether automatic torrent management is enabled by default.</summary>
    [JsonPropertyName("auto_tmm_enabled")]
    public bool? AutoTmmEnabled { get; set; }

    /// <summary>种子分类改变时是否重新定位种子。<br/>Whether to relocate a torrent when its category changes.</summary>
    [JsonPropertyName("torrent_changed_tmm_enabled")]
    public bool? TorrentChangedTmmEnabled { get; set; }

    /// <summary>默认保存路径改变时是否重新定位种子。<br/>Whether to relocate torrents when the default save path changes.</summary>
    [JsonPropertyName("save_path_changed_tmm_enabled")]
    public bool? SavePathChangedTmmEnabled { get; set; }

    /// <summary>分类保存路径改变时是否重新定位种子。<br/>Whether to relocate torrents when a category save path changes.</summary>
    [JsonPropertyName("category_changed_tmm_enabled")]
    public bool? CategoryChangedTmmEnabled { get; set; }

    /// <summary>默认种子保存路径。<br/>Default torrent save path.</summary>
    [JsonPropertyName("save_path")]
    public string? SavePath { get; set; }

    /// <summary>是否为未完成种子使用临时路径。<br/>Whether to use a temporary path for incomplete torrents.</summary>
    [JsonPropertyName("temp_path_enabled")]
    public bool? TempPathEnabled { get; set; }

    /// <summary>未完成种子的临时路径。<br/>Temporary path for incomplete torrents.</summary>
    [JsonPropertyName("temp_path")]
    public string? TempPath { get; set; }

    #endregion

    #region Scan Dirs & Export (监控与导出)

    /// <summary>自动扫描目录及其下载目标。<br/>Directories to scan automatically and their download destinations.</summary>
    [JsonPropertyName("scan_dirs")]
    public Dictionary<string, ScanDirDestination>? ScanDirs { get; set; }

    /// <summary>复制已添加种子文件的目录。<br/>Directory to which added torrent files are copied.</summary>
    [JsonPropertyName("export_dir")]
    public string? ExportDir { get; set; }

    /// <summary>复制已完成种子文件的目录。<br/>Directory to which completed torrent files are copied.</summary>
    [JsonPropertyName("export_dir_fin")]
    public string? ExportDirFinished { get; set; }

    #endregion

    #region Mail Notification (邮件通知)

    /// <summary>是否启用邮件通知。<br/>Whether email notifications are enabled.</summary>
    [JsonPropertyName("mail_notification_enabled")]
    public bool? MailNotificationEnabled { get; set; }

    /// <summary>邮件通知的发件人地址。<br/>Sender address for email notifications.</summary>
    [JsonPropertyName("mail_notification_sender")]
    public string? MailNotificationSender { get; set; }

    /// <summary>邮件通知的收件人地址。<br/>Recipient address for email notifications.</summary>
    [JsonPropertyName("mail_notification_email")]
    public string? MailNotificationEmail { get; set; }

    /// <summary>邮件通知使用的 SMTP 服务器。<br/>SMTP server used for email notifications.</summary>
    [JsonPropertyName("mail_notification_smtp")]
    public string? MailNotificationSmtp { get; set; }

    /// <summary>SMTP 连接是否使用 SSL。<br/>Whether the SMTP connection uses SSL.</summary>
    [JsonPropertyName("mail_notification_ssl_enabled")]
    public bool? MailNotificationSslEnabled { get; set; }

    /// <summary>SMTP 服务器是否需要身份验证。<br/>Whether the SMTP server requires authentication.</summary>
    [JsonPropertyName("mail_notification_auth_enabled")]
    public bool? MailNotificationAuthEnabled { get; set; }

    /// <summary>SMTP 身份验证用户名。<br/>Username for SMTP authentication.</summary>
    [JsonPropertyName("mail_notification_username")]
    public string? MailNotificationUserName { get; set; }

    /// <summary>SMTP 身份验证密码。<br/>Password for SMTP authentication.</summary>
    [JsonPropertyName("mail_notification_password")]
    public string? MailNotificationPassword { get; set; }

    #endregion

    #region Queueing & Limits (队列与限制)

    /// <summary>种子完成时是否运行外部程序。<br/>Whether to run an external program when a torrent finishes.</summary>
    [JsonPropertyName("autorun_enabled")]
    public bool? AutoRunEnabled { get; set; }

    /// <summary>种子完成时运行的命令。<br/>Command to run when a torrent finishes.</summary>
    [JsonPropertyName("autorun_program")]
    public string? AutoRunCommand { get; set; }

    /// <summary>是否启用种子队列。<br/>Whether torrent queueing is enabled.</summary>
    [JsonPropertyName("queueing_enabled")]
    public bool? EnableQueueing { get; set; }

    /// <summary>同时活动的最大下载数。<br/>Maximum number of active downloads.</summary>
    [JsonPropertyName("max_active_downloads")]
    public int? MaxActiveDownloads { get; set; }

    /// <summary>同时活动的最大种子数。<br/>Maximum number of active torrents.</summary>
    [JsonPropertyName("max_active_torrents")]
    public int? MaxActiveTorrents { get; set; }

    /// <summary>同时活动的最大上传数。<br/>Maximum number of active uploads.</summary>
    [JsonPropertyName("max_active_uploads")]
    public int? MaxActiveUploads { get; set; }

    /// <summary>是否从队列限制中排除慢速种子。<br/>Whether slow torrents are excluded from queue limits.</summary>
    [JsonPropertyName("dont_count_slow_torrents")]
    public bool? IgnoreSlowTorrents { get; set; }

    /// <summary>慢速下载速度阈值 (KiB/s)。<br/>Slow download rate threshold in KiB/s.</summary>
    [JsonPropertyName("slow_torrent_dl_rate_threshold")]
    public int? SlowDownloadRateKiB { get; set; }

    /// <summary>慢速上传速度阈值 (KiB/s)。<br/>Slow upload rate threshold in KiB/s.</summary>
    [JsonPropertyName("slow_torrent_ul_rate_threshold")]
    public int? SlowUploadRateKiB { get; set; }

    /// <summary>判定种子非活动的时间 (秒)。<br/>Seconds before a torrent is considered inactive.</summary>
    [JsonPropertyName("slow_torrent_inactive_timer")]
    public int? SlowInactiveSeconds { get; set; }

    #endregion

    #region Share Limits (分享限制)

    /// <summary>是否启用全局分享率限制。<br/>Whether the global share ratio limit is enabled.</summary>
    [JsonPropertyName("max_ratio_enabled")]
    public bool? MaxRatioEnabled { get; set; }

    /// <summary>全局分享率限制。<br/>Global share ratio limit.</summary>
    [JsonPropertyName("max_ratio")]
    public float? MaxRatio { get; set; }

    /// <summary>达到分享限制后执行的操作。<br/>Action to take when a share limit is reached.</summary>
    [JsonPropertyName("max_ratio_act")]
    public EnumMaxRatioAction? MaxRatioAction { get; set; }

    #endregion

    #region Network & Connection (网络与连接)

    /// <summary>传入连接的监听端口。<br/>Listening port for incoming connections.</summary>
    [JsonPropertyName("listen_port")]
    public int? ListenPort { get; set; }

    /// <summary>是否使用 UPnP/NAT-PMP 转发监听端口。<br/>Whether to use UPnP/NAT-PMP to forward the listening port.</summary>
    [JsonPropertyName("upnp")]
    public bool? UpnpNatPmpEnabled { get; set; }

    /// <summary>启动时是否随机选择监听端口。<br/>Whether to choose a random listening port on startup.</summary>
    [JsonPropertyName("random_port")]
    public bool? RandomPortEnabled { get; set; }

    /// <summary>全局下载限制 (KiB/s)。<br/>Global download limit in KiB/s.</summary>
    [JsonPropertyName("dl_limit")]
    public int? GlobalDlLimitKiB { get; set; }

    /// <summary>全局上传限制 (KiB/s)。<br/>Global upload limit in KiB/s.</summary>
    [JsonPropertyName("up_limit")]
    public int? GlobalUpLimitKiB { get; set; }

    /// <summary>全局最大连接数。<br/>Global maximum number of connections.</summary>
    [JsonPropertyName("max_connec")]
    public int? MaxConnections { get; set; }

    /// <summary>每个种子的最大连接数。<br/>Maximum number of connections per torrent.</summary>
    [JsonPropertyName("max_connec_per_torrent")]
    public int? MaxConnectionsPerTorrent { get; set; }

    /// <summary>全局最大上传槽位数。<br/>Global maximum number of upload slots.</summary>
    [JsonPropertyName("max_uploads")]
    public int? MaxUploads { get; set; }

    /// <summary>每个种子的最大上传槽位数。<br/>Maximum number of upload slots per torrent.</summary>
    [JsonPropertyName("max_uploads_per_torrent")]
    public int? MaxUploadsPerTorrent { get; set; }

    /// <summary>停止 Tracker 请求的超时时间（秒）。<br/>Timeout for stopping tracker requests, in seconds.</summary>
    [JsonPropertyName("stop_tracker_timeout")]
    public int? StopTrackerTimeoutSeconds { get; set; }

    /// <summary>是否启用分片范围亲和性。<br/>Whether piece extent affinity is enabled.</summary>
    [JsonPropertyName("enable_piece_extent_affinity")]
    public bool? PieceExtentAffinityEnabled { get; set; }

    /// <summary>启用的 BitTorrent 传输协议。<br/>Enabled BitTorrent transport protocol.</summary>
    [JsonPropertyName("bittorrent_protocol")]
    public EnumBittorrentProtocol? BittorrentProtocol { get; set; }

    /// <summary>是否将速度限制应用于 µTP 连接。<br/>Whether rate limits apply to µTP connections.</summary>
    [JsonPropertyName("limit_utp_rate")]
    public bool? LimitUtpRate { get; set; }

    /// <summary>是否将 TCP 开销计入速度限制。<br/>Whether TCP overhead is included in rate limits.</summary>
    [JsonPropertyName("limit_tcp_overhead")]
    public bool? LimitTcpOverhead { get; set; }

    /// <summary>是否将速度限制应用于局域网节点。<br/>Whether rate limits apply to LAN peers.</summary>
    [JsonPropertyName("limit_lan_peers")]
    public bool? LimitLanPeers { get; set; }

    #endregion

    #region Scheduler (速度调度器)

    /// <summary>备用下载速度限制（KiB/s）。<br/>Alternative download rate limit in KiB/s.</summary>
    [JsonPropertyName("alt_dl_limit")]
    public int? AlternativeDownloadLimitKiB { get; set; }

    /// <summary>备用上传速度限制（KiB/s）。<br/>Alternative upload rate limit in KiB/s.</summary>
    [JsonPropertyName("alt_up_limit")]
    public int? AlternativeUploadLimitKiB { get; set; }

    /// <summary>是否启用备用速度限制调度器。<br/>Whether the alternative rate limit scheduler is enabled.</summary>
    [JsonPropertyName("scheduler_enabled")]
    public bool? SchedulerEnabled { get; set; }

    /// <summary>调度器开始时间的小时部分。<br/>Hour at which the scheduler starts.</summary>
    [JsonPropertyName("scheduler_from_hour")]
    public int? SchedulerStartingHour { get; set; }

    /// <summary>调度器开始时间的分钟部分。<br/>Minute at which the scheduler starts.</summary>
    [JsonPropertyName("scheduler_from_min")]
    public int? SchedulerStartingMinute { get; set; }

    /// <summary>调度器结束时间的小时部分。<br/>Hour at which the scheduler ends.</summary>
    [JsonPropertyName("scheduler_to_hour")]
    public int? SchedulerEndingHour { get; set; }

    /// <summary>调度器结束时间的分钟部分。<br/>Minute at which the scheduler ends.</summary>
    [JsonPropertyName("scheduler_to_min")]
    public int? SchedulerEndingMinute { get; set; }

    /// <summary>应用调度器的日期范围。<br/>Days on which the scheduler applies.</summary>
    [JsonPropertyName("scheduler_days")]
    public EnumSchedulerDay? SchedulerDays { get; set; }

    #endregion

    #region Proxy & Privacy (代理与隐私)

    /// <summary>是否启用分布式哈希表（DHT）。<br/>Whether the distributed hash table (DHT) is enabled.</summary>
    [JsonPropertyName("dht")]
    public bool? DistributedHashTableEnabled { get; set; }

    /// <summary>是否启用节点交换（PeX）。<br/>Whether peer exchange (PeX) is enabled.</summary>
    [JsonPropertyName("pex")]
    public bool? PeerExchangeEnable { get; set; }

    /// <summary>是否启用本地节点发现（LSD）。<br/>Whether local peer discovery (LSD) is enabled.</summary>
    [JsonPropertyName("lsd")]
    public bool? LocalServiceDiscoveryEnabled { get; set; }

    /// <summary>BitTorrent 连接加密模式。<br/>Encryption mode for BitTorrent connections.</summary>
    [JsonPropertyName("encryption")]
    public EnumEncryptionMode? Encryption { get; set; }

    /// <summary>是否启用匿名模式。<br/>Whether anonymous mode is enabled.</summary>
    [JsonPropertyName("anonymous_mode")]
    public bool? AnonymousMode { get; set; }

    /// <summary>代理服务器类型。<br/>Proxy server type.</summary>
    [JsonPropertyName("proxy_type")]
    [JsonConverter(typeof(ProxyTypeConverter))]
    public EnumProxyType? ProxyType { get; set; }

    /// <summary>代理服务器的主机名或 IP 地址。<br/>Host name or IP address of the proxy server.</summary>
    [JsonPropertyName("proxy_ip")]
    public string? ProxyIp { get; set; }

    /// <summary>代理服务器端口。<br/>Proxy server port.</summary>
    [JsonPropertyName("proxy_port")]
    public int? ProxyPort { get; set; }

    /// <summary>节点连接是否使用代理。<br/>Whether peer connections use the proxy.</summary>
    [JsonPropertyName("proxy_peer_connections")]
    public bool? ProxyPeerConnections { get; set; }

    /// <summary>代理服务器是否需要身份验证。<br/>Whether the proxy server requires authentication.</summary>
    [JsonPropertyName("proxy_auth_enabled")]
    public bool? ProxyAuthEnabled { get; set; }

    /// <summary>代理身份验证用户名。<br/>Username for proxy authentication.</summary>
    [JsonPropertyName("proxy_username")]
    public string? ProxyUsername { get; set; }

    /// <summary>代理身份验证密码。<br/>Password for proxy authentication.</summary>
    [JsonPropertyName("proxy_password")]
    public string? ProxyPassword { get; set; }

    /// <summary>是否仅代理 BitTorrent 流量。<br/>Whether only BitTorrent traffic uses the proxy.</summary>
    [JsonPropertyName("proxy_torrents_only")]
    public bool? ProxyTorrentsOnly { get; set; }

    #endregion

    #region IP Filtering (IP 过滤)

    /// <summary>是否启用 IP 过滤。<br/>Whether IP filtering is enabled.</summary>
    [JsonPropertyName("ip_filter_enabled")]
    public bool? IpFilterEnabled { get; set; }

    /// <summary>IP 过滤规则文件路径。<br/>Path to the IP filter rules file.</summary>
    [JsonPropertyName("ip_filter_path")]
    public string? IpFilterPath { get; set; }

    /// <summary>是否将 IP 过滤应用于 Tracker。<br/>Whether IP filtering applies to trackers.</summary>
    [JsonPropertyName("ip_filter_trackers")]
    public bool? IpFilterTrackersEnabled { get; set; }

    #endregion

    #region Web UI Settings (Web 界面设置)

    /// <summary>允许访问 Web UI 的域名列表。<br/>List of domains allowed to access the Web UI.</summary>
    [JsonPropertyName("web_ui_domain_list")]
    public string? WebUiDomains { get; set; }

    /// <summary>Web UI 监听地址。<br/>Address on which the Web UI listens.</summary>
    [JsonPropertyName("web_ui_address")]
    public string? WebUiAddress { get; set; }

    /// <summary>Web UI 监听端口。<br/>Port on which the Web UI listens.</summary>
    [JsonPropertyName("web_ui_port")]
    public int? WebUiPort { get; set; }

    /// <summary>是否使用 UPnP/NAT-PMP 转发 Web UI 端口。<br/>Whether to use UPnP/NAT-PMP to forward the Web UI port.</summary>
    [JsonPropertyName("web_ui_upnp")]
    public bool? WebUiUpnp { get; set; }

    /// <summary>Web UI 身份验证用户名。<br/>Username for Web UI authentication.</summary>
    [JsonPropertyName("web_ui_username")]
    public string? WebUiUsername { get; set; }

    /// <summary>Web UI 身份验证密码。<br/>Password for Web UI authentication.</summary>
    [JsonPropertyName("web_ui_password")]
    public string? WebUiPassword { get; set; }

    /// <summary>是否启用 Web UI CSRF 防护。<br/>Whether Web UI CSRF protection is enabled.</summary>
    [JsonPropertyName("web_ui_csrf_protection_enabled")]
    public bool? WebUiCsrfProtectionEnabled { get; set; }

    /// <summary>是否启用 Web UI 点击劫持防护。<br/>Whether Web UI clickjacking protection is enabled.</summary>
    [JsonPropertyName("web_ui_clickjacking_protection_enabled")]
    public bool? WebUiClickjackingProtectionEnabled { get; set; }

    /// <summary>是否为 Web UI 会话 Cookie 启用 Secure 属性。<br/>Whether the Secure attribute is enabled for Web UI session cookies.</summary>
    [JsonPropertyName("web_ui_secure_cookie_enabled")]
    public bool? WebUiSecureCookieEnabled { get; set; }

    /// <summary>封禁客户端前允许的最大身份验证失败次数。<br/>Maximum authentication failures allowed before a client is banned.</summary>
    [JsonPropertyName("web_ui_max_auth_fail_count")]
    public int? WebUiMaxAuthFailCount { get; set; }

    /// <summary>Web UI 客户端封禁时长（秒）。<br/>Web UI client ban duration in seconds.</summary>
    [JsonPropertyName("web_ui_ban_duration")]
    public int? WebUiBanDurationSeconds { get; set; }

    /// <summary>Web UI 会话超时时间（秒）。<br/>Web UI session timeout in seconds.</summary>
    [JsonPropertyName("web_ui_session_timeout")]
    public int? WebUiSessionTimeoutSeconds { get; set; }

    /// <summary>是否启用 Web UI Host 请求头验证。<br/>Whether Web UI Host header validation is enabled.</summary>
    [JsonPropertyName("web_ui_host_header_validation_enabled")]
    public bool? WebUiHostHeaderValidationEnabled { get; set; }

    /// <summary>是否允许本地主机绕过 Web UI 身份验证。<br/>Whether localhost may bypass Web UI authentication.</summary>
    [JsonPropertyName("bypass_local_auth")]
    public bool? BypassLocalAuth { get; set; }

    /// <summary>是否允许白名单子网绕过 Web UI 身份验证。<br/>Whether whitelisted subnets may bypass Web UI authentication.</summary>
    [JsonPropertyName("bypass_auth_subnet_whitelist_enabled")]
    public bool? BypassAuthSubnetWhitelistEnabled { get; set; }

    /// <summary>可绕过 Web UI 身份验证的子网白名单。<br/>Subnet whitelist allowed to bypass Web UI authentication.</summary>
    [JsonPropertyName("bypass_auth_subnet_whitelist")]
    public string? BypassAuthSubnetWhitelist { get; set; }

    /// <summary>是否启用备用 Web UI。<br/>Whether the alternative Web UI is enabled.</summary>
    [JsonPropertyName("alternative_webui_enabled")]
    public bool? AlternativeWebUiEnabled { get; set; }

    /// <summary>备用 Web UI 文件路径。<br/>Path to the alternative Web UI files.</summary>
    [JsonPropertyName("alternative_webui_path")]
    public string? AlternativeWebUiPath { get; set; }

    /// <summary>Web UI 是否使用 HTTPS。<br/>Whether the Web UI uses HTTPS.</summary>
    [JsonPropertyName("use_https")]
    public bool? WebUiHttpsEnabled { get; set; }

    /// <summary>Web UI HTTPS 私钥内容。<br/>Private key content used by Web UI HTTPS.</summary>
    [JsonPropertyName("ssl_key")]
    public string? WebUiSslKey { get; set; }

    /// <summary>Web UI HTTPS 证书内容。<br/>Certificate content used by Web UI HTTPS.</summary>
    [JsonPropertyName("ssl_cert")]
    public string? WebUiSslCert { get; set; }

    /// <summary>Web UI HTTPS 私钥文件路径。<br/>Path to the private key file used by Web UI HTTPS.</summary>
    [JsonPropertyName("web_ui_https_key_path")]
    public string? WebUiHttpsKeyPath { get; set; }

    /// <summary>Web UI HTTPS 证书文件路径。<br/>Path to the certificate file used by Web UI HTTPS.</summary>
    [JsonPropertyName("web_ui_https_cert_path")]
    public string? WebUiHttpsCertPath { get; set; }

    /// <summary>是否启用动态 DNS。<br/>Whether dynamic DNS is enabled.</summary>
    [JsonPropertyName("dyndns_enabled")]
    public bool? DyndnsEnabled { get; set; }

    /// <summary>动态 DNS 服务提供商标识。<br/>Dynamic DNS service provider identifier.</summary>
    [JsonPropertyName("dyndns_service")]
    public int? DynamicalDnsService { get; set; }

    /// <summary>动态 DNS 服务用户名。<br/>Username for the dynamic DNS service.</summary>
    [JsonPropertyName("dyndns_username")]
    public string? DynamicalDnsUsername { get; set; }

    /// <summary>动态 DNS 服务密码。<br/>Password for the dynamic DNS service.</summary>
    [JsonPropertyName("dyndns_password")]
    public string? DynamicalDnsPassword { get; set; }

    /// <summary>动态 DNS 域名。<br/>Domain name managed by the dynamic DNS service.</summary>
    [JsonPropertyName("dyndns_domain")]
    public string? DynamicalDnsDomain { get; set; }

    #endregion

    #region RSS & Trackers (订阅与 Tracker)

    /// <summary>RSS 源刷新间隔。<br/>Interval between RSS feed refreshes.</summary>
    [JsonPropertyName("rss_refresh_interval")]
    [JsonConverter(typeof(MinutesTimeSpanConverter))]
    public TimeSpan? RssRefreshInterval { get; set; }

    /// <summary>每个 RSS 源保留的最大文章数。<br/>Maximum number of articles retained per RSS feed.</summary>
    [JsonPropertyName("rss_max_articles_per_feed")]
    public int? RssMaxArticlesPerFeed { get; set; }

    /// <summary>是否启用 RSS 处理。<br/>Whether RSS processing is enabled.</summary>
    [JsonPropertyName("rss_processing_enabled")]
    public bool? RssProcessingEnabled { get; set; }

    /// <summary>是否启用 RSS 自动下载。<br/>Whether automatic RSS downloading is enabled.</summary>
    [JsonPropertyName("rss_auto_downloading_enabled")]
    public bool? RssAutoDownloadingEnabled { get; set; }

    /// <summary>是否下载 Repack 或 Proper 版本的剧集。<br/>Whether to download Repack or Proper releases of episodes.</summary>
    [JsonPropertyName("rss_download_repack_proper_episodes")]
    public bool? RssDownloadRepackProperEpisodes { get; set; }

    /// <summary>RSS 智能剧集筛选规则。<br/>Smart episode filter rules for RSS.</summary>
    [JsonPropertyName("rss_smart_episode_filters")]
    public string? RssSmartEpisodeFilters { get; set; }

    /// <summary>是否自动向新种子添加 Tracker。<br/>Whether trackers are added to new torrents automatically.</summary>
    [JsonPropertyName("add_trackers_enabled")]
    public bool? AddTrackersEnabled { get; set; }

    /// <summary>自动添加到新种子的 Tracker 列表。<br/>List of trackers added to new torrents automatically.</summary>
    [JsonPropertyName("add_trackers")]
    public string? AddTrackers { get; set; }

    /// <summary>是否为 Web UI 使用自定义 HTTP 请求头。<br/>Whether the Web UI uses custom HTTP headers.</summary>
    [JsonPropertyName("web_ui_use_custom_http_headers_enabled")]
    public bool? WebUiUseCustomHttpHeadersEnabled { get; set; }

    /// <summary>Web UI 使用的自定义 HTTP 请求头。<br/>Custom HTTP headers used by the Web UI.</summary>
    [JsonPropertyName("web_ui_custom_http_headers")]
    public string? WebUiCustomHttpHeaders { get; set; }

    /// <summary>是否启用最长做种时间限制。<br/>Whether the maximum seeding time limit is enabled.</summary>
    [JsonPropertyName("max_seeding_time_enabled")]
    public bool? MaxSeedingTimeEnabled { get; set; }

    /// <summary>最长做种时间（分钟）。<br/>Maximum seeding time in minutes.</summary>
    [JsonPropertyName("max_seeding_time")]
    public int? MaxSeedingTimeMinutes { get; set; }

    /// <summary>向 Tracker 通告的 IP 地址。<br/>IP address announced to trackers.</summary>
    [JsonPropertyName("announce_ip")]
    public string? AnnounceIp { get; set; }

    /// <summary>是否向所有 Tracker 层级通告。<br/>Whether to announce to all tracker tiers.</summary>
    [JsonPropertyName("announce_to_all_tiers")]
    public bool? AnnounceToAllTiers { get; set; }

    /// <summary>是否向同一层级的所有 Tracker 通告。<br/>Whether to announce to all trackers in the same tier.</summary>
    [JsonPropertyName("announce_to_all_trackers")]
    public bool? AnnounceToAllTrackers { get; set; }

    #endregion

    #region Advanced & Libtorrent (高级设置)

    /// <summary>异步 I/O 线程数。<br/>Number of asynchronous I/O threads.</summary>
    [JsonPropertyName("async_io_threads")]
    public int? AsyncIoThreads { get; set; }

    /// <summary>被封禁的 IP 地址列表。<br/>List of banned IP addresses.</summary>
    [JsonPropertyName("banned_IPs")]
    public string? BannedIPs { get; set; }

    /// <summary>校验内存限制 (MiB)。<br/>Memory usage for checking in MiB.</summary>
    [JsonPropertyName("checking_memory_use")]
    public int? CheckingMemoryUseMiB { get; set; }

    /// <summary>当前绑定的网络接口地址。<br/>Address of the currently bound network interface.</summary>
    [JsonPropertyName("current_interface_address")]
    public string? CurrentInterfaceAddress { get; set; }

    /// <summary>当前绑定的网络接口名称。<br/>Name of the currently bound network interface.</summary>
    [JsonPropertyName("current_network_interface")]
    public string? CurrentNetworkInterface { get; set; }

    /// <summary>磁盘缓存大小 (MiB)。<br/>Disk cache size in MiB.</summary>
    [JsonPropertyName("disk_cache")]
    public int? DiskCacheMiB { get; set; }

    /// <summary>磁盘缓存到期时间。<br/>Disk cache expiry time.</summary>
    [JsonPropertyName("disk_cache_ttl")]
    [JsonConverter(typeof(SecondsTimeSpanConverter))]
    public TimeSpan? DiskCacheTtlSeconds { get; set; }

    /// <summary>内置 Tracker 的监听端口。<br/>Listening port of the embedded tracker.</summary>
    [JsonPropertyName("embedded_tracker_port")]
    public int? EmbeddedTrackerPort { get; set; }

    /// <summary>是否合并磁盘读写操作。<br/>Whether disk read and write operations are coalesced.</summary>
    [JsonPropertyName("enable_coalesce_read_write")]
    public bool? CoalesceReadWriteEnabled { get; set; }

    /// <summary>是否启用内置 Tracker。<br/>Whether the embedded tracker is enabled.</summary>
    [JsonPropertyName("enable_embedded_tracker")]
    public bool? EmbeddedTrackerEnabled { get; set; }

    /// <summary>是否允许来自同一 IP 的多个连接。<br/>Whether multiple connections from the same IP are allowed.</summary>
    [JsonPropertyName("enable_multi_connections_from_same_ip")]
    public bool? MultiConnectionsFromSameIpEnabled { get; set; }

    /// <summary>是否启用操作系统磁盘缓存。<br/>Whether the operating system disk cache is enabled.</summary>
    [JsonPropertyName("enable_os_cache")]
    public bool? OsCacheEnabled { get; set; }

    /// <summary>是否启用上传分片建议。<br/>Whether upload piece suggestions are enabled.</summary>
    [JsonPropertyName("enable_upload_suggestions")]
    public bool? UploadSuggestionsEnabled { get; set; }

    /// <summary>保持打开状态的文件池大小。<br/>Size of the pool of files kept open.</summary>
    [JsonPropertyName("file_pool_size")]
    public int? FilePoolSize { get; set; }

    /// <summary>传出连接端口范围的最大值。<br/>Maximum port in the outgoing connection port range.</summary>
    [JsonPropertyName("outgoing_ports_max")]
    public int? OutgoingPortsMax { get; set; }

    /// <summary>传出连接端口范围的最小值。<br/>Minimum port in the outgoing connection port range.</summary>
    [JsonPropertyName("outgoing_ports_min")]
    public int? OutgoingPortsMin { get; set; }

    /// <summary>是否重新校验已完成的种子。<br/>Whether completed torrents are rechecked.</summary>
    [JsonPropertyName("recheck_completed_torrents")]
    public bool? RecheckCompletedTorrents { get; set; }

    /// <summary>是否解析节点所在国家或地区。<br/>Whether peer countries or regions are resolved.</summary>
    [JsonPropertyName("resolve_peer_countries")]
    public bool? ResolvePeerCountries { get; set; }

    /// <summary>保存快速恢复数据的间隔（分钟）。<br/>Interval for saving resume data, in minutes.</summary>
    [JsonPropertyName("save_resume_data_interval")]
    public int? SaveResumeDataIntervalMinutes { get; set; }

    /// <summary>发送缓冲区低水位线（KiB）。<br/>Send buffer low watermark in KiB.</summary>
    [JsonPropertyName("send_buffer_low_watermark")]
    public int? SendBufferLowWatermarkKiB { get; set; }

    /// <summary>发送缓冲区水位线（KiB）。<br/>Send buffer watermark in KiB.</summary>
    [JsonPropertyName("send_buffer_watermark")]
    public int? SendBufferWatermarkKiB { get; set; }

    /// <summary>发送缓冲区水位线系数（百分比）。<br/>Send buffer watermark factor as a percentage.</summary>
    [JsonPropertyName("send_buffer_watermark_factor")]
    public int? SendBufferWatermarkFactorPercent { get; set; }

    /// <summary>套接字监听积压队列大小。<br/>Socket listen backlog size.</summary>
    [JsonPropertyName("socket_backlog_size")]
    public int? SocketBacklogSize { get; set; }

    /// <summary>上传阻塞算法。<br/>Upload choking algorithm.</summary>
    [JsonPropertyName("upload_choking_algorithm")]
    public EnumUploadChokingAlgorithm? UploadChokingAlgorithm { get; set; }

    /// <summary>上传槽位分配行为。<br/>Upload slot allocation behavior.</summary>
    [JsonPropertyName("upload_slots_behavior")]
    public EnumUploadSlotsBehavior? UploadSlotsBehavior { get; set; }

    /// <summary>UPnP 端口映射租约时长。<br/>Lease duration for UPnP port mappings.</summary>
    [JsonPropertyName("upnp_lease_duration")]
    public int? UpnpLeaseDuration { get; set; }

    /// <summary>µTP/TCP 混合模式策略。<br/>Policy for mixed µTP/TCP connections.</summary>
    [JsonPropertyName("utp_tcp_mixed_mode")]
    public EnumUtpTcpMixedMode? UtpTcpMixedMode { get; set; }

    #endregion

    /// <summary>
    /// 包含任何未明确映射到属性的额外 JSON 数据。<br/>
    /// Contains any additional JSON data not explicitly mapped to properties.
    /// </summary>
    [JsonExtensionData]
    public Dictionary<string, JsonElement>? ExtensionData { get; set; }
}
