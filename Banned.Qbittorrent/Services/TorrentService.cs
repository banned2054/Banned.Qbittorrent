using Banned.Qbittorrent.Models.Application;
using Banned.Qbittorrent.Models.Enums;
using Banned.Qbittorrent.Models.Requests;
using Banned.Qbittorrent.Models.Torrent;
using Banned.Qbittorrent.Utils;
using System.Net;
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
    /// 获取种子信息列表<br/>
    /// Get torrent information list
    /// </summary>
    /// <param name="filter">种子过滤条件<br/>Torrent filter condition</param>
    /// <param name="category">分类名称<br/>Category name</param>
    /// <param name="tag">标签名称<br/>Tag name</param>
    /// <param name="sort">排序字段<br/>Sort field</param>
    /// <param name="reverse">是否反向排序<br/>Whether to sort in reverse order</param>
    /// <param name="limit">返回结果数量限制<br/>Limit on the number of results returned</param>
    /// <param name="offset">结果偏移量<br/>Result offset</param>
    /// <param name="hash">单个种子的哈希值<br/>Hash value of a single torrent</param>
    /// <returns>种子信息列表<br/>List of torrent information</returns>
    public async Task<List<TorrentInfo>> GetTorrentInfoAsync(EnumTorrentFilter filter   = EnumTorrentFilter.All,
                                                             string?           category = null,
                                                             string?           tag      = null,
                                                             string?           sort     = null,
                                                             bool              reverse  = false,
                                                             int               limit    = 0,
                                                             int               offset   = 0,
                                                             string            hash     = "")
    {
        var hashList = hash == "" ? null : new List<string> { hash };
        return await GetTorrentInfosAsync(filter, category, tag, sort, reverse, limit, offset, hashList);
    }

    /// <summary>
    /// 获取种子信息列表<br/>
    /// Get torrent information list
    /// </summary>
    /// <param name="filter">种子过滤条件<br/>Torrent filter condition</param>
    /// <param name="category">分类名称<br/>Category name</param>
    /// <param name="tag">标签名称<br/>Tag name</param>
    /// <param name="sort">排序字段<br/>Sort field</param>
    /// <param name="reverse">是否反向排序<br/>Whether to sort in reverse order</param>
    /// <param name="limit">返回结果数量限制<br/>Limit on the number of results returned</param>
    /// <param name="offset">结果偏移量<br/>Result offset</param>
    /// <param name="hashList">种子哈希值列表<br/>List of torrent hash values</param>
    /// <returns>种子信息列表<br/>List of torrent information</returns>
    public async Task<List<TorrentInfo>> GetTorrentInfosAsync(EnumTorrentFilter filter   = EnumTorrentFilter.All,
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

        var response = await netUtils.PostAsync($"{BaseUrl}/info", parameters);
        return response.Item1 == HttpStatusCode.OK ? StringToTorrentInfoList(response.Item2) : new List<TorrentInfo>();
    }

    /// <summary>
    /// 使用请求对象获取种子信息列表<br/>
    /// Get torrent information list using request object
    /// </summary>
    /// <param name="request">获取种子信息列表的请求参数<br/>Request parameters for getting torrent information list</param>
    /// <returns>种子信息列表<br/>List of torrent information</returns>
    public async Task<List<TorrentInfo>> GetTorrentInfosAsync(GetTorrentInfoListRequest request)
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

        var response = await netUtils.PostAsync($"{BaseUrl}/info", parameters);
        return response.Item1 == HttpStatusCode.OK ? StringToTorrentInfoList(response.Item2) : [];
    }

    private List<TorrentInfo> StringToTorrentInfoList(string jsonString)
    {
        var options = new JsonSerializerOptions();
        if (apiVersion < _apiVersion5)
        {
            options.Converters.Add(new TorrentInfoConverterV4());
        }
        else
        {
            options.Converters.Add(new TorrentInfoConverterV5());
        }

        var torrentInfos = JsonSerializer.Deserialize<List<TorrentInfo>>(jsonString, options) ?? [];
        return torrentInfos;
    }

    /// <summary>
    /// 删除指定种子<br/>
    /// Delete specified torrent
    /// </summary>
    /// <param name="hash">种子哈希值<br/>Torrent hash value</param>
    /// <param name="deleteFile">是否同时删除文件<br/>Whether to delete files as well</param>
    public async Task DeleteTorrentAsync(string hash, bool deleteFile = false)
    {
        var parameters = new Dictionary<string, string>
        {
            { "hashes", hash },
            { "deleteFiles", deleteFile.ToString().ToLower() }
        };

        await netUtils.PostAsync($"{BaseUrl}/delete", parameters);
    }

    public async Task DeleteTorrentAsync(List<string> hashList, bool deleteFile = false) =>
        await DeleteTorrentAsync(string.Join('|', hashList.ToArray()), deleteFile);

    /// <summary>
    /// 继续下载/做种指定种子<br/>
    /// Resume specified torrent
    /// </summary>
    /// <param name="hash">种子哈希值<br/>Torrent hash value</param>
    public async Task ResumeTorrentAsync(string hash)
    {
        var parameters = new Dictionary<string, string>
        {
            { "hashes", hash },
        };
        if (apiVersion < _apiVersion5)
            await netUtils.PostAsync($"{BaseUrl}/resume", parameters);
        else
            await netUtils.PostAsync($"{BaseUrl}/start", parameters);
    }

    /// <summary>
    /// 继续下载/做种多个种子<br/>
    /// Resume multiple torrents
    /// </summary>
    /// <param name="hashList">种子哈希值列表<br/>List of torrent hash values</param>
    public async Task ResumeTorrentAsync(List<string> hashList)
    {
        var hash = string.Join('|', hashList.ToArray());
        await ResumeTorrentAsync(hash);
    }

    /// <summary>
    /// 重新校验指定种子的进度<br/>
    /// Recheck specified torrent progress
    /// </summary>
    /// <param name="hash">种子哈希值<br/>Torrent hash value</param>
    public async Task RecheckTorrentAsync(string hash)
    {
        var parameters = new Dictionary<string, string>
        {
            { "hashes", hash },
        };
        await netUtils.PostAsync($"{BaseUrl}/recheck", parameters);
    }

    /// <summary>
    /// 重新校验多个种子的进度<br/>
    /// Recheck multiple torrents progress
    /// </summary>
    /// <param name="hashList">种子哈希值列表<br/>List of torrent hash values</param>
    public async Task RecheckTorrentAsync(List<string> hashList) =>
        await RecheckTorrentAsync(string.Join('|', hashList.ToArray()));

    /// <summary>
    /// 暂停指定种子<br/>
    /// Pause specified torrent
    /// </summary>
    /// <param name="hash">种子哈希值<br/>Torrent hash value</param>
    public async Task StopTorrentAsync(string hash) => await PauseTorrentAsync(hash);

    /// <summary>
    /// 暂停指定种子<br/>
    /// Pause specified torrent
    /// </summary>
    /// <param name="hash">种子哈希值<br/>Torrent hash value</param>
    public async Task PauseTorrentAsync(string hash)
    {
        var parameters = new Dictionary<string, string>
        {
            { "hashes", hash },
        };
        if (apiVersion < _apiVersion5)
            await netUtils.PostAsync($"{BaseUrl}/pause", parameters);
        else
            await netUtils.PostAsync($"{BaseUrl}/stop", parameters);
    }

    /// <summary>
    /// 暂停多个种子<br/>
    /// Pause multiple torrents
    /// </summary>
    /// <param name="hashList">种子哈希值列表<br/>List of torrent hash values</param>
    public async Task StopTorrentAsync(List<string> hashList) => await PauseTorrentAsync(hashList);

    /// <summary>
    /// 暂停多个种子<br/>
    /// Pause multiple torrents
    /// </summary>
    /// <param name="hashList">种子哈希值列表<br/>List of torrent hash values</param>
    public async Task PauseTorrentAsync(List<string> hashList) =>
        await PauseTorrentAsync(string.Join('|', hashList.ToArray()));

    /// <summary>
    /// 添加种子文件或 URL<br/>
    /// Add torrent file or URL
    /// </summary>
    /// <param name="request">添加种子的请求参数<br/>Request parameters for adding torrent</param>
    /// <returns>操作结果信息<br/>Operation result information</returns>
    public async Task<string> AddTorrentAsync(AddTorrentRequest request)
    {
        var parameters = request.ToDictionary();

        if (request.FilePaths is { Count: > 0 })
        {
            var result = await netUtils.PostWithFilesAsync($"{BaseUrl}/add", parameters, request.FilePaths);
            return result.Item2;
        }

        if (request.Urls is { Count: > 0 })
        {
            var result = await netUtils.PostAsync($"{BaseUrl}/add", parameters);
            return result.Item2;
        }

        return "No torrent file or URL provided.";
    }

    /// <summary>
    /// 添加种子文件或 URL<br/>
    /// Add torrent file or URL
    /// </summary>
    /// <param name="filePaths">种子文件路径列表<br/>List of torrent file paths</param>
    /// <param name="urls">种子 URL 列表<br/>List of torrent URLs</param>
    /// <param name="savePath">保存路径<br/>Save path</param>
    /// <param name="category">分类名称<br/>Category name</param>
    /// <param name="tags">标签名称<br/>Tag name</param>
    /// <param name="skipChecking">是否跳过检查<br/>Whether to skip checking</param>
    /// <param name="stopped">是否停止下载<br/>Whether to stop downloading</param>
    /// <param name="paused">是否暂停下载<br/>Whether to pause downloading</param>
    /// <param name="rootFolder">是否创建根文件夹<br/>Whether to create root folder</param>
    /// <param name="rename">重命名<br/>Rename</param>
    /// <param name="uploadLimit">上传限制<br/>Upload limit</param>
    /// <param name="downloadLimit">下载限制<br/>Download limit</param>
    /// <param name="ratioLimit">分享率限制<br/>Share ratio limit</param>
    /// <param name="seedingTimeLimit">做种时间限制<br/>Seeding time limit</param>
    /// <param name="autoTmm">是否自动管理<br/>Whether to auto manage</param>
    /// <param name="sequentialDownload">是否顺序下载<br/>Whether to download sequentially</param>
    /// <param name="firstLastPiecePriority">是否优先下载首尾块<br/>Whether to prioritize first and last pieces</param>
    /// <returns>操作结果信息<br/>Operation result information</returns>
    public async Task<string> AddTorrentAsync(
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

        return await AddTorrentAsync(request);
    }

    /// <summary>
    /// 获取种子的文件列表<br/>
    /// Get torrent file list
    /// </summary>
    /// <param name="hash">种子哈希值<br/>Torrent hash value</param>
    /// <param name="indexes">文件索引列表<br/>List of file indexes</param>
    /// <returns>种子文件信息列表<br/>List of torrent file information</returns>
    /// <exception cref="ArgumentException">当种子哈希值为空时抛出<br/>Thrown when torrent hash is empty</exception>
    public async Task<List<TorrentFileInfo>> GetTorrentContentsAsync(string hash, List<int>? indexes = null)
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

        var response = await netUtils.PostAsync(requestUrl, parameters);

        if (response.Item1 == HttpStatusCode.NotFound)
        {
            return [];
        }

        var fileList = JsonSerializer.Deserialize<List<TorrentFileInfo>>(response.Item2, new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        });

        return fileList ?? [];
    }

    /// <summary>
    /// 重命名种子中的文件<br/>
    /// Rename file in torrent
    /// </summary>
    /// <param name="hash">种子哈希值<br/>Torrent hash value</param>
    /// <param name="oldPath">原文件路径<br/>Original file path</param>
    /// <param name="newPath">新文件路径<br/>New file path</param>
    /// <returns>操作是否成功<br/>Whether the operation was successful</returns>
    /// <exception cref="ArgumentException">当参数无效时抛出<br/>Thrown when parameters are invalid</exception>
    public async Task<bool> RenameTorrentFileAsync(string hash, string oldPath, string newPath)
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
            var fileList = await GetTorrentContentsAsync(hash);
            for (var i = 0; i < fileList.Count; i++)
            {
                if (fileList[i].Name == oldPath)
                {
                    return await RenameTorrentFileAsync(hash, i, newPath);
                }
            }

            throw new ArgumentException("File path doesn't exist.", nameof(oldPath));
        }

        var parameters = new Dictionary<string, string>
        {
            { "hash", hash },
            { "oldPath", oldPath },
            { "newPath", newPath }
        };

        var response = await netUtils.PostAsync($"{BaseUrl}/renameFile", parameters);

        switch (response.Item1)
        {
            case HttpStatusCode.OK :
                return true;
            case HttpStatusCode.BadRequest :
                Console.WriteLine("Error: Missing newPath parameter.");
                return false;
            case HttpStatusCode.Conflict :
                Console.WriteLine("Error: Invalid newPath, oldPath, or newPath is already in use.");
                return false;
            default :
                Console.WriteLine($"Unexpected error: {response.Item1}");
                return false;
        }
    }

    /// <summary>
    /// 重命名种子中的文件<br/>
    /// Rename file in torrent
    /// </summary>
    /// <param name="hash">种子哈希值<br/>Torrent hash value</param>
    /// <param name="index">文件索引<br/>File index</param>
    /// <param name="newPath">新文件路径<br/>New file path</param>
    /// <returns>操作是否成功<br/>Whether the operation was successful</returns>
    /// <exception cref="ArgumentException">当参数无效时抛出<br/>Thrown when parameters are invalid</exception>
    public async Task<bool> RenameTorrentFileAsync(string hash, int index, string newPath)
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
            var fileList = await GetTorrentContentsAsync(hash);
            return await RenameTorrentFileAsync(hash, fileList[index].Name, newPath);
        }

        var parameters = new Dictionary<string, string>
        {
            { "hash", hash },
            { "id", index.ToString() },
            { "newPath", newPath }
        };

        var response = await netUtils.PostAsync($"{BaseUrl}/renameFile", parameters);

        switch (response.Item1)
        {
            case HttpStatusCode.OK :
                return true;
            case HttpStatusCode.BadRequest :
                Console.WriteLine("Error: Missing newPath parameter.");
                return false;
            case HttpStatusCode.Conflict :
                Console.WriteLine("Error: Invalid newPath, oldPath, or newPath is already in use.");
                return false;
            default :
                Console.WriteLine($"Unexpected error: {response.Item1}");
                return false;
        }
    }

    /// <summary>
    /// 重命名种子中的文件夹<br/>
    /// Rename folder in torrent
    /// </summary>
    /// <param name="hash">种子哈希值<br/>Torrent hash value</param>
    /// <param name="oldPath">原文件夹路径<br/>Original folder path</param>
    /// <param name="newPath">新文件夹路径<br/>New folder path</param>
    /// <returns>操作是否成功<br/>Whether the operation was successful</returns>
    /// <exception cref="ArgumentException">当参数无效时抛出<br/>Thrown when parameters are invalid</exception>
    public async Task<bool> RenameTorrentFolderAsync(string hash, string oldPath, string newPath)
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

        var response = await netUtils.PostAsync($"{BaseUrl}/renameFolder", parameters);

        switch (response.Item1)
        {
            case HttpStatusCode.OK :
                return true;
            case HttpStatusCode.BadRequest :
                Console.WriteLine("Error: Missing newPath parameter.");
                return false;
            case HttpStatusCode.Conflict :
                Console.WriteLine("Error: Invalid newPath, oldPath, or newPath is already in use.");
                return false;
            default :
                Console.WriteLine($"Unexpected error: {response.Item1}");
                return false;
        }
    }

    /// <summary>
    /// 重新向 tracker 汇报指定种子<br/>
    /// Reannounce specified torrent to tracker
    /// </summary>
    /// <param name="hash">种子哈希值<br/>Torrent hash value</param>
    public async Task ReannounceTorrentAsync(string hash)
    {
        if (apiVersion < new ApiVersion("2.0.2"))
        {
            throw new Exception("This method was introduced with qBittorrent v4.1.2 (Web API v2.0.2).");
        }

        var parameters = new Dictionary<string, string>
        {
            { "hashes", hash },
        };

        await netUtils.PostAsync($"{BaseUrl}/reannounce", parameters);
    }

    /// <summary>
    /// 重新向 tracker 汇报多个种子<br/>
    /// Reannounce multiple torrents to tracker
    /// </summary>
    /// <param name="hashList">种子哈希值列表<br/>List of torrent hash values</param>
    public async Task ReannounceTorrentAsync(List<string> hashList) =>
        await ReannounceTorrentAsync(string.Join('|', hashList.ToArray()));

    /// <summary>
    /// 设置种子中文件的优先度<br/>
    /// Set file priority in torrent
    /// </summary>
    /// <param name="hash">种子哈希值<br/>Torrent hash value</param>
    /// <param name="fileIndex">文件索引<br/>File index</param>
    /// <param name="priority">文件优先度<br/>File priority</param>
    /// <returns>操作是否成功<br/>Whether the operation was successful</returns>
    /// <exception cref="ArgumentException">当参数无效时抛出<br/>Thrown when parameters are invalid</exception>
    public async Task<bool> SetFilePriorityAsync(string hash, int fileIndex, EnumTorrentFilePriority priority)
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

        var response = await netUtils.PostAsync($"{BaseUrl}/filePrio", parameters);
        return response.Item1 == HttpStatusCode.OK;
    }

    /// <summary>
    /// 设置种子中多个文件的优先度<br/>
    /// Set multiple files priority in torrent
    /// </summary>
    /// <param name="hash">种子哈希值<br/>Torrent hash value</param>
    /// <param name="fileIndexes">文件索引列表<br/>List of file indexes</param>
    /// <param name="priority">文件优先度<br/>File priority</param>
    /// <returns>操作是否成功<br/>Whether the operation was successful</returns>
    /// <exception cref="ArgumentException">当参数无效时抛出<br/>Thrown when parameters are invalid</exception>
    public async Task<bool> SetFilePriorityAsync(string hash, List<int> fileIndexes, EnumTorrentFilePriority priority)
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

        var response = await netUtils.PostAsync($"{BaseUrl}/filePrio", parameters);
        return response.Item1 == HttpStatusCode.OK;
    }

    public async Task SetLocationAsync(List<string> hashList, string newLocation)
        => await SetLocationAsync(string.Join('|', hashList), newLocation);

    public async Task SetLocationAsync(string hash, string newLocation)
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
        var response = await netUtils.PostAsync($"{BaseUrl}/setLocation", parameters);
        switch (response.Item1)
        {
            case HttpStatusCode.OK :
                return;
            case HttpStatusCode.BadRequest : // 400
                throw new HttpRequestException("Save path is empty");
            case HttpStatusCode.Forbidden : // 403
                throw new HttpRequestException("User does not have write access to directory");
            case HttpStatusCode.Conflict : // 409
                throw new HttpRequestException("Unable to create save path directory");
        }
    }
}
