using System.Collections.ObjectModel;
using System.Linq;
using Avalonia.Threading;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using ZincirApp.Extensions;
using ZincirApp.Messages;
using ZincirApp.Services;

namespace ZincirApp.ViewModels;

public partial class SettingViewModel : ViewModelBase
{
    private ObservableCollection<SelectionItem> Languages { get; } = [
        new("English", "en-US"), 
        new("Türkçe", "tr-TR")
    ];
    private readonly ISettingsService? _settingsService;
    private readonly INavigationService? _navService;
    [ObservableProperty] private SelectionItem? _languageSelectedItem;
    public SettingViewModel(ISettingsService settingsService,  INavigationService navService)
    {
        _settingsService = settingsService;
        _navService = navService;
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
        _navService.NavigateToSub<AppearanceSettingsViewModel>();
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
        _navService?.NavigateToSub<AppearanceSettingsViewModel>();
        WeakReferenceMessenger.Default.Send(new DrawerChangedMessage(true));
    }
}