using System;
using System.Collections.ObjectModel;
using System.Linq;
using Avalonia.Threading;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using Microsoft.Extensions.DependencyInjection;
using ZincirApp.Extensions;
using ZincirApp.Messages;
using ZincirApp.Services;

namespace ZincirApp.ViewModels;

public partial class SettingViewModel : ViewModelBase
{
    private ObservableCollection<SelectionItem> Languages { get; } = [
        new SelectionItem("English", "en-US"), 
        new SelectionItem("Türkçe", "tr-TR")
    ];
    private readonly ISettingsService? _settingsService;
    [ObservableProperty] private SelectionItem? _languageSelectedItem;
    public SettingViewModel(IServiceProvider serviceProvider) :base(serviceProvider)
    {
        _settingsService = serviceProvider.GetRequiredService<ISettingsService>();
        Dispatcher.UIThread.InvokeAsync(async () =>
        {
            if (_settingsService == null) return;
            var setting = await _settingsService.GetSettings();
            foreach (var item in Languages.Where(item => item.Code == setting.Language))
            {
                LanguageSelectedItem = item;
                break;
            }
        });
        NavService.NavigateToSub<AppearanceSettingsViewModel>();
    }
    
    partial void OnLanguageSelectedItemChanged(SelectionItem? value)
    {
        if (value==null) return;
        Dispatcher.UIThread.InvokeAsync(async () =>
        {
            if (_settingsService == null) return;
            var setting = await _settingsService.GetSettings();
            setting.Language = value.Code;
            _settingsService.ApplySettings(setting);
            await _settingsService.SaveSettings(setting);
        }, DispatcherPriority.Background);
    }
    
    [RelayCommand] private void OpenAppearanceSettings()
    {
        NavService.NavigateToSub<AppearanceSettingsViewModel>();
        WeakReferenceMessenger.Default.Send(new DrawerChangedMessage(true));
    }
}