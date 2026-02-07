using System;
using System.Diagnostics;
using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Data.Core.Plugins;
using System.Linq;
using Avalonia.Markup.Xaml;
using Avalonia.Threading;
using Microsoft.Extensions.DependencyInjection;
using ZincirApp.Assets;
using ZincirApp.Extensions;
using ZincirApp.Services;
using ZincirApp.ViewModels;
using ZincirApp.Views;

namespace ZincirApp;

public class App : Application
{
    public override void Initialize()
    {
        AvaloniaXamlLoader.Load(this);
    }

    public override void OnFrameworkInitializationCompleted()
    {
        var collection = new ServiceCollection();
        
        collection.AddSingleton<INavigationService, NavigationService>();
        collection.AddSingleton<ISettingsService, SettingsService>();
        collection.AddSingleton<IStorageService>(_ => 
            PlatformServices.StorageServiceFactory != null ?
                PlatformServices.StorageServiceFactory() :
                new StorageService());
        collection.AddSingleton<IDeviceIdService>(_ => 
            PlatformServices.DeviceIdServiceFactory != null ?
                PlatformServices.DeviceIdServiceFactory() :
                new DeviceIdService());
        collection.AddSingleton<IAesService>(_ => 
            PlatformServices.AesServiceFactory != null ?
                PlatformServices.AesServiceFactory() :
                new AesService());
        AddViews(collection);
        
        switch (ApplicationLifetime)
        {
            case IClassicDesktopStyleApplicationLifetime:
                collection.AddTransient<IMessageService, DesktopMessageBoxService>();
                break;
            case ISingleViewApplicationLifetime:
                collection.AddTransient<IMessageService, SingleViewMessageBoxService>();
                break;
        }

        collection.AddTransient<INotificationService>(_ => 
            PlatformServices.NotificationServiceFactory != null ? 
                PlatformServices.NotificationServiceFactory() : 
                new NotificationService(new SingleViewMessageBoxService()));
        collection.AddTransient<ITimerService>(_ => 
            PlatformServices.TimerServiceFactory != null ? 
                PlatformServices.TimerServiceFactory() : 
                new TimerService());
        
        
        var services = collection.BuildServiceProvider();
        
        var settings = services.GetRequiredService<ISettingsService>();
        Dispatcher.UIThread.InvokeAsync( async () =>
        {
            var upsetting = await settings.GetSettings();
            settings.ApplySettings(upsetting);
        });
        switch (ApplicationLifetime)
        {
            case IClassicDesktopStyleApplicationLifetime desktop:
                DisableAvaloniaDataAnnotationValidation();
                desktop.MainWindow = new MainWindow
                {
                    DataContext = services.GetRequiredService<MainViewModel>()
                };
                break;
            case ISingleViewApplicationLifetime singleViewPlatform:
                singleViewPlatform.MainView = new MainView
                {
                    DataContext = services.GetRequiredService<MainViewModel>()
                };
                break;
        }
        
        base.OnFrameworkInitializationCompleted();
    }

    private static void AddViews(ServiceCollection collection)
    {
        collection.AddSingleton<MainViewModel>();
        collection.AddSingleton<SettingViewModel>();
        collection.AddSingleton<AppearanceSettingsViewModel>();
        collection.AddSingleton<TodayViewModel>();
        collection.AddSingleton<TodoViewModel>();
        collection.AddSingleton<HabitViewModel>();
        collection.AddSingleton<PomodoroViewModel>();
    }

    private void DisableAvaloniaDataAnnotationValidation()
    {
        var dataValidationPluginsToRemove =
            BindingPlugins.DataValidators.OfType<DataAnnotationsValidationPlugin>().ToArray();

        foreach (var plugin in dataValidationPluginsToRemove)
        {
            BindingPlugins.DataValidators.Remove(plugin);
        }
    }
    
    
}