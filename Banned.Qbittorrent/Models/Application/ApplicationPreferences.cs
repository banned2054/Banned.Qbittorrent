using Banned.Qbittorrent.Models.Enums;
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

    /// <summary>
    /// Monitored directories and their download destinations.
    /// Key = monitored folder path.
    /// Value = behavior (0 = to same folder, 1 = to default save path, or a custom absolute path).
    /// </summary>
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
    public bool? AutoRunCommand { get; set; }

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

    /// <summary>
    /// UPNP / NAT-PMP port forwarding enabled.
    /// </summary>
    [JsonPropertyName("upnp")]
    public bool? UpnpNatPmpEnable { get; set; }

    [JsonPropertyName("random_port")]
    public bool? RandomPortEnable { get; set; }

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
    public bool? PieceExtentAffinityEnable { get; set; }

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
    public bool? SchedulerEnable { get; set; }

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
    public bool? IpFilterTrackersEnable { get; set; }

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

    [JsonPropertyName("upload_choking_algorithm")]
    public EnumUploadChokingAlgorithm? UploadChokingAlgorithm { get; set; }

    [JsonPropertyName("upload_slots_behavior")]
    public EnumUploadSlotsBehavior? UploadSlotsBehavior { get; set; }

    [JsonPropertyName("recheck_completed_torrents")]
    public bool? RecheckCompletedTorrents { get; set; }

    [JsonPropertyName("upnp_lease_duration")]
    public int? UpnpLeaseDuration { get; set; }

    [JsonPropertyName("utp_tcp_mixed_mode")]
    public EnumUtpTcpMixedMode? UtpTcpMixedMode { get; set; }

    [JsonExtensionData]
    public Dictionary<string, JsonElement>? ExtensionData { get; set; }
}
