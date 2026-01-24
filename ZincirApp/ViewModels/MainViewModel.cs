using System;
using System.Diagnostics;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using ZincirApp.Locale;
using ZincirApp.Messages;
using ZincirApp.Services;
using ZincirApp.Views;

namespace ZincirApp.ViewModels;

public partial class MainViewModel : ViewModelBase
{
    [ObservableProperty] private string _greeting = UiTexts.Greeting;
    [ObservableProperty] private bool _isRightPaneOpen;

    public MainViewModel(IServiceProvider serviceProvider) : base(serviceProvider)
    {
        ShowTodayView();
        WeakReferenceMessenger.Default.Register<DrawerChangedMessage>(this, (r, m) =>
        {
            IsRightPaneOpen = m.IsOpen;
        });
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