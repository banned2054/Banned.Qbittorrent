using System.Text.Json.Serialization;

namespace Banned.Qbittorrent.Models.Application;

[JsonConverter(typeof(ScanDirDestinationConverter))]
public sealed class ScanDirDestination
{
    public int?    Mode       { get; set; } // 0 or 1 if numeric
    public string? CustomPath { get; set; }

    public bool IsCustomPath => CustomPath != null;

    public override string ToString() => IsCustomPath ? CustomPath! : Mode?.ToString() ?? "null";
}
