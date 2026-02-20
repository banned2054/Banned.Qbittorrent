namespace Banned.Qbittorrent.Models.Enums;

/// <summary>
/// 表示计划任务（如速度限制计划）的执行日期。<br/>
/// Represents the days for the scheduler (e.g., speed limit schedules).
/// </summary>
public enum EnumSchedulerDay
{
    /// <summary>
    /// 每天。<br/>
    /// Every day.
    /// </summary>
    EveryDay = 0,

    /// <summary>
    /// 每个工作日（周一至周五）。<br/>
    /// Every weekday (Monday to Friday).
    /// </summary>
    EveryWeekday = 1,

    /// <summary>
    /// 每个周末（周六和周日）。<br/>
    /// Every weekend (Saturday and Sunday).
    /// </summary>
    EveryWeekend = 2,

    /// <summary>
    /// 每周一。<br/>
    /// Every Monday.
    /// </summary>
    EveryMonday = 3,

    /// <summary>
    /// 每周二。<br/>
    /// Every Tuesday.
    /// </summary>
    EveryTuesday = 4,

    /// <summary>
    /// 每周三。<br/>
    /// Every Wednesday.
    /// </summary>
    EveryWednesday = 5,

    /// <summary>
    /// 每周四。<br/>
    /// Every Thursday.
    /// </summary>
    EveryThursday = 6,

    /// <summary>
    /// 每周五。<br/>
    /// Every Friday.
    /// </summary>
    EveryFriday = 7,

    /// <summary>
    /// 每周六。<br/>
    /// Every Saturday.
    /// </summary>
    EverySaturday = 8,

    /// <summary>
    /// 每周日。<br/>
    /// Every Sunday.
    /// </summary>
    EverySunday = 9,
}
