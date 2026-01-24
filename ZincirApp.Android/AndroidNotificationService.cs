using System;
using Android;
using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.OS;
using Android.Util;
using AndroidX.Core.App;
using AndroidX.Core.Content;
using ZincirApp.Services;

namespace ZincirApp.Android;

public class AndroidNotificationService : INotificationService
{

    private readonly Activity _activity;
    private const string ChannelId = "default_channel";
    private const string ChannelName = "Default";
    private const string ChannelDescription = "Default Channel";

    public AndroidNotificationService(Activity activity)
    {
        _activity = activity;
        CreateNotificationChannel();
    }
    
    private void CreateNotificationChannel()
    {
        if (!OperatingSystem.IsAndroidVersionAtLeast(26)) return;
        var channel = new NotificationChannel(
            ChannelId, ChannelName, NotificationImportance.Default)
        {
            Description = ChannelDescription
        };
        var notificationManager = 
            Application.Context.GetSystemService(Context.NotificationService) as NotificationManager;
        notificationManager?.CreateNotificationChannel(channel);
    }
    
    public void RequestPermission()
    {
        if (!OperatingSystem.IsAndroidVersionAtLeast(33) || CheckPermission()) return;
        ActivityCompat.RequestPermissions(_activity, [Manifest.Permission.PostNotifications], 1001);
    }

    public bool CheckPermission()
    {
        if (!OperatingSystem.IsAndroidVersionAtLeast(33)) return true;
        return ContextCompat.CheckSelfPermission(
            Application.Context, Manifest.Permission.PostNotifications)==Permission.Granted;
    }

    public void ScheduleNotification(string title, string message, TimeSpan delay)
    {
        
    }

    public void ShowNotification(string title, string message)
    {
        Log.Debug("Notification", title);
        if (!CheckPermission()) return;
        var builder = new NotificationCompat.Builder(Application.Context, ChannelId)
            .SetSmallIcon(Resource.Drawable.chain_icon)//Resource ile ilgili uyarı çözülemedi
            .SetContentTitle(title)
            .SetContentText(message)
            .SetPriority(NotificationCompat.PriorityDefault)
            .SetAutoCancel(true);

        var notificationManager = NotificationManagerCompat.From(Application.Context);
        notificationManager?.Notify(1, builder?.Build());
    }


}