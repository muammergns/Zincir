using System;

namespace ZincirApp.Models;

public class AppSettings
{
    public string Language { get; set; } = "tr-TR"; // Default language (tr-TR or en-US)
    public string Theme { get; set; } = "Dark"; // Default theme (Light or Dark)
    public string PrimaryColor { get; set; } = "Amber";
    public string UserId {get; set;} = "";
    public string SecondaryColor { get; set; } = "Pink";
    public DateTime? CurrentSessionStartTime { get; set; }
    public TimeSpan AccumulatedTime { get; set; } = TimeSpan.Zero;
    public DateTime? NotificationScheduleDate { get; set; }
    public string SessionTitleText { get; set; } = "";
    public string? HashSignature { get; set; }
}