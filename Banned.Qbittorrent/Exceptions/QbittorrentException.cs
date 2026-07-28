namespace Banned.Qbittorrent.Exceptions;

/// <summary>
/// 表示调用 qBittorrent Web API 时发生的错误。<br/>
/// Represents an error returned while calling the qBittorrent Web API.
/// </summary>
/// <param name="message">错误消息。 / Error message.</param>
/// <param name="statusCode">关联的 HTTP 状态码。 / Associated HTTP status code.</param>
/// <param name="inner">导致当前异常的内部异常。 / Exception that caused the current exception.</param>
public class QbittorrentException(string message, int? statusCode = null, Exception? inner = null)
    : Exception(message, inner)
{
    /// <summary>获取关联的 HTTP 状态码。<br/>Gets the associated HTTP status code.</summary>
    public int? StatusCode { get; } = statusCode;
}
