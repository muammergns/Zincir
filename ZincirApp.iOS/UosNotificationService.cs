using System;
using System.Threading;
using System.Threading.Tasks;
using UserNotifications;
using ZincirApp.Services;

namespace ZincirApp.iOS;

public class UosNotificationService : INotificationService
{
    public UosNotificationService()
    {
        UNUserNotificationCenter.Current.Delegate = new NotificationDelegate();
    }

    public async Task<bool> RequestPermission()
    {
        (bool granted, _) = await UNUserNotificationCenter.Current
            .RequestAuthorizationAsync(UNAuthorizationOptions.Alert | UNAuthorizationOptions.Sound);
        return granted;
    }

    public async Task<bool> CheckPermission()
    {
        var settings = await UNUserNotificationCenter.Current.GetNotificationSettingsAsync();
    
        return settings.AuthorizationStatus is 
            UNAuthorizationStatus.Authorized or UNAuthorizationStatus.Provisional;
    }

    public void ScheduleNotification(string title, string message, TimeSpan delay)
    {
        var content = new UNMutableNotificationContent
        {
            Title = title,
            Body = message,
            Sound = UNNotificationSound.Default 
        };
        double seconds = delay.TotalSeconds < 1 ? 1 : delay.TotalSeconds;
        var trigger = UNTimeIntervalNotificationTrigger.CreateTrigger(seconds, false);
        var request = UNNotificationRequest.FromIdentifier(
            "1",
            content,
            trigger);
        UNUserNotificationCenter.Current.AddNotificationRequest(request, null);
    }

    public void ShowNotification(string title, string message)
    {
        var content = new UNMutableNotificationContent
        {
            Title = title,
            Body = message,
            //Sound = UNNotificationSound.Default // ses olmaması daha iyi
        };
        var trigger = UNTimeIntervalNotificationTrigger.CreateTrigger(1, false);
        var request = UNNotificationRequest.FromIdentifier(
            Guid.NewGuid().ToString(), //bildirimler üst üste binmeyecek, her bildirim ayrı olacak
            content, 
            trigger);
        UNUserNotificationCenter.Current.AddNotificationRequest(request, null);
    }
    
    private class NotificationDelegate : UNUserNotificationCenterDelegate
    {
        public override void WillPresentNotification(UNUserNotificationCenter center, UNNotification notification, Action<UNNotificationPresentationOptions> completionHandler)
        {
            if (OperatingSystem.IsIOSVersionAtLeast(14))
            {
                // iOS 14 ve sonrası için modern seçenekler
                completionHandler(UNNotificationPresentationOptions.Banner | 
                                  UNNotificationPresentationOptions.List | 
                                  UNNotificationPresentationOptions.Sound);
            }
            else
            {
                // iOS 10 - 13 arası için eski seçenek (Alert, modern cihazlarda hem Banner hem List demektir)
                completionHandler(UNNotificationPresentationOptions.Alert | 
                                  UNNotificationPresentationOptions.Sound);
            }
        }
    }
}