using System;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using ZincirApp.Messages;
using ZincirApp.Models;
using ZincirApp.Services;
using ZincirApp.Stores;

namespace ZincirApp.ViewModels;

public partial class HabitViewModel : ViewModelBase
{
    public IHabitStore Store { get; }
    private readonly INavigationService _navService;
    private readonly ISettingsService _settingsService;

    public HabitViewModel(IHabitStore store, INavigationService navigationService, ISettingsService settingsService)
    {
        Store = store;
        _navService = navigationService;
        _settingsService = settingsService;
    }

    [RelayCommand]
    private async Task ShowPomodoroView(HabitItemViewModel? model)
    {
        var appSettings = await _settingsService.GetSettings();
        if (model is not null &&  
            appSettings.HabitSessionId is null && 
            appSettings.TodoSessionId is null && 
            appSettings.CurrentSessionStartTime is null)
        {
            appSettings.HabitSessionId = model.Model.Id;
            await _settingsService.SaveSettings(appSettings);
            Console.WriteLine(@"Pomodoro init saved");
        }
        _navService.NavigateTo<PomodoroViewModel>();
        _navService.NavigateToSub<PomodoroHistoryViewModel>();
    }

    [RelayCommand]
    private void ShowHabitEditView(HabitItemViewModel? model)
    {
        _navService.NavigateToSub(model is not null
            ? new HabitEditViewModel(Store, _navService, model.Model)
            : new HabitEditViewModel(Store, _navService));
        WeakReferenceMessenger.Default.Send(new DrawerChangedMessage(true));
    }

    [RelayCommand]
    private void ShowHabitDetailView(HabitItemViewModel? model)
    {
        if (model is null) return;
        //_navService.NavigateToSub(model);
        Console.WriteLine($@"TODO - habit detay view oluşturmayı unutma: {model.Title}");
    }
    
    [RelayCommand]
    private void ShowAddHabitLogView(HabitItemViewModel? model)
    {
        if (model is null) return;
        //_navService.NavigateToSub(model);
        Console.WriteLine($@"TODO - habit add log view oluşturmayı unutma: {model.Title}");
        Task.Run(async () =>
        {
            await Store.AddLogAsync(new HabitLogModel
            {
                Date = DateTime.Now,
                HabitId = model.Model.Id,
                Value = 1
            });
            _navService.NavigateToSub<HabitEditViewModel>();
        });
    }
}