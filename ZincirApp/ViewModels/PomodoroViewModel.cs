using System;
using System.Threading.Tasks;
using Avalonia.Media;
using Avalonia.Threading;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using ZincirApp.Extensions;
using ZincirApp.Messages;
using ZincirApp.Models;
using ZincirApp.Services;
using ZincirApp.Stores;

namespace ZincirApp.ViewModels;

public partial class PomodoroViewModel : ViewModelBase
{
    
    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(StartCommand))]
    [NotifyCanExecuteChangedFor(nameof(ResetCommand))]
    private bool _isRunning;
    
    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(StartCommand))]
    [NotifyCanExecuteChangedFor(nameof(ResetCommand))]
    private bool _isTextBoxEnabled;
    
    [ObservableProperty]
    private double _progressValue;

    [ObservableProperty]
    private IBrush _statusColor = SolidColorBrush.Parse("#4A5568");

    [ObservableProperty]
    private TimeSpan _elapsedMinutes = TimeSpan.Zero;

    [ObservableProperty] private string _remainingTimeFormatted = "0";
    [ObservableProperty] private string? _pomodoroTitleText;
    
    private HabitModel? _habitModel;
    private TodoModel? _todoModel;

    private readonly ITimerService? _timerService;
    private readonly ISettingsService? _settingsService;
    private readonly INotificationService? _notificationService;
    private readonly INavigationService? _navigationService;
    private readonly IPomodoroStore? _pomodoroStore;
    private readonly IHabitStore? _habitStore;
    private readonly ITodoStore? _todoStore;
    public PomodoroViewModel(
        INavigationService navigationService,
        INotificationService notificationService, 
        ISettingsService settingsService, 
        IPomodoroStore pomodoroStore,
        IHabitStore habitStore,
        ITodoStore todoStore,
        ITimerService timerService)
    {
        _notificationService = notificationService;
        _timerService = timerService;
        _settingsService = settingsService;
        _navigationService = navigationService;
        _pomodoroStore = pomodoroStore;
        _habitStore = habitStore;
        _todoStore = todoStore;
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
                _habitModel = appSettings.HabitSessionId is not null ? await habitStore.GetById((Guid)appSettings.HabitSessionId) : null;
                _todoModel = appSettings.TodoSessionId is not null ? await todoStore.GetById((Guid)appSettings.TodoSessionId) : null;
                var title = _habitModel?.Title ?? _todoModel?.Title ?? null;
                _timerService?.LoadState(
                    appSettings.AccumulatedTime, 
                    appSettings.CurrentSessionStartTime, 
                    title);
                IsRunning = _timerService?.IsRunning ?? false;
                IsTextBoxEnabled = !IsRunning;
                var elapsed = _timerService?.ElapsedTime ?? TimeSpan.Zero;
                ElapsedMinutes = TimeSpan.FromTicks(elapsed.Ticks);
                RemainingTimeFormatted = UiUtils.FormatTimeSpan(elapsed);
                ProgressValue = ElapsedMinutes.TotalSeconds * 6;
                PomodoroTitleText = title;
                if (_habitModel != null || _todoModel != null)
                {
                    Start();
                }
            }
        },  DispatcherPriority.Background);
    }
    
    [RelayCommand(CanExecute = nameof(CanStart))]
    private void Start()
    {
        if (IsRunning) return;
        _timerService?.Start();
        if (string.IsNullOrWhiteSpace(PomodoroTitleText)) PomodoroTitleText = "Pomodoro";
        _timerService?.Title = PomodoroTitleText;
        IsRunning = true;
        IsTextBoxEnabled = !IsRunning;
        StatusColor = SolidColorBrush.Parse("#00FFFF");
        Task.Run(async () =>
        {
            if (_settingsService != null && _timerService != null)
            {
                var appSettings = await _settingsService.GetSettings();
                appSettings.AccumulatedTime = _timerService.AccumulatedTime;
                appSettings.CurrentSessionStartTime = _timerService.CurrentSessionStartTime;
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
            Title = PomodoroTitleText,
            SessionTimeSpan = ElapsedMinutes,
            HabitId = _habitModel?.Id,
            TodoId = _todoModel?.Id,
        };
        _timerService?.Reset();
        IsRunning = false;
        IsTextBoxEnabled = !IsRunning;
        StatusColor = SolidColorBrush.Parse("#4A5568");
        ElapsedMinutes = TimeSpan.Zero;
        PomodoroTitleText = null;
        
        
        Task.Run(async () =>
        {
            var habitView = false;
            var todoView = false;
            if (_habitModel != null && _habitStore != null && _pomodoroStore != null)
            {
                await _habitStore.AddPomodoroAsync(pomodoroModel);
                pomodoroModel.Habit = _habitModel;
                _pomodoroStore.AddUpdateCache(pomodoroModel);
                habitView = true;
            }
            else if (_todoModel != null && _todoStore != null && _pomodoroStore != null)
            {
                await _todoStore.AddPomodoroAsync(pomodoroModel);
                pomodoroModel.Todo = _todoModel;
                _pomodoroStore.AddUpdateCache(pomodoroModel);
                todoView = true;
            }
            else if (_pomodoroStore != null)
            {
                await _pomodoroStore.AddAsync(pomodoroModel);
            }
            if (_settingsService != null && _timerService != null)
            {
                var appSettings = await _settingsService.GetSettings();
                appSettings.AccumulatedTime = TimeSpan.Zero;
                appSettings.CurrentSessionStartTime = null;
                appSettings.HabitSessionId = null;
                appSettings.TodoSessionId = null;
                await _settingsService.SaveSettings(appSettings);
            }
            
            if (habitView)
            {
                _navigationService?.NavigateTo<HabitViewModel>();
                _navigationService?.NavigateToSub<HabitEditViewModel>();
            }
            
            if (todoView)
            {
                _navigationService?.NavigateTo<TodoViewModel>();
                _navigationService?.NavigateToSub<TodoEditViewModel>();
            }
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
                            _timerService.Title ?? "Pomodoro", 
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
        _navigationService?.NavigateToSub<PomodoroHistoryViewModel>();
        if (!int.TryParse(isOpen, out var index)) return;
        if (index==1)
        {
            WeakReferenceMessenger.Default.Send(new DrawerChangedMessage(true));
        }
    }
    
    
}