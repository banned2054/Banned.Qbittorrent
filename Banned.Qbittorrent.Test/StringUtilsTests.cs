using Banned.Qbittorrent.Models.Enums;
using Banned.Qbittorrent.Utils;
using static NUnit.Framework.Assert;

namespace Banned.Qbittorrent.Test;

[TestFixture]
public class StringUtilsTests
{
    [Test]
    public void NormalizeHash_RemovesBlankEntriesAndTrimsValues()
    {
        That(StringUtils.NormalizeHash(" abc | | def "), Is.EqualTo("abc|def"));
        That(StringUtils.NormalizeHash([" abc ", "", "def"]), Is.EqualTo("abc|def"));
    }

    [Test]
    public void NormalizeHash_RejectsInputWithoutHashes()
    {
        Throws<ArgumentException>(() => StringUtils.NormalizeHash("  |  "));
        Throws<ArgumentException>(() => StringUtils.NormalizeHash([]));
    }

    [Test]
    public void Join_HandlesNullStringsAndNonStringValues()
    {
        That(StringUtils.Join<string>(',', null), Is.Empty);
        That(StringUtils.Join(",", new int?[] { 1, null, 2 }), Is.EqualTo("1,2"));
    }

    [TestCase("connected", EnumConnectionStatus.Connected)]
    [TestCase("FIREWALLED", EnumConnectionStatus.Firewalled)]
    [TestCase("disconnected", EnumConnectionStatus.Disconnected)]
    [TestCase(null, EnumConnectionStatus.Unknown)]
    [TestCase("future", EnumConnectionStatus.Unknown)]
    public void String2ConnectionStatus_MapsKnownAndUnknownValues(
        string?              value,
        EnumConnectionStatus expected)
    {
        That(StringUtils.String2ConnectionStatus(value), Is.EqualTo(expected));
    }

    [TestCase(EnumStopCondition.Never, "Never")]
    [TestCase(EnumStopCondition.MetadataReceived, "MetadataReceived")]
    [TestCase(EnumStopCondition.TorrentAdded, "TorrentAdded")]
    public void StopCondition_RoundTripsKnownValues(EnumStopCondition value, string text)
    {
        That(value.StopCondition2String(), Is.EqualTo(text));
        That(StringUtils.String2StopCondition(text), Is.EqualTo(value));
    }

    [TestCase(EnumContentLayout.Original, "Original")]
    [TestCase(EnumContentLayout.Subfolder, "Subfolder")]
    [TestCase(EnumContentLayout.NoSubfolder, "NoSubfolder")]
    public void ContentLayout_RoundTripsKnownValues(EnumContentLayout value, string text)
    {
        That(value.ContentLayout2String(), Is.EqualTo(text));
        That(StringUtils.String2ContentLayout(text), Is.EqualTo(value));
    }

    [TestCase("Running", EnumSearchStatus.Running)]
    [TestCase("Stopped", EnumSearchStatus.Stopped)]
    [TestCase("running", EnumSearchStatus.Unknown)]
    [TestCase(null, EnumSearchStatus.Unknown)]
    public void String2SearchStatus_UsesApiCasing(string? text, EnumSearchStatus expected)
    {
        That(StringUtils.String2SearchStatus(text), Is.EqualTo(expected));
    }

    [TestCase(EnumTorrentFilter.All, "all")]
    [TestCase(EnumTorrentFilter.Downloading, "downloading")]
    [TestCase(EnumTorrentFilter.Seeding, "seeding")]
    [TestCase(EnumTorrentFilter.Completed, "completed")]
    [TestCase(EnumTorrentFilter.Paused, "paused")]
    [TestCase(EnumTorrentFilter.Active, "active")]
    [TestCase(EnumTorrentFilter.Inactive, "inactive")]
    [TestCase(EnumTorrentFilter.Resumed, "resumed")]
    [TestCase(EnumTorrentFilter.Stalled, "stalled")]
    [TestCase(EnumTorrentFilter.StalledUploading, "stalled_uploading")]
    [TestCase(EnumTorrentFilter.StalledDownloading, "stalled_downloading")]
    [TestCase(EnumTorrentFilter.Error, "errored")]
    public void TorrentFilter_RoundTripsKnownValues(EnumTorrentFilter value, string text)
    {
        That(value.TorrentFilter2String(), Is.EqualTo(text));
        That(StringUtils.String2TorrentFilter(text), Is.EqualTo(value));
    }

    [TestCase("pausedUP", EnumTorrentState.StoppedUpload, "pausedUP", "stoppedUP")]
    [TestCase("stoppedUP", EnumTorrentState.StoppedUpload, "pausedUP", "stoppedUP")]
    [TestCase("pausedDL", EnumTorrentState.StoppedDownload, "pausedDL", "stoppedDL")]
    [TestCase("stoppedDL", EnumTorrentState.StoppedDownload, "pausedDL", "stoppedDL")]
    [TestCase("downloading", EnumTorrentState.Downloading, "downloading", "downloading")]
    [TestCase("unknown", EnumTorrentState.Unknown, "unknown", "unknown")]
    public void TorrentState_SupportsV4AndV5Names(
        string           input,
        EnumTorrentState state,
        string           v4Name,
        string           v5Name)
    {
        That(StringUtils.String2TorrentState(input), Is.EqualTo(state));
        That(state.TorrentState2StringV4(), Is.EqualTo(v4Name));
        That(state.TorrentState2StringV5(), Is.EqualTo(v5Name));
    }

    [Test]
    public void UnknownEnumValues_UseDocumentedFallbacksOrThrow()
    {
        var unknownStopCondition = (EnumStopCondition)999;
        var unknownContentLayout = (EnumContentLayout)999;
        var unknownTorrentFilter = (EnumTorrentFilter)999;
        var unknownTorrentState  = (EnumTorrentState)999;

        Multiple(() =>
        {
            That(unknownStopCondition.StopCondition2String(), Is.EqualTo("Never"));
            That(unknownContentLayout.ContentLayout2String(), Is.EqualTo("Original"));
            Throws<ArgumentOutOfRangeException>(() => unknownTorrentFilter.TorrentFilter2String());
            Throws<ArgumentOutOfRangeException>(() => StringUtils.String2TorrentFilter("future"));
            Throws<ArgumentOutOfRangeException>(() => unknownTorrentState.TorrentState2StringV5());
            Throws<ArgumentOutOfRangeException>(() => StringUtils.String2TorrentState("future"));
        });
    }
}
