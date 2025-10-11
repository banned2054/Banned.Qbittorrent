using System.Globalization;
using Banned.Qbittorrent.Exceptions;
using Banned.Qbittorrent.Models.Application;
using Banned.Qbittorrent.Models.Enums;
using Banned.Qbittorrent.Models.Requests;
using Banned.Qbittorrent.Models.Torrent;
using Banned.Qbittorrent.Utils;
using System.Text.Json;

namespace Banned.Qbittorrent.Services;

/// <summary>
/// 提供与 qBittorrent 种子相关的服务<br/>
/// Provides services related to qBittorrent torrents
/// </summary>
public class TorrentService(NetUtils netUtils, ApiVersion apiVersion)
{
    private const string BaseUrl = "/api/v2/torrents";

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
    /// <returns>
    /// 种子信息列表。<br/>
    /// List of torrent information.
    /// </returns>
    public async Task<TorrentInfo?> GetTorrentInfo(string            hash,
                                                   EnumTorrentFilter filter   = EnumTorrentFilter.All,
                                                   string?           category = null,
                                                   string?           tag      = null,
                                                   string?           sort     = null,
                                                   bool              reverse  = false,
                                                   int               limit    = 0,
                                                   int               offset   = 0)
    {
        if (string.IsNullOrEmpty(hash))
        {
            throw new ArgumentException("Torrent hash cannot be null or empty", nameof(hash));
        }

        var result = await GetTorrentInfos([hash], filter, category, tag, sort, reverse, limit, offset);
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
    /// <returns>
    /// 种子信息列表。<br/>
    /// List of torrent information.
    /// </returns>
    public async Task<List<TorrentInfo>> GetTorrentInfos(List<string>?     hashes   = null,
                                                         EnumTorrentFilter filter   = EnumTorrentFilter.All,
                                                         string?           category = null,
                                                         string?           tag      = null,
                                                         string?           sort     = null,
                                                         bool              reverse  = false,
                                                         int               limit    = 0,
                                                         int               offset   = 0)
    {
        var parameters = new Dictionary<string, string>();

        if (filter != EnumTorrentFilter.All)
        {
            parameters.Add("filter", filter.ToString().ToLower());
        }

        if (!string.IsNullOrEmpty(category)) parameters.Add("category", category);
        if (!string.IsNullOrEmpty(tag)) parameters.Add("tag", tag);
        if (!string.IsNullOrEmpty(sort)) parameters.Add("sort", sort);
        if (reverse) parameters.Add("reverse", "true");
        if (limit  > 0) parameters.Add("limit", limit.ToString());
        if (offset > 0) parameters.Add("offset", offset.ToString());
        if (hashes is { Count: > 0 })
        {
            parameters.Add("hashes", StringUtils.Join('|', hashes));
        }

        var response = await netUtils.Post($"{BaseUrl}/info", parameters);
        return JsonSerializer.Deserialize<List<TorrentInfo>>(response) ?? [];
    }

    /// <summary>
    /// 使用请求对象获取种子信息列表。<br/>
    /// Get torrent information list using a request object.
    /// </summary>
    /// <param name="request">获取种子信息列表的请求参数。<br/>Request parameters for getting torrent information list.</param>
    /// <returns>
    /// 种子信息列表。<br/>
    /// List of torrent information.
    /// </returns>
    public async Task<List<TorrentInfo>> GetTorrentInfos(GetTorrentInfoListRequest request)
    {
        var parameters = new Dictionary<string, string>
        {
            { "filter", request.Filter.ToString().ToLower() }
        };

        if (!string.IsNullOrEmpty(request.Category)) parameters.Add("category", request.Category);
        if (!string.IsNullOrEmpty(request.Tag)) parameters.Add("tag", request.Tag);
        if (!string.IsNullOrEmpty(request.Sort)) parameters.Add("sort", request.Sort);
        if (request.Reverse) parameters.Add("reverse", "true");
        if (request.Limit  > 0) parameters.Add("limit", request.Limit.ToString());
        if (request.Offset > 0) parameters.Add("offset", request.Offset.ToString());
        if (request.HashList is { Count: > 0 })
        {
            parameters.Add("hashes", StringUtils.Join('|', request.HashList));
        }

        var response = await netUtils.Post($"{BaseUrl}/info", parameters);
        return JsonSerializer.Deserialize<List<TorrentInfo>>(response) ?? [];
    }

    /// <summary>
    /// 获取指定种子的通用属性。<br/>
    /// Get generic properties of the specified torrent.
    /// </summary>
    /// <param name="hash">种子哈希值。<br/>Torrent hash value.</param>
    /// <returns>
    /// 包含属性的 <see cref="TorrentProperties"/>；获取失败返回 <c>null</c>。<br/>
    /// A <see cref="TorrentProperties"/> if successful; otherwise <c>null</c>.
    /// </returns>
    public async Task<TorrentProperties?> GetTorrentGenericProperties(string hash) =>
        JsonSerializer.Deserialize<TorrentProperties>(await Put("properties", hash));

    /// <summary>
    /// 获取指定种子的 Tracker 信息。<br/>
    /// Get tracker information for the specified torrent.
    /// </summary>
    /// <param name="hash">种子哈希值。<br/>Torrent hash value.</param>
    /// <returns>
    /// 包含 Tracker 信息的列表；无数据返回空列表。<br/>
    /// A list of <see cref="TrackerInfo"/>; an empty list if no data is available.
    /// </returns>
    public async Task<List<TrackerInfo>?> GetTorrentTrackers(string hash) =>
        JsonSerializer.Deserialize<List<TrackerInfo>>(await Put("trackers", hash));

    /// <summary>
    /// 获取指定种子的 Web 种子列表。<br/>
    /// Get the list of web seeds for the specified torrent.
    /// </summary>
    /// <param name="hash">种子哈希值。<br/>Torrent hash value.</param>
    /// <returns>
    /// 包含 Web 种子信息的列表；获取失败返回 <c>null</c>。<br/>
    /// A list of <see cref="TorrentWebSeed"/> representing web seed information; <c>null</c> if retrieval fails.
    /// </returns>
    public async Task<List<TorrentWebSeed>?> GetTorrentWebSeeds(string hash) =>
        JsonSerializer.Deserialize<List<TorrentWebSeed>>(await Put("webseeds", hash));

    /// <summary>
    /// 获取种子的文件列表。<br/>
    /// Get the file list of a torrent.
    /// </summary>
    /// <param name="hash">种子哈希值。<br/>Torrent hash value.</param>
    /// <param name="indexes">文件索引列表。<br/>List of file indexes.</param>
    /// <returns>
    /// 种子文件信息列表。<br/>
    /// List of torrent file information.
    /// </returns>
    public async Task<List<TorrentFileInfo>?> GetTorrentFiles(string hash, List<int>? indexes = null)
    {
        if (string.IsNullOrEmpty(hash))
        {
            throw new ArgumentException("Torrent hash cannot be null or empty", nameof(hash));
        }

        const string requestUrl = $"{BaseUrl}/files";
        var parameters = new Dictionary<string, string>
        {
            { "hash", hash }
        };

        if (indexes is { Count: > 0 })
        {
            parameters["indexes"] = StringUtils.Join('|', indexes);
        }

        return JsonSerializer.Deserialize<List<TorrentFileInfo>>(await netUtils.Post(requestUrl, parameters));
    }

    /// <summary>
    /// 获取指定种子的每个分片状态。<br/>
    /// Get the state of each piece in the specified torrent.
    /// </summary>
    /// <param name="hash">种子哈希值。<br/>Torrent hash value.</param>
    /// <returns>
    /// 分片状态列表，包含每个分片的下载状态；获取失败返回 <c>null</c>。<br/>
    /// A list of <see cref="EnumPieceState"/> representing the state of each piece; <c>null</c> if retrieval fails.
    /// </returns>
    public async Task<List<EnumPieceState>?> GetTorrentPiecesStates(string hash) =>
        JsonSerializer.Deserialize<List<EnumPieceState>>(await Put("pieceStates", hash));

    /// <summary>
    /// 获取指定种子的每个分片哈希值。<br/>
    /// Get the hash of each piece in the specified torrent.
    /// </summary>
    /// <param name="hash">种子哈希值。<br/>Torrent hash value.</param>
    /// <returns>
    /// 分片哈希值列表；获取失败返回 <c>null</c>。<br/>
    /// A list of piece hash strings; <c>null</c> if retrieval fails.
    /// </returns>
    public async Task<List<string>?> GetTorrentPiecesHashes(string hash) =>
        JsonSerializer.Deserialize<List<string>>(await Put("pieceHashes", hash));

    /// <summary>
    /// 暂停指定种子。<br/>
    /// Pause the specified torrent.
    /// </summary>
    /// <param name="hash">种子哈希值。<br/>Torrent hash value.</param>
    public async Task PauseTorrent(string hash) =>
        await PutHashes(apiVersion < ApiVersion.V2_11_0 ? "pause" : "stop", hash);

    /// <summary>
    /// 暂停多个种子。<br/>
    /// Pause multiple torrents.
    /// </summary>
    /// <param name="hashes">种子哈希值列表。<br/>List of torrent hash values.</param>
    public async Task PauseTorrents(List<string> hashes) => await PauseTorrent(StringUtils.Join('|', hashes));

    /// <summary>
    /// 继续下载/做种指定种子。<br/>
    /// Resume the specified torrent.
    /// </summary>
    /// <param name="hash">种子哈希值。<br/>Torrent hash value.</param>
    public async Task ResumeTorrent(string hash) =>
        await PutHashes(apiVersion < ApiVersion.V2_11_0 ? "resume" : "start", hash);

    /// <summary>
    /// 继续下载/做种多个种子。<br/>
    /// Resume multiple torrents.
    /// </summary>
    /// <param name="hashes">种子哈希值列表。<br/>List of torrent hash values.</param>
    public async Task ResumeTorrents(List<string> hashes) => await ResumeTorrent(StringUtils.Join('|', hashes));

    /// <summary>
    /// 删除指定种子。<br/>
    /// Delete the specified torrent.
    /// </summary>
    /// <param name="hash">种子哈希值。<br/>Torrent hash value.</param>
    /// <param name="deleteFile">是否同时删除文件。<br/>Whether to delete files as well.</param>
    public async Task DeleteTorrent(string hash, bool deleteFile = false)
    {
        var parameters = new Dictionary<string, string>
        {
            { "hashes", hash },
            { "deleteFiles", deleteFile.ToString().ToLower() }
        };

        await netUtils.Post($"{BaseUrl}/delete", parameters);
    }

    /// <summary>
    /// 删除多个指定种子。<br/>
    /// Delete multiple specified torrents.
    /// </summary>
    /// <param name="hashes">种子哈希值列表。<br/>List of torrent hash values.</param>
    /// <param name="deleteFile">是否同时删除文件。<br/>Whether to delete files as well.</param>
    public async Task DeleteTorrents(List<string> hashes, bool deleteFile = false) =>
        await DeleteTorrent(StringUtils.Join('|', hashes), deleteFile);

    /// <summary>
    /// 重新校验指定种子的进度。<br/>
    /// Recheck the specified torrent's progress.
    /// </summary>
    /// <param name="hash">种子哈希值。<br/>Torrent hash value.</param>
    public async Task RecheckTorrent(string hash) => await PutHashes("recheck", hash);

    /// <summary>
    /// 重新校验多个种子的进度。<br/>
    /// Recheck progress for multiple torrents.
    /// </summary>
    /// <param name="hashes">种子哈希值列表。<br/>List of torrent hash values.</param>
    public async Task RecheckTorrents(List<string> hashes) => await RecheckTorrent(StringUtils.Join('|', hashes));

    /// <summary>
    /// 重新向 Tracker 汇报指定种子。<br/>
    /// Reannounce the specified torrent to the tracker.
    /// </summary>
    /// <param name="hash">种子哈希值。<br/>Torrent hash value.</param>
    public async Task ReannounceTorrent(string hash) => await PutHashes("reannounce", hash, ApiVersion.V2_0_2);

    /// <summary>
    /// 重新向 Tracker 汇报多个种子。<br/>
    /// Reannounce multiple torrents to the tracker.
    /// </summary>
    /// <param name="hashes">种子哈希值列表。<br/>List of torrent hash values.</param>
    public async Task ReannounceTorrents(List<string> hashes) => await ReannounceTorrent(StringUtils.Join('|', hashes));

    /// <summary>
    /// 编辑指定种子的 Tracker。<br/>
    /// Edit the tracker of the specified torrent.
    /// </summary>
    /// <param name="hash">种子哈希值。<br/>Torrent hash value.</param>
    /// <param name="originUrl">原始 Tracker 地址。<br/>Original tracker URL.</param>
    /// <param name="newUrl">新的 Tracker 地址。<br/>New tracker URL.</param>
    /// <exception cref="QbittorrentConflictException">
    /// 当新 URL 已存在或原始 URL 未找到时抛出。<br/>
    /// Thrown when the new URL already exists or the original URL is not found.
    /// </exception>
    /// <exception cref="QbittorrentBadRequestException">
    /// 当新 URL 格式无效时抛出。<br/>
    /// Thrown when the new URL is invalid.
    /// </exception>
    public async Task EditTorrentTracker(string hash, string originUrl, string newUrl)
    {
        if (string.IsNullOrWhiteSpace(hash))
        {
            throw new ArgumentException("Torrent hash cannot be null or empty", nameof(hash));
        }

        var parameters = new Dictionary<string, string>
        {
            { "hash", hash },
            { "origUrl", originUrl },
            { "newUrl", newUrl },
        };
        try
        {
            await netUtils.Post($"{BaseUrl}/editTracker", parameters);
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
    /// <exception cref="QbittorrentConflictException">
    /// 当所有指定的 Tracker 地址均未找到时抛出。<br/>
    /// Thrown when all specified tracker URLs are not found.
    /// </exception>
    public async Task RemoveTorrentTracker(string hash, string url)
    {
        if (string.IsNullOrWhiteSpace(hash))
        {
            throw new ArgumentException("Torrent hash cannot be null or empty", nameof(hash));
        }

        if (string.IsNullOrWhiteSpace(url))
        {
            throw new ArgumentException("Tracker url cannot be null or empty", nameof(url));
        }

        var parameters = new Dictionary<string, string>
        {
            { "hash", hash },
            { "urls", url },
        };
        try
        {
            await netUtils.Post($"{BaseUrl}/removeTrackers", parameters, ApiVersion.V2_2_0);
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
    public async Task RemoveTorrentTrackers(string hash, List<string> urls) =>
        await RemoveTorrentTracker(hash, StringUtils.Join('|', urls));

    /// <summary>
    /// 向指定种子添加一个 Peer。<br/>
    /// Add a peer to the specified torrent.
    /// </summary>
    /// <param name="hash">种子哈希值。<br/>Torrent hash value.</param>
    /// <param name="peer">要添加的 Peer 地址（可为 IP:Port 格式）。<br/>Peer address to add (can be in IP:Port format).</param>
    /// <exception cref="QbittorrentConflictException">
    /// 当所有指定的 Peer 地址均未找到或添加失败时抛出。<br/>
    /// Thrown when all specified peer addresses are not found or failed to add.
    /// </exception>
    public async Task AddTorrentPeer(string hash, string peer)
    {
        if (string.IsNullOrWhiteSpace(hash))
        {
            throw new ArgumentException("Torrent hash cannot be null or empty", nameof(hash));
        }

        if (string.IsNullOrWhiteSpace(peer))
        {
            throw new ArgumentException("Peer cannot be null or empty", nameof(peer));
        }

        var parameters = new Dictionary<string, string>
        {
            { "hash", hash },
            { "peers", peer },
        };
        try
        {
            await netUtils.Post($"{BaseUrl}/addPeers", parameters, ApiVersion.V2_3_0);
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
    public async Task AddTorrentPeers(string hash, List<string> peers) =>
        await AddTorrentPeer(hash, StringUtils.Join('|', peers));

    /// <summary>
    /// 向多个种子添加一个 Peer。<br/>
    /// Add a peer to multiple torrents.
    /// </summary>
    /// <param name="hashes">种子哈希值列表。<br/>List of torrent hash values.</param>
    /// <param name="peer">要添加的 Peer 地址。<br/>Peer address to add.</param>
    public async Task AddTorrentsPeer(List<string> hashes, string peer) =>
        await AddTorrentPeer(StringUtils.Join('|', hashes), peer);

    /// <summary>
    /// 向多个种子添加多个 Peer。<br/>
    /// Add multiple peers to multiple torrents.
    /// </summary>
    /// <param name="hashes">种子哈希值列表。<br/>List of torrent hash values.</param>
    /// <param name="peers">要添加的 Peer 地址列表。<br/>List of peer addresses to add.</param>
    public async Task AddTorrentsPeers(List<string> hashes, List<string> peers) =>
        await AddTorrentPeer(StringUtils.Join('|', hashes), StringUtils.Join('|', peers));

    /// <summary>
    /// 添加种子文件或 URL。<br/>
    /// Add torrent file(s) or URL(s).
    /// </summary>
    /// <param name="request">添加种子的请求参数。<br/>Request parameters for adding a torrent.</param>
    /// <returns>
    /// 操作结果信息。<br/>
    /// Operation result message.
    /// </returns>
    public async Task<string> AddTorrent(AddTorrentRequest request)
    {
        var parameters = request.ToDictionary();

        if (request.FilePaths is { Count: > 0 })
        {
            var result = await netUtils.PostWithFiles($"{BaseUrl}/add", parameters, request.FilePaths);
            return result;
        }

        if (request.Urls is { Count: > 0 })
        {
            var result = await netUtils.Post($"{BaseUrl}/add", parameters);
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
    /// <returns>
    /// 操作结果信息。<br/>
    /// Operation result message.
    /// </returns>
    public async Task<string> AddTorrent(
        List<string>? filePaths              = null,
        List<string>? urls                   = null,
        string?       savePath               = "/download",
        string?       category               = null,
        string?       tags                   = null,
        bool?         skipChecking           = null,
        bool?         stopped                = null,
        bool?         paused                 = null,
        bool?         rootFolder             = null,
        string?       rename                 = null,
        int?          uploadLimit            = null,
        int?          downloadLimit          = null,
        float?        ratioLimit             = null,
        int?          seedingTimeLimit       = null,
        bool?         autoTmm                = null,
        bool?         sequentialDownload     = null,
        bool?         firstLastPiecePriority = null)
    {
        var request = new AddTorrentRequest
        {
            FilePaths              = filePaths,
            Urls                   = urls,
            SavePath               = savePath,
            Category               = category,
            Tags                   = tags,
            SkipChecking           = skipChecking,
            RootFolder             = rootFolder,
            Rename                 = rename,
            UploadLimit            = uploadLimit,
            DownloadLimit          = downloadLimit,
            RatioLimit             = ratioLimit,
            SeedingTimeLimit       = seedingTimeLimit,
            AutoTmm                = autoTmm,
            SequentialDownload     = sequentialDownload,
            FirstLastPiecePriority = firstLastPiecePriority
        };
        if (apiVersion < ApiVersion.V2_11_0)
        {
            request.Paused = stopped ?? paused;
        }
        else
        {
            request.Stopped = stopped ?? paused;
        }

        return await AddTorrent(request);
    }

    /// <summary>
    /// 为指定种子添加 Tracker。<br/>
    /// Add a tracker to the specified torrent.
    /// </summary>
    /// <param name="hash">种子哈希值。<br/>Torrent hash value.</param>
    /// <param name="url">要添加的 Tracker 地址，多个以换行符分隔。<br/>Tracker URL(s) to add, separated by newline characters.</param>
    public async Task AddTorrentTracker(string hash, string url)
    {
        if (string.IsNullOrWhiteSpace(hash))
        {
            throw new ArgumentException("Torrent hash cannot be null or empty", nameof(hash));
        }

        if (string.IsNullOrWhiteSpace(url))
        {
            throw new ArgumentException("Tracker url cannot be null or empty", nameof(url));
        }

        var parameters = new Dictionary<string, string>
        {
            { "hash", hash },
            { "urls", url },
        };
        await netUtils.Post($"{BaseUrl}/addTrackers", parameters);
    }

    /// <summary>
    /// 为指定种子添加多个 Tracker。<br/>
    /// Add multiple trackers to the specified torrent.
    /// </summary>
    /// <param name="hash">种子哈希值。<br/>Torrent hash value.</param>
    /// <param name="urls">要添加的 Tracker 地址列表。<br/>List of tracker URLs to add.</param>
    public async Task AddTorrentTrackers(string hash, List<string> urls) =>
        await AddTorrentTracker(hash, StringUtils.Join('\n', urls));

    /// <summary>
    /// 提高指定种子的优先级。<br/>
    /// Increase the priority of the specified torrent.
    /// </summary>
    /// <param name="hash">种子哈希值。<br/>Torrent hash value.</param>
    public async Task IncreaseTorrentPriority(string hash) => await PutHashes("increasePrio", hash);

    /// <summary>
    /// 提高多个种子的优先级。<br/>
    /// Increase the priority of multiple torrents.
    /// </summary>
    /// <param name="hashes">种子哈希值列表。<br/>List of torrent hash values.</param>
    public async Task IncreaseTorrentsPriority(List<string> hashes) =>
        await IncreaseTorrentPriority(StringUtils.Join('|', hashes));

    /// <summary>
    /// 降低指定种子的优先级。<br/>
    /// Decrease the priority of the specified torrent.
    /// </summary>
    /// <param name="hash">种子哈希值。<br/>Torrent hash value.</param>
    public async Task DecreaseTorrentPriority(string hash) => await PutHashes("decreasePrio", hash);

    /// <summary>
    /// 降低多个种子的优先级。<br/>
    /// Decrease the priority of multiple torrents.
    /// </summary>
    /// <param name="hashes">种子哈希值列表。<br/>List of torrent hash values.</param>
    public async Task DecreaseTorrentsPriority(List<string> hashes) =>
        await DecreaseTorrentPriority(StringUtils.Join('|', hashes));

    /// <summary>
    /// 将指定种子的优先级提升至最高。<br/>
    /// Set the priority of the specified torrent to the maximum level.
    /// </summary>
    /// <param name="hash">种子哈希值。<br/>Torrent hash value.</param>
    public async Task MaximalTorrentPriority(string hash) => await PutHashes("topPrio", hash);

    /// <summary>
    /// 将多个种子的优先级提升至最高。<br/>
    /// Set the priority of multiple torrents to the maximum level.
    /// </summary>
    /// <param name="hashes">种子哈希值列表。<br/>List of torrent hash values.</param>
    public async Task MaximalTorrentsPriority(List<string> hashes) =>
        await MaximalTorrentPriority(StringUtils.Join('|', hashes));

    /// <summary>
    /// 将指定种子的优先级降低至最低。<br/>
    /// Set the priority of the specified torrent to the minimum level.
    /// </summary>
    /// <param name="hash">种子哈希值。<br/>Torrent hash value.</param>
    public async Task MinimalTorrentPriority(string hash) => await PutHashes("bottomPrio", hash);

    /// <summary>
    /// 将多个种子的优先级降低至最低。<br/>
    /// Set the priority of multiple torrents to the minimum level.
    /// </summary>
    /// <param name="hashes">种子哈希值列表。<br/>List of torrent hash values.</param>
    public async Task MinimalTorrentsPriority(List<string> hashes) =>
        await MinimalTorrentPriority(StringUtils.Join('|', hashes));

    /// <summary>
    /// 设置种子中文件的优先度。<br/>
    /// Set file priority in a torrent.
    /// </summary>
    /// <param name="hash">种子哈希值。<br/>Torrent hash value.</param>
    /// <param name="fileIndex">文件索引。<br/>File index.</param>
    /// <param name="priority">文件优先度。<br/>File priority.</param>
    public async Task SetFilePriority(string hash, int fileIndex, EnumTorrentFilePriority priority)
        => await SetFilesPriority(hash, [fileIndex], priority);

    /// <summary>
    /// 设置种子中多个文件的优先度。<br/>
    /// Set priorities for multiple files in a torrent.
    /// </summary>
    /// <param name="hash">种子哈希值。<br/>Torrent hash value.</param>
    /// <param name="fileIndexes">文件索引列表。<br/>List of file indexes.</param>
    /// <param name="priority">文件优先度。<br/>File priority.</param>
    public async Task SetFilesPriority(string hash, List<int> fileIndexes, EnumTorrentFilePriority priority)
    {
        if (string.IsNullOrWhiteSpace(hash))
        {
            throw new ArgumentException("Torrent hash cannot be null or empty", nameof(hash));
        }

        if (fileIndexes == null || fileIndexes.Count == 0)
        {
            throw new ArgumentException("File indexes cannot be null or empty", nameof(fileIndexes));
        }

        if (fileIndexes.Min() < 0)
        {
            throw new ArgumentException("File indexes has invalid index", nameof(fileIndexes));
        }

        var parameters = new Dictionary<string, string>
        {
            { "hash", hash },
            { "ids", StringUtils.Join('|', fileIndexes) },
            { "priority", ((int)priority).ToString() }
        };

        await netUtils.Post($"{BaseUrl}/filePrio", parameters);
    }

    /// <summary>
    /// 获取指定种子的下载限速。<br/>
    /// Get the download limit of the specified torrent.
    /// </summary>
    /// <param name="hash">种子哈希值。<br/>Torrent hash value.</param>
    /// <returns>
    /// 包含哈希与限速值的 <see cref="SpeedInfo"/>；未设置或获取失败返回 <c>null</c>。<br/>
    /// A <see cref="SpeedInfo"/> containing the hash and limit value; <c>null</c> if not set or retrieval fails.
    /// </returns>
    public async Task<SpeedInfo?> GetTorrentDownloadLimit(string hash) =>
        (await GetTorrentsDownloadLimit([hash]))?.FirstOrDefault();

    /// <summary>
    /// 获取多个种子的下载限速。<br/>
    /// Get the download limits of multiple torrents.
    /// </summary>
    /// <param name="hashes">种子哈希值列表。<br/>List of torrent hash values.</param>
    /// <returns>
    /// 包含哈希与限速值的列表；获取失败返回 <c>null</c>。<br/>
    /// A list of <see cref="SpeedInfo"/> objects; <c>null</c> if retrieval fails.
    /// </returns>
    public async Task<List<SpeedInfo>?> GetTorrentsDownloadLimit(List<string> hashes)
    {
        var response = await PutHashes("downloadLimit", string.Join('|', hashes));
        var dict     = JsonSerializer.Deserialize<Dictionary<string, long>>(response);
        return dict?.Select(kv => new SpeedInfo { Hash = kv.Key, Speed = kv.Value }).ToList();
    }

    /// <summary>
    /// 设置指定种子的下载限速。<br/>
    /// Set the download limit for the specified torrent.
    /// </summary>
    /// <param name="hash">种子哈希值。<br/>Torrent hash value.</param>
    /// <param name="limitSpeed">下载速度限制（字节/秒）。<br/>Download speed limit in bytes per second.</param>
    public async Task SetTorrentDownloadLimit(string hash, long limitSpeed)
    {
        if (string.IsNullOrWhiteSpace(hash)
         || hash.Split('|').All(string.IsNullOrWhiteSpace))
        {
            throw new ArgumentException("Torrent hash cannot be null or empty", nameof(hash));
        }

        var parameters = new Dictionary<string, string>
        {
            { "hashes", hash },
            { "limit", limitSpeed.ToString() },
        };
        await netUtils.Post($"{BaseUrl}/setDownloadLimit", parameters);
    }

    /// <summary>
    /// 设置多个种子的下载限速。<br/>
    /// Set the download limit for multiple torrents.
    /// </summary>
    /// <param name="hashes">种子哈希值列表。<br/>List of torrent hash values.</param>
    /// <param name="limitSpeed">下载速度限制（字节/秒）。<br/>Download speed limit in bytes per second.</param>
    public async Task SetTorrentsDownloadLimit(List<string> hashes, long limitSpeed) =>
        await SetTorrentDownloadLimit(string.Join('|', hashes), limitSpeed);

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
    public async Task SetTorrentShareLimit(string hash,
                                           float? ratioLimit               = null,
                                           int?   seedingTimeLimit         = null,
                                           int?   inactiveSeedingTimeLimit = null)
    {
        if (string.IsNullOrWhiteSpace(hash)
         || hash.Split('|').All(string.IsNullOrWhiteSpace))
        {
            throw new ArgumentException("Torrent hash cannot be null or empty", nameof(hash));
        }

        if (ratioLimit is null && seedingTimeLimit is null && inactiveSeedingTimeLimit is null)
            throw new
                ArgumentException("At least one of ratioLimit, seedingTimeLimit, or inactiveSeedingTimeLimit must be provided.");


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

        await netUtils.Post($"{BaseUrl}/setShareLimits", parameters);
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
    public async Task SetTorrentsShareLimit(List<string> hashes,
                                            float?       ratioLimit               = null,
                                            int?         seedingTimeLimit         = null,
                                            int?         inactiveSeedingTimeLimit = null) =>
        await SetTorrentShareLimit(string.Join('|', hashes), ratioLimit, seedingTimeLimit, inactiveSeedingTimeLimit);

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
    public async Task SetAllTorrentsShareLimit(float? ratioLimit               = null,
                                               int?   seedingTimeLimit         = null,
                                               int?   inactiveSeedingTimeLimit = null) =>
        await SetTorrentShareLimit("all", ratioLimit, seedingTimeLimit, inactiveSeedingTimeLimit);

    /// <summary>
    /// 获取指定种子的上传限速。<br/>
    /// Get the upload limit of the specified torrent.
    /// </summary>
    /// <param name="hash">种子哈希值。<br/>Torrent hash value.</param>
    /// <returns>
    /// 包含哈希与限速值的 <see cref="SpeedInfo"/>；未设置或获取失败返回 <c>null</c>。<br/>
    /// A <see cref="SpeedInfo"/> containing the hash and limit value; <c>null</c> if not set or retrieval fails.
    /// </returns>
    public async Task<SpeedInfo?> GetTorrentUploadLimit(string hash) =>
        (await GetTorrentsUploadLimit([hash]))?.FirstOrDefault();

    /// <summary>
    /// 获取多个种子的上传限速。<br/>
    /// Get the upload limits of multiple torrents.
    /// </summary>
    /// <param name="hashes">种子哈希值列表。<br/>List of torrent hash values.</param>
    /// <returns>
    /// 包含哈希与限速值的列表；获取失败返回 <c>null</c>。<br/>
    /// A list of <see cref="SpeedInfo"/> objects; <c>null</c> if retrieval fails.
    /// </returns>
    public async Task<List<SpeedInfo>?> GetTorrentsUploadLimit(List<string> hashes)
    {
        var response = await PutHashes("uploadLimit", string.Join('|', hashes));
        var dict     = JsonSerializer.Deserialize<Dictionary<string, long>>(response);
        return dict?.Select(kv => new SpeedInfo { Hash = kv.Key, Speed = kv.Value }).ToList();
    }

    /// <summary>
    /// 设置指定种子的上传限速。<br/>
    /// Set the upload limit for the specified torrent.
    /// </summary>
    /// <param name="hash">种子哈希值。<br/>Torrent hash value.</param>
    /// <param name="limitSpeed">上传速度限制（字节/秒）。<br/>Upload speed limit in bytes per second.</param>
    public async Task SetTorrentUploadLimit(string hash, long limitSpeed)
    {
        if (string.IsNullOrWhiteSpace(hash)
         || hash.Split('|').All(string.IsNullOrWhiteSpace))
        {
            throw new ArgumentException("Torrent hash cannot be null or empty", nameof(hash));
        }

        var parameters = new Dictionary<string, string>
        {
            { "hashes", hash },
            { "limit", limitSpeed.ToString() },
        };
        await netUtils.Post($"{BaseUrl}/setUploadLimit", parameters);
    }

    /// <summary>
    /// 设置多个种子的上传限速。<br/>
    /// Set the upload limit for multiple torrents.
    /// </summary>
    /// <param name="hashes">种子哈希值列表。<br/>List of torrent hash values.</param>
    /// <param name="limitSpeed">上传速度限制（字节/秒）。<br/>Upload speed limit in bytes per second.</param>
    public async Task SetTorrentsUploadLimit(List<string> hashes, long limitSpeed) =>
        await SetTorrentUploadLimit(string.Join('|', hashes), limitSpeed);

    /// <summary>
    /// 设置指定种子的存储位置。<br/>
    /// Set the storage location for the specified torrent(s).
    /// </summary>
    /// <param name="hash">种子哈希值<br/>Torrent hash value.</param>
    /// <param name="newLocation">新的存储路径。<br/>New storage location.</param>
    public async Task SetTorrentLocation(string hash, string newLocation)
    {
        if (string.IsNullOrWhiteSpace(hash)
         || hash.Split('|').All(string.IsNullOrWhiteSpace))
        {
            throw new ArgumentException("Torrent hash cannot be null or empty", nameof(hash));
        }

        var parameters = new Dictionary<string, string>
        {
            { "hashes", hash },
            { "location", newLocation },
        };
        await netUtils.Post($"{BaseUrl}/setLocation", parameters);
    }

    /// <summary>
    /// 设置多个种子的存储位置。<br/>
    /// Set the storage location for multiple torrents.
    /// </summary>
    /// <param name="hashes">种子哈希值列表。<br/>List of torrent hash values.</param>
    /// <param name="newLocation">新的存储路径。<br/>New storage location.</param>
    public async Task SetTorrentsLocation(List<string> hashes, string newLocation) =>
        await SetTorrentLocation(StringUtils.Join('|', hashes), newLocation);

    /// <summary>
    /// 重命名指定种子。<br/>
    /// Rename the specified torrent.
    /// </summary>
    /// <param name="hash">种子哈希值。<br/>Torrent hash value.</param>
    /// <param name="newName">新的种子名称。<br/>New torrent name.</param>
    public async Task RenameTorrent(string hash, string newName)
    {
        if (string.IsNullOrWhiteSpace(hash))
        {
            throw new ArgumentException("Torrent hash cannot be null or empty", nameof(hash));
        }

        if (string.IsNullOrWhiteSpace(newName))
        {
            throw new ArgumentException("Torrent new name cannot be null or empty", nameof(newName));
        }

        var parameters = new Dictionary<string, string>
        {
            { "hash", hash },
            { "name", newName },
        };
        await netUtils.Post($"{BaseUrl}/rename", parameters);
    }

    /// <summary>
    /// 为指定种子设置分类。<br/>
    /// Set the category for the specified torrent.
    /// </summary>
    /// <param name="hash">种子哈希值。<br/>Torrent hash value.</param>
    /// <param name="category">分类名称。<br/>Category name.</param>
    public async Task SetTorrentCategory(string hash, string category)
    {
        if (string.IsNullOrWhiteSpace(hash))
        {
            throw new ArgumentException("Torrent hash cannot be null or empty", nameof(hash));
        }

        if (string.IsNullOrWhiteSpace(category))
        {
            throw new ArgumentException("Category cannot be null or empty", nameof(category));
        }

        var parameters = new Dictionary<string, string>
        {
            { "hash", hash },
            { "category", category },
        };
        await netUtils.Post($"{BaseUrl}/setCategory", parameters);
    }

    /// <summary>
    /// 为多个种子设置分类。<br/>
    /// Set the category for multiple torrents.
    /// </summary>
    /// <param name="hashes">种子哈希值列表。<br/>List of torrent hash values.</param>
    /// <param name="category">分类名称。<br/>Category name.</param>
    public async Task SetTorrentsCategory(List<string> hashes, string category) =>
        await SetTorrentCategory(StringUtils.Join('|', hashes), category);

    /// <summary>
    /// 获取所有分类。<br/>
    /// Get all categories.
    /// </summary>
    /// <returns>
    /// 分类信息列表；获取失败返回 <c>null</c>。<br/>
    /// A list of <see cref="TorrentCategory"/> objects; <c>null</c> if retrieval fails.
    /// </returns>
    public async Task<List<TorrentCategory>?> GetAllCategories() => JsonSerializer.Deserialize<List<TorrentCategory>>(
         await netUtils.Get($"{BaseUrl}/categories", targetVersion : ApiVersion.V2_1_1));

    /// <summary>
    /// 创建一个分类。<br/>
    /// Create a category.
    /// </summary>
    /// <param name="name">要创建的分类名称。<br/>The name of the category to create.</param>
    /// <param name="savePath">分类保存路径。<br/>The save path of the category.</param>
    /// <param name="downloadPath">可选的下载路径。<br/>Optional download path.</param>
    /// <param name="downloadPathEnable">是否启用下载路径。<br/>Whether to enable the download path.</param>
    /// <exception cref="QbittorrentBadRequestException">
    /// 当分类名称为空时抛出。<br/>
    /// Thrown when the category name is empty.
    /// </exception>
    /// <exception cref="QbittorrentConflictException">
    /// 当分类名称无效或已存在时抛出。<br/>
    /// Thrown when the category name is invalid or already exists.
    /// </exception>
    public async Task CreateCategory(string  name,
                                     string  savePath,
                                     string? downloadPath       = null,
                                     bool?   downloadPathEnable = null)
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
            await netUtils.Post($"{BaseUrl}/createCategory", parameters);
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
    /// <exception cref="QbittorrentBadRequestException">
    /// 当分类名称为空时抛出。<br/>
    /// Thrown when the category name is empty.
    /// </exception>
    /// <exception cref="QbittorrentConflictException">
    /// 当分类编辑失败时抛出。<br/>
    /// Thrown when the category editing operation fails.
    /// </exception>
    public async Task EditCategory(string  name,
                                   string  savePath,
                                   string? downloadPath       = null,
                                   bool?   downloadPathEnable = null)
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
            await netUtils.Post($"{BaseUrl}/editCategory", parameters, ApiVersion.V2_1_0);
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
    public async Task DeleteCategory(string category)
    {
        var parameters = new Dictionary<string, string>
        {
            { "categories", category }
        };

        await netUtils.Post($"{BaseUrl}/removeCategories", parameters);
    }


    /// <summary>
    /// 删除多个分类。<br/>
    /// Delete multiple categories.
    /// </summary>
    /// <param name="categories">要删除的分类名称列表。<br/>List of category names to delete.</param>
    public async Task DeleteCategories(List<string> categories) =>
        await DeleteCategory(StringUtils.Join('\n', categories));


    /// <summary>
    /// 为指定种子添加一个标签。<br/>
    /// Add a tag to the specified torrent.
    /// </summary>
    /// <param name="hash">种子哈希值。<br/>Torrent hash value.</param>
    /// <param name="tag">要添加的标签名称。<br/>The name of the tag to add.</param>
    public async Task AddTorrentTag(string hash, string tag)
    {
        if (string.IsNullOrWhiteSpace(hash))
        {
            throw new ArgumentException("Torrent hash cannot be null or empty", nameof(hash));
        }

        if (string.IsNullOrWhiteSpace(tag))
        {
            throw new ArgumentException("Tag cannot be null or empty", nameof(tag));
        }

        var parameters = new Dictionary<string, string>
        {
            { "hashes", hash },
            { "tags", tag }
        };
        await netUtils.Post($"{BaseUrl}/addTags", parameters, ApiVersion.V2_3_0);
    }

    /// <summary>
    /// 为多个种子添加一个标签。<br/>
    /// Add a tag to multiple torrents.
    /// </summary>
    /// <param name="hashes">种子哈希值列表。<br/>List of torrent hash values.</param>
    /// <param name="tag">要添加的标签名称。<br/>The name of the tag to add.</param>
    public async Task AddTorrentsTag(List<string> hashes, string tag) =>
        await AddTorrentTag(StringUtils.Join('|', hashes), tag);

    /// <summary>
    /// 为指定种子添加多个标签。<br/>
    /// Add multiple tags to the specified torrent.
    /// </summary>
    /// <param name="hash">种子哈希值。<br/>Torrent hash value.</param>
    /// <param name="tags">要添加的标签名称列表。<br/>List of tag names to add.</param>
    public async Task AddTorrentTags(string hash, List<string> tags) =>
        await AddTorrentTag(hash, StringUtils.Join(',', tags));

    /// <summary>
    /// 为多个种子添加多个标签。<br/>
    /// Add multiple tags to multiple torrents.
    /// </summary>
    /// <param name="hashes">种子哈希值列表。<br/>List of torrent hash values.</param>
    /// <param name="tags">要添加的标签名称列表。<br/>List of tag names to add.</param>
    public async Task AddTorrentsTags(List<string> hashes, List<string> tags) =>
        await AddTorrentTag(StringUtils.Join('|', hashes), StringUtils.Join(',', tags));

    /// <summary>
    /// 移除指定种子的标签。<br/>
    /// Remove a tag from the specified torrent.
    /// </summary>
    /// <param name="hash">种子哈希值。<br/>Torrent hash value.</param>
    /// <param name="tag">要移除的标签名称。<br/>The name of the tag to remove.</param>
    public async Task RemoveTorrentTag(string hash, string tag)
    {
        if (string.IsNullOrWhiteSpace(hash))
        {
            throw new ArgumentException("Torrent hash cannot be null or empty", nameof(hash));
        }

        if (string.IsNullOrWhiteSpace(tag))
        {
            throw new ArgumentException("Tag cannot be null or empty", nameof(tag));
        }

        var parameters = new Dictionary<string, string>
        {
            { "hashes", hash },
            { "tags", tag }
        };
        await netUtils.Post($"{BaseUrl}/removeTags", parameters, ApiVersion.V2_3_0);
    }

    /// <summary>
    /// 从多个种子中移除一个标签。<br/>
    /// Remove a tag from multiple torrents.
    /// </summary>
    /// <param name="hashes">种子哈希值列表。<br/>List of torrent hash values.</param>
    /// <param name="tag">要移除的标签名称。<br/>The name of the tag to remove.</param>
    public async Task RemoveTorrentsTag(List<string> hashes, string tag) =>
        await RemoveTorrentTag(StringUtils.Join('|', hashes), tag);

    /// <summary>
    /// 从指定种子中移除多个标签。<br/>
    /// Remove multiple tags from the specified torrent.
    /// </summary>
    /// <param name="hash">种子哈希值。<br/>Torrent hash value.</param>
    /// <param name="tags">要移除的标签名称列表。<br/>List of tag names to remove.</param>
    public async Task RemoveTorrentTags(string hash, List<string> tags) =>
        await RemoveTorrentTag(hash, StringUtils.Join(',', tags));

    /// <summary>
    /// 从多个种子中移除多个标签。<br/>
    /// Remove multiple tags from multiple torrents.
    /// </summary>
    /// <param name="hashes">种子哈希值列表。<br/>List of torrent hash values.</param>
    /// <param name="tags">要移除的标签名称列表。<br/>List of tag names to remove.</param>
    public async Task RemoveTorrentsTags(List<string> hashes, List<string> tags) =>
        await RemoveTorrentTag(StringUtils.Join('|', hashes), StringUtils.Join(',', tags));

    /// <summary>
    /// 获取所有标签。<br/>
    /// Get all tags.
    /// </summary>
    /// <returns>
    /// 标签名称列表；获取失败返回 <c>null</c>。<br/>
    /// A list of tag names; <c>null</c> if retrieval fails.
    /// </returns>
    public async Task<List<string>?> GetAllTags() => JsonSerializer.Deserialize<List<string>>(
         await netUtils.Get($"{BaseUrl}/tags", targetVersion : ApiVersion.V2_3_0));

    /// <summary>
    /// 创建一个标签。<br/>
    /// Create a tag.
    /// </summary>
    /// <param name="tag">要创建的标签名称。<br/>The name of the tag to create.</param>
    public async Task CreateTag(string tag)
    {
        if (string.IsNullOrWhiteSpace(tag))
        {
            throw new ArgumentException("Tag cannot be null or empty", nameof(tag));
        }

        var parameters = new Dictionary<string, string>
        {
            { "tags", tag }
        };
        await netUtils.Post($"{BaseUrl}/createTags", parameters, ApiVersion.V2_3_0);
    }

    /// <summary>
    /// 创建多个标签。<br/>
    /// Create multiple tags.
    /// </summary>
    /// <param name="tags">要创建的标签名称列表。<br/>List of tag names to create.</param>
    public async Task CreateTags(List<string> tags) => await CreateTag(StringUtils.Join(',', tags));

    /// <summary>
    /// 删除一个标签。<br/>
    /// Delete a tag.
    /// </summary>
    /// <param name="tag">要删除的标签名称。<br/>The name of the tag to delete.</param>
    public async Task DeleteTag(string tag)
    {
        if (string.IsNullOrWhiteSpace(tag))
        {
            throw new ArgumentException("Tag cannot be null or empty", nameof(tag));
        }

        var parameters = new Dictionary<string, string>
        {
            { "tags", tag }
        };
        await netUtils.Post($"{BaseUrl}/deleteTags", parameters, ApiVersion.V2_3_0);
    }

    /// <summary>
    /// 删除多个标签。<br/>
    /// Delete multiple tags.
    /// </summary>
    /// <param name="tags">要删除的标签名称列表。<br/>List of tag names to delete.</param>
    public async Task DeleteTags(List<string> tags) => await DeleteTag(StringUtils.Join(',', tags));

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
    public async Task SetTorrentAutoManagement(string hash, bool enable = false)
    {
        if (string.IsNullOrWhiteSpace(hash))
        {
            throw new ArgumentException("Torrent hash cannot be null or empty", nameof(hash));
        }

        var parameters = new Dictionary<string, string>
        {
            ["hashes"] = hash,
            ["enable"] = enable.ToString().ToLowerInvariant()
        };

        await netUtils.Post($"{BaseUrl}/setAutoManagement", parameters);
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
    public async Task SetTorrentsAutoManagement(List<string> hashes, bool enable = false) =>
        await SetTorrentAutoManagement(string.Join('|', hashes), enable);

    /// <summary>
    /// 启用或禁用所有种子的自动管理。<br/>
    /// Enable or disable automatic management for all torrents.
    /// </summary>
    /// <param name="enable">
    /// 是否启用自动管理（默认禁用）。<br/>
    /// Whether to enable automatic management (disabled by default).
    /// </param>
    public async Task SetAllTorrentsAutoManagement(bool enable = false) =>
        await SetTorrentAutoManagement("all", enable);

    /// <summary>
    /// 切换指定种子的顺序下载模式。<br/>
    /// Toggle sequential download mode for the specified torrent.
    /// </summary>
    /// <param name="hash">
    /// 种子哈希值，可为单个或多个哈希值。<br/>
    /// Torrent hash value, can be a single hash or multiple hashes separated by '|'.
    /// </param>
    public async Task ToggleTorrentSequentialDownload(string hash) =>
        await PutHashes("toggleSequentialDownload", hash);

    /// <summary>
    /// 切换多个种子的顺序下载模式。<br/>
    /// Toggle sequential download mode for multiple torrents.
    /// </summary>
    /// <param name="hashes">种子哈希值列表。<br/>List of torrent hash values.</param>
    public async Task ToggleTorrentsSequentialDownload(List<string> hashes) =>
        await ToggleTorrentSequentialDownload(string.Join('|', hashes));

    /// <summary>
    /// 切换所有种子的顺序下载模式。<br/>
    /// Toggle sequential download mode for all torrents.
    /// </summary>
    public async Task ToggleAllTorrentsSequentialDownload() =>
        await ToggleTorrentSequentialDownload("all");

    /// <summary>
    /// 切换指定种子的首尾片段优先下载模式。<br/>
    /// Toggle first and last piece priority mode for the specified torrent.
    /// </summary>
    /// <param name="hash">
    /// 种子哈希值，可为单个或多个哈希值。<br/>
    /// Torrent hash value, can be a single hash or multiple hashes separated by '|'.
    /// </param>
    public async Task ToggleTorrentFirstLastPiecePriority(string hash) =>
        await PutHashes("toggleFirstLastPiecePrio", hash);

    /// <summary>
    /// 切换多个种子的首尾片段优先下载模式。<br/>
    /// Toggle first and last piece priority mode for multiple torrents.
    /// </summary>
    /// <param name="hashes">种子哈希值列表。<br/>List of torrent hash values.</param>
    public async Task ToggleTorrentsFirstLastPiecePriority(List<string> hashes) =>
        await ToggleTorrentFirstLastPiecePriority(string.Join('|', hashes));

    /// <summary>
    /// 切换所有种子的首尾片段优先下载模式。<br/>
    /// Toggle first and last piece priority mode for all torrents.
    /// </summary>
    public async Task ToggleAllTorrentsFirstLastPiecePriority() =>
        await ToggleTorrentFirstLastPiecePriority("all");

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
    public async Task SetTorrentForceStart(string hash, bool enable = true)
    {
        if (string.IsNullOrWhiteSpace(hash))
        {
            throw new ArgumentException("Torrent hash cannot be null or empty", nameof(hash));
        }

        var parameters = new Dictionary<string, string>
        {
            ["hashes"] = hash,
            ["value"]  = enable.ToString().ToLowerInvariant()
        };

        await netUtils.Post($"{BaseUrl}/setForceStart", parameters);
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
    public async Task SetTorrentsForceStart(List<string> hashes, bool enable = true) =>
        await SetTorrentForceStart(string.Join('|', hashes), enable);

    /// <summary>
    /// 启用或禁用所有种子的强制启动。<br/>
    /// Enable or disable force start for all torrents.
    /// </summary>
    /// <param name="enable">
    /// 是否启用强制启动（默认启用）。<br/>
    /// Whether to enable force start (enabled by default).
    /// </param>
    public async Task SetAllTorrentsForceStart(bool enable = true) => await SetTorrentForceStart("all", enable);

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
    public async Task SetTorrentSuperSeeding(string hash, bool enable = true)
    {
        if (string.IsNullOrWhiteSpace(hash))
        {
            throw new ArgumentException("Torrent hash cannot be null or empty", nameof(hash));
        }

        var parameters = new Dictionary<string, string>
        {
            ["hashes"] = hash,
            ["value"]  = enable.ToString().ToLowerInvariant()
        };

        await netUtils.Post($"{BaseUrl}/setSuperSeeding", parameters);
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
    public async Task SetTorrentSuperSeeding(List<string> hashes, bool enable = true) =>
        await SetTorrentSuperSeeding(string.Join('|', hashes), enable);

    /// <summary>
    /// 启用或禁用所有种子的超级做种模式。<br/>
    /// Enable or disable super seeding mode for all torrents.
    /// </summary>
    /// <param name="enable">
    /// 是否启用超级做种模式（默认启用）。<br/>
    /// Whether to enable super seeding mode (enabled by default).
    /// </param>
    public async Task SetTorrentSuperSeeding(bool enable = true) => await SetTorrentSuperSeeding("all", enable);

    /// <summary>
    /// 重命名种子中的文件。<br/>
    /// Rename a file in the torrent.
    /// </summary>
    /// <param name="hash">种子哈希值。<br/>Torrent hash value.</param>
    /// <param name="oldPath">原文件路径。<br/>Original file path.</param>
    /// <param name="newPath">新文件路径。<br/>New file path.</param>
    public async Task RenameTorrentFile(string hash, string oldPath, string newPath)
    {
        if (string.IsNullOrWhiteSpace(hash))
        {
            throw new ArgumentException("Torrent hash cannot be null or empty", nameof(hash));
        }

        if (string.IsNullOrWhiteSpace(oldPath))
        {
            throw new ArgumentException("Old path cannot be null or empty", nameof(oldPath));
        }

        if (string.IsNullOrWhiteSpace(newPath))
        {
            throw new ArgumentException("New path cannot be null or empty", nameof(newPath));
        }

        if (apiVersion < ApiVersion.V2_7_0)
        {
            var fileList = await GetTorrentFiles(hash);
            if (fileList == null || fileList.Count == 0) return;
            var index = fileList.FindIndex(f => f.Name == oldPath);
            if (index == -1) throw new ArgumentException("File path doesn't exist.", nameof(oldPath));
            await RenameTorrentFile(hash, index, newPath);
            return;
        }

        var parameters = new Dictionary<string, string>
        {
            { "hash", hash },
            { "oldPath", oldPath },
            { "newPath", newPath }
        };
        await netUtils.Post($"{BaseUrl}/renameFile", parameters);
    }

    /// <summary>
    /// 重命名种子中的文件（按索引）。<br/>
    /// Rename a file in the torrent by index.
    /// </summary>
    /// <param name="hash">种子哈希值。<br/>Torrent hash value.</param>
    /// <param name="index">文件索引。<br/>File index.</param>
    /// <param name="newPath">新文件路径。<br/>New file path.</param>
    public async Task RenameTorrentFile(string hash, int index, string newPath)
    {
        if (string.IsNullOrWhiteSpace(hash))
        {
            throw new ArgumentException("Torrent hash cannot be null or empty", nameof(hash));
        }

        if (index < 0)
        {
            throw new ArgumentException("Index should start from 0", nameof(index));
        }

        if (string.IsNullOrWhiteSpace(newPath))
        {
            throw new ArgumentException("New path cannot be null or empty", nameof(newPath));
        }

        if (apiVersion >= ApiVersion.V2_7_0)
        {
            var fileList = await GetTorrentFiles(hash);
            if (fileList is { Count: > 0 })
                await RenameTorrentFile(hash, fileList[index].Name, newPath);
            return;
        }

        var parameters = new Dictionary<string, string>
        {
            { "hash", hash },
            { "id", index.ToString() },
            { "newPath", newPath }
        };

        await netUtils.Post($"{BaseUrl}/renameFile", parameters);
    }

    /// <summary>
    /// 重命名种子中的文件夹。<br/>
    /// Rename a folder in the torrent.
    /// </summary>
    /// <param name="hash">种子哈希值。<br/>Torrent hash value.</param>
    /// <param name="oldPath">原文件夹路径。<br/>Original folder path.</param>
    /// <param name="newPath">新文件夹路径。<br/>New folder path.</param>
    public async Task RenameTorrentFolder(string hash, string oldPath, string newPath)
    {
        if (string.IsNullOrWhiteSpace(hash))
        {
            throw new ArgumentException("Torrent hash cannot be null or empty", nameof(hash));
        }

        if (string.IsNullOrWhiteSpace(oldPath))
        {
            throw new ArgumentException("Old path cannot be null or empty", nameof(oldPath));
        }

        if (string.IsNullOrWhiteSpace(newPath))
        {
            throw new ArgumentException("New path cannot be null or empty", nameof(newPath));
        }

        var parameters = new Dictionary<string, string>
        {
            { "hash", hash },
            { "oldPath", oldPath },
            { "newPath", newPath }
        };

        await netUtils.Post($"{BaseUrl}/renameFolder", parameters);
    }

    private async Task<string> Put(string subPath, string hash, ApiVersion? targetVersion = null)
    {
        if (string.IsNullOrWhiteSpace(hash))
        {
            throw new ArgumentException("Torrent hash cannot be null or empty", nameof(hash));
        }

        var parameters = new Dictionary<string, string>
        {
            { "hash", hash },
        };
        return await netUtils.Post($"{BaseUrl}/{subPath}", parameters, targetVersion);
    }

    private async Task<string> PutHashes(string subPath, string hash, ApiVersion? targetVersion = null)
    {
        if (string.IsNullOrWhiteSpace(hash)
         || hash.Split('|').All(string.IsNullOrWhiteSpace))
        {
            throw new ArgumentException("Torrent hash cannot be null or empty", nameof(hash));
        }

        var parameters = new Dictionary<string, string>
        {
            { "hashes", hash },
        };
        return await netUtils.Post($"{BaseUrl}/{subPath}", parameters, targetVersion);
    }
}
