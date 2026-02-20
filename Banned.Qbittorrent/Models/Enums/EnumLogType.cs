namespace Banned.Qbittorrent.Models.Enums;

/// <summary>
/// 表示 qBittorrent 日志消息的类型。<br/>
/// Represents the type of qBittorrent log messages.
/// </summary>
/// <remarks>
/// 这些值可以作为标志进行组合，用于过滤特定类型的日志输出。<br/>
/// These values can be combined as flags to filter specific types of log output.
/// </remarks>
public enum EnumLogType
{
    /// <summary>
    /// 普通日志消息。<br/>
    /// Normal log message.
    /// </summary>
    /// <remarks>
    /// 通常用于记录常规操作，如开始下载或种子已完成。<br/>
    /// Typically used for routine operations, such as starting a download or a torrent completion.
    /// </remarks>
    Normal = 1,

    /// <summary>
    /// 信息类消息。<br/>
    /// Information message.
    /// </summary>
    /// <remarks>
    /// 提供有关系统状态或后台进程的补充说明信息。<br/>
    /// Provides supplementary descriptive information about system status or background processes.
    /// </remarks>
    Info = 2,

    /// <summary>
    /// 警告消息。<br/>
    /// Warning message.
    /// </summary>
    /// <remarks>
    /// 表示发生了非致命问题，虽然不影响运行但需要注意。<br/>
    /// Indicates a non-fatal issue that doesn't stop operation but warrants attention.
    /// </remarks>
    Warning = 4,

    /// <summary>
    /// 严重错误消息。<br/>
    /// Critical error message.
    /// </summary>
    /// <remarks>
    /// 表示发生了严重问题，可能导致功能失效或操作中断。<br/>
    /// Indicates a serious issue that may lead to functional failure or operation interruption.
    /// </remarks>
    Critical = 8,
}
