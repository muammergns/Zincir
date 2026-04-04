using System;
using System.Threading.Tasks;
using Android;
using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.OS;
using Android.Provider;
using Android.Util;
using AndroidX.Core.App;
using AndroidX.Core.Content;
using ZincirApp.Extensions;
using ZincirApp.Services;

namespace ZincirApp.Android;

public class AndroidNotificationService : INotificationService
{

    private readonly Activity _activity;
    internal static TaskCompletionSource<bool>? PermissionTcs;
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
    
    public async Task<bool> RequestPermission()
    {
        if (!OperatingSystem.IsAndroidVersionAtLeast(33)) return true;
        bool result = await CheckPermission();
        if (result) return true;
        if (PermissionTcs != null) return await PermissionTcs.Task;// aynı anda birden fazla komut gelirse tutarlı geri dönüş sağlamak için.
        PermissionTcs = new TaskCompletionSource<bool>();// static olmak zorunda. android yapısı gereği böyle.
        ActivityCompat.RequestPermissions(_activity, [Manifest.Permission.PostNotifications], 1001);
        bool resultTask = await PermissionTcs.Task;
        PermissionTcs = null;
        return resultTask;
    }

    public Task<bool> CheckPermission()
    {
        if (!OperatingSystem.IsAndroidVersionAtLeast(33)) return Task.FromResult(true);
        return  Task.FromResult(ContextCompat.CheckSelfPermission(
            Application.Context, Manifest.Permission.PostNotifications)==Permission.Granted) ;
    }

    public void ScheduleNotification(string title, string message, TimeSpan delay)
    {
        var intent = new Intent(AlarmClock.ActionSetTimer);
        intent.PutExtra(AlarmClock.ExtraLength, delay.Hours * 3600 + delay.Minutes * 60 + delay.Seconds);
        intent.PutExtra(AlarmClock.ExtraSkipUi, true);
        intent.PutExtra(AlarmClock.ExtraMessage, message);
        intent.AddFlags(ActivityFlags.NewTask);
        Application.Context.StartActivity(intent);
    }
    
    public void ShowNotification(string title, string message)
    {
        int notificationId = new Random().Next(1000, 9999);
        var builder = new NotificationCompat.Builder(Application.Context, ChannelId)
            .SetSmallIcon(Resource.Drawable.chain_icon) //Resource ile ilgili uyarı çözülemedi
            .SetContentTitle(title)
            .SetContentText(message)
            .SetPriority(NotificationCompat.PriorityDefault) 
            .SetAutoCancel(true);
        var notificationManager = NotificationManagerCompat.From(Application.Context);
        notificationManager?.Notify(notificationId, builder?.Build());
    }


}