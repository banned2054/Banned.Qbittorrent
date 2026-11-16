using Banned.Qbittorrent.Models.Application;
using static NUnit.Framework.Assert;

namespace Banned.Qbittorrent.Test;

[TestFixture]
public class ApplicationVersionTests
{
    [TestCase("4.3", 4, 3, 0, 0)]
    [TestCase("v4.3.3", 4, 3, 3, 0)]
    [TestCase(" V5.4.0alpha1 ", 5, 4, 0, 0)]
    [TestCase("5.3.0beta1", 5, 3, 0, 0)]
    [TestCase("5.0.0-rc1", 5, 0, 0, 0)]
    [TestCase("4.3.4.1", 4, 3, 4, 1)]
    [TestCase("v4.3.3.0", 4, 3, 3, 0)]
    [TestCase("004.003.003", 4, 3, 3, 0)]
    public void TryParse_ValidVersion_ReturnsNumericComponents(
        string value, int major, int minor, int patch, int revision)
    {
        var parsed = ApplicationVersion.TryParse(value, out var version);

        Multiple(() =>
        {
            That(parsed, Is.True);
            That(version.Major, Is.EqualTo(major));
            That(version.Minor, Is.EqualTo(minor));
            That(version.Patch, Is.EqualTo(patch));
            That(version.Revision, Is.EqualTo(revision));
            That(ApplicationVersion.Parse(value), Is.EqualTo(version));
            That(version.ToString(), Is.EqualTo(value));
            That(ApplicationVersion.Parse(value).ToString(), Is.EqualTo(value));
        });
    }

    [TestCase("")]
    [TestCase(" ")]
    [TestCase("v")]
    [TestCase("4")]
    [TestCase("invalid")]
    [TestCase("4.x")]
    [TestCase("4..3")]
    [TestCase("4.3.3.1.2")]
    [TestCase("2147483648.3.3")]
    [TestCase("4.2147483648.3")]
    public void TryParse_InvalidVersion_ReturnsFalse(string value)
    {
        That(ApplicationVersion.TryParse(value, out var version), Is.False);
        That(version, Is.EqualTo(default(ApplicationVersion)));
    }

    [Test]
    public void Parse_InvalidInput_ThrowsMeaningfulException()
    {
        Throws<ArgumentNullException>(() => ApplicationVersion.Parse(null!));
        Throws<FormatException>(() => ApplicationVersion.Parse("invalid"));
    }

    [TestCase("4.3.2", "4.3.3")]
    [TestCase("4.9.9", "4.10.0")]
    [TestCase("4.9.9", "5.0.0")]
    [TestCase("4.3.4", "4.3.4.1")]
    public void ComparisonAndEquality_UseAllNumericComponents(string olderValue, string newerValue)
    {
        var older = ApplicationVersion.Parse(olderValue);
        var newer = ApplicationVersion.Parse(newerValue);
        var same  = new ApplicationVersion(newer.Major, newer.Minor, newer.Patch, newer.Revision);

        Multiple(() =>
        {
            That(older < newer, Is.True);
            That(newer > older, Is.True);
            That(newer >= same, Is.True);
            That(newer <= same, Is.True);
            That(newer == same, Is.True);
            That(newer != older, Is.True);
            That(newer.Equals((object)same), Is.True);
            That(newer.Equals(null), Is.False);
            That(newer.GetHashCode(), Is.EqualTo(same.GetHashCode()));
            That(newer.ToString(), Is.EqualTo(newerValue));
        });
    }

    [Test]
    public void KnownVersion_AndPrereleaseSuffixes_UseNumericComparison()
    {
        Multiple(() =>
        {
            That(ApplicationVersion.Parse("v4.3.3"), Is.EqualTo(ApplicationVersion.V4_3_3));
            That(ApplicationVersion.Parse("4.3.3.0"), Is.EqualTo(ApplicationVersion.V4_3_3));
            That(ApplicationVersion.Parse("5.4.0alpha1"), Is.EqualTo(new ApplicationVersion(5, 4)));
            That(ApplicationVersion.Parse("v4.3.3").GetHashCode(), Is.EqualTo(ApplicationVersion.V4_3_3.GetHashCode()));
            That(ApplicationVersion.V4_3_3.ToString(), Is.EqualTo("4.3.3"));
            That(new ApplicationVersion(4, 3, 4, 1).ToString(), Is.EqualTo("4.3.4.1"));
            That(default(ApplicationVersion).ToString(), Is.EqualTo("0.0.0"));
        });
    }
}
