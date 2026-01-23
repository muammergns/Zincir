using System;
using System.Globalization;
using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Data.Core;
using Avalonia.Data.Core.Plugins;
using System.Linq;
using System.Threading;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Markup.Xaml;
using Avalonia.Media;
using Avalonia.Styling;
using Avalonia.Threading;
using Material.Colors;
using Material.Styles.Themes;
using Material.Styles.Themes.Base;
using Microsoft.Extensions.DependencyInjection;
using ZincirApp.Services;
using ZincirApp.Settings;
using ZincirApp.ViewModels;
using ZincirApp.Views;

namespace ZincirApp;

public partial class App : Application
{
    public static SettingsService? Settings { get; private set; }
    
    
    
    public override void Initialize()
    {
        AvaloniaXamlLoader.Load(this);
    }

    public override void OnFrameworkInitializationCompleted()
    {
        var collection = new ServiceCollection();
        collection.AddSingleton<INavigationService, NavigationService>();
        collection.AddScoped<MainViewModel>();
        collection.AddTransient<SettingViewModel>();
        collection.AddScoped<TodayViewModel>();
        collection.AddScoped<TodoViewModel>();
        collection.AddScoped<HabitViewModel>();
        collection.AddScoped<PomodoroViewModel>();
        var appSettings = Settings?.Load();
        ApplySettings(appSettings ?? new AppSettings());
        var services = collection.BuildServiceProvider();
        switch (ApplicationLifetime)
        {
            case IClassicDesktopStyleApplicationLifetime desktop:
                // Avoid duplicate validations from both Avalonia and the CommunityToolkit. 
                // More info: https://docs.avaloniaui.net/docs/guides/development-guides/data-validation#manage-validationplugins
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

    private void DisableAvaloniaDataAnnotationValidation()
    {
        // Get an array of plugins to remove
        var dataValidationPluginsToRemove =
            BindingPlugins.DataValidators.OfType<DataAnnotationsValidationPlugin>().ToArray();

        // remove each entry found
        foreach (var plugin in dataValidationPluginsToRemove)
        {
            BindingPlugins.DataValidators.Remove(plugin);
        }
    }
    
    public static void ApplySettings(AppSettings settings)
    {
        // Tema
        if (Current != null)
        {
            var newVariant = settings.Theme == "Dark" ? ThemeVariant.Dark : ThemeVariant.Light;
            Current.RequestedThemeVariant = newVariant;
            var materialTheme = Current.Styles.OfType<MaterialTheme>().FirstOrDefault();
            if (materialTheme != null)
            {
                materialTheme.BaseTheme = newVariant == ThemeVariant.Dark 
                    ? BaseThemeMode.Dark 
                    : BaseThemeMode.Light;
                if (Enum.TryParse(settings.PrimaryColor, out PrimaryColor primaryColor))
                {
                    materialTheme.PrimaryColor = primaryColor;
                }
                if (Enum.TryParse(settings.SecondaryColor, out SecondaryColor secondaryColor))
                {
                    materialTheme.SecondaryColor = secondaryColor;
                }
            }
        }

        // Dil
        Locale.UiTexts.Culture = new CultureInfo(settings.Language);
        Thread.CurrentThread.CurrentCulture = new CultureInfo(settings.Language);
        Thread.CurrentThread.CurrentUICulture = new CultureInfo(settings.Language);
        
    }
    
    public static void SetMaterialBackground(Control? control, string resourceKey)
    {
        if (control == null) return;

        var app = Application.Current;
        if (app == null) return;

        if (app.TryGetResource(resourceKey, app.ActualThemeVariant, out object? res) && res is IBrush brush)
        {
            control.SetValue(TemplatedControl.BackgroundProperty, brush);
        }
    }
    
    public static string ObjToString(object? value, int decimalPlaces = 2)
    {
        switch (value)
        {
            case null:
                return string.Empty;
            case IConvertible:
                try
                {
                    double numericValue = Convert.ToDouble(value, CultureInfo.InvariantCulture);
                    string format = $"N{decimalPlaces}";
                    return numericValue.ToString(format, CultureInfo.CurrentCulture);
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                }
                break;
        }
        try
        {
            return value.ToString() ?? string.Empty;
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            return string.Empty;
        }
    }
}