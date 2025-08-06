using Banned.Qbittorrent.Models.Enums;

namespace Banned.Qbittorrent.Models.Torrent;

public class TorrentInfo
{
    public DateTime         AddedOn                { get; set; }
    public long             AmountLeft             { get; set; }
    public bool             AutoTmm                { get; set; }
    public float            Availability           { get; set; }
    public string?          Category               { get; set; }
    public long             Completed              { get; set; }
    public DateTime         CompletionOn           { get; set; }
    public string?          ContentPath            { get; set; }
    public long             DlLimit                { get; set; }
    public long             DownloadSpeed          { get; set; }
    public long             Downloaded             { get; set; }
    public long             DownloadedSession      { get; set; }
    public TimeSpan         Eta                    { get; set; }
    public bool             FirstLastPiecePriority { get; set; }
    public bool             ForceStart             { get; set; }
    public string?          Hash                   { get; set; }
    public bool             IsPrivate              { get; set; }
    public DateTime         LastActivity           { get; set; }
    public string?          MagnetUri              { get; set; }
    public float            MaxRatio               { get; set; }
    public TimeSpan         MaxSeedingTime         { get; set; }
    public string?          Name                   { get; set; }
    public long             NumComplete            { get; set; }
    public long             NumIncomplete          { get; set; }
    public long             NumLeechs              { get; set; }
    public long             NumSeeds               { get; set; }
    public long             Priority               { get; set; }
    public float            Progress               { get; set; }
    public float            Ratio                  { get; set; }
    public float            RatioLimit             { get; set; }
    public string?          SavePath               { get; set; }
    public TimeSpan         SeedingTime            { get; set; }
    public TimeSpan         SeedingTimeLimit       { get; set; }
    public DateTime         SeenComplete           { get; set; }
    public bool             SeqDl                  { get; set; }
    public long             Size                   { get; set; }
    public EnumTorrentState State                  { get; set; } // 新增的枚举字段
    public bool             SuperSeeding           { get; set; }
    public List<string>     TagList                { get; set; } = [];
    public TimeSpan         TimeActive             { get; set; }
    public long             TotalSize              { get; set; }
    public string?          Tracker                { get; set; }
    public long             UpLimit                { get; set; }
    public long             Uploaded               { get; set; }
    public long             UploadedSession        { get; set; }
    public long             UploadSpeed            { get; set; }
}
