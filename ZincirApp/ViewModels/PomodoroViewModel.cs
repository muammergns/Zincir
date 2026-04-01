using System;
using Avalonia.Media;
using Avalonia.Threading;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.DependencyInjection;
using ZincirApp.Extensions;
using ZincirApp.Services;

namespace ZincirApp.ViewModels;

public partial class PomodoroViewModel : ViewModelBase
{
    
    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(StartCommand))]
    [NotifyCanExecuteChangedFor(nameof(PauseCommand))]
    [NotifyCanExecuteChangedFor(nameof(ResetCommand))]
    private bool _isRunning;

    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(ResetCommand))]
    private bool _hasProgress;
    
    [ObservableProperty]
    private double _progressValue;

    [ObservableProperty]
    private IBrush _statusColor = SolidColorBrush.Parse("#4A5568");

    [ObservableProperty]
    private TimeSpan _elapsedMinutes = TimeSpan.Zero;

    [ObservableProperty] private string _remainingTimeFormatted = "0";

    private readonly ITimerService? _timerService;
    private readonly ISettingsService? _settingsService;
    private readonly INotificationService? _notificationService;
    public PomodoroViewModel(IServiceProvider serviceProvider) :base(serviceProvider)
    {
        _notificationService = serviceProvider.GetService<INotificationService>();
        _timerService = serviceProvider.GetService<ITimerService>();
        _settingsService = serviceProvider.GetService<ISettingsService>();
        Dispatcher.UIThread.InvokeAsync(async () =>
        {
            if (_notificationService != null)
            {
                var result = await _notificationService.CheckPermission();
                if (!result)
                {
                    await _notificationService.RequestPermission();
                }
            }
        },  DispatcherPriority.Background);
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
                _timerService?.LoadState(appSettings.AccumulatedTime, appSettings.CurrentSessionStartTime);
                IsRunning = _timerService?.IsRunning ?? false;
                var span = _timerService?.ElapsedTime;
                var elapsed = span ?? TimeSpan.Zero;
                HasProgress = elapsed != TimeSpan.Zero;
                ElapsedMinutes = elapsed;
                RemainingTimeFormatted = UiUtils.FormatTimeSpan(elapsed);
                ProgressValue = ElapsedMinutes.TotalSeconds * 6;
            }
        },  DispatcherPriority.Background);
        
    }
    
    [RelayCommand(CanExecute = nameof(CanStart))]
    private void Start()
    {
        if (IsRunning) return;
        _timerService?.Start();
        IsRunning = true;
        HasProgress = true;
        StatusColor = SolidColorBrush.Parse("#00FFFF");
        Dispatcher.UIThread.InvokeAsync(async () =>
        {
            if (_settingsService != null && _timerService != null)
            {
                var appSettings = await _settingsService.GetSettings();
                appSettings.AccumulatedTime = _timerService.AccumulatedTime;
                appSettings.CurrentSessionStartTime = _timerService.CurrentSessionStartTime;
                await _settingsService.SaveSettings(appSettings);
            }
        }, DispatcherPriority.Background);
    }

    [RelayCommand(CanExecute = nameof(CanPause))]
    private void Pause()
    {
        _timerService?.Pause();
        IsRunning = false;
        StatusColor = SolidColorBrush.Parse("#4A5568");
        Dispatcher.UIThread.InvokeAsync(async () =>
        {
            if (_settingsService != null && _timerService != null)
            {
                var appSettings = await _settingsService.GetSettings();
                appSettings.AccumulatedTime = _timerService.AccumulatedTime;
                appSettings.CurrentSessionStartTime = _timerService.CurrentSessionStartTime;
                await _settingsService.SaveSettings(appSettings);
            }
        }, DispatcherPriority.Background);
    }

    [RelayCommand(CanExecute = nameof(CanReset))]
    private void Reset()
    {
        _timerService?.Reset();
        IsRunning = false;
        HasProgress = false;
        StatusColor = SolidColorBrush.Parse("#4A5568");
        ElapsedMinutes = TimeSpan.Zero;
        Dispatcher.UIThread.InvokeAsync(async () =>
        {
            if (_settingsService != null && _timerService != null)
            {
                var appSettings = await _settingsService.GetSettings();
                appSettings.AccumulatedTime = _timerService.AccumulatedTime;
                appSettings.CurrentSessionStartTime = _timerService.CurrentSessionStartTime;
                await _settingsService.SaveSettings(appSettings);
            }
        }, DispatcherPriority.Background);
    }

    [RelayCommand]
    private void SetTimer(string minutes)
    {
        if (int.TryParse(minutes, out var minute))
        {
            Dispatcher.UIThread.InvokeAsync(async () =>
            {
                if (_notificationService != null && _timerService != null)
                {
                    var result = await _notificationService.CheckPermission();
                    if (result)
                    {
                        _notificationService.ScheduleNotification(
                            _timerService.Title, 
                            _timerService.Title, 
                            TimeSpan.FromMinutes(minute));
                        Start();
                    }
                }
            },  DispatcherPriority.Background);
        }
    }
    
    private bool CanStart() => !IsRunning;
    private bool CanPause() => IsRunning;
    private bool CanReset() => !IsRunning && HasProgress;
    

    
}