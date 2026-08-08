using Banned.Qbittorrent.Models.Application;
using static NUnit.Framework.Assert;

namespace Banned.Qbittorrent.Test;

[TestFixture]
public class ApiVersionTests
{
    [TestCase("2", 2, 0, 0)]
    [TestCase("v2.6", 2, 6, 0)]
    [TestCase("V2.15.1", 2, 15, 1)]
    public void TryParse_ValidVersion_ReturnsComponents(string value, int major, int minor, int patch)
    {
        var parsed = ApiVersion.TryParse(value, out var version);

        Multiple(() =>
        {
            That(parsed, Is.True);
            That(version.Major, Is.EqualTo(major));
            That(version.Minor, Is.EqualTo(minor));
            That(version.Patch, Is.EqualTo(patch));
        });
    }

    [TestCase("")]
    [TestCase("v")]
    [TestCase("two")]
    [TestCase("2.x")]
    [TestCase("2.1.x")]
    [TestCase("2.1.0.1")]
    public void TryParse_InvalidVersion_ReturnsFalse(string value)
    {
        That(ApiVersion.TryParse(value, out var version), Is.False);
        That(version, Is.EqualTo(default(ApiVersion)));
    }

    [Test]
    public void Parse_InvalidInput_ThrowsMeaningfulException()
    {
        Throws<ArgumentNullException>(() => ApiVersion.Parse(null!));
        Throws<FormatException>(() => ApiVersion.Parse("invalid"));
    }

    [Test]
    public void ComparisonAndEquality_UseAllVersionComponents()
    {
        var older = new ApiVersion(2, 9, 3);
        var newer = new ApiVersion(2, 10, 0);
        var same  = new ApiVersion(2, 10);

        Multiple(() =>
        {
            That(older < newer, Is.True);
            That(newer > older, Is.True);
            That(newer >= same, Is.True);
            That(newer <= same, Is.True);
            That(newer == same, Is.True);
            That(newer != older, Is.True);
            That(newer.Equals((object)same), Is.True);
            That(newer.GetHashCode(), Is.EqualTo(same.GetHashCode()));
            That(newer.ToString(), Is.EqualTo("2.10.0"));
        });
    }
}
