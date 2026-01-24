using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Data.Core.Plugins;
using System.Linq;
using Avalonia.Markup.Xaml;
using Microsoft.Extensions.DependencyInjection;
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
        collection.AddTransient<ISettingsService, SettingsService>();
        collection.AddTransient<IStorageService, StorageService>();
        
        AddViews(collection);
        
        var services = collection.BuildServiceProvider();
        
        var settings = services.GetRequiredService<ISettingsService>();
        settings.ApplySettings(settings.GetSettings());
        
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
        collection.AddTransient<SettingViewModel>();
        collection.AddTransient<AppearanceSettingsViewModel>();
        collection.AddSingleton<TodayViewModel>();
        collection.AddScoped<TodoViewModel>();
        collection.AddScoped<HabitViewModel>();
        collection.AddScoped<PomodoroViewModel>();
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