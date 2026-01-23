using System;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using ZincirApp.Locale;
using ZincirApp.Services;
using ZincirApp.Views;

namespace ZincirApp.ViewModels;

public partial class MainViewModel : ViewModelBase
{
    [ObservableProperty] private string _greeting = UiTexts.Greeting;

    public MainViewModel(IServiceProvider serviceProvider) : base(serviceProvider)
    {
        ShowTodayView();
    }

    [RelayCommand] private void ShowSettingView()
    {
        NavService.NavigateTo<SettingViewModel>();
    }
    [RelayCommand] private void ShowTodayView()
    {
        NavService.NavigateTo<TodayViewModel>();
    }
    [RelayCommand] private void ShowTodoView()
    {
        NavService.NavigateTo<TodoViewModel>();
    }
    [RelayCommand] private void ShowHabitView()
    {
        NavService.NavigateTo<HabitViewModel>();
    }
    [RelayCommand] private void ShowPomodoroView()
    {
        NavService.NavigateTo<PomodoroViewModel>();
    }
}