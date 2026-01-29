using System;
using System.Diagnostics;
using System.Runtime.InteropServices.JavaScript;
using System.Threading.Tasks;
using ZincirApp.Services;


namespace ZincirApp.Browser;

public partial class BrowserNotificationService : INotificationService
{

    // JavaScript modülündeki fonksiyonları bağlıyoruz
    [JSImport("enableWakeLock", "NotificationModule")]
    private static partial Task JS_EnableWakeLock();

    [JSImport("disableWakeLock", "NotificationModule")]
    private static partial void JS_DisableWakeLock();

    [JSImport("playBell", "NotificationModule")]
    private static partial void JS_PlayBell();
    
    [JSImport("initAudio", "NotificationModule")]
    private static partial void JS_InitAudio();

    private bool _moduleLoaded = false;

    private async Task EnsureModuleLoaded()
    {
        if (!_moduleLoaded)
        {
            // JavaScript dosyasını modül olarak sisteme tanıtıyoruz
            await JSHost.ImportAsync("NotificationModule", "/notifications.js");
            _moduleLoaded = true;
        }
    }
    
    public Task<bool> RequestPermission()
    {
        return Task.FromResult(true);
    }

    public Task<bool> CheckPermission()
    {
        return Task.FromResult(true);
    }

    public void ScheduleNotification(string title, string message, TimeSpan delay)
    {
        ScheduleNotification(delay).ConfigureAwait(false);
    }

    private async Task ScheduleNotification(TimeSpan delay)
    {
        try
        {
            await EnsureModuleLoaded();
            
            JS_InitAudio();
            
            await JS_EnableWakeLock();
            
            await Task.Delay(delay);
            
            JS_PlayBell();
            JS_DisableWakeLock();
        }
        catch (Exception ex)
        {
            Console.WriteLine($@"WASM Notification Error: {ex.Message}");
        }
    }

    public void ShowNotification(string title, string message)
    {
        Console.WriteLine(title);
        //JS_PlayBell();
    }
}