namespace Banned.Qbittorrent.Models.Application;

[AttributeUsage(AttributeTargets.Property)]
public sealed class TargetApiVersionAttribute : Attribute
{
    public ApiVersion? MinVersion { get; set; }
    public ApiVersion? MaxVersion { get; set; }
}
