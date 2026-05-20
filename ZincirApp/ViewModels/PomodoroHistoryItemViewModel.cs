using System;
using CommunityToolkit.Mvvm.ComponentModel;
using ZincirApp.Models;

namespace ZincirApp.ViewModels;

public partial class PomodoroHistoryItemViewModel(PomodoroModel? pomodoroModel) : ObservableObject
{
    public PomodoroModel? Model { get; private set; } = pomodoroModel;
    [ObservableProperty] private string _titleText = pomodoroModel?.Title ?? @"Pomodoro";

    [ObservableProperty] private string _createDateText = pomodoroModel is not null ?
        $"{pomodoroModel.CreateDate.ToLongDateString()} {pomodoroModel.CreateDate.ToLongTimeString()}" : "Pomodoro";
    
    [ObservableProperty] private string _sessionDurationText =
        pomodoroModel?.SessionTimeSpan.ToString(@"hh\:mm\:ss") ??
        @"00:00:00";

    public void Update(PomodoroModel? pomodoroModel)
    {
        TitleText = pomodoroModel?.Title ?? @"Pomodoro";
        
        CreateDateText = pomodoroModel is not null ?
            $"{pomodoroModel.CreateDate.ToLongDateString()} {pomodoroModel.CreateDate.ToLongTimeString()}" : "Pomodoro";
        
        SessionDurationText = 
            pomodoroModel?.SessionTimeSpan.ToString(@"hh\:mm\:ss") ??
            @"00:00:00";
        
        Model = pomodoroModel;
    }
    
}