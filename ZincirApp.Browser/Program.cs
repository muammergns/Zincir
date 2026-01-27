using System;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Browser;
using ZincirApp;
using ZincirApp.Browser;
using ZincirApp.Extensions;

internal sealed partial class Program
{
    private static Task Main(string[] args)
    {
        return BuildAvaloniaApp()
            .WithInterFont()
            .StartBrowserAppAsync("out");
    }

    public static AppBuilder BuildAvaloniaApp()
    {
        Console.WriteLine(@"BuildAvaloniaApp()");
        PlatformServices.NotificationServiceFactory = () => new BrowserNotificationService();
        return AppBuilder.Configure<App>();
    }
}