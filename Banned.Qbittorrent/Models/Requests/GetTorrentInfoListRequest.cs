using Banned.Qbittorrent.Models.Enums;

namespace Banned.Qbittorrent.Models.Requests;

public class GetTorrentInfoListRequest
{
    public EnumTorrentFilter Filter   { get; set; }
    public string?           Category { get; set; }
    public string?           Tag      { get; set; }
    public string?           Sort     { get; set; }
    public bool              Reverse  { get; set; }
    public int               Limit    { get; set; }
    public int               Offset   { get; set; }
    public List<string>?     HashList { get; set; } = [];

    public override string ToString()
    {
        var parameters = new List<string>();

        if (Filter != EnumTorrentFilter.All)
        {
            parameters.Add($"filter={Uri.EscapeDataString(Filter.ToTorrentFilterString())}");
        }

        if (!string.IsNullOrEmpty(Category))
        {
            parameters.Add($"category={Uri.EscapeDataString(Category)}");
        }

        if (!string.IsNullOrEmpty(Tag))
        {
            parameters.Add($"tag={Uri.EscapeDataString(Tag)}");
        }

        if (!string.IsNullOrEmpty(Sort))
        {
            parameters.Add($"sort={Uri.EscapeDataString(Sort)}");
        }

        if (Reverse) parameters.Add($"reverse={Reverse.ToString().ToLower()}");

        if (Limit > 0)
        {
            parameters.Add($"limit={Limit}");
        }

        if (Offset > 0)
        {
            parameters.Add($"offset={Offset}");
        }

        if (HashList == null || !HashList.Any()) return string.Join("&", parameters);
        var hashes = string.Join("|", HashList.Select(Uri.EscapeDataString));
        parameters.Add($"hashes={hashes}");

        return string.Join("&", parameters);
    }
}
