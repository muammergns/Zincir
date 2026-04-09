using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using ZincirApp.Assets;
using ZincirApp.Messages;
using ZincirApp.Services;

namespace ZincirApp.ViewModels;

public partial class MainViewModel : ViewModelBase
{
    [ObservableProperty] private bool _isRightPaneOpen;
    [ObservableProperty] private bool _isLeftPaneOpen;
    [ObservableProperty] private INavigationService _navService;

    public MainViewModel(INavigationService navService, IDatabaseService dbService, IStorageService storageService)
    {
        NavService = navService;
        Task.Run(async () =>
        {
            dbService.SetPath(storageService.GetPath("zincir.db"));
            dbService.SetSalt(Keys.DatabaseId);
            dbService.SetPin("1234");
            await dbService.EnsureInitializedAsync();
        });
        ShowTodayView();
        WeakReferenceMessenger.Default.Register<DrawerChangedMessage>(this, (_, m) =>
        {
            IsRightPaneOpen = m.IsOpen;
        });
    }

    [RelayCommand] private void ShowSettingView()
    {
        NavService.NavigateTo<SettingViewModel>();
        IsLeftPaneOpen = false;
    }
    [RelayCommand] private void ShowTodayView()
    {
        NavService.NavigateTo<TodayViewModel>();
        IsLeftPaneOpen = false;
    }
    [RelayCommand] private void ShowTodoView()
    {
        NavService.NavigateTo<TodoViewModel>();
        IsLeftPaneOpen = false;
    }
    [RelayCommand] private void ShowHabitView()
    {
        NavService.NavigateTo<HabitViewModel>();
        IsLeftPaneOpen = false;
    }
    [RelayCommand] private void ShowPomodoroView()
    {
        NavService.NavigateTo<PomodoroViewModel>();
        IsLeftPaneOpen = false;
    }
}