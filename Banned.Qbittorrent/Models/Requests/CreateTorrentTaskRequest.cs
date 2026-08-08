using Banned.Qbittorrent.Models.Enums;
using System.Globalization;

namespace Banned.Qbittorrent.Models.Requests;

/// <summary>
/// 创建种子任务的请求参数。<br/>
/// Request parameters for a torrent creation task.
/// </summary>
public class CreateTorrentTaskRequest
{
    /// <summary>作为种子内容来源的服务器端路径。<br/>Server-side source path for the torrent content.</summary>
    public required string SourcePath { get; set; }

    /// <summary>保存生成的 .torrent 文件的服务器端路径。<br/>Server-side path for the generated .torrent file.</summary>
    public string? TorrentFilePath { get; set; }

    /// <summary>种子格式；为空时由服务器使用默认值。<br/>Torrent format; the server default is used when null.</summary>
    public EnumTorrentCreatorFormat? Format { get; set; }

    /// <summary>创建完成后是否开始做种。<br/>Whether to start seeding after creation.</summary>
    public bool? StartSeeding { get; set; }

    /// <summary>是否创建私有种子。<br/>Whether to create a private torrent.</summary>
    public bool? IsPrivate { get; set; }

    /// <summary>是否优化文件对齐。<br/>Whether to optimize file alignment.</summary>
    public bool? OptimizeAlignment { get; set; }

    /// <summary>填充文件大小限制。<br/>Padding file size limit.</summary>
    public long? PaddedFileSizeLimit { get; set; }

    /// <summary>分片大小。<br/>Piece size.</summary>
    public long? PieceSize { get; set; }

    /// <summary>种子备注。<br/>Torrent comment.</summary>
    public string? Comment { get; set; }

    /// <summary>Tracker URL 列表。<br/>Tracker URL list.</summary>
    public List<string>? Trackers { get; set; }

    /// <summary>Web 种子 URL 列表。<br/>Web seed URL list.</summary>
    public List<string>? UrlSeeds { get; set; }

    /// <summary>将请求转换为 Web API 表单参数。<br/>Converts the request to Web API form parameters.</summary>
    /// <returns>表单参数。<br/>Form parameters.</returns>
    public Dictionary<string, string> ToDictionary()
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(SourcePath);
        var parameters = new Dictionary<string, string> { { "sourcePath", SourcePath } };

        if (!string.IsNullOrEmpty(TorrentFilePath)) parameters["torrentFilePath"] = TorrentFilePath;
        if (Format.HasValue)
            parameters["format"] = Format.Value switch
            {
                EnumTorrentCreatorFormat.V1     => "v1",
                EnumTorrentCreatorFormat.V2     => "v2",
                EnumTorrentCreatorFormat.Hybrid => "hybrid",
                _                               => throw new ArgumentOutOfRangeException(nameof(Format))
            };
        if (StartSeeding.HasValue) parameters["startSeeding"] = ToBooleanString(StartSeeding.Value);
        if (IsPrivate.HasValue) parameters["private"] = ToBooleanString(IsPrivate.Value);
        if (OptimizeAlignment.HasValue) parameters["optimizeAlignment"] = ToBooleanString(OptimizeAlignment.Value);
        if (PaddedFileSizeLimit.HasValue)
            parameters["paddedFileSizeLimit"] = PaddedFileSizeLimit.Value.ToString(CultureInfo.InvariantCulture);
        if (PieceSize.HasValue) parameters["pieceSize"] = PieceSize.Value.ToString(CultureInfo.InvariantCulture);
        if (Comment != null) parameters["comment"] = Comment;
        if (Trackers != null) parameters["trackers"] = string.Join('|', Trackers);
        if (UrlSeeds != null) parameters["urlSeeds"] = string.Join('|', UrlSeeds);

        return parameters;
    }

    private static string ToBooleanString(bool value) => value ? "true" : "false";
}
