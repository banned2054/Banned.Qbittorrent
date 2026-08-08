using Banned.Qbittorrent.Exceptions;
using Banned.Qbittorrent.Models.Application;
using Banned.Qbittorrent.Models.Enums;
using Banned.Qbittorrent.Models.Requests;
using Banned.Qbittorrent.Models.Torrent;
using Banned.Qbittorrent.Serialization;
using Banned.Qbittorrent.Utils;
using System.Globalization;

namespace Banned.Qbittorrent.Services;

/// <summary>
/// 提供与 qBittorrent 种子相关的服务<br/>
/// Provides services related to qBittorrent torrents
/// </summary>
public class TorrentService(NetService netService, ApiVersion apiVersion)
{
    private const           string   BaseUrl                = "/api/v2/torrents";
    private static readonly Version  PathBasedRenameVersion = new(4, 3, 3);
    private readonly        Version? _applicationVersion;

    internal TorrentService(NetService netService, ApiVersion apiVersion, Version applicationVersion) :
        this(netService, apiVersion)
    {
        _applicationVersion = applicationVersion;
    }

    /// <summary>
    /// 获取单个种子信息列表。<br/>
    /// Get torrent information list.
    /// </summary>
    /// <param name="hash">单个种子的哈希值。<br/>Hash value of a single torrent.</param>
    /// <param name="filter">种子过滤条件。<br/>Torrent filter condition.</param>
    /// <param name="category">分类名称。<br/>Category name.</param>
    /// <param name="tag">标签名称。<br/>Tag name.</param>
    /// <param name="sort">排序字段。<br/>Sort field.</param>
    /// <param name="reverse">是否反向排序。<br/>Whether to sort in reverse order.</param>
    /// <param name="limit">返回结果数量限制。<br/>Limit on the number of results returned.</param>
    /// <param name="offset">结果偏移量。<br/>Result offset.</param>
    /// <param name="cancellationToken">取消请求的令牌。<br/>Token used to cancel the request.</param>
    /// <returns>
    /// 种子信息列表。<br/>
    /// List of torrent information.
    /// </returns>
    public async Task<TorrentInfo?> GetTorrentInfo(string  hash, EnumTorrentFilter filter = EnumTorrentFilter.All,
                                                   string? category = null,
                                                   string? tag      = null,
                                                   string? sort     = null,
                                                   bool    reverse  = false,
                                                   int     limit    = 0,
                                                   int     offset   = 0, CancellationToken cancellationToken = default)
    {
        hash = StringUtils.NormalizeHash(hash);
        var result = await GetTorrentInfos([hash], filter, category, tag, sort, reverse, limit, offset,
                                           cancellationToken);
        return result.FirstOrDefault();
    }

    /// <summary>
    /// 获取种子信息列表。<br/>
    /// Get torrent information list.
    /// </summary>
    /// <param name="hashes">种子哈希值列表。<br/>List of torrent hash values.</param>
    /// <param name="filter">种子过滤条件。<br/>Torrent filter condition.</param>
    /// <param name="category">分类名称。<br/>Category name.</param>
    /// <param name="tag">标签名称。<br/>Tag name.</param>
    /// <param name="sort">排序字段。<br/>Sort field.</param>
    /// <param name="reverse">是否反向排序。<br/>Whether to sort in reverse order.</param>
    /// <param name="limit">返回结果数量限制。<br/>Limit on the number of results returned.</param>
    /// <param name="offset">结果偏移量。<br/>Result offset.</param>
    /// <param name="cancellationToken">取消请求的令牌。<br/>Token used to cancel the request.</param>
    /// <returns>
    /// 种子信息列表。<br/>
    /// List of torrent information.
    /// </returns>
    public async Task<List<TorrentInfo>> GetTorrentInfos(List<string>?     hashes            = null,
                                                         EnumTorrentFilter filter            = EnumTorrentFilter.All,
                                                         string?           category          = null,
                                                         string?           tag               = null,
                                                         string?           sort              = null,
                                                         bool              reverse           = false,
                                                         int               limit             = 0,
                                                         int               offset            = 0,
                                                         CancellationToken cancellationToken = default)
    {
        var parameters = new Dictionary<string, string>();

        if (filter != EnumTorrentFilter.All)
        {
            parameters.Add("filter", filter.TorrentFilter2String(apiVersion));
        }

        if (!string.IsNullOrEmpty(category)) parameters.Add("category", category);
        if (!string.IsNullOrEmpty(tag)) parameters.Add("tag", tag);
        if (!string.IsNullOrEmpty(sort)) parameters.Add("sort", sort);
        if (reverse) parameters.Add("reverse", "true");
        if (limit  > 0) parameters.Add("limit", limit.ToString());
        if (offset > 0) parameters.Add("offset", offset.ToString());
        if (hashes is { Count: > 0 }) parameters.Add("hashes", StringUtils.NormalizeHash(hashes));

        var response = await netService.Post($"{BaseUrl}/info", parameters, ct : cancellationToken);
        return QBittorrentJsonSerializer.Deserialize<List<TorrentInfo>>(response) ?? [];
    }

    /// <summary>
    /// 使用请求对象获取种子信息列表。<br/>
    /// Get torrent information list using a request object.
    /// </summary>
    /// <param name="request">获取种子信息列表的请求参数。<br/>Request parameters for getting torrent information list.</param>
    /// <param name="cancellationToken">取消请求的令牌。<br/>Token used to cancel the request.</param>
    /// <returns>
    /// 种子信息列表。<br/>
    /// List of torrent information.
    /// </returns>
    public async Task<List<TorrentInfo>> GetTorrentInfos(GetTorrentInfoListRequest request,
                                                         CancellationToken         cancellationToken = default)
    {
        var parameters = new Dictionary<string, string>
            { { "filter", request.Filter.TorrentFilter2String(apiVersion) } };

        if (!string.IsNullOrEmpty(request.Category)) parameters.Add("category", request.Category);
        if (!string.IsNullOrEmpty(request.Tag)) parameters.Add("tag", request.Tag);
        if (!string.IsNullOrEmpty(request.Sort)) parameters.Add("sort", request.Sort);
        if (request.ReverseEnabled) parameters.Add("reverse", "true");
        if (request.Limit  > 0) parameters.Add("limit", request.Limit.ToString());
        if (request.Offset > 0) parameters.Add("offset", request.Offset.ToString());
        if (request.HashList is { Count: > 0 }) parameters.Add("hashes", StringUtils.NormalizeHash(request.HashList));
        var response = await netService.Post($"{BaseUrl}/info", parameters, ct : cancellationToken);
        return QBittorrentJsonSerializer.Deserialize<List<TorrentInfo>>(response) ?? [];
    }

    /// <summary>
    /// 获取当前种子数量。<br/>
    /// Gets the current torrent count.
    /// </summary>
    /// <param name="cancellationToken">取消请求的令牌。<br/>Token used to cancel the request.</param>
    /// <returns>种子数量。<br/>The torrent count.</returns>
    public async Task<int> GetTorrentCount(CancellationToken cancellationToken = default)
    {
        var response =
            await netService.Post($"{BaseUrl}/count", targetVersion : ApiVersion.V2_9_3, ct : cancellationToken);
        return int.Parse(response, CultureInfo.InvariantCulture);
    }

    /// <summary>
    /// 获取指定种子的通用属性。<br/>
    /// Get generic properties of the specified torrent.
    /// </summary>
    /// <param name="hash">种子哈希值。<br/>Torrent hash value.</param>
    /// <param name="cancellationToken">取消请求的令牌。<br/>Token used to cancel the request.</param>
    /// <returns>
    /// 包含属性的 <see cref="TorrentProperties"/>；获取失败返回 <c>null</c>。<br/>
    /// A <see cref="TorrentProperties"/> if successful; otherwise <c>null</c>.
    /// </returns>
    public async Task<TorrentProperties?>
        GetTorrentGenericProperties(string hash, CancellationToken cancellationToken = default) =>
        QBittorrentJsonSerializer.Deserialize<TorrentProperties>(await Put("properties", hash,
                                                                           cancellationToken : cancellationToken));

    /// <summary>
    /// 获取指定种子的 Tracker 信息。<br/>
    /// Get tracker information for the specified torrent.
    /// </summary>
    /// <param name="hash">种子哈希值。<br/>Torrent hash value.</param>
    /// <param name="cancellationToken">取消请求的令牌。<br/>Token used to cancel the request.</param>
    /// <returns>
    /// 包含 Tracker 信息的列表；无数据返回空列表。<br/>
    /// A list of <see cref="TrackerInfo"/>; an empty list if no data is available.
    /// </returns>
    public async Task<List<TrackerInfo>?>
        GetTorrentTrackers(string hash, CancellationToken cancellationToken = default) =>
        QBittorrentJsonSerializer.Deserialize<List<TrackerInfo>>(await Put("trackers", hash,
                                                                           cancellationToken : cancellationToken));

    /// <summary>
    /// 获取指定种子的 Web 种子列表。<br/>
    /// Get the list of web seeds for the specified torrent.
    /// </summary>
    /// <param name="hash">种子哈希值。<br/>Torrent hash value.</param>
    /// <param name="cancellationToken">取消请求的令牌。<br/>Token used to cancel the request.</param>
    /// <returns>
    /// 包含 Web 种子信息的列表；获取失败返回 <c>null</c>。<br/>
    /// A list of <see cref="TorrentWebSeed"/> representing web seed information; <c>null</c> if retrieval fails.
    /// </returns>
    public async Task<List<TorrentWebSeed>?>
        GetTorrentWebSeeds(string hash, CancellationToken cancellationToken = default) =>
        QBittorrentJsonSerializer.Deserialize<List<TorrentWebSeed>>(await Put("webseeds", hash,
                                                                              cancellationToken : cancellationToken));

    /// <summary>
    /// 向指定种子添加 Web 种子。<br/>
    /// Adds web seeds to the specified torrent.
    /// </summary>
    /// <param name="hash">种子哈希值。<br/>Torrent hash value.</param>
    /// <param name="urls">Web 种子 URL 列表。<br/>Web seed URL list.</param>
    /// <param name="cancellationToken">取消请求的令牌。<br/>Token used to cancel the request.</param>
    public async Task AddTorrentWebSeeds(string hash, List<string> urls, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(urls);
        if (urls.Count == 0) throw new ArgumentException("At least one web seed URL is required.", nameof(urls));

        var parameters = new Dictionary<string, string>
        {
            { "hash", StringUtils.NormalizeHash(hash) },
            { "urls", StringUtils.Join('|', urls) }
        };
        await netService.Post($"{BaseUrl}/addWebSeeds", parameters, ApiVersion.V2_11_3, ct : cancellationToken);
    }

    /// <summary>
    /// 编辑指定种子的 Web 种子。<br/>
    /// Edits a web seed for the specified torrent.
    /// </summary>
    /// <param name="hash">种子哈希值。<br/>Torrent hash value.</param>
    /// <param name="originalUrl">要替换的 Web 种子 URL。<br/>Web seed URL to replace.</param>
    /// <param name="newUrl">新的 Web 种子 URL。<br/>New web seed URL.</param>
    /// <param name="cancellationToken">取消请求的令牌。<br/>Token used to cancel the request.</param>
    public async Task EditTorrentWebSeed(string hash,
                                         string originalUrl,
                                         string newUrl, CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(originalUrl);
        ArgumentException.ThrowIfNullOrWhiteSpace(newUrl);

        var parameters = new Dictionary<string, string>
        {
            { "hash", StringUtils.NormalizeHash(hash) },
            { "origUrl", originalUrl },
            { "newUrl", newUrl }
        };
        await netService.Post($"{BaseUrl}/editWebSeed", parameters, ApiVersion.V2_11_3, ct : cancellationToken);
    }

    /// <summary>
    /// 从指定种子移除 Web 种子。<br/>
    /// Removes web seeds from the specified torrent.
    /// </summary>
    /// <param name="hash">种子哈希值。<br/>Torrent hash value.</param>
    /// <param name="urls">要移除的 Web 种子 URL 列表。<br/>Web seed URL list to remove.</param>
    /// <param name="cancellationToken">取消请求的令牌。<br/>Token used to cancel the request.</param>
    public async Task RemoveTorrentWebSeeds(string       hash,
                                            List<string> urls, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(urls);
        if (urls.Count == 0) throw new ArgumentException("At least one web seed URL is required.", nameof(urls));

        var parameters = new Dictionary<string, string>
        {
            { "hash", StringUtils.NormalizeHash(hash) },
            { "urls", StringUtils.Join('|', urls) }
        };
        await netService.Post($"{BaseUrl}/removeWebSeeds", parameters, ApiVersion.V2_11_3, ct : cancellationToken);
    }

    /// <summary>
    /// 获取种子的文件列表。<br/>
    /// Get the file list of a torrent.
    /// </summary>
    /// <param name="hash">种子哈希值。<br/>Torrent hash value.</param>
    /// <param name="indexes">文件索引列表。<br/>List of file indexes.</param>
    /// <param name="cancellationToken">取消请求的令牌。<br/>Token used to cancel the request.</param>
    /// <returns>
    /// 种子文件信息列表。<br/>
    /// List of torrent file information.
    /// </returns>
    public async Task<List<TorrentFileInfo>?> GetTorrentFiles(string            hash,
                                                              List<int>?        indexes           = null,
                                                              CancellationToken cancellationToken = default)
    {
        hash = StringUtils.NormalizeHash(hash);
        const string requestUrl = $"{BaseUrl}/files";
        var parameters = new Dictionary<string, string>
        {
            { "hash", hash }
        };

        if (indexes is { Count: > 0 })
        {
            parameters["indexes"] = StringUtils.Join('|', indexes);
        }

        return QBittorrentJsonSerializer.Deserialize<List<TorrentFileInfo>>(await netService.Post(requestUrl,
                                                                                     parameters,
                                                                                     ct : cancellationToken));
    }

    /// <summary>
    /// 获取指定种子的每个分片状态。<br/>
    /// Get the state of each piece in the specified torrent.
    /// </summary>
    /// <param name="hash">种子哈希值。<br/>Torrent hash value.</param>
    /// <param name="cancellationToken">取消请求的令牌。<br/>Token used to cancel the request.</param>
    /// <returns>
    /// 分片状态列表，包含每个分片的下载状态；获取失败返回 <c>null</c>。<br/>
    /// A list of <see cref="EnumPieceState"/> representing the state of each piece; <c>null</c> if retrieval fails.
    /// </returns>
    public async Task<List<EnumPieceState>?> GetTorrentPiecesStates(string            hash,
                                                                    CancellationToken cancellationToken = default) =>
        QBittorrentJsonSerializer.Deserialize<List<EnumPieceState>>(await Put("pieceStates", hash,
                                                                              cancellationToken : cancellationToken));

    /// <summary>
    /// 获取指定种子的每个分片哈希值。<br/>
    /// Get the hash of each piece in the specified torrent.
    /// </summary>
    /// <param name="hash">种子哈希值。<br/>Torrent hash value.</param>
    /// <param name="cancellationToken">取消请求的令牌。<br/>Token used to cancel the request.</param>
    /// <returns>
    /// 分片哈希值列表；获取失败返回 <c>null</c>。<br/>
    /// A list of piece hash strings; <c>null</c> if retrieval fails.
    /// </returns>
    public async Task<List<string>?>
        GetTorrentPiecesHashes(string hash, CancellationToken cancellationToken = default) =>
        QBittorrentJsonSerializer.Deserialize<List<string>>(await Put("pieceHashes", hash,
                                                                      cancellationToken : cancellationToken));

    /// <summary>
    /// 导出指定种子的 .torrent 文件。<br/>
    /// Exports the .torrent file for the specified torrent.
    /// </summary>
    /// <param name="hash">种子哈希值。<br/>Torrent hash value.</param>
    /// <param name="cancellationToken">取消请求的令牌。<br/>Token used to cancel the request.</param>
    /// <returns>.torrent 文件内容。<br/>The .torrent file content.</returns>
    public async Task<byte[]> ExportTorrent(string hash, CancellationToken cancellationToken = default)
    {
        var parameters = new Dictionary<string, string> { { "hash", StringUtils.NormalizeHash(hash) } };
        return await netService.PostBytes($"{BaseUrl}/export", parameters, ApiVersion.V2_8_14,
                                          ct : cancellationToken);
    }

    /// <summary>
    /// 暂停指定种子。<br/>
    /// Pause the specified torrent.
    /// </summary>
    /// <param name="hash">种子哈希值。<br/>Torrent hash value.</param>
    /// <param name="cancellationToken">取消请求的令牌。<br/>Token used to cancel the request.</param>
    public async Task PauseTorrent(string hash, CancellationToken cancellationToken = default) =>
        await PutHashes(apiVersion < ApiVersion.V2_11_0 ? "pause" : "stop", hash,
                        cancellationToken : cancellationToken);

    /// <summary>
    /// 暂停多个种子。<br/>
    /// Pause multiple torrents.
    /// </summary>
    /// <param name="hashes">种子哈希值列表。<br/>List of torrent hash values.</param>
    /// <param name="cancellationToken">取消请求的令牌。<br/>Token used to cancel the request.</param>
    public async Task PauseTorrents(List<string> hashes, CancellationToken cancellationToken = default) =>
        await PauseTorrent(StringUtils.Join('|', hashes), cancellationToken);

    /// <summary>
    /// 继续下载/做种指定种子。<br/>
    /// Resume the specified torrent.
    /// </summary>
    /// <param name="hash">种子哈希值。<br/>Torrent hash value.</param>
    /// <param name="cancellationToken">取消请求的令牌。<br/>Token used to cancel the request.</param>
    public async Task ResumeTorrent(string hash, CancellationToken cancellationToken = default) =>
        await PutHashes(apiVersion < ApiVersion.V2_11_0 ? "resume" : "start", hash,
                        cancellationToken : cancellationToken);

    /// <summary>
    /// 继续下载/做种多个种子。<br/>
    /// Resume multiple torrents.
    /// </summary>
    /// <param name="hashes">种子哈希值列表。<br/>List of torrent hash values.</param>
    /// <param name="cancellationToken">取消请求的令牌。<br/>Token used to cancel the request.</param>
    public async Task ResumeTorrents(List<string> hashes, CancellationToken cancellationToken = default) =>
        await ResumeTorrent(StringUtils.Join('|', hashes), cancellationToken);

    /// <summary>
    /// 删除指定种子。<br/>
    /// Delete the specified torrent.
    /// </summary>
    /// <param name="hash">种子哈希值。<br/>Torrent hash value.</param>
    /// <param name="deleteFile">是否同时删除文件。<br/>Whether to delete files as well.</param>
    /// <param name="cancellationToken">取消请求的令牌。<br/>Token used to cancel the request.</param>
    public async Task DeleteTorrent(string hash, bool deleteFile = false, CancellationToken cancellationToken = default)
    {
        hash = StringUtils.NormalizeHash(hash);
        var parameters = new Dictionary<string, string>
        {
            { "hashes", hash },
            { "deleteFiles", deleteFile.ToString().ToLower() }
        };

        await netService.Post($"{BaseUrl}/delete", parameters, ct : cancellationToken);
    }

    /// <summary>
    /// 删除多个指定种子。<br/>
    /// Delete multiple specified torrents.
    /// </summary>
    /// <param name="hashes">种子哈希值列表。<br/>List of torrent hash values.</param>
    /// <param name="deleteFile">是否同时删除文件。<br/>Whether to delete files as well.</param>
    /// <param name="cancellationToken">取消请求的令牌。<br/>Token used to cancel the request.</param>
    public async Task DeleteTorrents(List<string> hashes,
                                     bool         deleteFile = false, CancellationToken cancellationToken = default) =>
        await DeleteTorrent(StringUtils.Join('|', hashes), deleteFile, cancellationToken);

    /// <summary>
    /// 重新校验指定种子的进度。<br/>
    /// Recheck the specified torrent's progress.
    /// </summary>
    /// <param name="hash">种子哈希值。<br/>Torrent hash value.</param>
    /// <param name="cancellationToken">取消请求的令牌。<br/>Token used to cancel the request.</param>
    public async Task RecheckTorrent(string hash, CancellationToken cancellationToken = default) =>
        await PutHashes("recheck", hash, cancellationToken : cancellationToken);

    /// <summary>
    /// 重新校验多个种子的进度。<br/>
    /// Recheck progress for multiple torrents.
    /// </summary>
    /// <param name="hashes">种子哈希值列表。<br/>List of torrent hash values.</param>
    /// <param name="cancellationToken">取消请求的令牌。<br/>Token used to cancel the request.</param>
    public async Task RecheckTorrents(List<string> hashes, CancellationToken cancellationToken = default) =>
        await RecheckTorrent(StringUtils.Join('|', hashes), cancellationToken);

    /// <summary>
    /// 重新向 Tracker 汇报指定种子。<br/>
    /// Reannounce the specified torrent to the tracker.
    /// </summary>
    /// <param name="hash">种子哈希值。<br/>Torrent hash value.</param>
    /// <param name="cancellationToken">取消请求的令牌。<br/>Token used to cancel the request.</param>
    public async Task ReannounceTorrent(string hash, CancellationToken cancellationToken = default) =>
        await PutHashes("reannounce", hash, ApiVersion.V2_0_2, cancellationToken);

    /// <summary>
    /// 重新向 Tracker 汇报多个种子。<br/>
    /// Reannounce multiple torrents to the tracker.
    /// </summary>
    /// <param name="hashes">种子哈希值列表。<br/>List of torrent hash values.</param>
    /// <param name="cancellationToken">取消请求的令牌。<br/>Token used to cancel the request.</param>
    public async Task ReannounceTorrents(List<string> hashes, CancellationToken cancellationToken = default) =>
        await ReannounceTorrent(StringUtils.Join('|', hashes), cancellationToken);

    /// <summary>
    /// 编辑指定种子的 Tracker。<br/>
    /// Edit the tracker of the specified torrent.
    /// </summary>
    /// <param name="hash">种子哈希值。<br/>Torrent hash value.</param>
    /// <param name="originUrl">原始 Tracker 地址。<br/>Original tracker URL.</param>
    /// <param name="newUrl">新的 Tracker 地址。<br/>New tracker URL.</param>
    /// <param name="cancellationToken">取消请求的令牌。<br/>Token used to cancel the request.</param>
    /// <exception cref="QbittorrentConflictException">
    /// 当新 URL 已存在或原始 URL 未找到时抛出。<br/>
    /// Thrown when the new URL already exists or the original URL is not found.
    /// </exception>
    /// <exception cref="QbittorrentBadRequestException">
    /// 当新 URL 格式无效时抛出。<br/>
    /// Thrown when the new URL is invalid.
    /// </exception>
    public async Task EditTorrentTracker(string hash,
                                         string originUrl,
                                         string newUrl, CancellationToken cancellationToken = default)
    {
        hash = StringUtils.NormalizeHash(hash);
        var parameters = new Dictionary<string, string>
        {
            { "hash", hash },
            { "origUrl", originUrl },
            { "newUrl", newUrl },
        };
        try
        {
            await netService.Post($"{BaseUrl}/editTracker", parameters, ApiVersion.V2_2_0, ct : cancellationToken);
        }
        catch (QbittorrentConflictException)
        {
            throw new QbittorrentConflictException("NewUrl already exists for the torrent. Or origUrl was not found.");
        }
        catch (QbittorrentBadRequestException)
        {
            throw new QbittorrentBadRequestException("NewUrl is not a valid URL");
        }
    }

    /// <summary>
    /// 删除指定种子的 Tracker。<br/>
    /// Remove tracker(s) from the specified torrent.
    /// </summary>
    /// <param name="hash">种子哈希值。<br/>Torrent hash value.</param>
    /// <param name="url">
    /// 要删除的 Tracker 地址<br/>
    /// Tracker URL to remove.
    /// </param>
    /// <param name="cancellationToken">取消请求的令牌。<br/>Token used to cancel the request.</param>
    /// <exception cref="QbittorrentConflictException">
    /// 当所有指定的 Tracker 地址均未找到时抛出。<br/>
    /// Thrown when all specified tracker URLs are not found.
    /// </exception>
    public async Task RemoveTorrentTracker(string hash, string url, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(url))
        {
            throw new ArgumentException("Tracker url cannot be null or empty", nameof(url));
        }

        hash = StringUtils.NormalizeHash(hash);
        var parameters = new Dictionary<string, string>
        {
            { "hash", hash },
            { "urls", url },
        };
        try
        {
            await netService.Post($"{BaseUrl}/removeTrackers", parameters, ApiVersion.V2_2_0, ct : cancellationToken);
        }
        catch (QbittorrentConflictException)
        {
            throw new QbittorrentConflictException("All urls were not found");
        }
    }

    /// <summary>
    /// 删除指定种子的多个 Tracker。<br/>
    /// Remove multiple trackers from the specified torrent.
    /// </summary>
    /// <param name="hash">种子哈希值。<br/>Torrent hash value.</param>
    /// <param name="urls">要删除的 Tracker 地址列表。<br/>List of tracker URLs to remove.</param>
    /// <param name="cancellationToken">取消请求的令牌。<br/>Token used to cancel the request.</param>
    public async Task RemoveTorrentTrackers(string       hash,
                                            List<string> urls, CancellationToken cancellationToken = default) =>
        await RemoveTorrentTracker(hash, StringUtils.Join('|', urls), cancellationToken);

    /// <summary>
    /// 向指定种子添加一个 Peer。<br/>
    /// Add a peer to the specified torrent.
    /// </summary>
    /// <param name="hash">种子哈希值。<br/>Torrent hash value.</param>
    /// <param name="peer">要添加的 Peer 地址（可为 IP:Port 格式）。<br/>Peer address to add (can be in IP:Port format).</param>
    /// <param name="cancellationToken">取消请求的令牌。<br/>Token used to cancel the request.</param>
    /// <exception cref="QbittorrentConflictException">
    /// 当所有指定的 Peer 地址均未找到或添加失败时抛出。<br/>
    /// Thrown when all specified peer addresses are not found or failed to add.
    /// </exception>
    public async Task AddTorrentPeer(string hash, string peer, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(peer))
        {
            throw new ArgumentException("Peer cannot be null or empty", nameof(peer));
        }

        hash = StringUtils.NormalizeHash(hash);
        var parameters = new Dictionary<string, string>
        {
            { "hash", hash },
            { "peers", peer },
        };
        try
        {
            await netService.Post($"{BaseUrl}/addPeers", parameters, ApiVersion.V2_3_0,
                                  ct : cancellationToken);
        }
        catch (QbittorrentConflictException)
        {
            throw new QbittorrentConflictException("All urls were not found");
        }
    }

    /// <summary>
    /// 向指定种子添加多个 Peer。<br/>
    /// Add multiple peers to the specified torrent.
    /// </summary>
    /// <param name="hash">种子哈希值。<br/>Torrent hash value.</param>
    /// <param name="peers">要添加的 Peer 地址列表。<br/>List of peer addresses to add.</param>
    /// <param name="cancellationToken">取消请求的令牌。<br/>Token used to cancel the request.</param>
    public async Task AddTorrentPeers(string hash, List<string> peers, CancellationToken cancellationToken = default) =>
        await AddTorrentPeer(hash, StringUtils.Join('|', peers), cancellationToken);

    /// <summary>
    /// 向多个种子添加一个 Peer。<br/>
    /// Add a peer to multiple torrents.
    /// </summary>
    /// <param name="hashes">种子哈希值列表。<br/>List of torrent hash values.</param>
    /// <param name="peer">要添加的 Peer 地址。<br/>Peer address to add.</param>
    /// <param name="cancellationToken">取消请求的令牌。<br/>Token used to cancel the request.</param>
    public async Task AddTorrentsPeer(List<string> hashes,
                                      string       peer, CancellationToken cancellationToken = default) =>
        await AddTorrentPeer(StringUtils.Join('|', hashes), peer, cancellationToken);

    /// <summary>
    /// 向多个种子添加多个 Peer。<br/>
    /// Add multiple peers to multiple torrents.
    /// </summary>
    /// <param name="hashes">种子哈希值列表。<br/>List of torrent hash values.</param>
    /// <param name="peers">要添加的 Peer 地址列表。<br/>List of peer addresses to add.</param>
    /// <param name="cancellationToken">取消请求的令牌。<br/>Token used to cancel the request.</param>
    public async Task AddTorrentsPeers(List<string> hashes,
                                       List<string> peers, CancellationToken cancellationToken = default) =>
        await AddTorrentPeer(StringUtils.Join('|', hashes), StringUtils.Join('|', peers), cancellationToken);

    /// <summary>
    /// 添加种子文件或 URL。<br/>
    /// Add torrent file(s) or URL(s).
    /// </summary>
    /// <param name="request">添加种子的请求参数。<br/>Request parameters for adding a torrent.</param>
    /// <param name="cancellationToken">取消请求的令牌。<br/>Token used to cancel the request.</param>
    /// <returns>
    /// 操作结果信息。<br/>
    /// Operation result message.
    /// </returns>
    public async Task<string> AddTorrent(AddTorrentRequest request, CancellationToken cancellationToken = default)
    {
        var parameters = request.ToDictionary(apiVersion);

        if (request.FilePaths is { Count: > 0 })
        {
            var result =
                await netService.PostWithFiles($"{BaseUrl}/add", parameters, request.FilePaths, cancellationToken);
            return result;
        }

        if (request.Urls is { Count: > 0 })
        {
            var result = await netService.Post($"{BaseUrl}/add", parameters, ct : cancellationToken);
            return result;
        }

        return "No torrent file or URL provided.";
    }

    /// <summary>
    /// 添加种子文件或 URL。<br/>
    /// Add torrent file(s) or URL(s).
    /// </summary>
    /// <param name="filePaths">种子文件路径列表。<br/>List of torrent file paths.</param>
    /// <param name="urls">种子 URL 列表。<br/>List of torrent URLs.</param>
    /// <param name="savePath">保存路径。<br/>Save path.</param>
    /// <param name="category">分类名称。<br/>Category name.</param>
    /// <param name="tags">标签名称。<br/>Tag(s).</param>
    /// <param name="skipChecking">是否跳过检查。<br/>Whether to skip checking.</param>
    /// <param name="stopped">是否停止下载。<br/>Whether to start in stopped state.</param>
    /// <param name="paused">是否暂停下载（向后兼容）。<br/>Whether to start paused (backward compatibility).</param>
    /// <param name="rootFolder">是否创建根文件夹。<br/>Whether to create a root folder.</param>
    /// <param name="rename">重命名。<br/>Rename.</param>
    /// <param name="uploadLimit">上传限制。<br/>Upload limit.</param>
    /// <param name="downloadLimit">下载限制。<br/>Download limit.</param>
    /// <param name="ratioLimit">分享率限制。<br/>Share ratio limit.</param>
    /// <param name="seedingTimeLimit">做种时间限制（分钟）。<br/>Seeding time limit (minutes).</param>
    /// <param name="autoTmm">是否自动管理。<br/>Whether to use automatic torrent management.</param>
    /// <param name="sequentialDownload">是否顺序下载。<br/>Whether to download sequentially.</param>
    /// <param name="firstLastPiecePriority">是否优先下载首尾块。<br/>Whether to prioritize first and last pieces.</param>
    /// <param name="cancellationToken">取消请求的令牌。<br/>Token used to cancel the request.</param>
    /// <returns>
    /// 操作结果信息。<br/>
    /// Operation result message.
    /// </returns>
    public async Task<string> AddTorrent(
        List<string>?     filePaths              = null,
        List<string>?     urls                   = null,
        string?           savePath               = "/download",
        string?           category               = null,
        string?           tags                   = null,
        bool?             skipChecking           = null,
        bool?             stopped                = null,
        bool?             paused                 = null,
        bool?             rootFolder             = null,
        string?           rename                 = null,
        int?              uploadLimit            = null,
        int?              downloadLimit          = null,
        float?            ratioLimit             = null,
        int?              seedingTimeLimit       = null,
        bool?             autoTmm                = null,
        bool?             sequentialDownload     = null,
        bool?             firstLastPiecePriority = null,
        CancellationToken cancellationToken      = default)
    {
        var request = new AddTorrentRequest
        {
            FilePaths                     = filePaths,
            Urls                          = urls,
            SavePath                      = savePath,
            Category                      = category,
            Tags                          = tags,
            SkipCheckingEnabled           = skipChecking,
            RootFolderEnabled             = rootFolder,
            Rename                        = rename,
            UploadLimit                   = uploadLimit,
            DownloadLimit                 = downloadLimit,
            RatioLimit                    = ratioLimit,
            SeedingTimeLimit              = seedingTimeLimit,
            AutoTmmEnabled                = autoTmm,
            SequentialDownloadEnabled     = sequentialDownload,
            FirstLastPiecePriorityEnabled = firstLastPiecePriority,
            PausedEnabled                 = stopped ?? paused
        };

        return await AddTorrent(request, cancellationToken);
    }

    /// <summary>
    /// 为指定种子添加 Tracker。<br/>
    /// Add a tracker to the specified torrent.
    /// </summary>
    /// <param name="hash">种子哈希值。<br/>Torrent hash value.</param>
    /// <param name="url">要添加的 Tracker 地址，多个以换行符分隔。<br/>Tracker URL(s) to add, separated by newline characters.</param>
    /// <param name="cancellationToken">取消请求的令牌。<br/>Token used to cancel the request.</param>
    public async Task AddTorrentTracker(string hash, string url, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(url))
        {
            throw new ArgumentException("Tracker url cannot be null or empty", nameof(url));
        }

        hash = StringUtils.NormalizeHash(hash);
        var parameters = new Dictionary<string, string>
        {
            { "hash", hash },
            { "urls", url },
        };
        await netService.Post($"{BaseUrl}/addTrackers", parameters, ct : cancellationToken);
    }

    /// <summary>
    /// 为指定种子添加多个 Tracker。<br/>
    /// Add multiple trackers to the specified torrent.
    /// </summary>
    /// <param name="hash">种子哈希值。<br/>Torrent hash value.</param>
    /// <param name="urls">要添加的 Tracker 地址列表。<br/>List of tracker URLs to add.</param>
    /// <param name="cancellationToken">取消请求的令牌。<br/>Token used to cancel the request.</param>
    public async Task AddTorrentTrackers(string       hash,
                                         List<string> urls, CancellationToken cancellationToken = default) =>
        await AddTorrentTracker(hash, StringUtils.Join('\n', urls), cancellationToken);

    /// <summary>
    /// 提高指定种子的优先级。<br/>
    /// Increase the priority of the specified torrent.
    /// </summary>
    /// <param name="hash">种子哈希值。<br/>Torrent hash value.</param>
    /// <param name="cancellationToken">取消请求的令牌。<br/>Token used to cancel the request.</param>
    public async Task IncreaseTorrentPriority(string hash, CancellationToken cancellationToken = default) =>
        await PutHashes("increasePrio", hash, cancellationToken : cancellationToken);

    /// <summary>
    /// 提高多个种子的优先级。<br/>
    /// Increase the priority of multiple torrents.
    /// </summary>
    /// <param name="hashes">种子哈希值列表。<br/>List of torrent hash values.</param>
    /// <param name="cancellationToken">取消请求的令牌。<br/>Token used to cancel the request.</param>
    public async Task IncreaseTorrentsPriority(List<string> hashes, CancellationToken cancellationToken = default) =>
        await IncreaseTorrentPriority(StringUtils.Join('|', hashes), cancellationToken);

    /// <summary>
    /// 降低指定种子的优先级。<br/>
    /// Decrease the priority of the specified torrent.
    /// </summary>
    /// <param name="hash">种子哈希值。<br/>Torrent hash value.</param>
    /// <param name="cancellationToken">取消请求的令牌。<br/>Token used to cancel the request.</param>
    public async Task DecreaseTorrentPriority(string hash, CancellationToken cancellationToken = default) =>
        await PutHashes("decreasePrio", hash, cancellationToken : cancellationToken);

    /// <summary>
    /// 降低多个种子的优先级。<br/>
    /// Decrease the priority of multiple torrents.
    /// </summary>
    /// <param name="hashes">种子哈希值列表。<br/>List of torrent hash values.</param>
    /// <param name="cancellationToken">取消请求的令牌。<br/>Token used to cancel the request.</param>
    public async Task DecreaseTorrentsPriority(List<string> hashes, CancellationToken cancellationToken = default) =>
        await DecreaseTorrentPriority(StringUtils.Join('|', hashes), cancellationToken);

    /// <summary>
    /// 将指定种子的优先级提升至最高。<br/>
    /// Set the priority of the specified torrent to the maximum level.
    /// </summary>
    /// <param name="hash">种子哈希值。<br/>Torrent hash value.</param>
    /// <param name="cancellationToken">取消请求的令牌。<br/>Token used to cancel the request.</param>
    public async Task MaximalTorrentPriority(string hash, CancellationToken cancellationToken = default) =>
        await PutHashes("topPrio", hash, cancellationToken : cancellationToken);

    /// <summary>
    /// 将多个种子的优先级提升至最高。<br/>
    /// Set the priority of multiple torrents to the maximum level.
    /// </summary>
    /// <param name="hashes">种子哈希值列表。<br/>List of torrent hash values.</param>
    /// <param name="cancellationToken">取消请求的令牌。<br/>Token used to cancel the request.</param>
    public async Task MaximalTorrentsPriority(List<string> hashes, CancellationToken cancellationToken = default) =>
        await MaximalTorrentPriority(StringUtils.Join('|', hashes), cancellationToken);

    /// <summary>
    /// 将指定种子的优先级降低至最低。<br/>
    /// Set the priority of the specified torrent to the minimum level.
    /// </summary>
    /// <param name="hash">种子哈希值。<br/>Torrent hash value.</param>
    /// <param name="cancellationToken">取消请求的令牌。<br/>Token used to cancel the request.</param>
    public async Task MinimalTorrentPriority(string hash, CancellationToken cancellationToken = default) =>
        await PutHashes("bottomPrio", hash, cancellationToken : cancellationToken);

    /// <summary>
    /// 将多个种子的优先级降低至最低。<br/>
    /// Set the priority of multiple torrents to the minimum level.
    /// </summary>
    /// <param name="hashes">种子哈希值列表。<br/>List of torrent hash values.</param>
    /// <param name="cancellationToken">取消请求的令牌。<br/>Token used to cancel the request.</param>
    public async Task MinimalTorrentsPriority(List<string> hashes, CancellationToken cancellationToken = default) =>
        await MinimalTorrentPriority(StringUtils.Join('|', hashes), cancellationToken);

    /// <summary>
    /// 设置种子中文件的优先度。<br/>
    /// Set file priority in a torrent.
    /// </summary>
    /// <param name="hash">种子哈希值。<br/>Torrent hash value.</param>
    /// <param name="fileIndex">文件索引。<br/>File index.</param>
    /// <param name="priority">文件优先度。<br/>File priority.</param>
    /// <param name="cancellationToken">取消请求的令牌。<br/>Token used to cancel the request.</param>
    public async Task SetFilePriority(string                  hash,
                                      int                     fileIndex,
                                      EnumTorrentFilePriority priority,
                                      CancellationToken       cancellationToken = default)
        => await SetFilesPriority(hash, [fileIndex], priority, cancellationToken);

    /// <summary>
    /// 设置种子中多个文件的优先度。<br/>
    /// Set priorities for multiple files in a torrent.
    /// </summary>
    /// <param name="hash">种子哈希值。<br/>Torrent hash value.</param>
    /// <param name="fileIndexes">文件索引列表。<br/>List of file indexes.</param>
    /// <param name="priority">文件优先度。<br/>File priority.</param>
    /// <param name="cancellationToken">取消请求的令牌。<br/>Token used to cancel the request.</param>
    public async Task SetFilesPriority(string                  hash,
                                       List<int>               fileIndexes,
                                       EnumTorrentFilePriority priority,
                                       CancellationToken       cancellationToken = default)
    {
        if (fileIndexes == null || fileIndexes.Count == 0)
        {
            throw new ArgumentException("File indexes cannot be null or empty", nameof(fileIndexes));
        }

        if (fileIndexes.Min() < 0)
        {
            throw new ArgumentException("File indexes has invalid index", nameof(fileIndexes));
        }

        hash = StringUtils.NormalizeHash(hash);
        var parameters = new Dictionary<string, string>
        {
            { "hash", hash },
            { "ids", StringUtils.Join('|', fileIndexes) },
            { "priority", ((int)priority).ToString() }
        };

        await netService.Post($"{BaseUrl}/filePrio", parameters, ct : cancellationToken);
    }

    /// <summary>
    /// 获取指定种子的下载限速。<br/>
    /// Get the download limit of the specified torrent.
    /// </summary>
    /// <param name="hash">种子哈希值。<br/>Torrent hash value.</param>
    /// <param name="cancellationToken">取消请求的令牌。<br/>Token used to cancel the request.</param>
    /// <returns>
    /// 包含哈希与限速值的 <see cref="SpeedInfo"/>；未设置或获取失败返回 <c>null</c>。<br/>
    /// A <see cref="SpeedInfo"/> containing the hash and limit value; <c>null</c> if not set or retrieval fails.
    /// </returns>
    public async Task<SpeedInfo?> GetTorrentDownloadLimit(string hash, CancellationToken cancellationToken = default) =>
        (await GetTorrentsDownloadLimit([hash], cancellationToken))?.FirstOrDefault();

    /// <summary>
    /// 获取多个种子的下载限速。<br/>
    /// Get the download limits of multiple torrents.
    /// </summary>
    /// <param name="hashes">种子哈希值列表。<br/>List of torrent hash values.</param>
    /// <param name="cancellationToken">取消请求的令牌。<br/>Token used to cancel the request.</param>
    /// <returns>
    /// 包含哈希与限速值的列表；获取失败返回 <c>null</c>。<br/>
    /// A list of <see cref="SpeedInfo"/> objects; <c>null</c> if retrieval fails.
    /// </returns>
    public async Task<List<SpeedInfo>?> GetTorrentsDownloadLimit(List<string>      hashes,
                                                                 CancellationToken cancellationToken = default)
    {
        var response =
            await PutHashes("downloadLimit", string.Join('|', hashes), cancellationToken : cancellationToken);
        var dict = QBittorrentJsonSerializer.Deserialize<Dictionary<string, long>>(response);
        return dict?.Select(kv => new SpeedInfo { Hash = kv.Key, Speed = kv.Value }).ToList();
    }

    /// <summary>
    /// 设置指定种子的下载限速。<br/>
    /// Set the download limit for the specified torrent.
    /// </summary>
    /// <param name="hash">种子哈希值。<br/>Torrent hash value.</param>
    /// <param name="limitSpeed">下载速度限制（字节/秒）。<br/>Download speed limit in bytes per second.</param>
    /// <param name="cancellationToken">取消请求的令牌。<br/>Token used to cancel the request.</param>
    public async Task SetTorrentDownloadLimit(string hash,
                                              long   limitSpeed, CancellationToken cancellationToken = default)
    {
        hash = StringUtils.NormalizeHash(hash);
        var parameters = new Dictionary<string, string>
        {
            { "hashes", hash },
            { "limit", limitSpeed.ToString() },
        };
        await netService.Post($"{BaseUrl}/setDownloadLimit", parameters, ct : cancellationToken);
    }

    /// <summary>
    /// 设置多个种子的下载限速。<br/>
    /// Set the download limit for multiple torrents.
    /// </summary>
    /// <param name="hashes">种子哈希值列表。<br/>List of torrent hash values.</param>
    /// <param name="limitSpeed">下载速度限制（字节/秒）。<br/>Download speed limit in bytes per second.</param>
    /// <param name="cancellationToken">取消请求的令牌。<br/>Token used to cancel the request.</param>
    public async Task SetTorrentsDownloadLimit(List<string>      hashes,
                                               long              limitSpeed,
                                               CancellationToken cancellationToken = default) =>
        await SetTorrentDownloadLimit(string.Join('|', hashes), limitSpeed, cancellationToken);

    /// <summary>
    /// Set download speed limit for all torrents.
    ///
    /// 为所有种子设置下载速度上限。
    /// </summary>
    /// <param name="limitSpeed">
    /// Download limit in bytes per second.  
    /// 下载限速（单位：字节/秒）。
    /// </param>
    /// <param name="cancellationToken">取消请求的令牌。<br/>Token used to cancel the request.</param>
    public async Task SetAllTorrentsDownloadLimit(long limitSpeed, CancellationToken cancellationToken = default) =>
        await SetTorrentDownloadLimit("all", limitSpeed, cancellationToken);

    /// <summary>
    /// 设置指定种子的分享限制。<br/>
    /// Set the share limits for the specified torrent.
    /// </summary>
    /// <param name="hash">
    /// 种子哈希值。<br/>
    /// Torrent hash value.
    /// </param>
    /// <param name="ratioLimit">
    /// 最大分享率（-2 表示使用全局值，-1 表示无限制）。<br/>
    /// Maximum share ratio (-2 uses global value, -1 means no limit).
    /// </param>
    /// <param name="seedingTimeLimit">
    /// 最大做种时间（分钟）（-2 表示使用全局值，-1 表示无限制）。<br/>
    /// Maximum seeding time in minutes (-2 uses global value, -1 means no limit).
    /// </param>
    /// <param name="inactiveSeedingTimeLimit">
    /// 最大非活动做种时间（分钟）（-2 表示使用全局值，-1 表示无限制）。<br/>
    /// Maximum inactive seeding time in minutes (-2 uses global value, -1 means no limit).
    /// </param>
    /// <param name="cancellationToken">取消请求的令牌。<br/>Token used to cancel the request.</param>
    public async Task SetTorrentShareLimit(string            hash,
                                           float?            ratioLimit               = null,
                                           int?              seedingTimeLimit         = null,
                                           int?              inactiveSeedingTimeLimit = null,
                                           CancellationToken cancellationToken        = default)
    {
        if (ratioLimit is null && seedingTimeLimit is null && inactiveSeedingTimeLimit is null)
            throw new
                ArgumentException("At least one of ratioLimit, seedingTimeLimit, or inactiveSeedingTimeLimit must be provided.");

        hash = StringUtils.NormalizeHash(hash);
        var parameters = new Dictionary<string, string>
        {
            { "hashes", hash },
        };
        if (ratioLimit is not null)
            parameters["ratioLimit"] = ratioLimit.Value.ToString(CultureInfo.InvariantCulture);

        if (seedingTimeLimit is not null)
            parameters["seedingTimeLimit"] = seedingTimeLimit.Value.ToString();

        if (inactiveSeedingTimeLimit is not null)
            parameters["inactiveSeedingTimeLimit"] = inactiveSeedingTimeLimit.Value.ToString();

        await netService.Post($"{BaseUrl}/setShareLimits", parameters, ApiVersion.V2_0_1, ct : cancellationToken);
    }

    /// <summary>
    /// 设置多个种子的分享限制。<br/>
    /// Set the share limits for multiple torrents.
    /// </summary>
    /// <param name="hashes">
    /// 种子哈希值列表。<br/>
    /// List of torrent hash values.
    /// </param>
    /// <param name="ratioLimit">
    /// 最大分享率（-2 表示使用全局值，-1 表示无限制）。<br/>
    /// Maximum share ratio (-2 uses global value, -1 means no limit).
    /// </param>
    /// <param name="seedingTimeLimit">
    /// 最大做种时间（分钟）（-2 表示使用全局值，-1 表示无限制）。<br/>
    /// Maximum seeding time in minutes (-2 uses global value, -1 means no limit).
    /// </param>
    /// <param name="inactiveSeedingTimeLimit">
    /// 最大非活动做种时间（分钟）（-2 表示使用全局值，-1 表示无限制）。<br/>
    /// Maximum inactive seeding time in minutes (-2 uses global value, -1 means no limit).
    /// </param>
    /// <param name="cancellationToken">取消请求的令牌。<br/>Token used to cancel the request.</param>
    public async Task SetTorrentsShareLimit(List<string>      hashes,
                                            float?            ratioLimit               = null,
                                            int?              seedingTimeLimit         = null,
                                            int?              inactiveSeedingTimeLimit = null,
                                            CancellationToken cancellationToken        = default) =>
        await SetTorrentShareLimit(string.Join('|', hashes), ratioLimit, seedingTimeLimit, inactiveSeedingTimeLimit,
                                   cancellationToken);

    /// <summary>
    /// 设置所有种子的分享限制。<br/>
    /// Set the share limits for all torrents.
    /// </summary>
    /// <param name="ratioLimit">
    /// 最大分享率（-2 表示使用全局值，-1 表示无限制）。<br/>
    /// Maximum share ratio (-2 uses global value, -1 means no limit).
    /// </param>
    /// <param name="seedingTimeLimit">
    /// 最大做种时间（分钟）（-2 表示使用全局值，-1 表示无限制）。<br/>
    /// Maximum seeding time in minutes (-2 uses global value, -1 means no limit).
    /// </param>
    /// <param name="inactiveSeedingTimeLimit">
    /// 最大非活动做种时间（分钟）（-2 表示使用全局值，-1 表示无限制）。<br/>
    /// Maximum inactive seeding time in minutes (-2 uses global value, -1 means no limit).
    /// </param>
    /// <param name="cancellationToken">取消请求的令牌。<br/>Token used to cancel the request.</param>
    public async Task SetAllTorrentsShareLimit(float?            ratioLimit               = null,
                                               int?              seedingTimeLimit         = null,
                                               int?              inactiveSeedingTimeLimit = null,
                                               CancellationToken cancellationToken        = default) =>
        await SetTorrentShareLimit("all", ratioLimit, seedingTimeLimit, inactiveSeedingTimeLimit,
                                   cancellationToken);

    /// <summary>
    /// 获取指定种子的上传限速。<br/>
    /// Get the upload limit of the specified torrent.
    /// </summary>
    /// <param name="hash">种子哈希值。<br/>Torrent hash value.</param>
    /// <param name="cancellationToken">取消请求的令牌。<br/>Token used to cancel the request.</param>
    /// <returns>
    /// 包含哈希与限速值的 <see cref="SpeedInfo"/>；未设置或获取失败返回 <c>null</c>。<br/>
    /// A <see cref="SpeedInfo"/> containing the hash and limit value; <c>null</c> if not set or retrieval fails.
    /// </returns>
    public async Task<SpeedInfo?> GetTorrentUploadLimit(string hash, CancellationToken cancellationToken = default) =>
        (await GetTorrentsUploadLimit([hash], cancellationToken))?.FirstOrDefault();

    /// <summary>
    /// 获取多个种子的上传限速。<br/>
    /// Get the upload limits of multiple torrents.
    /// </summary>
    /// <param name="hashes">种子哈希值列表。<br/>List of torrent hash values.</param>
    /// <param name="cancellationToken">取消请求的令牌。<br/>Token used to cancel the request.</param>
    /// <returns>
    /// 包含哈希与限速值的列表；获取失败返回 <c>null</c>。<br/>
    /// A list of <see cref="SpeedInfo"/> objects; <c>null</c> if retrieval fails.
    /// </returns>
    public async Task<List<SpeedInfo>?> GetTorrentsUploadLimit(List<string>      hashes,
                                                               CancellationToken cancellationToken = default)
    {
        var response = await PutHashes("uploadLimit", string.Join('|', hashes),
                                       cancellationToken : cancellationToken);
        var dict = QBittorrentJsonSerializer.Deserialize<Dictionary<string, long>>(response);
        return dict?.Select(kv => new SpeedInfo { Hash = kv.Key, Speed = kv.Value }).ToList();
    }

    /// <summary>
    /// 设置指定种子的上传限速。<br/>
    /// Set the upload limit for the specified torrent.
    /// </summary>
    /// <param name="hash">种子哈希值。<br/>Torrent hash value.</param>
    /// <param name="limitSpeed">上传速度限制（字节/秒）。<br/>Upload speed limit in bytes per second.</param>
    /// <param name="cancellationToken">取消请求的令牌。<br/>Token used to cancel the request.</param>
    public async Task SetTorrentUploadLimit(string hash, long limitSpeed, CancellationToken cancellationToken = default)
    {
        hash = StringUtils.NormalizeHash(hash);
        var parameters = new Dictionary<string, string>
        {
            { "hashes", hash },
            { "limit", limitSpeed.ToString() },
        };
        await netService.Post($"{BaseUrl}/setUploadLimit", parameters, ct : cancellationToken);
    }

    /// <summary>
    /// 设置多个种子的上传限速。<br/>
    /// Set the upload limit for multiple torrents.
    /// </summary>
    /// <param name="hashes">种子哈希值列表。<br/>List of torrent hash values.</param>
    /// <param name="limitSpeed">上传速度限制（字节/秒）。<br/>Upload speed limit in bytes per second.</param>
    /// <param name="cancellationToken">取消请求的令牌。<br/>Token used to cancel the request.</param>
    public async Task SetTorrentsUploadLimit(List<string> hashes,
                                             long         limitSpeed, CancellationToken cancellationToken = default) =>
        await SetTorrentUploadLimit(string.Join('|', hashes), limitSpeed, cancellationToken);

    /// <summary>
    /// Set upload speed limit for all torrents.
    ///
    /// 为所有种子设置上传速度上限。
    /// </summary>
    /// <param name="limitSpeed">
    /// Upload limit in bytes per second.  
    /// 上传限速（单位：字节/秒）。
    /// </param>
    /// <param name="cancellationToken">取消请求的令牌。<br/>Token used to cancel the request.</param>
    public async Task SetAllTorrentsUploadLimit(long limitSpeed, CancellationToken cancellationToken = default) =>
        await SetTorrentUploadLimit("all", limitSpeed, cancellationToken);

    /// <summary>
    /// 设置指定种子的存储位置。<br/>
    /// Set the storage location for the specified torrent(s).
    /// </summary>
    /// <param name="hash">种子哈希值<br/>Torrent hash value.</param>
    /// <param name="newLocation">新的存储路径。<br/>New storage location.</param>
    /// <param name="cancellationToken">取消请求的令牌。<br/>Token used to cancel the request.</param>
    public async Task SetTorrentLocation(string hash, string newLocation, CancellationToken cancellationToken = default)
    {
        hash = StringUtils.NormalizeHash(hash);
        var parameters = new Dictionary<string, string>
        {
            { "hashes", hash },
            { "location", newLocation },
        };
        await netService.Post($"{BaseUrl}/setLocation", parameters, ct : cancellationToken);
    }

    /// <summary>
    /// 设置多个种子的存储位置。<br/>
    /// Set the storage location for multiple torrents.
    /// </summary>
    /// <param name="hashes">种子哈希值列表。<br/>List of torrent hash values.</param>
    /// <param name="newLocation">新的存储路径。<br/>New storage location.</param>
    /// <param name="cancellationToken">取消请求的令牌。<br/>Token used to cancel the request.</param>
    public async Task SetTorrentsLocation(List<string> hashes,
                                          string       newLocation, CancellationToken cancellationToken = default) =>
        await SetTorrentLocation(StringUtils.Join('|', hashes), newLocation, cancellationToken);

    /// <summary>
    /// 设置指定种子的最终保存路径。<br/>
    /// Sets the final save path for the specified torrent.
    /// </summary>
    /// <param name="hash">种子哈希值。<br/>Torrent hash value.</param>
    /// <param name="savePath">最终保存路径。<br/>Final save path.</param>
    /// <param name="cancellationToken">取消请求的令牌。<br/>Token used to cancel the request.</param>
    public async Task SetTorrentSavePath(string hash,
                                         string savePath, CancellationToken cancellationToken = default) =>
        await SetTorrentPath("setSavePath", hash, savePath, cancellationToken);

    /// <summary>
    /// 设置多个种子的最终保存路径。<br/>
    /// Sets the final save path for multiple torrents.
    /// </summary>
    /// <param name="hashes">种子哈希值列表。<br/>Torrent hash value list.</param>
    /// <param name="savePath">最终保存路径。<br/>Final save path.</param>
    /// <param name="cancellationToken">取消请求的令牌。<br/>Token used to cancel the request.</param>
    public async Task SetTorrentsSavePath(List<string> hashes,
                                          string       savePath, CancellationToken cancellationToken = default) =>
        await SetTorrentSavePath(StringUtils.NormalizeHash(hashes), savePath, cancellationToken);

    /// <summary>
    /// 设置所有种子的最终保存路径。<br/>
    /// Sets the final save path for all torrents.
    /// </summary>
    /// <param name="savePath">最终保存路径。<br/>Final save path.</param>
    /// <param name="cancellationToken">取消请求的令牌。<br/>Token used to cancel the request.</param>
    public async Task SetAllTorrentsSavePath(string savePath, CancellationToken cancellationToken = default) =>
        await SetTorrentSavePath("all", savePath, cancellationToken);

    /// <summary>
    /// 设置指定种子完成下载前使用的下载路径。<br/>
    /// Sets the download path used by the specified torrent before completion.
    /// </summary>
    /// <param name="hash">种子哈希值。<br/>Torrent hash value.</param>
    /// <param name="downloadPath">下载路径。<br/>Download path.</param>
    /// <param name="cancellationToken">取消请求的令牌。<br/>Token used to cancel the request.</param>
    public async Task SetTorrentDownloadPath(string hash,
                                             string downloadPath, CancellationToken cancellationToken = default) =>
        await SetTorrentPath("setDownloadPath", hash, downloadPath, cancellationToken);

    /// <summary>
    /// 设置多个种子完成下载前使用的下载路径。<br/>
    /// Sets the download path used by multiple torrents before completion.
    /// </summary>
    /// <param name="hashes">种子哈希值列表。<br/>Torrent hash value list.</param>
    /// <param name="downloadPath">下载路径。<br/>Download path.</param>
    /// <param name="cancellationToken">取消请求的令牌。<br/>Token used to cancel the request.</param>
    public async Task SetTorrentsDownloadPath(List<string> hashes,
                                              string downloadPath, CancellationToken cancellationToken = default) =>
        await SetTorrentDownloadPath(StringUtils.NormalizeHash(hashes), downloadPath, cancellationToken);

    /// <summary>
    /// 设置所有种子完成下载前使用的下载路径。<br/>
    /// Sets the download path used by all torrents before completion.
    /// </summary>
    /// <param name="downloadPath">下载路径。<br/>Download path.</param>
    /// <param name="cancellationToken">取消请求的令牌。<br/>Token used to cancel the request.</param>
    public async Task SetAllTorrentsDownloadPath(string downloadPath, CancellationToken cancellationToken = default) =>
        await SetTorrentDownloadPath("all", downloadPath, cancellationToken);

    /// <summary>
    /// 重命名指定种子。<br/>
    /// Rename the specified torrent.
    /// </summary>
    /// <param name="hash">种子哈希值。<br/>Torrent hash value.</param>
    /// <param name="newName">新的种子名称。<br/>New torrent name.</param>
    /// <param name="cancellationToken">取消请求的令牌。<br/>Token used to cancel the request.</param>
    public async Task RenameTorrent(string hash, string newName, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(newName))
        {
            throw new ArgumentException("Torrent new name cannot be null or empty", nameof(newName));
        }

        hash = StringUtils.NormalizeHash(hash);
        var parameters = new Dictionary<string, string>
        {
            { "hash", hash },
            { "name", newName },
        };
        await netService.Post($"{BaseUrl}/rename", parameters, ct : cancellationToken);
    }

    /// <summary>
    /// 为指定种子设置分类。<br/>
    /// Set the category for the specified torrent.
    /// </summary>
    /// <param name="hash">种子哈希值。<br/>Torrent hash value.</param>
    /// <param name="category">分类名称。<br/>Category name.</param>
    /// <param name="cancellationToken">取消请求的令牌。<br/>Token used to cancel the request.</param>
    public async Task SetTorrentCategory(string hash, string category, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(category))
        {
            throw new ArgumentException("Category cannot be null or empty", nameof(category));
        }

        hash = StringUtils.NormalizeHash(hash);
        var parameters = new Dictionary<string, string>
        {
            { "hash", hash },
            { "category", category },
        };
        await netService.Post($"{BaseUrl}/setCategory", parameters, ct : cancellationToken);
    }

    /// <summary>
    /// 为多个种子设置分类。<br/>
    /// Set the category for multiple torrents.
    /// </summary>
    /// <param name="hashes">种子哈希值列表。<br/>List of torrent hash values.</param>
    /// <param name="category">分类名称。<br/>Category name.</param>
    /// <param name="cancellationToken">取消请求的令牌。<br/>Token used to cancel the request.</param>
    public async Task SetTorrentsCategory(List<string> hashes,
                                          string       category, CancellationToken cancellationToken = default) =>
        await SetTorrentCategory(StringUtils.Join('|', hashes), category, cancellationToken);

    /// <summary>
    /// 获取所有分类。<br/>
    /// Get all categories.
    /// </summary>
    /// <param name="cancellationToken">取消请求的令牌。<br/>Token used to cancel the request.</param>
    /// <returns>
    /// 以分类名称为键的分类信息字典；获取失败返回 <c>null</c>。<br/>
    /// A dictionary of category information keyed by category name; <c>null</c> if retrieval fails.
    /// </returns>
    public async Task<Dictionary<string, TorrentCategory>?> GetAllCategories(
        CancellationToken cancellationToken = default) =>
        QBittorrentJsonSerializer
           .Deserialize<Dictionary<string, TorrentCategory>>(await netService.Get($"{BaseUrl}/categories",
                                                                      targetVersion : ApiVersion.V2_1_1,
                                                                      ct : cancellationToken));

    /// <summary>
    /// 创建一个分类。<br/>
    /// Create a category.
    /// </summary>
    /// <param name="name">要创建的分类名称。<br/>The name of the category to create.</param>
    /// <param name="savePath">分类保存路径。<br/>The save path of the category.</param>
    /// <param name="downloadPath">可选的下载路径。<br/>Optional download path.</param>
    /// <param name="downloadPathEnable">是否启用下载路径。<br/>Whether to enable the download path.</param>
    /// <param name="cancellationToken">取消请求的令牌。<br/>Token used to cancel the request.</param>
    /// <exception cref="QbittorrentBadRequestException">
    /// 当分类名称为空时抛出。<br/>
    /// Thrown when the category name is empty.
    /// </exception>
    /// <exception cref="QbittorrentConflictException">
    /// 当分类名称无效或已存在时抛出。<br/>
    /// Thrown when the category name is invalid or already exists.
    /// </exception>
    public async Task CreateCategory(string            name,
                                     string            savePath,
                                     string?           downloadPath       = null,
                                     bool?             downloadPathEnable = null,
                                     CancellationToken cancellationToken  = default)
    {
        var parameters = new Dictionary<string, string>
        {
            { "category", name },
            { "savePath", savePath },
        };

        if (!string.IsNullOrWhiteSpace(downloadPath))
            parameters["downloadPath"] = downloadPath;

        if (downloadPathEnable != null)
            parameters["downloadPathEnabled"] = downloadPathEnable.Value ? "True" : "False";

        try
        {
            await netService.Post($"{BaseUrl}/createCategory", parameters, ct : cancellationToken);
        }
        catch (QbittorrentBadRequestException)
        {
            throw new QbittorrentBadRequestException("Category name is empty");
        }
        catch (QbittorrentConflictException)
        {
            throw new QbittorrentConflictException("Category name is invalid or already exists");
        }
    }


    /// <summary>
    /// 编辑一个分类。<br/>
    /// Edit a category.
    /// </summary>
    /// <param name="name">要编辑的分类名称。<br/>The name of the category to edit.</param>
    /// <param name="savePath">分类保存路径。<br/>The save path of the category.</param>
    /// <param name="downloadPath">可选的下载路径。<br/>Optional download path.</param>
    /// <param name="downloadPathEnable">是否启用下载路径。<br/>Whether to enable the download path.</param>
    /// <param name="cancellationToken">取消请求的令牌。<br/>Token used to cancel the request.</param>
    /// <exception cref="QbittorrentBadRequestException">
    /// 当分类名称为空时抛出。<br/>
    /// Thrown when the category name is empty.
    /// </exception>
    /// <exception cref="QbittorrentConflictException">
    /// 当分类编辑失败时抛出。<br/>
    /// Thrown when the category editing operation fails.
    /// </exception>
    public async Task EditCategory(string            name,
                                   string            savePath,
                                   string?           downloadPath       = null,
                                   bool?             downloadPathEnable = null,
                                   CancellationToken cancellationToken  = default)
    {
        var parameters = new Dictionary<string, string>
        {
            { "category", name },
            { "savePath", savePath },
        };

        if (!string.IsNullOrWhiteSpace(downloadPath))
            parameters["downloadPath"] = downloadPath;

        if (downloadPathEnable != null)
            parameters["downloadPathEnabled"] = downloadPathEnable.Value ? "True" : "False";

        try
        {
            await netService.Post($"{BaseUrl}/editCategory", parameters, ApiVersion.V2_1_0, ct : cancellationToken);
        }
        catch (QbittorrentBadRequestException)
        {
            throw new QbittorrentBadRequestException("Category name is empty");
        }
        catch (QbittorrentConflictException)
        {
            throw new QbittorrentConflictException("Category editing failed");
        }
    }


    /// <summary>
    /// 删除一个分类。<br/>
    /// Delete a category.
    /// </summary>
    /// <param name="category">要删除的分类名称。<br/>The name of the category to delete.</param>
    /// <param name="cancellationToken">取消请求的令牌。<br/>Token used to cancel the request.</param>
    public async Task DeleteCategory(string category, CancellationToken cancellationToken = default)
    {
        var parameters = new Dictionary<string, string>
        {
            { "categories", category }
        };

        await netService.Post($"{BaseUrl}/removeCategories", parameters, ct : cancellationToken);
    }


    /// <summary>
    /// 删除多个分类。<br/>
    /// Delete multiple categories.
    /// </summary>
    /// <param name="categories">要删除的分类名称列表。<br/>List of category names to delete.</param>
    /// <param name="cancellationToken">取消请求的令牌。<br/>Token used to cancel the request.</param>
    public async Task DeleteCategories(List<string> categories, CancellationToken cancellationToken = default) =>
        await DeleteCategory(StringUtils.Join('\n', categories), cancellationToken);


    /// <summary>
    /// 为指定种子添加一个标签。<br/>
    /// Add a tag to the specified torrent.
    /// </summary>
    /// <param name="hash">种子哈希值。<br/>Torrent hash value.</param>
    /// <param name="tag">要添加的标签名称。<br/>The name of the tag to add.</param>
    /// <param name="cancellationToken">取消请求的令牌。<br/>Token used to cancel the request.</param>
    public async Task AddTorrentTag(string hash, string tag, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(tag))
        {
            throw new ArgumentException("Tag cannot be null or empty", nameof(tag));
        }

        hash = StringUtils.NormalizeHash(hash);
        var parameters = new Dictionary<string, string>
        {
            { "hashes", hash },
            { "tags", tag }
        };
        await netService.Post($"{BaseUrl}/addTags", parameters, ApiVersion.V2_3_0, ct : cancellationToken);
    }

    /// <summary>
    /// 为多个种子添加一个标签。<br/>
    /// Add a tag to multiple torrents.
    /// </summary>
    /// <param name="hashes">种子哈希值列表。<br/>List of torrent hash values.</param>
    /// <param name="tag">要添加的标签名称。<br/>The name of the tag to add.</param>
    /// <param name="cancellationToken">取消请求的令牌。<br/>Token used to cancel the request.</param>
    public async Task AddTorrentsTag(List<string> hashes, string tag, CancellationToken cancellationToken = default) =>
        await AddTorrentTag(StringUtils.Join('|', hashes), tag, cancellationToken);

    /// <summary>
    /// 为指定种子添加多个标签。<br/>
    /// Add multiple tags to the specified torrent.
    /// </summary>
    /// <param name="hash">种子哈希值。<br/>Torrent hash value.</param>
    /// <param name="tags">要添加的标签名称列表。<br/>List of tag names to add.</param>
    /// <param name="cancellationToken">取消请求的令牌。<br/>Token used to cancel the request.</param>
    public async Task AddTorrentTags(string hash, List<string> tags, CancellationToken cancellationToken = default) =>
        await AddTorrentTag(hash, StringUtils.Join(',', tags), cancellationToken);

    /// <summary>
    /// 为多个种子添加多个标签。<br/>
    /// Add multiple tags to multiple torrents.
    /// </summary>
    /// <param name="hashes">种子哈希值列表。<br/>List of torrent hash values.</param>
    /// <param name="tags">要添加的标签名称列表。<br/>List of tag names to add.</param>
    /// <param name="cancellationToken">取消请求的令牌。<br/>Token used to cancel the request.</param>
    public async Task AddTorrentsTags(List<string>      hashes, List<string> tags,
                                      CancellationToken cancellationToken = default) =>
        await AddTorrentTag(StringUtils.Join('|', hashes), StringUtils.Join(',', tags), cancellationToken);

    /// <summary>
    /// 替换指定种子的标签，并自动创建尚不存在的标签。<br/>
    /// Replaces tags for the specified torrent and creates missing tags automatically.
    /// </summary>
    /// <param name="hash">种子哈希值。<br/>Torrent hash value.</param>
    /// <param name="tags">新的标签列表。<br/>New tag list.</param>
    /// <param name="cancellationToken">取消请求的令牌。<br/>Token used to cancel the request.</param>
    public async Task SetTorrentTags(string hash, List<string> tags, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(tags);
        var parameters = new Dictionary<string, string>
        {
            { "hashes", StringUtils.NormalizeHash(hash) },
            { "tags", StringUtils.Join(',', tags) }
        };
        await netService.Post($"{BaseUrl}/setTags", parameters, ApiVersion.V2_11_4,
                              ct : cancellationToken);
    }

    /// <summary>
    /// 替换多个种子的标签。<br/>
    /// Replaces tags for multiple torrents.
    /// </summary>
    /// <param name="hashes">种子哈希值列表。<br/>Torrent hash value list.</param>
    /// <param name="tags">新的标签列表。<br/>New tag list.</param>
    /// <param name="cancellationToken">取消请求的令牌。<br/>Token used to cancel the request.</param>
    public async Task SetTorrentsTags(List<string> hashes,
                                      List<string> tags, CancellationToken cancellationToken = default) =>
        await SetTorrentTags(StringUtils.NormalizeHash(hashes), tags, cancellationToken);

    /// <summary>
    /// 替换所有种子的标签。<br/>
    /// Replaces tags for all torrents.
    /// </summary>
    /// <param name="tags">新的标签列表。<br/>New tag list.</param>
    /// <param name="cancellationToken">取消请求的令牌。<br/>Token used to cancel the request.</param>
    public async Task SetAllTorrentsTags(List<string> tags, CancellationToken cancellationToken = default) =>
        await SetTorrentTags("all", tags, cancellationToken);

    /// <summary>
    /// 移除指定种子的标签。<br/>
    /// Remove a tag from the specified torrent.
    /// </summary>
    /// <param name="hash">种子哈希值。<br/>Torrent hash value.</param>
    /// <param name="tag">要移除的标签名称。<br/>The name of the tag to remove.</param>
    /// <param name="cancellationToken">取消请求的令牌。<br/>Token used to cancel the request.</param>
    public async Task RemoveTorrentTag(string hash, string tag, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(tag))
        {
            throw new ArgumentException("Tag cannot be null or empty", nameof(tag));
        }

        hash = StringUtils.NormalizeHash(hash);
        var parameters = new Dictionary<string, string>
        {
            { "hashes", hash },
            { "tags", tag }
        };
        await netService.Post($"{BaseUrl}/removeTags", parameters, ApiVersion.V2_3_0, ct : cancellationToken);
    }

    /// <summary>
    /// 从多个种子中移除一个标签。<br/>
    /// Remove a tag from multiple torrents.
    /// </summary>
    /// <param name="hashes">种子哈希值列表。<br/>List of torrent hash values.</param>
    /// <param name="tag">要移除的标签名称。<br/>The name of the tag to remove.</param>
    /// <param name="cancellationToken">取消请求的令牌。<br/>Token used to cancel the request.</param>
    public async Task RemoveTorrentsTag(List<string> hashes,
                                        string       tag, CancellationToken cancellationToken = default) =>
        await RemoveTorrentTag(StringUtils.Join('|', hashes), tag, cancellationToken);

    /// <summary>
    /// 从指定种子中移除多个标签。<br/>
    /// Remove multiple tags from the specified torrent.
    /// </summary>
    /// <param name="hash">种子哈希值。<br/>Torrent hash value.</param>
    /// <param name="tags">要移除的标签名称列表。<br/>List of tag names to remove.</param>
    /// <param name="cancellationToken">取消请求的令牌。<br/>Token used to cancel the request.</param>
    public async Task RemoveTorrentTags(string       hash,
                                        List<string> tags, CancellationToken cancellationToken = default) =>
        await RemoveTorrentTag(hash, StringUtils.Join(',', tags), cancellationToken);

    /// <summary>
    /// 从多个种子中移除多个标签。<br/>
    /// Remove multiple tags from multiple torrents.
    /// </summary>
    /// <param name="hashes">种子哈希值列表。<br/>List of torrent hash values.</param>
    /// <param name="tags">要移除的标签名称列表。<br/>List of tag names to remove.</param>
    /// <param name="cancellationToken">取消请求的令牌。<br/>Token used to cancel the request.</param>
    public async Task RemoveTorrentsTags(List<string> hashes,
                                         List<string> tags, CancellationToken cancellationToken = default) =>
        await RemoveTorrentTag(StringUtils.Join('|', hashes), StringUtils.Join(',', tags), cancellationToken);

    /// <summary>
    /// 获取所有标签。<br/>
    /// Get all tags.
    /// </summary>
    /// <param name="cancellationToken">取消请求的令牌。<br/>Token used to cancel the request.</param>
    /// <returns>
    /// 标签名称列表；获取失败返回 <c>null</c>。<br/>
    /// A list of tag names; <c>null</c> if retrieval fails.
    /// </returns>
    public async Task<List<string>?> GetAllTags(CancellationToken cancellationToken = default) =>
        QBittorrentJsonSerializer.Deserialize<List<string>>(await netService.Get($"{BaseUrl}/tags",
                                                                     targetVersion : ApiVersion.V2_3_0,
                                                                     ct : cancellationToken));

    /// <summary>
    /// 创建一个标签。<br/>
    /// Create a tag.
    /// </summary>
    /// <param name="tag">要创建的标签名称。<br/>The name of the tag to create.</param>
    /// <param name="cancellationToken">取消请求的令牌。<br/>Token used to cancel the request.</param>
    public async Task CreateTag(string tag, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(tag))
        {
            throw new ArgumentException("Tag cannot be null or empty", nameof(tag));
        }

        var parameters = new Dictionary<string, string>
        {
            { "tags", tag }
        };
        await netService.Post($"{BaseUrl}/createTags", parameters, ApiVersion.V2_3_0, ct : cancellationToken);
    }

    /// <summary>
    /// 创建多个标签。<br/>
    /// Create multiple tags.
    /// </summary>
    /// <param name="tags">要创建的标签名称列表。<br/>List of tag names to create.</param>
    /// <param name="cancellationToken">取消请求的令牌。<br/>Token used to cancel the request.</param>
    public async Task CreateTags(List<string> tags, CancellationToken cancellationToken = default) =>
        await CreateTag(StringUtils.Join(',', tags), cancellationToken);

    /// <summary>
    /// 删除一个标签。<br/>
    /// Delete a tag.
    /// </summary>
    /// <param name="tag">要删除的标签名称。<br/>The name of the tag to delete.</param>
    /// <param name="cancellationToken">取消请求的令牌。<br/>Token used to cancel the request.</param>
    public async Task DeleteTag(string tag, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(tag))
        {
            throw new ArgumentException("Tag cannot be null or empty", nameof(tag));
        }

        var parameters = new Dictionary<string, string>
        {
            { "tags", tag }
        };
        await netService.Post($"{BaseUrl}/deleteTags", parameters, ApiVersion.V2_3_0, ct : cancellationToken);
    }

    /// <summary>
    /// 删除多个标签。<br/>
    /// Delete multiple tags.
    /// </summary>
    /// <param name="tags">要删除的标签名称列表。<br/>List of tag names to delete.</param>
    /// <param name="cancellationToken">取消请求的令牌。<br/>Token used to cancel the request.</param>
    public async Task DeleteTags(List<string> tags, CancellationToken cancellationToken = default) =>
        await DeleteTag(StringUtils.Join(',', tags), cancellationToken);

    /// <summary>
    /// 启用或禁用指定种子的自动管理。<br/>
    /// Enable or disable automatic management for the specified torrent.
    /// </summary>
    /// <param name="hash">
    /// 种子哈希值，可为单个或多个哈希值。<br/>
    /// Torrent hash value, can be a single hash or multiple hashes separated by '|'.
    /// </param>
    /// <param name="enable">
    /// 是否启用自动管理（默认禁用）。<br/>
    /// Whether to enable automatic management (disabled by default).
    /// </param>
    /// <param name="cancellationToken">取消请求的令牌。<br/>Token used to cancel the request.</param>
    public async Task SetTorrentAutoManagement(string hash,
                                               bool   enable = false, CancellationToken cancellationToken = default)
    {
        hash = StringUtils.NormalizeHash(hash);
        var parameters = new Dictionary<string, string>
        {
            ["hashes"] = hash,
            ["enable"] = enable.ToString().ToLowerInvariant()
        };

        await netService.Post($"{BaseUrl}/setAutoManagement", parameters, ct : cancellationToken);
    }

    /// <summary>
    /// 启用或禁用多个种子的自动管理。<br/>
    /// Enable or disable automatic management for multiple torrents.
    /// </summary>
    /// <param name="hashes">种子哈希值列表。<br/>List of torrent hash values.</param>
    /// <param name="enable">
    /// 是否启用自动管理（默认禁用）。<br/>
    /// Whether to enable automatic management (disabled by default).
    /// </param>
    /// <param name="cancellationToken">取消请求的令牌。<br/>Token used to cancel the request.</param>
    public async Task SetTorrentsAutoManagement(List<string> hashes,
                                                bool enable = false, CancellationToken cancellationToken = default) =>
        await SetTorrentAutoManagement(string.Join('|', hashes), enable, cancellationToken);

    /// <summary>
    /// 启用或禁用所有种子的自动管理。<br/>
    /// Enable or disable automatic management for all torrents.
    /// </summary>
    /// <param name="enable">
    /// 是否启用自动管理（默认禁用）。<br/>
    /// Whether to enable automatic management (disabled by default).
    /// </param>
    /// <param name="cancellationToken">取消请求的令牌。<br/>Token used to cancel the request.</param>
    public async Task
        SetAllTorrentsAutoManagement(bool enable = false, CancellationToken cancellationToken = default) =>
        await SetTorrentAutoManagement("all", enable, cancellationToken);

    /// <summary>
    /// 切换指定种子的顺序下载模式。<br/>
    /// Toggle sequential download mode for the specified torrent.
    /// </summary>
    /// <param name="hash">
    /// 种子哈希值，可为单个或多个哈希值。<br/>
    /// Torrent hash value, can be a single hash or multiple hashes separated by '|'.
    /// </param>
    /// <param name="cancellationToken">取消请求的令牌。<br/>Token used to cancel the request.</param>
    public async Task ToggleTorrentSequentialDownload(string hash, CancellationToken cancellationToken = default) =>
        await PutHashes("toggleSequentialDownload", hash, cancellationToken : cancellationToken);

    /// <summary>
    /// 切换多个种子的顺序下载模式。<br/>
    /// Toggle sequential download mode for multiple torrents.
    /// </summary>
    /// <param name="hashes">种子哈希值列表。<br/>List of torrent hash values.</param>
    /// <param name="cancellationToken">取消请求的令牌。<br/>Token used to cancel the request.</param>
    public async Task ToggleTorrentsSequentialDownload(List<string>      hashes,
                                                       CancellationToken cancellationToken = default) =>
        await ToggleTorrentSequentialDownload(string.Join('|', hashes), cancellationToken);

    /// <summary>
    /// 切换所有种子的顺序下载模式。<br/>
    /// Toggle sequential download mode for all torrents.
    /// </summary>
    /// <param name="cancellationToken">取消请求的令牌。<br/>Token used to cancel the request.</param>
    public async Task ToggleAllTorrentsSequentialDownload(CancellationToken cancellationToken = default) =>
        await ToggleTorrentSequentialDownload("all", cancellationToken);

    /// <summary>
    /// 切换指定种子的首尾片段优先下载模式。<br/>
    /// Toggle first and last piece priority mode for the specified torrent.
    /// </summary>
    /// <param name="hash">
    /// 种子哈希值，可为单个或多个哈希值。<br/>
    /// Torrent hash value, can be a single hash or multiple hashes separated by '|'.
    /// </param>
    /// <param name="cancellationToken">取消请求的令牌。<br/>Token used to cancel the request.</param>
    public async Task ToggleTorrentFirstLastPiecePriority(string hash, CancellationToken cancellationToken = default) =>
        await PutHashes("toggleFirstLastPiecePrio", hash, cancellationToken : cancellationToken);

    /// <summary>
    /// 切换多个种子的首尾片段优先下载模式。<br/>
    /// Toggle first and last piece priority mode for multiple torrents.
    /// </summary>
    /// <param name="hashes">种子哈希值列表。<br/>List of torrent hash values.</param>
    /// <param name="cancellationToken">取消请求的令牌。<br/>Token used to cancel the request.</param>
    public async Task ToggleTorrentsFirstLastPiecePriority(List<string>      hashes,
                                                           CancellationToken cancellationToken = default) =>
        await ToggleTorrentFirstLastPiecePriority(string.Join('|', hashes), cancellationToken);

    /// <summary>
    /// 切换所有种子的首尾片段优先下载模式。<br/>
    /// Toggle first and last piece priority mode for all torrents.
    /// </summary>
    /// <param name="cancellationToken">取消请求的令牌。<br/>Token used to cancel the request.</param>
    public async Task ToggleAllTorrentsFirstLastPiecePriority(CancellationToken cancellationToken = default) =>
        await ToggleTorrentFirstLastPiecePriority("all", cancellationToken);

    /// <summary>
    /// 启用或禁用指定种子的强制启动。<br/>
    /// Enable or disable force start for the specified torrent.
    /// </summary>
    /// <param name="hash">
    /// 种子哈希值，可为单个或多个哈希值。<br/>
    /// Torrent hash value, can be a single hash or multiple hashes separated by '|'.
    /// </param>
    /// <param name="enable">
    /// 是否启用强制启动（默认启用）。<br/>
    /// Whether to enable force start (enabled by default).
    /// </param>
    /// <param name="cancellationToken">取消请求的令牌。<br/>Token used to cancel the request.</param>
    public async Task SetTorrentForceStart(string hash,
                                           bool   enable = true, CancellationToken cancellationToken = default)
    {
        hash = StringUtils.NormalizeHash(hash);
        var parameters = new Dictionary<string, string>
        {
            ["hashes"] = hash,
            ["value"]  = enable.ToString().ToLowerInvariant()
        };

        await netService.Post($"{BaseUrl}/setForceStart", parameters, ct : cancellationToken);
    }

    /// <summary>
    /// 启用或禁用多个种子的强制启动。<br/>
    /// Enable or disable force start for multiple torrents.
    /// </summary>
    /// <param name="hashes">种子哈希值列表。<br/>List of torrent hash values.</param>
    /// <param name="enable">
    /// 是否启用强制启动（默认启用）。<br/>
    /// Whether to enable force start (enabled by default).
    /// </param>
    /// <param name="cancellationToken">取消请求的令牌。<br/>Token used to cancel the request.</param>
    public async Task SetTorrentsForceStart(List<string> hashes,
                                            bool enable = true, CancellationToken cancellationToken = default) =>
        await SetTorrentForceStart(string.Join('|', hashes), enable, cancellationToken);

    /// <summary>
    /// 启用或禁用所有种子的强制启动。<br/>
    /// Enable or disable force start for all torrents.
    /// </summary>
    /// <param name="enable">
    /// 是否启用强制启动（默认启用）。<br/>
    /// Whether to enable force start (enabled by default).
    /// </param>
    /// <param name="cancellationToken">取消请求的令牌。<br/>Token used to cancel the request.</param>
    public async Task SetAllTorrentsForceStart(bool enable = true, CancellationToken cancellationToken = default) =>
        await SetTorrentForceStart("all", enable, cancellationToken);

    /// <summary>
    /// 启用或禁用指定种子的超级做种模式。<br/>
    /// Enable or disable super seeding mode for the specified torrent.
    /// </summary>
    /// <param name="hash">
    /// 种子哈希值，可为单个或多个哈希值。<br/>
    /// Torrent hash value, can be a single hash or multiple hashes separated by '|'.
    /// </param>
    /// <param name="enable">
    /// 是否启用超级做种模式（默认启用）。<br/>
    /// Whether to enable super seeding mode (enabled by default).
    /// </param>
    /// <param name="cancellationToken">取消请求的令牌。<br/>Token used to cancel the request.</param>
    public async Task SetTorrentSuperSeeding(string hash,
                                             bool   enable = true, CancellationToken cancellationToken = default)
    {
        hash = StringUtils.NormalizeHash(hash);
        var parameters = new Dictionary<string, string>
        {
            ["hashes"] = hash,
            ["value"]  = enable.ToString().ToLowerInvariant()
        };

        await netService.Post($"{BaseUrl}/setSuperSeeding", parameters, ct : cancellationToken);
    }

    /// <summary>
    /// 启用或禁用多个种子的超级做种模式。<br/>
    /// Enable or disable super seeding mode for multiple torrents.
    /// </summary>
    /// <param name="hashes">种子哈希值列表。<br/>List of torrent hash values.</param>
    /// <param name="enable">
    /// 是否启用超级做种模式（默认启用）。<br/>
    /// Whether to enable super seeding mode (enabled by default).
    /// </param>
    /// <param name="cancellationToken">取消请求的令牌。<br/>Token used to cancel the request.</param>
    public async Task SetTorrentSuperSeeding(List<string> hashes,
                                             bool enable = true, CancellationToken cancellationToken = default) =>
        await SetTorrentSuperSeeding(string.Join('|', hashes), enable, cancellationToken);

    /// <summary>
    /// 启用或禁用所有种子的超级做种模式。<br/>
    /// Enable or disable super seeding mode for all torrents.
    /// </summary>
    /// <param name="enable">
    /// 是否启用超级做种模式（默认启用）。<br/>
    /// Whether to enable super seeding mode (enabled by default).
    /// </param>
    /// <param name="cancellationToken">取消请求的令牌。<br/>Token used to cancel the request.</param>
    public async Task SetAllTorrentsSuperSeeding(bool enable = true, CancellationToken cancellationToken = default) =>
        await SetTorrentSuperSeeding("all", enable, cancellationToken);

    /// <summary>
    /// 重命名种子中的文件。<br/>
    /// Rename a file in the torrent.
    /// </summary>
    /// <param name="hash">种子哈希值。<br/>Torrent hash value.</param>
    /// <param name="oldPath">原文件路径。<br/>Original file path.</param>
    /// <param name="newPath">新文件路径。<br/>New file path.</param>
    /// <param name="cancellationToken">取消请求的令牌。<br/>Token used to cancel the request.</param>
    public async Task RenameTorrentFile(string hash,
                                        string oldPath,
                                        string newPath, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(oldPath))
        {
            throw new ArgumentException("Old path cannot be null or empty", nameof(oldPath));
        }

        if (string.IsNullOrWhiteSpace(newPath))
        {
            throw new ArgumentException("New path cannot be null or empty", nameof(newPath));
        }

        hash = StringUtils.NormalizeHash(hash);
        netService.EnsureApiVersionSupported($"{BaseUrl}/renameFile", new ApiVersionRange(ApiVersion.V2_4_0));
        if (!UsesPathBasedRename)
        {
            var fileList = await GetTorrentFiles(hash, cancellationToken : cancellationToken);
            if (fileList == null || fileList.Count == 0) return;
            var index = fileList.FindIndex(f => f.Name == oldPath);
            if (index == -1) throw new ArgumentException("File path doesn't exist.", nameof(oldPath));
            await RenameTorrentFile(hash, index, newPath, cancellationToken);
            return;
        }

        var parameters = new Dictionary<string, string>
        {
            { "hash", hash },
            { "oldPath", oldPath },
            { "newPath", newPath }
        };
        await netService.Post($"{BaseUrl}/renameFile", parameters, ApiVersion.V2_4_0, ct : cancellationToken);
    }

    /// <summary>
    /// 重命名种子中的文件（按索引）。<br/>
    /// Rename a file in the torrent by index.
    /// </summary>
    /// <param name="hash">种子哈希值。<br/>Torrent hash value.</param>
    /// <param name="index">文件索引。<br/>File index.</param>
    /// <param name="newPath">新文件路径。<br/>New file path.</param>
    /// <param name="cancellationToken">取消请求的令牌。<br/>Token used to cancel the request.</param>
    public async Task RenameTorrentFile(string hash,
                                        int    index,
                                        string newPath, CancellationToken cancellationToken = default)
    {
        if (index < 0)
        {
            throw new ArgumentException("Index should start from 0", nameof(index));
        }

        if (string.IsNullOrWhiteSpace(newPath))
        {
            throw new ArgumentException("New path cannot be null or empty", nameof(newPath));
        }

        hash = StringUtils.NormalizeHash(hash);
        netService.EnsureApiVersionSupported($"{BaseUrl}/renameFile", new ApiVersionRange(ApiVersion.V2_4_0));
        if (UsesPathBasedRename)
        {
            var fileList = await GetTorrentFiles(hash, cancellationToken : cancellationToken);
            if (fileList is { Count: > 0 })
                await RenameTorrentFile(hash, fileList[index].Name, newPath, cancellationToken);
            return;
        }

        var parameters = new Dictionary<string, string>
        {
            { "hash", hash },
            { "id", index.ToString() },
            { "name", newPath }
        };

        await netService.Post($"{BaseUrl}/renameFile", parameters, ApiVersion.V2_4_0, ct : cancellationToken);
    }

    /// <summary>
    /// 重命名种子中的文件夹。<br/>
    /// Rename a folder in the torrent.
    /// </summary>
    /// <param name="hash">种子哈希值。<br/>Torrent hash value.</param>
    /// <param name="oldPath">原文件夹路径。<br/>Original folder path.</param>
    /// <param name="newPath">新文件夹路径。<br/>New folder path.</param>
    /// <param name="cancellationToken">取消请求的令牌。<br/>Token used to cancel the request.</param>
    public async Task RenameTorrentFolder(string hash,
                                          string oldPath,
                                          string newPath, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(oldPath))
        {
            throw new ArgumentException("Old path cannot be null or empty", nameof(oldPath));
        }

        if (string.IsNullOrWhiteSpace(newPath))
        {
            throw new ArgumentException("New path cannot be null or empty", nameof(newPath));
        }

        hash = StringUtils.NormalizeHash(hash);
        netService.EnsureApiVersionSupported($"{BaseUrl}/renameFolder", new ApiVersionRange(ApiVersion.V2_7_0));
        if (_applicationVersion is not null && _applicationVersion < PathBasedRenameVersion)
            throw new
                NotSupportedException($"The endpoint '{BaseUrl}/renameFolder' requires qBittorrent >= {PathBasedRenameVersion}, but server is {_applicationVersion}.");

        var parameters = new Dictionary<string, string>
        {
            { "hash", hash },
            { "oldPath", oldPath },
            { "newPath", newPath }
        };

        await netService.Post($"{BaseUrl}/renameFolder", parameters, ApiVersion.V2_7_0, ct : cancellationToken);
    }

    /// <summary>
    /// 为多个种子设置备注（Comment）。<br/>
    /// Set the comment for multiple torrents.
    /// </summary>
    /// <param name="hashes">种子哈希值列表。<br/>List of torrent hash values.</param>
    /// <param name="comment">要设置的备注内容。<br/>The comment text to set.</param>
    /// <param name="cancellationToken">取消请求的令牌。<br/>Token used to cancel the request.</param>
    public async Task SetTorrentsComment(List<string> hashes,
                                         string       comment, CancellationToken cancellationToken = default) =>
        await SetTorrentComment(StringUtils.Join('|', hashes), comment, cancellationToken);

    /// <summary>
    /// 为指定种子设置备注（Comment）。<br/>
    /// Set the comment for the specified torrent.
    /// </summary>
    /// <param name="hash">
    /// 种子哈希值，可为单个或多个哈希值。<br/>
    /// Torrent hash value, can be a single hash or multiple hashes separated by '|'.
    /// </param>
    /// <param name="comment">要设置的备注内容。<br/>The comment text to set.</param>
    /// <param name="cancellationToken">取消请求的令牌。<br/>Token used to cancel the request.</param>
    public async Task SetTorrentComment(string hash, string comment, CancellationToken cancellationToken = default)
    {
        hash = StringUtils.NormalizeHash(hash);
        var parameters = new Dictionary<string, string>
        {
            { "hash", hash },
            { "comment", comment }
        };

        await netService.Post($"{BaseUrl}/setComment", parameters, ApiVersion.V2_12_1, ct : cancellationToken);
    }

    private async Task SetTorrentPath(string subPath, string hash, string path, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(path);
        var parameters = new Dictionary<string, string>
        {
            { "id", StringUtils.NormalizeHash(hash) },
            { "path", path }
        };
        await netService.Post($"{BaseUrl}/{subPath}", parameters, ApiVersion.V2_8_4, ct : cancellationToken);
    }

    private bool UsesPathBasedRename =>
        _applicationVersion is not null
            ? _applicationVersion >= PathBasedRenameVersion
            : apiVersion          >= ApiVersion.V2_7_0;

    private async Task<string> Put(string            subPath, string hash, ApiVersion? targetVersion = null,
                                   CancellationToken cancellationToken = default)
    {
        hash = StringUtils.NormalizeHash(hash);
        var parameters = new Dictionary<string, string>
        {
            { "hash", hash },
        };
        return await netService.Post($"{BaseUrl}/{subPath}", parameters, targetVersion,
                                     ct : cancellationToken);
    }

    private async Task<string> PutHashes(string            subPath,
                                         string            hash,
                                         ApiVersion?       targetVersion     = null,
                                         CancellationToken cancellationToken = default)
    {
        hash = StringUtils.NormalizeHash(hash);
        var parameters = new Dictionary<string, string>
        {
            { "hashes", hash },
        };
        return await netService.Post($"{BaseUrl}/{subPath}", parameters, targetVersion, ct : cancellationToken);
    }
}
