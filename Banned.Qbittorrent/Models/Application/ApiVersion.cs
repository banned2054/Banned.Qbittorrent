namespace Banned.Qbittorrent.Models.Application;

public readonly struct ApiVersion(int major, int minor = 0, int patch = 0)
    : IComparable<ApiVersion>, IEquatable<ApiVersion>
{
    public int Major { get; } = major;
    public int Minor { get; } = minor;
    public int Patch { get; } = patch;

    // 预定义常用版本（按你的使用场景自行增减）
    public static readonly ApiVersion V2_0_0  = new(2);
    public static readonly ApiVersion V2_0_2  = new(2, 0, 2);
    public static readonly ApiVersion V2_1_0  = new(2, 1);
    public static readonly ApiVersion V2_1_1  = new(2, 1, 1);
    public static readonly ApiVersion V2_2_0  = new(2, 2);
    public static readonly ApiVersion V2_3_0  = new(2, 3);
    public static readonly ApiVersion V2_7_0  = new(2, 7);
    public static readonly ApiVersion V2_11_0 = new(2, 11);
    public static readonly ApiVersion V2_11_3 = new(2, 11, 3);

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

    public static ApiVersion Parse(string versionString)
    {
        if (versionString is null) throw new ArgumentNullException(nameof(versionString));
        if (TryParse(versionString.AsSpan(), out var v)) return v;
        throw new FormatException($"Invalid version string: '{versionString}'.");
    }

    public int CompareTo(ApiVersion other)
    {
        if (Major != other.Major) return Major.CompareTo(other.Major);
        return Minor != other.Minor ? Minor.CompareTo(other.Minor) : Patch.CompareTo(other.Patch);
    }

    public bool Equals(ApiVersion other) => Major == other.Major && Minor == other.Minor && Patch == other.Patch;
    public override bool Equals(object? obj) => obj is ApiVersion other && Equals(other);
    public override int GetHashCode() => HashCode.Combine(Major, Minor, Patch);

    public static bool operator >(ApiVersion  a, ApiVersion b) => a.CompareTo(b) > 0;
    public static bool operator <(ApiVersion  a, ApiVersion b) => a.CompareTo(b) < 0;
    public static bool operator >=(ApiVersion a, ApiVersion b) => a.CompareTo(b) >= 0;
    public static bool operator <=(ApiVersion a, ApiVersion b) => a.CompareTo(b) <= 0;
    public static bool operator ==(ApiVersion a, ApiVersion b) => a.Equals(b);
    public static bool operator !=(ApiVersion a, ApiVersion b) => !a.Equals(b);

    public override string ToString() => $"{Major}.{Minor}.{Patch}";
}
