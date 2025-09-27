namespace Banned.Qbittorrent.Models.Enums;

public enum EnumTrackerStatus
{
    Disabled     = 0, // Tracker is disabled (used for DHT, PeX, and LSD)
    NotContacted = 1, // Tracker has not been contacted yet
    Working      = 2, // Tracker has been contacted and is working
    Updating     = 3, // Tracker is updating
    NotWorking   = 4  // Tracker has been contacted, but it is not working (or doesn't send proper replies)
}
