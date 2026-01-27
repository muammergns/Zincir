using Android.App;
using Android.Content.PM;
using Android.Runtime;
using Avalonia;
using Avalonia.Android;
using ZincirApp.Extensions;

namespace ZincirApp.Android;

[Activity(
    Label = "Zincir",
    Theme = "@style/MyTheme.NoActionBar",
    Icon = "@drawable/chain_icon",
    MainLauncher = true,
    ConfigurationChanges = ConfigChanges.Orientation | ConfigChanges.ScreenSize | ConfigChanges.UiMode)]
public class MainActivity : AvaloniaMainActivity<App>
{
    protected override AppBuilder CustomizeAppBuilder(AppBuilder builder)
    {
        
        PlatformServices.TimerServiceFactory = () => new AndroidTimerService();
        PlatformServices.NotificationServiceFactory = () => new AndroidNotificationService(this, PlatformServices.TimerServiceFactory.Invoke());
        
        return base.CustomizeAppBuilder(builder)
            .WithInterFont();
    }
    
    public override void OnRequestPermissionsResult(int requestCode, string[] permissions, [GeneratedEnum] Permission[] grantResults)
    {
        base.OnRequestPermissionsResult(requestCode, permissions, grantResults);

        if (requestCode!=1001) return;
        bool isGranted = grantResults.Length > 0 && grantResults[0] == Permission.Granted;
        AndroidNotificationService.PermissionTcs?.TrySetResult(isGranted);
    }
}