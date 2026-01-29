using System;
using System.Collections.ObjectModel;
using System.Linq;
using Avalonia.Controls;
using Avalonia.Threading;
using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.Extensions.DependencyInjection;
using ZincirApp.Extensions;
using ZincirApp.Locale;
using ZincirApp.Services;

namespace ZincirApp.ViewModels;

public partial class AppearanceSettingsViewModel : ViewModelBase
{
    /*  Red, Pink, Purple, DeepPurple, Indigo, Blue, LightBlue
        Cyan, Teal, Green, LightGreen, Lime, Yellow, Amber
        Orange, DeepOrange, Brown, Grey, BlueGrey */
    private ObservableCollection<SelectionItem> ThemeColors { get; } = [
        new SelectionItem("Red", "Red"), 
        new SelectionItem("Pink", "Pink"), 
        new SelectionItem("Purple", "Purple"), 
        new SelectionItem("DeepPurple", "DeepPurple"), 
        new SelectionItem("Indigo", "Indigo"), 
        new SelectionItem("Blue", "Blue"), 
        new SelectionItem("LightBlue", "LightBlue"), 
        new SelectionItem("Cyan", "Cyan"), 
        new SelectionItem("Teal", "Teal"), 
        new SelectionItem("Green", "Green"), 
        new SelectionItem("LightGreen", "LightGreen"), 
        new SelectionItem("Lime", "Lime"), 
        new SelectionItem("Yellow", "Yellow"), 
        new SelectionItem("Amber", "Amber"), 
        new SelectionItem("Orange", "Orange"), 
        new SelectionItem("DeepOrange", "DeepOrange"), 
        new SelectionItem("Brown", "Brown"), 
        new SelectionItem("Grey", "Grey"), 
        new SelectionItem("BlueGrey", "BlueGrey"), 
    ];
    private ObservableCollection<SelectionItem> ThemeTypes { get; } = [
        new SelectionItem(UiTexts.Light, "Light"), 
        new SelectionItem(UiTexts.Dark, "Dark"), 
    ];
    [ObservableProperty] private SelectionItem? _themeSelectedItem;
    [ObservableProperty] private SelectionItem? _primarySelectedItem;
    [ObservableProperty] private SelectionItem? _secondarySelectedItem;
    private readonly ISettingsService? _settingsService;
    
    public AppearanceSettingsViewModel(IServiceProvider serviceProvider) : base(serviceProvider)
    {
        _settingsService = serviceProvider.GetRequiredService<ISettingsService>();
        Dispatcher.UIThread.InvokeAsync(async () =>
        {
            if (_settingsService == null) return;
            var setting = await _settingsService.GetSettings();
            foreach (var item in ThemeTypes.Where(item => item.Code == setting.Theme))
            {
                ThemeSelectedItem = item;
                break;
            }
            foreach (var item in ThemeColors.Where(item => item.Code == setting.PrimaryColor))
            {
                PrimarySelectedItem = item;
                break;
            }
            foreach (var item in ThemeColors.Where(item => item.Code == setting.SecondaryColor))
            {
                SecondarySelectedItem = item;
                break;
            }
        });
    }
    
    partial void OnThemeSelectedItemChanged(SelectionItem? value)
    {
        if (value==null) return;
        Dispatcher.UIThread.InvokeAsync(async () =>
        {
            if (_settingsService == null) return;
            var setting = await _settingsService.GetSettings();
            setting.Theme = value.Code;
            _settingsService.ApplySettings(setting);
            await _settingsService.SaveSettings(setting);
        }, DispatcherPriority.Background);
    }
    partial void OnPrimarySelectedItemChanged(SelectionItem? value)
    {
        if (value==null) return;
        Dispatcher.UIThread.InvokeAsync(async () =>
        {
            if (_settingsService == null) return;
            var setting = await _settingsService.GetSettings();
            setting.PrimaryColor = value.Code;
            _settingsService.ApplySettings(setting);
            await _settingsService.SaveSettings(setting);
        }, DispatcherPriority.Background);
    }
    partial void OnSecondarySelectedItemChanged(SelectionItem? value)
    {
        if (value==null) return;
        Dispatcher.UIThread.InvokeAsync(async () =>
        {
            if (_settingsService == null) return;
            var setting = await _settingsService.GetSettings();
            setting.SecondaryColor = value.Code;
            _settingsService.ApplySettings(setting);
            await _settingsService.SaveSettings(setting);
        }, DispatcherPriority.Background);
    }
    
}