using System.Globalization;

namespace Banned.Qbittorrent.Models.Requests;

public class AddTorrentRequest
{
    public List<string>? FilePaths              { get; set; }
    public List<string>? Urls                   { get; set; }
    public string?       SavePath               { get; set; } = "/download";
    public string?       Category               { get; set; }
    public string?       Tags                   { get; set; }
    public bool?         SkipChecking           { get; set; }
    public bool?         Paused                 { get; set; }
    public bool?         Stopped                { get; set; }
    public bool?         RootFolder             { get; set; }
    public string?       Rename                 { get; set; }
    public int?          UploadLimit            { get; set; }
    public int?          DownloadLimit          { get; set; }
    public float?        RatioLimit             { get; set; }
    public int?          SeedingTimeLimit       { get; set; }
    public bool?         AutoTmm                { get; set; }
    public bool?         SequentialDownload     { get; set; }
    public bool?         FirstLastPiecePriority { get; set; }

    public Dictionary<string, string> ToDictionary()
    {
        var parameters = new Dictionary<string, string>();

        if (Urls is { Count: > 0 })
        {
            parameters["urls"] = string.Join("\n", Urls);
        }

        if (!string.IsNullOrEmpty(SavePath)) parameters["savepath"] = SavePath;
        if (!string.IsNullOrEmpty(Category)) parameters["category"] = Category;
        if (!string.IsNullOrEmpty(Tags)) parameters["tags"]         = Tags;
        if (SkipChecking.HasValue) parameters["skip_checking"]      = SkipChecking.Value.ToString().ToLower();

        if (Stopped.HasValue)
        {
            parameters["paused"]  = Stopped.Value.ToString().ToLower();
            parameters["stopped"] = Stopped.Value.ToString().ToLower();
        }
        else if (Paused.HasValue)
        {
            parameters["paused"]  = Paused.Value.ToString().ToLower();
            parameters["stopped"] = Paused.Value.ToString().ToLower();
        }

        if (RootFolder.HasValue) parameters["root_folder"] = RootFolder.Value.ToString().ToLower();
        if (!string.IsNullOrEmpty(Rename)) parameters["rename"] = Rename;
        if (UploadLimit.HasValue) parameters["upLimit"] = UploadLimit.Value.ToString();
        if (DownloadLimit.HasValue) parameters["dlLimit"] = DownloadLimit.Value.ToString();
        if (RatioLimit.HasValue) parameters["ratioLimit"] = RatioLimit.Value.ToString(CultureInfo.InvariantCulture);
        if (SeedingTimeLimit.HasValue) parameters["seedingTimeLimit"] = SeedingTimeLimit.Value.ToString();
        if (AutoTmm.HasValue) parameters["autoTMM"] = AutoTmm.Value.ToString().ToLower();
        if (SequentialDownload.HasValue)
            parameters["sequentialDownload"] = SequentialDownload.Value.ToString().ToLower();
        if (FirstLastPiecePriority.HasValue)
            parameters["firstLastPiecePrio"] = FirstLastPiecePriority.Value.ToString().ToLower();

        return parameters;
    }

    public override string ToString()
    {
        return string.Join("&", ToDictionary().Select(p => $"{p.Key}={Uri.EscapeDataString(p.Value)}"));
    }
}
