using Banned.Qbittorrent.Models.Enums;
using Banned.Qbittorrent.Serialization;
using System.Text.Json;
using System.Text.Json.Serialization;
using static NUnit.Framework.Assert;

namespace Banned.Qbittorrent.Test;

[TestFixture]
public class SerializationConverterTests
{
    [Test]
    public void CommaSeparatedList_ReadsTrimmedValuesAndIgnoresEmptyEntries()
    {
        var options = Options(new CommaSeparatedListConverter());

        var result    = JsonSerializer.Deserialize<List<string>>("\"linux, iso, ,release\"", options);
        var nonString = JsonSerializer.Deserialize<List<string>>("123", options);

        Multiple(() =>
        {
            That(result, Is.EqualTo(["linux", "iso", "release"]));
            That(nonString, Is.Empty);
        });
    }

    [Test]
    public void CommaSeparatedList_WritesApiString()
    {
        var options = Options(new CommaSeparatedListConverter());

        Multiple(() =>
        {
            That(JsonSerializer.Serialize(new List<string> { "linux", "iso" }, options),
                 Is.EqualTo("\"linux,iso\""));
            That(JsonSerializer.Serialize(new List<string>(), options), Is.EqualTo("\"\""));
        });
    }

    [Test]
    public void UnixTimestamp_RoundTripsSecondsAndHandlesNull()
    {
        var options  = Options(new UnixTimestampConverter());
        var expected = DateTimeOffset.FromUnixTimeSeconds(1_700_000_000);

        var parsed = JsonSerializer.Deserialize<DateTimeOffset?>("1700000000", options);

        Multiple(() =>
        {
            That(parsed, Is.EqualTo(expected));
            That(JsonSerializer.Serialize<DateTimeOffset?>(expected, options), Is.EqualTo("1700000000"));
            That(JsonSerializer.Deserialize<DateTimeOffset?>("null", options), Is.Null);
            That(JsonSerializer.Serialize<DateTimeOffset?>(null, options), Is.EqualTo("null"));
        });
    }

    [TestCase(false, "90", 90d)]
    [TestCase(true, "90", 5400d)]
    public void TimeSpan_ReadsConfiguredUnit(bool minutes, string json, double expectedSeconds)
    {
        var options = Options(minutes ? new MinutesTimeSpanConverter() : new SecondsTimeSpanConverter());

        var result = JsonSerializer.Deserialize<TimeSpan?>(json, options);

        That(result, Is.EqualTo(TimeSpan.FromSeconds(expectedSeconds)));
    }

    [TestCase("-1")]
    [TestCase("8640000")]
    public void TimeSpan_ReadsUnlimitedSentinels(string json)
    {
        var options = Options(new SecondsTimeSpanConverter());

        That(JsonSerializer.Deserialize<TimeSpan?>(json, options), Is.EqualTo(TimeSpan.MaxValue));
        That(JsonSerializer.Serialize<TimeSpan?>(TimeSpan.MaxValue, options), Is.EqualTo("8640000"));
    }

    [Test]
    public void TimeSpan_WritesConfiguredUnitAndNull()
    {
        var seconds = Options(new SecondsTimeSpanConverter());
        var minutes = Options(new MinutesTimeSpanConverter());

        Multiple(() =>
        {
            That(JsonSerializer.Serialize<TimeSpan?>(TimeSpan.FromSeconds(90), seconds), Is.EqualTo("90"));
            That(JsonSerializer.Serialize<TimeSpan?>(TimeSpan.FromMinutes(90), minutes), Is.EqualTo("90"));
            That(JsonSerializer.Serialize<TimeSpan?>(null, seconds), Is.EqualTo("null"));
            That(JsonSerializer.Deserialize<TimeSpan?>("null", seconds), Is.Null);
        });
    }

    [TestCase("pausedUP", EnumTorrentState.StoppedUpload)]
    [TestCase("stoppedUP", EnumTorrentState.StoppedUpload)]
    [TestCase("pausedDL", EnumTorrentState.StoppedDownload)]
    [TestCase("stoppedDL", EnumTorrentState.StoppedDownload)]
    [TestCase("", EnumTorrentState.Unknown)]
    public void TorrentState_ReadsV4V5AndEmptyValues(string jsonValue, EnumTorrentState expected)
    {
        var options = Options(new TorrentStateConverter());
        var json    = JsonSerializer.Serialize(jsonValue);

        That(JsonSerializer.Deserialize<EnumTorrentState>(json, options), Is.EqualTo(expected));
    }

    [Test]
    public void TorrentState_WritesLatestV5Name()
    {
        var options = Options(new TorrentStateConverter());

        Multiple(() =>
        {
            That(JsonSerializer.Serialize(EnumTorrentState.StoppedUpload, options), Is.EqualTo("\"stoppedUP\""));
            That(JsonSerializer.Serialize(EnumTorrentState.StoppedDownload, options), Is.EqualTo("\"stoppedDL\""));
        });
    }

    private static JsonSerializerOptions Options(JsonConverter converter)
    {
        var options = new JsonSerializerOptions();
        options.Converters.Add(converter);
        return options;
    }
}
