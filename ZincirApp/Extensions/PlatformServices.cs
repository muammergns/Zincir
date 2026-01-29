using System;
using ZincirApp.Services;

namespace ZincirApp.Extensions;

public static class PlatformServices
{
    public static Func<INotificationService>? NotificationServiceFactory { get; set; }
    public static Func<ITimerService>? TimerServiceFactory { get; set; }
    public static Func<IStorageService>? StorageServiceFactory { get; set; }
}