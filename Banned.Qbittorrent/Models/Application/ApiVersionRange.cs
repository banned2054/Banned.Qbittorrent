namespace Banned.Qbittorrent.Models.Application;

/// <summary>
/// 表示 Web API 端点的有效版本区间。<br/>
/// Represents the Web API version range in which an endpoint is available.
/// </summary>
/// <param name="Introduced">引入端点的版本（包含）。 / Version that introduced the endpoint, inclusive.</param>
/// <param name="Removed">移除端点的版本（不包含）。 / Version that removed the endpoint, exclusive.</param>
public readonly record struct ApiVersionRange(ApiVersion? Introduced = null, ApiVersion? Removed = null);
