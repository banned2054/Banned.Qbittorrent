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
    private const    string     BaseUrl      = "/api/v2/torrents";
    private readonly ApiVersion _apiVersion5 = new("2.11.0");

    /// <summary>
    /// 获取种子信息列表。<br/>
    /// Get torrent information list.
    /// </summary>
    /// <param name="filter">种子过滤条件。<br/>Torrent filter condition.</param>
    /// <param name="category">分类名称。<br/>Category name.</param>
    /// <param name="tag">标签名称。<br/>Tag name.</param>
    /// <param name="sort">排序字段。<br/>Sort field.</param>
    /// <param name="reverse">是否反向排序。<br/>Whether to sort in reverse order.</param>
    /// <param name="limit">返回结果数量限制。<br/>Limit on the number of results returned.</param>
    /// <param name="offset">结果偏移量。<br/>Result offset.</param>
    /// <param name="hash">单个种子的哈希值。<br/>Hash value of a single torrent.</param>
    /// <returns>
    /// 种子信息列表。<br/>
    /// List of torrent information.
    /// </returns>
    public async Task<List<TorrentInfo>> GetTorrentInfo(EnumTorrentFilter filter   = EnumTorrentFilter.All,
                                                        string?           category = null,
                                                        string?           tag      = null,
                                                        string?           sort     = null,
                                                        bool              reverse  = false,
                                                        int               limit    = 0,
                                                        int               offset   = 0,
                                                        string            hash     = "")
    {
        var hashList = hash == "" ? null : new List<string> { hash };
        return await GetTorrentInfos(filter, category, tag, sort, reverse, limit, offset, hashList);
    }

    /// <summary>
    /// 获取种子信息列表。<br/>
    /// Get torrent information list.
    /// </summary>
    /// <param name="filter">种子过滤条件。<br/>Torrent filter condition.</param>
    /// <param name="category">分类名称。<br/>Category name.</param>
    /// <param name="tag">标签名称。<br/>Tag name.</param>
    /// <param name="sort">排序字段。<br/>Sort field.</param>
    /// <param name="reverse">是否反向排序。<br/>Whether to sort in reverse order.</param>
    /// <param name="limit">返回结果数量限制。<br/>Limit on the number of results returned.</param>
    /// <param name="offset">结果偏移量。<br/>Result offset.</param>
    /// <param name="hashList">种子哈希值列表。<br/>List of torrent hash values.</param>
    /// <returns>
    /// 种子信息列表。<br/>
    /// List of torrent information.
    /// </returns>
    public async Task<List<TorrentInfo>> GetTorrentInfos(EnumTorrentFilter filter   = EnumTorrentFilter.All,
                                                         string?           category = null,
                                                         string?           tag      = null,
                                                         string?           sort     = null,
                                                         bool              reverse  = false,
                                                         int               limit    = 0,
                                                         int               offset   = 0,
                                                         List<string>?     hashList = null)
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
        if (hashList is { Count: > 0 })
        {
            parameters.Add("hashes", string.Join("|", hashList));
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
            parameters.Add("hashes", string.Join("|", request.HashList));
        }

        var response = await netUtils.Post($"{BaseUrl}/info", parameters);
        return JsonSerializer.Deserialize<List<TorrentInfo>>(response) ?? [];
    }

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
    /// <param name="hashList">种子哈希值列表。<br/>List of torrent hash values.</param>
    /// <param name="deleteFile">是否同时删除文件。<br/>Whether to delete files as well.</param>
    public async Task DeleteTorrent(List<string> hashList, bool deleteFile = false) =>
        await DeleteTorrent(string.Join('|', hashList.ToArray()), deleteFile);

    /// <summary>
    /// 继续下载/做种指定种子。<br/>
    /// Resume the specified torrent.
    /// </summary>
    /// <param name="hash">种子哈希值。<br/>Torrent hash value.</param>
    public async Task ResumeTorrent(string hash)
    {
        var parameters = new Dictionary<string, string>
        {
            { "hashes", hash },
        };
        if (apiVersion < _apiVersion5)
            await netUtils.Post($"{BaseUrl}/resume", parameters);
        else
            await netUtils.Post($"{BaseUrl}/start", parameters);
    }

    /// <summary>
    /// 继续下载/做种多个种子。<br/>
    /// Resume multiple torrents.
    /// </summary>
    /// <param name="hashList">种子哈希值列表。<br/>List of torrent hash values.</param>
    public async Task ResumeTorrent(List<string> hashList)
    {
        var hash = string.Join('|', hashList.ToArray());
        await ResumeTorrent(hash);
    }

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
    /// <param name="hashList">种子哈希值列表。<br/>List of torrent hash values.</param>
    public async Task RecheckTorrent(List<string> hashList) =>
        await RecheckTorrent(string.Join('|', hashList.ToArray()));

    /// <summary>
    /// 暂停指定种子。<br/>
    /// Pause the specified torrent.
    /// </summary>
    /// <param name="hash">种子哈希值。<br/>Torrent hash value.</param>
    public async Task PauseTorrent(string hash) => await PutHashes(apiVersion < _apiVersion5 ? "pause" : "stop", hash);

    /// <summary>
    /// 暂停多个种子。<br/>
    /// Pause multiple torrents.
    /// </summary>
    /// <param name="hashList">种子哈希值列表。<br/>List of torrent hash values.</param>
    public async Task PauseTorrent(List<string> hashList) => await PauseTorrent(string.Join('|', hashList.ToArray()));

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
        if (apiVersion < _apiVersion5)
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
    /// 获取种子的文件列表。<br/>
    /// Get the file list of a torrent.
    /// </summary>
    /// <param name="hash">种子哈希值。<br/>Torrent hash value.</param>
    /// <param name="indexes">文件索引列表。<br/>List of file indexes.</param>
    /// <returns>
    /// 种子文件信息列表。<br/>
    /// List of torrent file information.
    /// </returns>
    public async Task<List<TorrentFileInfo>> GetTorrentFiles(string hash, List<int>? indexes = null)
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
            parameters["indexes"] = string.Join("|", indexes);
        }

        try
        {
            var response = await netUtils.Post(requestUrl, parameters);
            var fileList = JsonSerializer.Deserialize<List<TorrentFileInfo>>(response);

            return fileList ?? [];
        }
        catch (QbittorrentNotFoundException)
        {
            return [];
        }
    }

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


        if (apiVersion < new ApiVersion(2, 7))
        {
            var fileList = await GetTorrentFiles(hash);
            var index    = fileList.FindIndex(f => f.Name == oldPath);
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

        if (apiVersion >= new ApiVersion(2, 7))
        {
            var fileList = await GetTorrentFiles(hash);
            await RenameTorrentFile(hash, fileList[index].Name, newPath);
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

    /// <summary>
    /// 重新向 Tracker 汇报指定种子。<br/>
    /// Reannounce the specified torrent to the tracker.
    /// </summary>
    /// <param name="hash">种子哈希值。<br/>Torrent hash value.</param>
    public async Task ReannounceTorrent(string hash) => await PutHashes("reannounce", hash, new ApiVersion("2.0.2"));

    /// <summary>
    /// 重新向 Tracker 汇报多个种子。<br/>
    /// Reannounce multiple torrents to the tracker.
    /// </summary>
    /// <param name="hashList">种子哈希值列表。<br/>List of torrent hash values.</param>
    public async Task ReannounceTorrent(List<string> hashList) =>
        await ReannounceTorrent(string.Join('|', hashList.ToArray()));

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
    /// 设置种子中文件的优先度。<br/>
    /// Set file priority in a torrent.
    /// </summary>
    /// <param name="hash">种子哈希值。<br/>Torrent hash value.</param>
    /// <param name="fileIndex">文件索引。<br/>File index.</param>
    /// <param name="priority">文件优先度。<br/>File priority.</param>
    public async Task SetFilePriority(string hash, int fileIndex, EnumTorrentFilePriority priority)
    {
        if (string.IsNullOrWhiteSpace(hash))
        {
            throw new ArgumentException("Torrent hash cannot be null or empty", nameof(hash));
        }

        if (fileIndex < 0)
        {
            throw new ArgumentException("File index should start from 0", nameof(fileIndex));
        }

        var parameters = new Dictionary<string, string>
        {
            { "hash", hash },
            { "id", fileIndex.ToString() },
            { "priority", ((int)priority).ToString() }
        };

        await netUtils.Post($"{BaseUrl}/filePrio", parameters);
    }

    /// <summary>
    /// 设置种子中多个文件的优先度。<br/>
    /// Set priorities for multiple files in a torrent.
    /// </summary>
    /// <param name="hash">种子哈希值。<br/>Torrent hash value.</param>
    /// <param name="fileIndexes">文件索引列表。<br/>List of file indexes.</param>
    /// <param name="priority">文件优先度。<br/>File priority.</param>
    public async Task SetFilePriority(string hash, List<int> fileIndexes, EnumTorrentFilePriority priority)
    {
        if (string.IsNullOrWhiteSpace(hash))
        {
            throw new ArgumentException("Torrent hash cannot be null or empty", nameof(hash));
        }

        if (fileIndexes == null || fileIndexes.Count == 0)
        {
            throw new ArgumentException("File indexes cannot be null or empty", nameof(fileIndexes));
        }

        var parameters = new Dictionary<string, string>
        {
            { "hash", hash },
            { "ids", string.Join("|", fileIndexes) },
            { "priority", ((int)priority).ToString() }
        };

        await netUtils.Post($"{BaseUrl}/filePrio", parameters);
    }

    /// <summary>
    /// 设置多个种子的存储位置。<br/>
    /// Set the storage location for multiple torrents.
    /// </summary>
    /// <param name="hashList">种子哈希值列表。<br/>List of torrent hash values.</param>
    /// <param name="newLocation">新的存储路径。<br/>New storage location.</param>
    public async Task SetLocation(List<string> hashList, string newLocation) =>
        await SetLocation(string.Join('|', hashList), newLocation);

    /// <summary>
    /// 设置指定种子的存储位置。<br/>
    /// Set the storage location for the specified torrent(s).
    /// </summary>
    /// <param name="hash">
    /// 种子哈希值，或由“|”分隔的多个哈希值。<br/>
    /// Torrent hash value, or multiple hashes separated by '|'.
    /// </param>
    /// <param name="newLocation">新的存储路径。<br/>New storage location.</param>
    public async Task SetLocation(string hash, string newLocation)
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
    /// 获取指定种子的通用属性。<br/>
    /// Get generic properties of the specified torrent.
    /// </summary>
    /// <param name="hash">种子哈希值。<br/>Torrent hash value.</param>
    /// <returns>
    /// 包含属性的 <see cref="TorrentProperties"/>；获取失败返回 <c>null</c>。<br/>
    /// A <see cref="TorrentProperties"/> if successful; otherwise <c>null</c>.
    /// </returns>
    public async Task<TorrentProperties?> GetTorrentGenericProperties(string hash) =>
        JsonSerializer.Deserialize<TorrentProperties>(await Put(hash, "properties"));

    /// <summary>
    /// 获取指定种子的 Tracker 信息。<br/>
    /// Get tracker information for the specified torrent.
    /// </summary>
    /// <param name="hash">种子哈希值。<br/>Torrent hash value.</param>
    /// <returns>
    /// 包含 Tracker 信息的列表；无数据返回空列表。<br/>
    /// A list of <see cref="TrackerInfo"/>; an empty list if no data is available.
    /// </returns>
    public async Task<List<TrackerInfo>> GetTorrentTrackers(string hash) =>
        JsonSerializer.Deserialize<List<TrackerInfo>>(await Put(hash, "trackers")) ?? [];

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
    /// 删除指定种子的多个 Tracker。<br/>
    /// Remove multiple trackers from the specified torrent.
    /// </summary>
    /// <param name="hash">种子哈希值。<br/>Torrent hash value.</param>
    /// <param name="urlList">要删除的 Tracker 地址列表。<br/>List of tracker URLs to remove.</param>
    public async Task RemoveTorrentTrackers(string hash, List<string> urlList) =>
        await RemoveTorrentTrackers(hash, string.Join("|", urlList));

    /// <summary>
    /// 删除指定种子的 Tracker。<br/>
    /// Remove tracker(s) from the specified torrent.
    /// </summary>
    /// <param name="hash">种子哈希值。<br/>Torrent hash value.</param>
    /// <param name="url">
    /// 要删除的 Tracker 地址，多个以“|”分隔。<br/>
    /// Tracker URL(s) to remove, separated by '|'.
    /// </param>
    /// <exception cref="QbittorrentConflictException">
    /// 当所有指定的 Tracker 地址均未找到时抛出。<br/>
    /// Thrown when all specified tracker URLs are not found.
    /// </exception>
    public async Task RemoveTorrentTrackers(string hash, string url)
    {
        if (string.IsNullOrWhiteSpace(hash))
        {
            throw new ArgumentException("Torrent hash cannot be null or empty", nameof(hash));
        }

        var parameters = new Dictionary<string, string>
        {
            { "hash", hash },
            { "urls", url },
        };
        try
        {
            await netUtils.Post($"{BaseUrl}/removeTrackers", parameters, new ApiVersion(2, 2));
        }
        catch (QbittorrentConflictException)
        {
            throw new QbittorrentConflictException("All urls were not found");
        }
    }

    /// <summary>
    /// 向指定种子添加一个或多个 Peer。<br/>
    /// Add one or more peers to the specified torrent.
    /// </summary>
    /// <param name="hash">种子哈希值。<br/>Torrent hash value.</param>
    /// <param name="peer">要添加的 Peer 地址（可为 IP:Port 格式）。<br/>Peer address to add (can be in IP:Port format).</param>
    /// <exception cref="ArgumentException">
    /// 当 <paramref name="hash"/> 或 <paramref name="peer"/> 为空时抛出。<br/>
    /// Thrown when <paramref name="hash"/> or <paramref name="peer"/> is null or empty.
    /// </exception>
    /// <exception cref="QbittorrentConflictException">
    /// 当所有指定的 Peer 地址均未找到或添加失败时抛出。<br/>
    /// Thrown when all specified peer addresses are not found or failed to add.
    /// </exception>
    public async Task AddPeers(string hash, string peer)
    {
        if (string.IsNullOrWhiteSpace(hash))
        {
            throw new ArgumentException("Torrent hash cannot be null or empty", nameof(hash));
        }

        if (string.IsNullOrWhiteSpace(peer))
        {
            throw new ArgumentException("Peer cannot be null or empty", nameof(hash));
        }

        var parameters = new Dictionary<string, string>
        {
            { "hash", hash },
            { "peers", peer },
        };
        try
        {
            await netUtils.Post($"{BaseUrl}/addPeers", parameters, new ApiVersion(2, 3));
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
    /// <param name="peer">要添加的 Peer 地址列表。<br/>List of peer addresses to add.</param>
    public async Task AddPeers(string hash, List<string> peer) => await AddPeers(hash, string.Join('|', peer));

    /// <summary>
    /// 向多个种子添加一个 Peer。<br/>
    /// Add a peer to multiple torrents.
    /// </summary>
    /// <param name="hash">种子哈希值列表。<br/>List of torrent hash values.</param>
    /// <param name="peer">要添加的 Peer 地址。<br/>Peer address to add.</param>
    public async Task AddPeers(List<string> hash, string peer) => await AddPeers(string.Join('|', hash), peer);

    /// <summary>
    /// 向多个种子添加多个 Peer。<br/>
    /// Add multiple peers to multiple torrents.
    /// </summary>
    /// <param name="hash">种子哈希值列表。<br/>List of torrent hash values.</param>
    /// <param name="peer">要添加的 Peer 地址列表。<br/>List of peer addresses to add.</param>
    public async Task AddPeers(List<string> hash, List<string> peer) =>
        await AddPeers(string.Join('|', hash), string.Join('|', peer));

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

    private async Task PutHashes(string subPath, string hash, ApiVersion? targetVersion = null)
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
        await netUtils.Post($"{BaseUrl}/{subPath}", parameters, targetVersion);
    }
}
