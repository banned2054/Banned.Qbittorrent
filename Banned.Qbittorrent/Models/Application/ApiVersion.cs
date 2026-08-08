namespace Banned.Qbittorrent.Models.Application;

/// <summary>
/// 表示 qBittorrent API 版本号。<br/>
/// Represents the qBittorrent API version.
/// </summary>
public readonly struct ApiVersion(int major, int minor = 0, int patch = 0)
    : IComparable<ApiVersion>, IEquatable<ApiVersion>
{
    /// <summary>主版本号。<br/>Major version number.</summary>
    public int Major { get; } = major;

    /// <summary>次版本号。<br/>Minor version number.</summary>
    public int Minor { get; } = minor;

    /// <summary>修订版本号。<br/>Patch version number.</summary>
    public int Patch { get; } = patch;

    #region Known Versions

    /// <summary>qBittorrent Web API 2.0.2。<br/>qBittorrent Web API version 2.0.2.</summary>
    public static readonly ApiVersion V2_0_2 = new(2, 0, 2);

    /// <summary>qBittorrent Web API 2.1.0。<br/>qBittorrent Web API version 2.1.0.</summary>
    public static readonly ApiVersion V2_1_0 = new(2, 1);

    /// <summary>qBittorrent Web API 2.1.1。<br/>qBittorrent Web API version 2.1.1.</summary>
    public static readonly ApiVersion V2_1_1 = new(2, 1, 1);

    /// <summary>qBittorrent Web API 2.2.0。<br/>qBittorrent Web API version 2.2.0.</summary>
    public static readonly ApiVersion V2_2_0 = new(2, 2);

    /// <summary>qBittorrent Web API 2.3.0。<br/>qBittorrent Web API version 2.3.0.</summary>
    public static readonly ApiVersion V2_3_0 = new(2, 3);

    /// <summary>qBittorrent Web API 2.5.1。<br/>qBittorrent Web API version 2.5.1.</summary>
    public static readonly ApiVersion V2_5_1 = new(2, 5, 1);

    /// <summary>qBittorrent Web API 2.6.0。<br/>qBittorrent Web API version 2.6.0.</summary>
    public static readonly ApiVersion V2_6_0 = new(2, 6);

    /// <summary>qBittorrent Web API 2.7.0。<br/>qBittorrent Web API version 2.7.0.</summary>
    public static readonly ApiVersion V2_7_0 = new(2, 7);

    /// <summary>qBittorrent Web API 2.8.4。<br/>qBittorrent Web API version 2.8.4.</summary>
    public static readonly ApiVersion V2_8_4 = new(2, 8, 4);

    /// <summary>qBittorrent Web API 2.8.14。<br/>qBittorrent Web API version 2.8.14.</summary>
    public static readonly ApiVersion V2_8_14 = new(2, 8, 14);

    /// <summary>qBittorrent Web API 2.9.1。<br/>qBittorrent Web API version 2.9.1.</summary>
    public static readonly ApiVersion V2_9_1 = new(2, 9, 1);

    /// <summary>qBittorrent Web API 2.9.3。<br/>qBittorrent Web API version 2.9.3.</summary>
    public static readonly ApiVersion V2_9_3 = new(2, 9, 3);

    /// <summary>qBittorrent Web API 2.10.4。<br/>qBittorrent Web API version 2.10.4.</summary>
    public static readonly ApiVersion V2_10_4 = new(2, 10, 4);

    /// <summary>qBittorrent Web API 2.11.0。<br/>qBittorrent Web API version 2.11.0.</summary>
    public static readonly ApiVersion V2_11_0 = new(2, 11);

    /// <summary>qBittorrent Web API 2.11.3。<br/>qBittorrent Web API version 2.11.3.</summary>
    public static readonly ApiVersion V2_11_3 = new(2, 11, 3);

    /// <summary>qBittorrent Web API 2.11.4。<br/>qBittorrent Web API version 2.11.4.</summary>
    public static readonly ApiVersion V2_11_4 = new(2, 11, 4);

    /// <summary>qBittorrent Web API 2.11.5。<br/>qBittorrent Web API version 2.11.5.</summary>
    public static readonly ApiVersion V2_11_5 = new(2, 11, 5);

    /// <summary>qBittorrent Web API 2.11.8。<br/>qBittorrent Web API version 2.11.8.</summary>
    public static readonly ApiVersion V2_11_8 = new(2, 11, 8);

    /// <summary>qBittorrent Web API 2.12.1。<br/>qBittorrent Web API version 2.12.1.</summary>
    public static readonly ApiVersion V2_12_1 = new(2, 12, 1);

    /// <summary>qBittorrent Web API 2.15.1。<br/>qBittorrent Web API version 2.15.1.</summary>
    public static readonly ApiVersion V2_15_1 = new(2, 15, 1);

    #endregion

    /// <summary>
    /// 尝试解析版本字符串。<br/>
    /// Tries to parse the version string.
    /// </summary>
    /// <param name="span">包含版本信息的字符范围。<br/>The span containing the version information.</param>
    /// <param name="version">解析成功后的版本对象。<br/>The parsed version object if successful.</param>
    /// <returns>是否解析成功。<br/>True if parsing was successful; otherwise, false.</returns>
    public static bool TryParse(ReadOnlySpan<char> span, out ApiVersion version)
    {
        if (span.IsEmpty)
        {
            version = default;
            return false;
        }

        if (span[0] == 'v' || span[0] == 'V')
            span = span[1..];

        int major, minor = 0, patch = 0;

        var dot1 = span.IndexOf('.');
        if (dot1 < 0)
        {
            if (!int.TryParse(span, out major))
            {
                version = default;
                return false;
            }
        }
        else
        {
            if (!int.TryParse(span[..dot1], out major))
            {
                version = default;
                return false;
            }

            var rest = span[(dot1 + 1)..];

            var dot2 = rest.IndexOf('.');
            if (dot2 < 0)
            {
                if (!int.TryParse(rest, out minor))
                {
                    version = default;
                    return false;
                }
            }
            else
            {
                if (!int.TryParse(rest[..dot2], out minor) || !int.TryParse(rest[(dot2 + 1)..], out patch))
                {
                    version = default;
                    return false;
                }
            }
        }

        version = new ApiVersion(major, minor, patch);
        return true;
    }

    /// <summary>
    /// 解析版本字符串。<br/>
    /// Parses the version string.
    /// </summary>
    /// <param name="versionString">版本字符串（如 "2.1.0" 或 "v2.1"）。<br/>The version string (e.g., "2.1.0" or "v2.1").</param>
    /// <returns>解析出的 <see cref="ApiVersion"/> 实例。<br/>The parsed <see cref="ApiVersion"/> instance.</returns>
    /// <exception cref="ArgumentNullException">当输入字符串为空时抛出。<br/>Thrown when the input string is null.</exception>
    /// <exception cref="FormatException">当字符串格式无效时抛出。<br/>Thrown when the version string format is invalid.</exception>
    public static ApiVersion Parse(string versionString)
    {
        ArgumentNullException.ThrowIfNull(versionString);
        if (TryParse(versionString.AsSpan(), out var v)) return v;
        throw new FormatException($"Invalid version string: '{versionString}'.");
    }

    /// <inheritdoc />
    public int CompareTo(ApiVersion other)
    {
        if (Major != other.Major) return Major.CompareTo(other.Major);
        return Minor != other.Minor ? Minor.CompareTo(other.Minor) : Patch.CompareTo(other.Patch);
    }

    /// <inheritdoc />
    public bool Equals(ApiVersion other) => Major == other.Major && Minor == other.Minor && Patch == other.Patch;

    /// <inheritdoc />
    public override bool Equals(object? obj) => obj is ApiVersion other && Equals(other);

    /// <inheritdoc />
    public override int GetHashCode() => HashCode.Combine(Major, Minor, Patch);

    /// <summary>判断左侧版本是否高于右侧版本。<br/>Determines whether the left version is greater than the right version.</summary>
    /// <param name="a">左侧版本。 / The left version.</param>
    /// <param name="b">右侧版本。 / The right version.</param>
    /// <returns>左侧版本较高时为 <see langword="true"/>。 / <see langword="true"/> when the left version is greater.</returns>
    public static bool operator >(ApiVersion a, ApiVersion b) => a.CompareTo(b) > 0;

    /// <summary>判断左侧版本是否低于右侧版本。<br/>Determines whether the left version is less than the right version.</summary>
    /// <param name="a">左侧版本。 / The left version.</param>
    /// <param name="b">右侧版本。 / The right version.</param>
    /// <returns>左侧版本较低时为 <see langword="true"/>。 / <see langword="true"/> when the left version is less.</returns>
    public static bool operator <(ApiVersion a, ApiVersion b) => a.CompareTo(b) < 0;

    /// <summary>判断左侧版本是否不低于右侧版本。<br/>Determines whether the left version is greater than or equal to the right version.</summary>
    /// <param name="a">左侧版本。 / The left version.</param>
    /// <param name="b">右侧版本。 / The right version.</param>
    /// <returns>左侧版本不低于右侧版本时为 <see langword="true"/>。 / <see langword="true"/> when the left version is greater than or equal.</returns>
    public static bool operator >=(ApiVersion a, ApiVersion b) => a.CompareTo(b) >= 0;

    /// <summary>判断左侧版本是否不高于右侧版本。<br/>Determines whether the left version is less than or equal to the right version.</summary>
    /// <param name="a">左侧版本。 / The left version.</param>
    /// <param name="b">右侧版本。 / The right version.</param>
    /// <returns>左侧版本不高于右侧版本时为 <see langword="true"/>。 / <see langword="true"/> when the left version is less than or equal.</returns>
    public static bool operator <=(ApiVersion a, ApiVersion b) => a.CompareTo(b) <= 0;

    /// <summary>判断两个版本是否相等。<br/>Determines whether two versions are equal.</summary>
    /// <param name="a">左侧版本。 / The left version.</param>
    /// <param name="b">右侧版本。 / The right version.</param>
    /// <returns>两个版本相等时为 <see langword="true"/>。 / <see langword="true"/> when the versions are equal.</returns>
    public static bool operator ==(ApiVersion a, ApiVersion b) => a.Equals(b);

    /// <summary>判断两个版本是否不相等。<br/>Determines whether two versions are not equal.</summary>
    /// <param name="a">左侧版本。 / The left version.</param>
    /// <param name="b">右侧版本。 / The right version.</param>
    /// <returns>两个版本不相等时为 <see langword="true"/>。 / <see langword="true"/> when the versions are not equal.</returns>
    public static bool operator !=(ApiVersion a, ApiVersion b) => !a.Equals(b);

    /// <inheritdoc />
    public override string ToString() => $"{Major}.{Minor}.{Patch}";
}
