using Android.App;
using Android.Content.PM;
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
        PlatformServices.NotificationServiceFactory = () => new AndroidNotificationService(this);
        PlatformServices.TimerServiceFactory = () => new AndroidTimerService();

        
        return base.CustomizeAppBuilder(builder)
            .WithInterFont();
    }
}