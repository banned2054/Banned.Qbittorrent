using Banned.Qbittorrent.Models.Enums;
using Banned.Qbittorrent.Utils;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Banned.Qbittorrent.Models.Application;

public sealed class ApplicationPreferences
{
    [JsonPropertyName("locale")]
    public string? Locale { get; set; }

    [JsonPropertyName("create_subfolder_enabled")]
    public bool? CreateSubfolderEnabled { get; set; }

    [JsonPropertyName("start_paused_enabled")]
    public bool? StartPausedEnabled { get; set; }

    [JsonPropertyName("auto_delete_mode")]
    public EnumAutoDeleteMode? AutoDeleteMode { get; set; }

    [JsonPropertyName("preallocate_all")]
    public bool? PreallocateAll { get; set; }

    [JsonPropertyName("incomplete_files_ext")]
    public bool? IncompleteFilesExt { get; set; }

    [JsonPropertyName("auto_tmm_enabled")]
    public bool? AutoTmmEnabled { get; set; }

    [JsonPropertyName("torrent_changed_tmm_enabled")]
    public bool? TorrentChangedTmmEnabled { get; set; }

    [JsonPropertyName("save_path_changed_tmm_enabled")]
    public bool? SavePathChangedTmmEnabled { get; set; }

    [JsonPropertyName("category_changed_tmm_enabled")]
    public bool? CategoryChangedTmmEnabled { get; set; }

    [JsonPropertyName("save_path")]
    public string? SavePath { get; set; }

    [JsonPropertyName("temp_path_enabled")]
    public bool? TempPathEnabled { get; set; }

    [JsonPropertyName("temp_path")]
    public string? TempPath { get; set; }

    [JsonPropertyName("scan_dirs")]
    public Dictionary<string, ScanDirDestination>? ScanDirs { get; set; }

    [JsonPropertyName("export_dir")]
    public string? ExportDir { get; set; }

    [JsonPropertyName("export_dir_fin")]
    public string? ExportDirFinished { get; set; }

    [JsonPropertyName("mail_notification_enabled")]
    public bool? MailNotificationEnabled { get; set; }

    [JsonPropertyName("mail_notification_sender")]
    public string? MailNotificationSender { get; set; }

    [JsonPropertyName("mail_notification_email")]
    public string? MailNotificationEmail { get; set; }

    [JsonPropertyName("mail_notification_smtp")]
    public string? MailNotificationSmtp { get; set; }

    [JsonPropertyName("mail_notification_ssl_enabled")]
    public bool? MailNotificationSslEnabled { get; set; }

    [JsonPropertyName("mail_notification_auth_enabled")]
    public bool? MailNotificationAuthEnabled { get; set; }

    [JsonPropertyName("mail_notification_username")]
    public string? MailNotificationUserName { get; set; }

    [JsonPropertyName("mail_notification_password")]
    public string? MailNotificationPassword { get; set; }

    [JsonPropertyName("autorun_enabled")]
    public bool? AutoRunEnabled { get; set; }

    [JsonPropertyName("autorun_program")]
    public string? AutoRunCommand { get; set; }

    [JsonPropertyName("queueing_enabled")]
    public bool? QueueingEnabled { get; set; }

    [JsonPropertyName("max_active_downloads")]
    public int? MaxActiveDownloads { get; set; }

    [JsonPropertyName("max_active_torrents")]
    public int? MaxActiveTorrents { get; set; }

    [JsonPropertyName("max_active_uploads")]
    public int? MaxActiveUploads { get; set; }

    [JsonPropertyName("dont_count_slow_torrents")]
    public bool? IgnoreSlowTorrents { get; set; }

    [JsonPropertyName("slow_torrent_dl_rate_threshold")]
    public int? SlowDownloadRateKiB { get; set; }

    [JsonPropertyName("slow_torrent_ul_rate_threshold")]
    public int? SlowUploadRateKiB { get; set; }

    [JsonPropertyName("slow_torrent_inactive_timer")]
    public int? SlowInactiveSeconds { get; set; }

    [JsonPropertyName("max_ratio_enabled")]
    public bool? MaxRatioEnabled { get; set; }

    [JsonPropertyName("max_ratio")]
    public float? MaxRatio { get; set; }

    [JsonPropertyName("max_ratio_act")]
    public EnumMaxRatioAction? MaxRatioAction { get; set; }

    [JsonPropertyName("listen_port")]
    public int? ListenPort { get; set; }

    [JsonPropertyName("upnp")]
    public bool? UpnpNatPmpEnabled { get; set; }

    [JsonPropertyName("random_port")]
    public bool? RandomPortEnabled { get; set; }

    [JsonPropertyName("dl_limit")]
    public int? GlobalDlLimitKiB { get; set; }

    [JsonPropertyName("up_limit")]
    public int? GlobalUpLimitKiB { get; set; }

    [JsonPropertyName("max_connec")]
    public int? MaxConnections { get; set; }

    [JsonPropertyName("max_connec_per_torrent")]
    public int? MaxConnectionsPerTorrent { get; set; }

    [JsonPropertyName("max_uploads")]
    public int? MaxUploads { get; set; }

    [JsonPropertyName("max_uploads_per_torrent")]
    public int? MaxUploadsPerTorrent { get; set; }

    [JsonPropertyName("stop_tracker_timeout")]
    public int? StopTrackerTimeoutSeconds { get; set; }

    [JsonPropertyName("enable_piece_extent_affinity")]
    public bool? PieceExtentAffinityEnabled { get; set; }

    [JsonPropertyName("bittorrent_protocol")]
    public EnumBittorrentProtocol? BittorrentProtocol { get; set; }

    [JsonPropertyName("limit_utp_rate")]
    public bool? LimitUtpRate { get; set; }

    [JsonPropertyName("limit_tcp_overhead")]
    public bool? LimitTcpOverhead { get; set; }

    [JsonPropertyName("limit_lan_peers")]
    public bool? LimitLanPeers { get; set; }

    [JsonPropertyName("alt_dl_limit")]
    public int? AlternativeDownloadLimitKiB { get; set; }

    [JsonPropertyName("alt_up_limit")]
    public int? AlternativeUploadLimitKiB { get; set; }

    [JsonPropertyName("scheduler_enabled")]
    public bool? SchedulerEnabled { get; set; }

    [JsonPropertyName("scheduler_from_hour")]
    public int? SchedulerStartingHour { get; set; }

    [JsonPropertyName("scheduler_from_min")]
    public int? SchedulerStartingMinute { get; set; }

    [JsonPropertyName("scheduler_to_hour")]
    public int? SchedulerEndingHour { get; set; }

    [JsonPropertyName("scheduler_to_min")]
    public int? SchedulerEndingMinute { get; set; }

    [JsonPropertyName("scheduler_days")]
    public EnumSchedulerDay? SchedulerDays { get; set; }

    [JsonPropertyName("dht")]
    public bool? Dht { get; set; }

    [JsonPropertyName("pex")]
    public bool? Pex { get; set; }

    [JsonPropertyName("lsd")]
    public bool? Lsd { get; set; }

    [JsonPropertyName("encryption")]
    public EnumEncryptionMode? Encryption { get; set; }

    [JsonPropertyName("anonymous_mode")]
    public bool? AnonymousMode { get; set; }

    [JsonPropertyName("proxy_type")]
    [JsonConverter(typeof(ProxyTypeJsonConverter))]
    public EnumProxyType? ProxyType { get; set; }

    [JsonPropertyName("proxy_ip")]
    public string? ProxyIp { get; set; }

    [JsonPropertyName("proxy_port")]
    public int? ProxyPort { get; set; }

    [JsonPropertyName("proxy_peer_connections")]
    public bool? ProxyPeerConnections { get; set; }

    [JsonPropertyName("proxy_auth_enabled")]
    public bool? ProxyAuthEnabled { get; set; }

    [JsonPropertyName("proxy_username")]
    public string? ProxyUsername { get; set; }

    [JsonPropertyName("proxy_password")]
    public string? ProxyPassword { get; set; }

    [JsonPropertyName("proxy_torrents_only")]
    public bool? ProxyTorrentsOnly { get; set; }

    [JsonPropertyName("ip_filter_enabled")]
    public bool? IpFilterEnabled { get; set; }

    [JsonPropertyName("ip_filter_path")]
    public string? IpFilterPath { get; set; }

    [JsonPropertyName("ip_filter_trackers")]
    public bool? IpFilterTrackersEnabled { get; set; }

    [JsonPropertyName("web_ui_domain_list")]
    public string? WebUiDomains { get; set; }

    [JsonPropertyName("web_ui_address")]
    public string? WebUiAddress { get; set; }

    [JsonPropertyName("web_ui_port")]
    public int? WebUiPort { get; set; }

    [JsonPropertyName("web_ui_upnp")]
    public bool? WebUiUpnp { get; set; }

    [JsonPropertyName("web_ui_username")]
    public string? WebUiUsername { get; set; }

    [JsonPropertyName("web_ui_password")]
    public string? WebUiPassword { get; set; }

    [JsonPropertyName("web_ui_csrf_protection_enabled")]
    public bool? WebUiCsrfProtectionEnabled { get; set; }

    [JsonPropertyName("web_ui_clickjacking_protection_enabled")]
    public bool? WebUiClickjackingProtectionEnabled { get; set; }

    [JsonPropertyName("web_ui_secure_cookie_enabled")]
    public bool? WebUiSecureCookieEnabled { get; set; }

    [JsonPropertyName("web_ui_max_auth_fail_count")]
    public int? WebUiMaxAuthFailCount { get; set; }

    [JsonPropertyName("web_ui_ban_duration")]
    public int? WebUiBanDurationSeconds { get; set; }

    [JsonPropertyName("web_ui_session_timeout")]
    public int? WebUiSessionTimeoutSeconds { get; set; }

    [JsonPropertyName("web_ui_host_header_validation_enabled")]
    public bool? WebUiHostHeaderValidationEnabled { get; set; }

    [JsonPropertyName("bypass_local_auth")]
    public bool? BypassLocalAuth { get; set; }

    [JsonPropertyName("bypass_auth_subnet_whitelist_enabled")]
    public bool? BypassAuthSubnetWhitelistEnabled { get; set; }

    [JsonPropertyName("bypass_auth_subnet_whitelist")]
    public string? BypassAuthSubnetWhitelist { get; set; }

    [JsonPropertyName("alternative_webui_enabled")]
    public bool? AlternativeWebUiEnabled { get; set; }

    [JsonPropertyName("alternative_webui_path")]
    public string? AlternativeWebUiPath { get; set; }

    [JsonPropertyName("use_https")]
    public bool? WebUiHttpsEnabled { get; set; }

    [JsonPropertyName("ssl_key")]
    public string? WebUiSslKey { get; set; }

    [JsonPropertyName("ssl_cert")]
    public string? WebUiSslCert { get; set; }

    [JsonPropertyName("web_ui_https_key_path")]
    public string? WebUiHttpsKeyPath { get; set; }

    [JsonPropertyName("web_ui_https_cert_path")]
    public string? WebUiHttpsCertPath { get; set; }

    [JsonPropertyName("dyndns_enabled")]
    public bool? DyndnsEnabled { get; set; }

    [JsonPropertyName("dyndns_service")]
    public int? DynamicalDnsService { get; set; }

    [JsonPropertyName("dyndns_username")]
    public string? DynamicalDnsUsername { get; set; }

    [JsonPropertyName("dyndns_password")]
    public string? DynamicalDnsPassword { get; set; }

    [JsonPropertyName("dyndns_domain")]
    public string? DynamicalDnsDomain { get; set; }

    [JsonPropertyName("rss_refresh_interval")]
    public int? RssRefreshInterval { get; set; }

    [JsonPropertyName("rss_max_articles_per_feed")]
    public int? RssMaxArticlesPerFeed { get; set; }

    [JsonPropertyName("rss_processing_enabled")]
    public bool? RssProcessingEnabled { get; set; }

    [JsonPropertyName("rss_auto_downloading_enabled")]
    public bool? RssAutoDownloadingEnabled { get; set; }

    [JsonPropertyName("rss_download_repack_proper_episodes")]
    public bool? RssDownloadRepackProperEpisodes { get; set; }

    [JsonPropertyName("rss_smart_episode_filters")]
    public string? RssSmartEpisodeFilters { get; set; }

    [JsonPropertyName("add_trackers_enabled")]
    public bool? AddTrackersEnabled { get; set; }

    [JsonPropertyName("add_trackers")]
    public string? AddTrackers { get; set; }

    [JsonPropertyName("web_ui_use_custom_http_headers_enabled")]
    public bool? WebUiUseCustomHttpHeadersEnabled { get; set; }

    [JsonPropertyName("web_ui_custom_http_headers")]
    public string? WebUiCustomHttpHeaders { get; set; }

    [JsonPropertyName("max_seeding_time_enabled")]
    public bool? MaxSeedingTimeEnabled { get; set; }

    [JsonPropertyName("max_seeding_time")]
    public int? MaxSeedingTimeMinutes { get; set; }

    [JsonPropertyName("announce_ip")]
    public string? AnnounceIp { get; set; }

    [JsonPropertyName("announce_to_all_tiers")]
    public bool? AnnounceToAllTiers { get; set; }

    [JsonPropertyName("announce_to_all_trackers")]
    public bool? AnnounceToAllTrackers { get; set; }

    [JsonPropertyName("async_io_threads")]
    public int? AsyncIoThreads { get; set; }

    [JsonPropertyName("banned_IPs")]
    public string? BannedIPs { get; set; }

    [JsonPropertyName("checking_memory_use")]
    public int? CheckingMemoryUseMiB { get; set; }

    [JsonPropertyName("current_interface_address")]
    public string? CurrentInterfaceAddress { get; set; }

    [JsonPropertyName("current_network_interface")]
    public string? CurrentNetworkInterface { get; set; }

    [JsonPropertyName("disk_cache")]
    public int? DiskCacheMiB { get; set; }

    [JsonPropertyName("disk_cache_ttl")]
    public int? DiskCacheTtlSeconds { get; set; }

    [JsonPropertyName("embedded_tracker_port")]
    public int? EmbeddedTrackerPort { get; set; }

    [JsonPropertyName("enable_coalesce_read_write")]
    public bool? EnableCoalesceReadWrite { get; set; }

    [JsonPropertyName("enable_embedded_tracker")]
    public bool? EnableEmbeddedTracker { get; set; }

    [JsonPropertyName("enable_multi_connections_from_same_ip")]
    public bool? EnableMultiConnectionsFromSameIp { get; set; }

    [JsonPropertyName("enable_os_cache")]
    public bool? EnableOsCache { get; set; }

    [JsonPropertyName("enable_upload_suggestions")]
    public bool? EnableUploadSuggestions { get; set; }

    [JsonPropertyName("file_pool_size")]
    public int? FilePoolSize { get; set; }

    [JsonPropertyName("outgoing_ports_max")]
    public int? OutgoingPortsMax { get; set; }

    [JsonPropertyName("outgoing_ports_min")]
    public int? OutgoingPortsMin { get; set; }

    [JsonPropertyName("recheck_completed_torrents")]
    public bool? RecheckCompletedTorrents { get; set; }

    [JsonPropertyName("resolve_peer_countries")]
    public bool? ResolvePeerCountries { get; set; }

    [JsonPropertyName("save_resume_data_interval")]
    public int? SaveResumeDataIntervalMinutes { get; set; }

    [JsonPropertyName("send_buffer_low_watermark")]
    public int? SendBufferLowWatermarkKiB { get; set; }

    [JsonPropertyName("send_buffer_watermark")]
    public int? SendBufferWatermarkKiB { get; set; }

    [JsonPropertyName("send_buffer_watermark_factor")]
    public int? SendBufferWatermarkFactorPercent { get; set; }

    [JsonPropertyName("socket_backlog_size")]
    public int? SocketBacklogSize { get; set; }

    [JsonPropertyName("upload_choking_algorithm")]
    public EnumUploadChokingAlgorithm? UploadChokingAlgorithm { get; set; }

    [JsonPropertyName("upload_slots_behavior")]
    public EnumUploadSlotsBehavior? UploadSlotsBehavior { get; set; }

    [JsonPropertyName("upnp_lease_duration")]
    public int? UpnpLeaseDuration { get; set; }

    [JsonPropertyName("utp_tcp_mixed_mode")]
    public EnumUtpTcpMixedMode? UtpTcpMixedMode { get; set; }

    [JsonExtensionData]
    public Dictionary<string, JsonElement>? ExtensionData { get; set; }
}
