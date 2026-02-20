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

    public static EnumSearchStatus String2EnumSearchStatus(string? value)
    {
        return value switch
        {
            "Running" => EnumSearchStatus.Running,
            "Stopped" => EnumSearchStatus.Stopped,
            _         => EnumSearchStatus.Unknown,
        };
    }
}
