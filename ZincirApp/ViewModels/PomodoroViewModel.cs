using System;
using System.Threading.Tasks;
using Avalonia.Media;
using Avalonia.Threading;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using Microsoft.Extensions.DependencyInjection;
using ZincirApp.Extensions;
using ZincirApp.Messages;
using ZincirApp.Models;
using ZincirApp.Services;

namespace ZincirApp.ViewModels;

public partial class PomodoroViewModel : ViewModelBase
{
    
    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(StartCommand))]
    [NotifyCanExecuteChangedFor(nameof(ResetCommand))]
    private bool _isRunning;
    
    [ObservableProperty]
    private double _progressValue;

    [ObservableProperty]
    private IBrush _statusColor = SolidColorBrush.Parse("#4A5568");

    [ObservableProperty]
    private TimeSpan _elapsedMinutes = TimeSpan.Zero;

    [ObservableProperty] private string _remainingTimeFormatted = "0";
    [ObservableProperty] private string _pomodoroTitleText = "Pomodoro";

    private readonly ITimerService? _timerService;
    private readonly ISettingsService? _settingsService;
    private readonly INotificationService? _notificationService;
    public PomodoroViewModel(IServiceProvider serviceProvider) :base(serviceProvider)
    {
        _notificationService = serviceProvider.GetService<INotificationService>();
        _timerService = serviceProvider.GetService<ITimerService>();
        _settingsService = serviceProvider.GetService<ISettingsService>();
        Task.Run(async () =>
        {
            if (_notificationService != null)
            {
                var result = await _notificationService.CheckPermission();
                if (!result)
                {
                    await _notificationService.RequestPermission();
                }
            }
        });
        _timerService?.Tick += (_, span) =>
        {
            ElapsedMinutes = span;
            RemainingTimeFormatted = UiUtils.FormatTimeSpan(span);
            ProgressValue = ElapsedMinutes.TotalSeconds * 6;
        };
        Dispatcher.UIThread.InvokeAsync(async () =>
        {
            if (_settingsService != null)
            {
                var appSettings = await _settingsService.GetSettings();
                _timerService?.LoadState(appSettings.AccumulatedTime, appSettings.CurrentSessionStartTime, appSettings.SessionTitleText);
                IsRunning = _timerService?.IsRunning ?? false;
                var elapsed = _timerService?.ElapsedTime ?? TimeSpan.Zero;
                ElapsedMinutes = TimeSpan.FromTicks(elapsed.Ticks);
                RemainingTimeFormatted = UiUtils.FormatTimeSpan(elapsed);
                ProgressValue = ElapsedMinutes.TotalSeconds * 6;
                PomodoroTitleText = string.IsNullOrEmpty(_timerService?.Title) ? "Pomodoro" : _timerService.Title;
            }
        },  DispatcherPriority.Background);
        OpenPomodoroHistoryView("0");
    }
    
    [RelayCommand(CanExecute = nameof(CanStart))]
    private void Start()
    {
        if (IsRunning) return;
        _timerService?.Start();
        PomodoroTitleText = _timerService?.Title ?? "Pomodoro";
        IsRunning = true;
        StatusColor = SolidColorBrush.Parse("#00FFFF");
        Task.Run(async () =>
        {
            if (_settingsService != null && _timerService != null)
            {
                var appSettings = await _settingsService.GetSettings();
                appSettings.AccumulatedTime = _timerService.AccumulatedTime;
                appSettings.CurrentSessionStartTime = _timerService.CurrentSessionStartTime;
                appSettings.SessionTitleText = _timerService.Title;
                await _settingsService.SaveSettings(appSettings);
            }
        });
    }

    [RelayCommand(CanExecute = nameof(CanReset))]
    private void Reset()
    {
        var pomodoroModel = new PomodoroModel
        {
            CreateDate = _timerService?.CurrentSessionStartTime ?? DateTime.Now,
            SessionTimeSpan = ElapsedMinutes
        };
        _timerService?.Reset();
        IsRunning = false;
        StatusColor = SolidColorBrush.Parse("#4A5568");
        ElapsedMinutes = TimeSpan.Zero;
        PomodoroTitleText = "Pomodoro";
        
        Task.Run(async () =>
        {
            if (_settingsService != null && _timerService != null)
            {
                var appSettings = await _settingsService.GetSettings();
                appSettings.AccumulatedTime = _timerService.AccumulatedTime;
                appSettings.CurrentSessionStartTime = _timerService.CurrentSessionStartTime;
                appSettings.SessionTitleText = _timerService.Title;
                await _settingsService.SaveSettings(appSettings);
            }
        });
        Task.Run( async () =>
        {
            await DbService.InsertAsync(pomodoroModel);
            WeakReferenceMessenger.Default.Send(new PomodoroListUpdated([pomodoroModel], DbState.Insert));
        });
    }

    [RelayCommand]
    private void SetTimer(string minutes)
    {
        if (int.TryParse(minutes, out var minute))
        {
            Dispatcher.UIThread.InvokeAsync(async () =>
            {
                if (_notificationService != null && _timerService != null && _settingsService != null)
                {
                    var result = await _notificationService.CheckPermission();
                    if (result)
                    {
                        Start();
                        _notificationService.ScheduleNotification(
                            "Pomodoro", 
                            _timerService.Title, 
                            TimeSpan.FromMinutes(minute));
                        
                    }
                }
            }, DispatcherPriority.Background);
        }
    }
    
    private bool CanStart() => !IsRunning;
    private bool CanReset() => IsRunning;

    [RelayCommand]
    private void OpenPomodoroHistoryView(string isOpen)
    {
        NavService.NavigateToSub<PomodoroHistoryViewModel>();
        if (int.TryParse(isOpen, out var index))
        {
            if (index==1)
            {
                WeakReferenceMessenger.Default.Send(new DrawerChangedMessage(true));
            }
        }
        Task.Run(async () =>
        {
            var list = await DbService.GetAllAsync<PomodoroModel>();
            WeakReferenceMessenger.Default.Send(new PomodoroListUpdated(list, DbState.Get));
        });
    }
    
    
}