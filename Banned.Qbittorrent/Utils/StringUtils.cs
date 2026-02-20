using Banned.Qbittorrent.Models.Enums;

namespace Banned.Qbittorrent.Utils;

public static class StringUtils
{
    /// <summary>
    /// 使用指定分隔符连接集合中的元素。<br/>
    /// Automatically ignores null or whitespace items when the element type is string.
    /// </summary>
    /// <typeparam name="T">集合中元素的类型。</typeparam>
    /// <param name="separator">分隔符（字符串）。</param>
    /// <param name="values">要连接的集合。</param>
    /// <returns>连接后的字符串；若集合为空或全为空白则返回空字符串。</returns>
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
    /// 使用指定分隔符连接集合中的元素。<br/>
    /// Automatically ignores null or whitespace items when the element type is string.
    /// </summary>
    /// <typeparam name="T">集合中元素的类型。</typeparam>
    /// <param name="separator">分隔符（字符）。</param>
    /// <param name="values">要连接的集合。</param>
    /// <returns>连接后的字符串；若集合为空或全为空白则返回空字符串。</returns>
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

    public static string StopCondition2String(this EnumStopCondition value)
    {
        return value switch
        {
            EnumStopCondition.Never            => "Never",
            EnumStopCondition.MetadataReceived => "MetadataReceived",
            EnumStopCondition.TorrentAdded     => "TorrentAdded",
            _                                  => "Never" // 默认返回 Never 比较安全
        };
    }

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

    public static EnumSearchStatus String2SearchStatus(string? value)
    {
        return value switch
        {
            "Running" => EnumSearchStatus.Running,
            "Stopped" => EnumSearchStatus.Stopped,
            _         => EnumSearchStatus.Unknown,
        };
    }

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
