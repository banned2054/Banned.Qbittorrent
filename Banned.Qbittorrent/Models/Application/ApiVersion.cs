namespace Banned.Qbittorrent.Models.Application;

public class ApiVersion : IComparable<ApiVersion>
{
    public int Major { get; }
    public int Minor { get; }
    public int Patch { get; }

    public ApiVersion(int major, int minor = 0, int patch = 0)
    {
        Major = major;
        Minor = minor;
        Patch = patch;
    }

    public ApiVersion(string versionString)
    {
        if (string.IsNullOrWhiteSpace(versionString))
            throw new ArgumentException("Version string cannot be null or empty.", nameof(versionString));

        if (versionString.StartsWith('v'))
            versionString = versionString[1..];

        var parts = versionString.Split('.');
        Major = parts.Length > 0 ? int.Parse(parts[0]) : 0;
        Minor = parts.Length > 1 ? int.Parse(parts[1]) : 0;
        Patch = parts.Length > 2 ? int.Parse(parts[2]) : 0;
    }

    public int CompareTo(ApiVersion? other)
    {
        if (other == null) return 1;
        if (Major != other.Major)
            return Major.CompareTo(other.Major);
        return Minor != other.Minor ? Minor.CompareTo(other.Minor) : Patch.CompareTo(other.Patch);
    }

    public override string ToString()
    {
        return $"{Major}.{Minor}.{Patch}";
    }

    public static bool operator >(ApiVersion  a, ApiVersion b) => a.CompareTo(b) > 0;
    public static bool operator <(ApiVersion  a, ApiVersion b) => a.CompareTo(b) < 0;
    public static bool operator >=(ApiVersion a, ApiVersion b) => a.CompareTo(b) >= 0;
    public static bool operator <=(ApiVersion a, ApiVersion b) => a.CompareTo(b) <= 0;

    public static bool operator ==(ApiVersion? a, ApiVersion? b)
    {
        if (ReferenceEquals(a, b)) return true;   // 同一个引用或都是 null
        if (a is null || b is null) return false; // 有一个 null，另一个不是
        return a.CompareTo(b) == 0;
    }

    public static bool operator !=(ApiVersion? a, ApiVersion? b) => !(a == b);

    public override bool Equals(object? obj)
    {
        return obj is ApiVersion other && this == other;
    }

    public override int GetHashCode()
    {
        return (Major, Minor, Patch).GetHashCode();
    }
}
