using System;
using CommunityToolkit.Mvvm.ComponentModel;
using ZincirApp.Models;

namespace ZincirApp.ViewModels;

public partial class PomodoroHistoryItemViewModel(PomodoroModel? pomodoroModel) : ObservableObject
{
    public PomodoroModel? Model { get; private set; } = pomodoroModel;
    [ObservableProperty] private string _titleText = 
        pomodoroModel?.Habit?.Title ??
        pomodoroModel?.Todo?.Title ??
        pomodoroModel?.CreateDate.ToLongDateString() ?? 
        @"Pomodoro";
    
    [ObservableProperty] private string _createDateText = 
        pomodoroModel?.Habit is not null || pomodoroModel?.Todo is not null ? 
        $"{pomodoroModel.CreateDate.ToLongDateString()} {pomodoroModel.CreateDate.ToLongTimeString()}" : 
        pomodoroModel?.CreateDate.ToLongTimeString() ?? 
        DateTime.Today.ToLongDateString();
    
    [ObservableProperty] private string _sessionDurationText =
        pomodoroModel?.SessionTimeSpan.ToString(@"hh\:mm\:ss") ??
        @"00:00:00";

    public void Update(PomodoroModel? pomodoroModel)
    {
        TitleText = 
            pomodoroModel?.Habit?.Title ??
            pomodoroModel?.Todo?.Title ??
            pomodoroModel?.CreateDate.ToLongDateString() ?? 
            @"Pomodoro";
        
        CreateDateText = 
            pomodoroModel?.Habit is not null || pomodoroModel?.Todo is not null ? 
            $"{pomodoroModel.CreateDate.ToLongDateString()} {pomodoroModel.CreateDate.ToLongTimeString()}" : 
            pomodoroModel?.CreateDate.ToLongTimeString() ?? 
            DateTime.Today.ToLongDateString();
        
        SessionDurationText = 
            pomodoroModel?.SessionTimeSpan.ToString(@"hh\:mm\:ss") ??
            @"00:00:00";
        
        Model = pomodoroModel;
    }
    
}