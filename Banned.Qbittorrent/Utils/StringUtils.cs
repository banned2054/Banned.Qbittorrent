using Banned.Qbittorrent.Models.Enums;

namespace Banned.Qbittorrent.Utils;

/// <summary>
/// 提供字符串处理与枚举转换的辅助方法。<br/>
/// Provides helper methods for string processing and enum conversion.
/// </summary>
public static class StringUtils
{
    /// <summary>
    /// 使用指定分隔符连接集合中的元素，并自动忽略空值。<br/>
    /// Joins elements of a collection using a specified separator, automatically ignoring null or whitespace.
    /// </summary>
    /// <typeparam name="T">集合中元素的类型。 / The type of elements in the collection.</typeparam>
    /// <param name="separator">分隔符（字符串）。 / The separator string.</param>
    /// <param name="values">要连接的集合。 / The collection to join.</param>
    /// <returns>连接后的字符串。 / The joined string.</returns>
    public static string Join<T>(string separator, IEnumerable<T>? values)
    {
        if (values == null)
            return string.Empty;

        IEnumerable<string> filtered;

        if (typeof(T) == typeof(string))
        {
            filtered = values
                      .Cast<string>()
                      .Where(v => !string.IsNullOrWhiteSpace(v))
                      .Select(v => v.Trim());
        }
        else
        {
            filtered = values
                      .Where(v => v != null)
                      .Select(v => v!.ToString()!);
        }

        return string.Join(separator, filtered);
    }

    /// <summary>
    /// 使用指定分隔符连接集合中的元素，并自动忽略空值。<br/>
    /// Joins elements of a collection using a specified separator, automatically ignoring null or whitespace.
    /// </summary>
    /// <typeparam name="T">集合中元素的类型。 / The type of elements in the collection.</typeparam>
    /// <param name="separator">分隔符（字符）。 / The separator character.</param>
    /// <param name="values">要连接的集合。 / The collection to join.</param>
    /// <returns>连接后的字符串。 / The joined string.</returns>
    public static string Join<T>(char separator, IEnumerable<T>? values)
    {
        if (values == null)
            return string.Empty;

        IEnumerable<string> filtered;

        if (typeof(T) == typeof(string))
        {
            filtered = values
                      .Cast<string>()
                      .Where(v => !string.IsNullOrWhiteSpace(v))
                      .Select(v => v.Trim());
        }
        else
        {
            filtered = values
                      .Where(v => v != null)
                      .Select(v => v!.ToString()!);
        }

        return string.Join(separator, filtered);
    }

    /// <summary>
    /// 将连接状态字符串转换为枚举。<br/>
    /// Converts a connection status string to an enum.
    /// </summary>
    public static EnumConnectionStatus String2ConnectionStatus(string? status)
    {
        return status?.ToLower() switch
        {
            "connected"    => EnumConnectionStatus.Connected,
            "firewalled"   => EnumConnectionStatus.Firewalled,
            "disconnected" => EnumConnectionStatus.Disconnected,
            _              => EnumConnectionStatus.Unknown
        };
    }

    /// <summary>
    /// 将停止条件字符串转换为枚举。<br/>
    /// Converts a stop condition string to an enum.
    /// </summary>
    public static EnumStopCondition String2StopCondition(string? value)
    {
        return value?.ToLower() switch
        {
            "never"            => EnumStopCondition.Never,
            "metadatareceived" => EnumStopCondition.MetadataReceived,
            "torrentadded"     => EnumStopCondition.TorrentAdded,
            _                  => EnumStopCondition.Unknown
        };
    }

    /// <summary>
    /// 将停止条件枚举转换为 API 识别的字符串。<br/>
    /// Converts a stop condition enum to an API-recognized string.
    /// </summary>
    public static string StopCondition2String(this EnumStopCondition value)
    {
        return value switch
        {
            EnumStopCondition.Never            => "Never",
            EnumStopCondition.MetadataReceived => "MetadataReceived",
            EnumStopCondition.TorrentAdded     => "TorrentAdded",
            _                                  => "Never"
        };
    }

    /// <summary>
    /// 将内容布局字符串转换为枚举。<br/>
    /// Converts a content layout string to an enum.
    /// </summary>
    public static EnumContentLayout String2ContentLayout(string? value)
    {
        return value?.ToLower() switch
        {
            "original"    => EnumContentLayout.Original,
            "subfolder"   => EnumContentLayout.Subfolder,
            "nosubfolder" => EnumContentLayout.NoSubfolder,
            _             => EnumContentLayout.Unknown
        };
    }

    /// <summary>
    /// 将内容布局枚举转换为 API 识别的字符串。<br/>
    /// Converts a content layout enum to an API-recognized string.
    /// </summary>
    public static string ContentLayout2String(this EnumContentLayout value)
    {
        return value switch
        {
            EnumContentLayout.Original    => "Original",
            EnumContentLayout.Subfolder   => "Subfolder",
            EnumContentLayout.NoSubfolder => "NoSubfolder",
            _                             => "Original"
        };
    }

    /// <summary>
    /// 将搜索状态字符串转换为枚举。<br/>
    /// Converts a search status string to an enum.
    /// </summary>
    public static EnumSearchStatus String2SearchStatus(string? value)
    {
        return value switch
        {
            "Running" => EnumSearchStatus.Running,
            "Stopped" => EnumSearchStatus.Stopped,
            _         => EnumSearchStatus.Unknown,
        };
    }

    /// <summary>
    /// 将种子过滤器枚举转换为 API 识别的小写字符串。<br/>
    /// Converts a torrent filter enum to an API-recognized lowercase string.
    /// </summary>
    public static string TorrentFilter2String(this EnumTorrentFilter value)
    {
        return value switch
        {
            EnumTorrentFilter.All                => "all",
            EnumTorrentFilter.Downloading        => "downloading",
            EnumTorrentFilter.Seeding            => "seeding",
            EnumTorrentFilter.Completed          => "completed",
            EnumTorrentFilter.Paused             => "paused",
            EnumTorrentFilter.Active             => "active",
            EnumTorrentFilter.Inactive           => "inactive",
            EnumTorrentFilter.Resumed            => "resumed",
            EnumTorrentFilter.Stalled            => "stalled",
            EnumTorrentFilter.StalledUploading   => "stalled_uploading",
            EnumTorrentFilter.StalledDownloading => "stalled_downloading",
            EnumTorrentFilter.Error              => "errored",

            _ => throw new ArgumentOutOfRangeException(nameof(value), value, null)
        };
    }

    /// <summary>
    /// 将 API 返回的过滤器字符串转换为枚举。<br/>
    /// Converts a filter string from the API to an enum.
    /// </summary>
    public static EnumTorrentFilter String2TorrentFilter(string value)
    {
        return value switch
        {
            "all"                 => EnumTorrentFilter.All,
            "downloading"         => EnumTorrentFilter.Downloading,
            "seeding"             => EnumTorrentFilter.Seeding,
            "completed"           => EnumTorrentFilter.Completed,
            "paused"              => EnumTorrentFilter.Paused,
            "active"              => EnumTorrentFilter.Active,
            "inactive"            => EnumTorrentFilter.Inactive,
            "resumed"             => EnumTorrentFilter.Resumed,
            "stalled"             => EnumTorrentFilter.Stalled,
            "stalled_uploading"   => EnumTorrentFilter.StalledUploading,
            "stalled_downloading" => EnumTorrentFilter.StalledDownloading,
            "errored"             => EnumTorrentFilter.Error,
            _                     => throw new ArgumentOutOfRangeException(nameof(value), value, null)
        };
    }

    /// <summary>
    /// 将种子状态字符串（兼容 v4/v5）转换为枚举。<br/>
    /// Converts a torrent state string (v4/v5 compatible) to an enum.
    /// </summary>
    public static EnumTorrentState String2TorrentState(string value)
    {
        return value switch
        {
            "error"              => EnumTorrentState.Error,
            "missingFiles"       => EnumTorrentState.MissingFiles,
            "uploading"          => EnumTorrentState.Uploading,
            "pausedUP"           => EnumTorrentState.StoppedUpload,
            "stoppedUP"          => EnumTorrentState.StoppedUpload,
            "queuedUP"           => EnumTorrentState.QueuedUpload,
            "stalledUP"          => EnumTorrentState.StalledUpload,
            "checkingUP"         => EnumTorrentState.CheckingUpload,
            "forcedUP"           => EnumTorrentState.ForcedUpload,
            "allocating"         => EnumTorrentState.Allocating,
            "downloading"        => EnumTorrentState.Downloading,
            "metaDL"             => EnumTorrentState.MetaDownload,
            "pausedDL"           => EnumTorrentState.StoppedDownload,
            "stoppedDL"          => EnumTorrentState.StoppedDownload,
            "queuedDL"           => EnumTorrentState.QueuedDownload,
            "stalledDL"          => EnumTorrentState.StalledDownload,
            "checkingDL"         => EnumTorrentState.CheckingDownload,
            "forcedDL"           => EnumTorrentState.ForcedDownload,
            "checkingResumeData" => EnumTorrentState.CheckingResumeData,
            "moving"             => EnumTorrentState.Moving,
            "unknown"            => EnumTorrentState.Unknown,
            _                    => throw new ArgumentOutOfRangeException(nameof(value), value, null)
        };
    }

    /// <summary>
    /// 将状态枚举转换为 qBittorrent v4.x 使用的字符串格式。<br/>
    /// Converts the state enum to the string format used by qBittorrent v4.x.
    /// </summary>
    /// <remarks>使用 "pausedUP" / "pausedDL" 等字样。 / Uses terms like "pausedUP" / "pausedDL".</remarks>
    public static string TorrentState2StringV4(this EnumTorrentState value)
    {
        return value switch
        {
            EnumTorrentState.Error              => "error",
            EnumTorrentState.MissingFiles       => "missingFiles",
            EnumTorrentState.Uploading          => "uploading",
            EnumTorrentState.StoppedUpload      => "pausedUP",
            EnumTorrentState.QueuedUpload       => "queuedUP",
            EnumTorrentState.StalledUpload      => "stalledUP",
            EnumTorrentState.CheckingUpload     => "checkingUP",
            EnumTorrentState.ForcedUpload       => "forcedUP",
            EnumTorrentState.Allocating         => "allocating",
            EnumTorrentState.Downloading        => "downloading",
            EnumTorrentState.MetaDownload       => "metaDL",
            EnumTorrentState.StoppedDownload    => "pausedDL",
            EnumTorrentState.QueuedDownload     => "queuedDL",
            EnumTorrentState.StalledDownload    => "stalledDL",
            EnumTorrentState.CheckingDownload   => "checkingDL",
            EnumTorrentState.ForcedDownload     => "forcedDL",
            EnumTorrentState.CheckingResumeData => "checkingResumeData",
            EnumTorrentState.Moving             => "moving",
            EnumTorrentState.Unknown            => "unknown",
            _                                   => throw new ArgumentOutOfRangeException(nameof(value), value, null)
        };
    }

    /// <summary>
    /// 将状态枚举转换为 qBittorrent v5.x 使用的字符串格式。<br/>
    /// Converts the state enum to the string format used by qBittorrent v5.x.
    /// </summary>
    /// <remarks>使用 "stoppedUP" / "stoppedDL" 等字样。 / Uses terms like "stoppedUP" / "stoppedDL".</remarks>
    public static string TorrentState2StringV5(this EnumTorrentState value)
    {
        return value switch
        {
            EnumTorrentState.Error              => "error",
            EnumTorrentState.MissingFiles       => "missingFiles",
            EnumTorrentState.Uploading          => "uploading",
            EnumTorrentState.StoppedUpload      => "stoppedUP",
            EnumTorrentState.QueuedUpload       => "queuedUP",
            EnumTorrentState.StalledUpload      => "stalledUP",
            EnumTorrentState.CheckingUpload     => "checkingUP",
            EnumTorrentState.ForcedUpload       => "forcedUP",
            EnumTorrentState.Allocating         => "allocating",
            EnumTorrentState.Downloading        => "downloading",
            EnumTorrentState.MetaDownload       => "metaDL",
            EnumTorrentState.StoppedDownload    => "stoppedDL",
            EnumTorrentState.QueuedDownload     => "queuedDL",
            EnumTorrentState.StalledDownload    => "stalledDL",
            EnumTorrentState.CheckingDownload   => "checkingDL",
            EnumTorrentState.ForcedDownload     => "forcedDL",
            EnumTorrentState.CheckingResumeData => "checkingResumeData",
            EnumTorrentState.Moving             => "moving",
            EnumTorrentState.Unknown            => "unknown",
            _                                   => throw new ArgumentOutOfRangeException(nameof(value), value, null)
        };
    }
}
