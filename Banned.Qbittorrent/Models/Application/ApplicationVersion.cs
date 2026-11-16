using System.Globalization;

namespace Banned.Qbittorrent.Models.Application;

/// <summary>
///     表示 qBittorrent 应用版本，并保留原始文本。比较和相等判断仅使用数字部分。<br />
///     Represents a qBittorrent application version and preserves its original text. Comparisons and equality use only
///     numeric components.
/// </summary>
public readonly struct ApplicationVersion(int major, int minor = 0, int patch = 0, int revision = 0)
    : IComparable<ApplicationVersion>, IEquatable<ApplicationVersion>
{
    private readonly string? _originalText;

    private ApplicationVersion(int major, int minor, int patch, int revision, string originalText) :
        this(major, minor, patch, revision) => _originalText = originalText;

    /// <summary>主版本号。<br />Major version number.</summary>
    public int Major { get; } = major;

    /// <summary>次版本号。<br />Minor version number.</summary>
    public int Minor { get; } = minor;

    /// <summary>修订版本号。<br />Patch version number.</summary>
    public int Patch { get; } = patch;

    /// <summary>可选的第四段版本号。<br />Optional fourth version component.</summary>
    public int Revision { get; } = revision;

    #region Known Versions

    /// <summary>
    ///     qBittorrent 4.3.3：文件重命名改用路径参数，并引入文件夹重命名。<br />
    ///     qBittorrent 4.3.3: path-based file renaming and folder renaming.
    /// </summary>
    public static readonly ApplicationVersion V4_3_3 = new(4, 3, 3);

    #endregion

    /// <summary>
    ///     尝试解析应用版本，接受 v 前缀和预发布后缀。<br />
    ///     Tries to parse an application version, accepting a v prefix and prerelease suffixes.
    /// </summary>
    /// <param name="span">应用版本字符串。 / Application version string.</param>
    /// <param name="version">保留原始文本的版本。 / Parsed version retaining the original text.</param>
    /// <returns>解析成功时为 true。 / True when parsing succeeds.</returns>
    public static bool TryParse(ReadOnlySpan<char> span, out ApplicationVersion version)
    {
        version = default;
        var originalText = span;
        span = span.Trim();
        if (span.IsEmpty) return false;
        if (span[0] is 'v' or 'V') span = span[1..];

        var numericLength = 0;
        while (numericLength < span.Length && (char.IsAsciiDigit(span[numericLength]) || span[numericLength] == '.'))
            numericLength++;

        var       numeric    = span[..numericLength].TrimEnd('.');
        Span<int> components = stackalloc int[4];
        components.Clear();
        var count = 0;
        while (!numeric.IsEmpty)
        {
            if (count == components.Length) return false;
            var separator = numeric.IndexOf('.');
            var component = separator < 0 ? numeric : numeric[..separator];
            if (!int.TryParse(component, NumberStyles.None, CultureInfo.InvariantCulture, out components[count]))
                return false;

            count++;
            if (separator < 0) break;
            numeric = numeric[(separator + 1)..];
        }

        if (count < 2) return false;
        version = new ApplicationVersion(components[0], components[1], components[2], components[3],
                                         originalText.ToString());
        return true;
    }

    /// <summary>解析 qBittorrent 应用版本。<br />Parses a qBittorrent application version.</summary>
    /// <param name="versionString">应用版本字符串，例如 v4.3.3 或 5.4.0alpha1。 / Application version string.</param>
    /// <returns>保留原始文本的版本。 / Parsed version retaining the original text.</returns>
    /// <exception cref="ArgumentNullException">输入为 null。 / The input is null.</exception>
    /// <exception cref="FormatException">版本格式无效。 / The version format is invalid.</exception>
    public static ApplicationVersion Parse(string versionString)
    {
        ArgumentNullException.ThrowIfNull(versionString);
        if (TryParse(versionString.AsSpan(), out var version)) return version;
        throw new FormatException($"Invalid qBittorrent version string: '{versionString}'.");
    }

    /// <inheritdoc />
    public int CompareTo(ApplicationVersion other)
    {
        if (Major != other.Major) return Major.CompareTo(other.Major);
        if (Minor != other.Minor) return Minor.CompareTo(other.Minor);
        return Patch != other.Patch ? Patch.CompareTo(other.Patch) : Revision.CompareTo(other.Revision);
    }

    /// <inheritdoc />
    public bool Equals(ApplicationVersion other) =>
        Major == other.Major && Minor == other.Minor && Patch == other.Patch && Revision == other.Revision;

    /// <inheritdoc />
    public override bool Equals(object? obj) => obj is ApplicationVersion other && Equals(other);

    /// <inheritdoc />
    public override int GetHashCode() => HashCode.Combine(Major, Minor, Patch, Revision);

    /// <summary>判断左侧版本是否较高。<br />Determines whether the left version is greater.</summary>
    /// <param name="a">左侧版本。 / Left version.</param>
    /// <param name="b">右侧版本。 / Right version.</param>
    /// <returns>比较结果。 / Comparison result.</returns>
    public static bool operator >(ApplicationVersion a, ApplicationVersion b) => a.CompareTo(b) > 0;

    /// <summary>判断左侧版本是否较低。<br />Determines whether the left version is less.</summary>
    /// <param name="a">左侧版本。 / Left version.</param>
    /// <param name="b">右侧版本。 / Right version.</param>
    /// <returns>比较结果。 / Comparison result.</returns>
    public static bool operator <(ApplicationVersion a, ApplicationVersion b) => a.CompareTo(b) < 0;

    /// <summary>判断左侧版本是否不低于右侧版本。<br />Determines whether the left version is greater than or equal.</summary>
    /// <param name="a">左侧版本。 / Left version.</param>
    /// <param name="b">右侧版本。 / Right version.</param>
    /// <returns>比较结果。 / Comparison result.</returns>
    public static bool operator >=(ApplicationVersion a, ApplicationVersion b) => a.CompareTo(b) >= 0;

    /// <summary>判断左侧版本是否不高于右侧版本。<br />Determines whether the left version is less than or equal.</summary>
    /// <param name="a">左侧版本。 / Left version.</param>
    /// <param name="b">右侧版本。 / Right version.</param>
    /// <returns>比较结果。 / Comparison result.</returns>
    public static bool operator <=(ApplicationVersion a, ApplicationVersion b) => a.CompareTo(b) <= 0;

    /// <summary>判断两个版本是否相等。<br />Determines whether the versions are equal.</summary>
    /// <param name="a">左侧版本。 / Left version.</param>
    /// <param name="b">右侧版本。 / Right version.</param>
    /// <returns>比较结果。 / Comparison result.</returns>
    public static bool operator ==(ApplicationVersion a, ApplicationVersion b) => a.Equals(b);

    /// <summary>判断两个版本是否不相等。<br />Determines whether the versions differ.</summary>
    /// <param name="a">左侧版本。 / Left version.</param>
    /// <param name="b">右侧版本。 / Right version.</param>
    /// <returns>比较结果。 / Comparison result.</returns>
    public static bool operator !=(ApplicationVersion a, ApplicationVersion b) => !a.Equals(b);

    /// <summary>
    ///     返回解析时的原始文本；直接通过数字构造时返回规范化版本号。<br />
    ///     Returns the original parsed text, or the normalized version when constructed from numeric components.
    /// </summary>
    /// <returns>版本文本。 / Version text.</returns>
    public override string ToString() => _originalText ?? (Revision == 0
        ? $"{Major}.{Minor}.{Patch}"
        : $"{Major}.{Minor}.{Patch}.{Revision}");
}
