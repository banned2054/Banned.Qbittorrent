using Banned.Qbittorrent.Models.Application;
using Banned.Qbittorrent.Models.Rss;
using Banned.Qbittorrent.Serialization;
using System.Text.Json;

namespace Banned.Qbittorrent.Services;

/// <summary>
/// 提供与 qBittorrent RSS 相关的服务<br/>
/// Provides services related to qBittorrent RSS
/// </summary>
public class RssService(NetService netService)
{
    private const string BaseUrl = "/api/v2/rss";

    /// <summary>
    /// 添加 RSS 文件夹。<br/>
    /// Add RSS folder.
    /// </summary>
    /// <param name="path">文件夹路径。Folder path.</param>
    /// <param name="cancellationToken">取消请求的令牌。<br/>Token used to cancel the request.</param>
    public async Task AddFolder(string path, CancellationToken cancellationToken = default)
    {
        var parameters = new Dictionary<string, string>
        {
            { "path", path }
        };
        await netService.Post($"{BaseUrl}/addFolder", parameters, ct : cancellationToken);
    }

    /// <summary>
    /// 添加 RSS 订阅源。<br/>
    /// Add RSS feed.
    /// </summary>
    /// <param name="url">订阅源 URL。Feed URL.</param>
    /// <param name="path">保存订阅源的完整路径。Full path of the folder where the feed will be placed.</param>
    /// <param name="refreshInterval">刷新间隔（分钟）。Refresh interval (in minutes).</param>
    /// <param name="cancellationToken">取消请求的令牌。<br/>Token used to cancel the request.</param>
    public async Task AddFeed(string  url,
                              string? path            = null,
                              int?    refreshInterval = null, CancellationToken cancellationToken = default)
    {
        var parameters = new Dictionary<string, string?>
            {
                { "url", url },
                { "path", path },
                { "refreshInterval", refreshInterval?.ToString() }
            }.Where(kv => kv.Value != null)
             .ToDictionary(kv => kv.Key, kv => kv.Value!);

        await netService.Post($"{BaseUrl}/addFeed", parameters, ct : cancellationToken);
    }

    /// <summary>
    /// 更新现有 RSS 订阅源的 URL。<br/>
    /// Update the URL for an existing RSS feed.
    /// </summary>
    /// <param name="itemPath">订阅源的项目路径（例如：Folder\Subfolder\FeedName）。Name and/or path for feed.</param>
    /// <param name="url">新的 RSS 订阅源 URL。New URL of RSS feed.</param>
    /// <param name="cancellationToken">取消请求的令牌。<br/>Token used to cancel the request.</param>
    /// <remarks>
    /// 此方法自 qBittorrent v4.6.0 (Web API v2.9.1) 起引入。<br/>
    /// This method was introduced with qBittorrent v4.6.0 (Web API v2.9.1).
    /// </remarks>
    public async Task SetFeedUrl(string itemPath, string url, CancellationToken cancellationToken = default)
    {
        var parameters = new Dictionary<string, string>
        {
            { "path", itemPath },
            { "url", url }
        };
        await netService.Post($"{BaseUrl}/setFeedURL", parameters, ApiVersion.V2_9_1, ct : cancellationToken);
    }

    /// <summary>
    /// 更新 RSS 订阅源的刷新间隔。<br/>
    /// Update the refresh interval for the RSS feed.
    /// </summary>
    /// <param name="itemPath">订阅源的项目路径。Name and/or path for feed.</param>
    /// <param name="refreshInterval">刷新间隔（分钟）。Refresh interval in minutes.</param>
    /// <param name="cancellationToken">取消请求的令牌。<br/>Token used to cancel the request.</param>
    /// <remarks>
    /// 此方法自 qBittorrent v5.0.0 (Web API v2.11.5) 起引入。<br/>
    /// The method was introduced with qBittorrent v5.0.0 (Web API v2.11.5).
    /// </remarks>
    public async Task SetFeedRefreshInterval(string itemPath,
                                             int    refreshInterval, CancellationToken cancellationToken = default)
    {
        var parameters = new Dictionary<string, string>
        {
            { "path", itemPath },
            { "refreshInterval", refreshInterval.ToString() }
        };
        await netService.Post($"{BaseUrl}/setFeedRefreshInterval", parameters, ApiVersion.V2_11_5,
                              ct : cancellationToken);
    }

    /// <summary>
    /// 移除 RSS 项目（订阅源或文件夹）。<br/>
    /// Remove RSS item (feed or folder).
    /// </summary>
    /// <param name="path">项目路径。Item path.</param>
    /// <param name="cancellationToken">取消请求的令牌。<br/>Token used to cancel the request.</param>
    public async Task RemoveItem(string path, CancellationToken cancellationToken = default)
    {
        var parameters = new Dictionary<string, string>
        {
            { "path", path }
        };
        await netService.Post($"{BaseUrl}/removeItem", parameters, ct : cancellationToken);
    }

    /// <summary>
    /// 移动 RSS 项目（订阅源或文件夹）。<br/>
    /// Move RSS item (feed or folder).
    /// </summary>
    /// <param name="path">当前项目路径。Current item path.</param>
    /// <param name="newPath">新的项目路径。New item path.</param>
    /// <param name="cancellationToken">取消请求的令牌。<br/>Token used to cancel the request.</param>
    public async Task MoveItem(string path, string newPath, CancellationToken cancellationToken = default)
    {
        var parameters = new Dictionary<string, string>
        {
            { "itemPath", path },
            { "destPath", newPath }
        };
        await netService.Post($"{BaseUrl}/moveItem", parameters, ct : cancellationToken);
    }

    /// <summary>
    /// 获取所有 RSS 项目。<br/>
    /// Get all RSS items.
    /// </summary>
    /// <param name="withData">是否包含订阅源数据。Whether to include feed data.</param>
    /// <param name="cancellationToken">取消请求的令牌。<br/>Token used to cancel the request.</param>
    /// <returns>包含所有 RSS 项目的字典。A dictionary containing all RSS items.</returns>
    public async Task<Dictionary<string, object>?> GetAllItems(bool?             withData          = null,
                                                               CancellationToken cancellationToken = default)
    {
        var path = $"{BaseUrl}/items";
        if (withData.HasValue)
        {
            path += $"?withData={withData.Value.ToString().ToLower()}";
        }

        var response = await netService.Get(path, ct : cancellationToken);
        var rawData  = QBittorrentJsonSerializer.Deserialize<Dictionary<string, JsonElement>>(response);
        if (rawData == null) return null;
        return ProcessRssElement(rawData);
    }

    /// <summary>
    /// 将项目或文章标记为已读。<br/>
    /// Mark item or article as read.
    /// </summary>
    /// <param name="itemPath">订阅源路径。Feed path.</param>
    /// <param name="articleId">文章 ID。Article ID.</param>
    /// <param name="cancellationToken">取消请求的令牌。<br/>Token used to cancel the request.</param>
    public async Task MarkAsRead(string  itemPath,
                                 string? articleId = null, CancellationToken cancellationToken = default)
    {
        var parameters = new Dictionary<string, string>
        {
            { "itemPath", itemPath }
        };
        if (!string.IsNullOrWhiteSpace(articleId))
        {
            parameters.Add("articleId", articleId);
        }

        await netService.Post($"{BaseUrl}/markAsRead", parameters, ApiVersion.V2_5_1, ct : cancellationToken);
    }

    /// <summary>
    /// 刷新 RSS 项目。<br/>
    /// Refresh RSS item.
    /// </summary>
    /// <param name="itemPath">项目路径。Item path.</param>
    /// <param name="cancellationToken">取消请求的令牌。<br/>Token used to cancel the request.</param>
    public async Task RefreshItem(string itemPath, CancellationToken cancellationToken = default)
    {
        var parameters = new Dictionary<string, string>
        {
            { "itemPath", itemPath }
        };
        await netService.Post($"{BaseUrl}/refreshItem", parameters, ApiVersion.V2_2_0, ct : cancellationToken);
    }

    /// <summary>
    /// 设置自动下载规则。<br/>
    /// Set auto-downloading rule.
    /// </summary>
    /// <param name="ruleName">规则名称。Rule name.</param>
    /// <param name="rule">规则对象。Rule object.</param>
    /// <param name="cancellationToken">取消请求的令牌。<br/>Token used to cancel the request.</param>
    public async Task SetAutoDownloadingRule(string            ruleName,
                                             AutoDownloadRule  rule,
                                             CancellationToken cancellationToken = default)
    {
        var parameters = new Dictionary<string, string>
        {
            { "ruleName", ruleName },
            { "ruleRef", QBittorrentJsonSerializer.Serialize(rule) }
        };
        await netService.Post($"{BaseUrl}/setRule", parameters, ct : cancellationToken);
    }

    /// <summary>
    /// 重命名自动下载规则。<br/>
    /// Rename auto-downloading rule.
    /// </summary>
    /// <param name="ruleName">旧规则名称。Old rule name.</param>
    /// <param name="newName">新规则名称。New rule name.</param>
    /// <param name="cancellationToken">取消请求的令牌。<br/>Token used to cancel the request.</param>
    public async Task RenameAutoDownloadingRule(string ruleName,
                                                string newName, CancellationToken cancellationToken = default)
    {
        var parameters = new Dictionary<string, string>
        {
            { "ruleName", ruleName },
            { "newRuleName", newName }
        };
        await netService.Post($"{BaseUrl}/renameRule", parameters, ct : cancellationToken);
    }

    /// <summary>
    /// 移除自动下载规则。<br/>
    /// Remove auto-downloading rule.
    /// </summary>
    /// <param name="ruleName">要移除的规则名称。Rule name to remove.</param>
    /// <param name="cancellationToken">取消请求的令牌。<br/>Token used to cancel the request.</param>
    public async Task RemoveAutoDownloadingRule(string ruleName, CancellationToken cancellationToken = default)
    {
        var parameters = new Dictionary<string, string>
        {
            { "ruleName", ruleName },
        };
        await netService.Post($"{BaseUrl}/removeRule", parameters, ct : cancellationToken);
    }

    /// <summary>
    /// 获取所有自动下载规则。<br/>
    /// Get all auto-downloading rules.
    /// </summary>
    /// <param name="cancellationToken">取消请求的令牌。<br/>Token used to cancel the request.</param>
    /// <returns>规则名称与规则对象的映射。A mapping of rule names to rule objects.</returns>
    public async Task<IReadOnlyDictionary<string, AutoDownloadRule>?> GetAllAutoDownloadingRule(
        CancellationToken cancellationToken = default)
    {
        var response = await netService.Get($"{BaseUrl}/rules", ct : cancellationToken);
        return QBittorrentJsonSerializer.Deserialize<Dictionary<string, AutoDownloadRule>>(response);
    }

    /// <summary>
    /// 获取与规则匹配的文章。<br/>
    /// Get articles matching the rule.
    /// </summary>
    /// <param name="ruleName">规则名称。Rule name.</param>
    /// <param name="cancellationToken">取消请求的令牌。<br/>Token used to cancel the request.</param>
    /// <returns>匹配的文章列表。A list of matching articles.</returns>
    public async Task<IReadOnlyDictionary<string, string[]>?> GetMatchingArticles(
        string ruleName, CancellationToken cancellationToken = default)
    {
        // 注意：原代码中 Post 缺少了 parameters，通常此接口需要 ruleName
        var parameters = new Dictionary<string, string> { { "ruleName", ruleName } };
        var response =
            await netService.Post($"{BaseUrl}/matchingArticles", parameters, targetVersion : ApiVersion.V2_5_1,
                                  ct : cancellationToken);
        return QBittorrentJsonSerializer.Deserialize<Dictionary<string, string[]>>(response);
    }

    /// <summary>
    /// 递归处理 RSS 元素的内部方法。<br/>
    /// Internal method to recursively process RSS elements.
    /// </summary>
    private Dictionary<string, object> ProcessRssElement(Dictionary<string, JsonElement> elements)
    {
        var result = new Dictionary<string, object>();
        foreach (var kvp in elements)
        {
            if (kvp.Value.ValueKind == JsonValueKind.String)
            {
                result.Add(kvp.Key, kvp.Value.GetString() ?? string.Empty);
            }
            else if (kvp.Value.ValueKind == JsonValueKind.Object)
            {
                var subItems = QBittorrentJsonSerializer.Deserialize<Dictionary<string, JsonElement>>(
                     kvp.Value.GetRawText());
                if (subItems != null)
                {
                    result.Add(kvp.Key, ProcessRssElement(subItems));
                }
            }
        }

        return result;
    }
}
