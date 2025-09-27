namespace Banned.Qbittorrent.Models.Enums;

public enum EnumTorrentFilter
{
    All,
    Downloading,
    Seeding,
    Completed,
    Paused,
    Active,
    Inactive,
    Resumed,
    Stalled,
    StalledUploading,
    StalledDownloading,
    Error
}

public static class EnumTorrentFilterExtensions
{
    public static string ToTorrentFilterString(this EnumTorrentFilter value)
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

    public static EnumTorrentFilter FromTorrentFilterString(string value)
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
}
