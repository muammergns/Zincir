using System;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using ZincirApp.Messages;
using ZincirApp.Models;
using ZincirApp.Services;
using ZincirApp.Stores;

namespace ZincirApp.ViewModels;

public partial class HabitEditViewModel : ViewModelBase
{
    private HabitModel? _habitModel;
    private readonly IHabitStore? _habitStore;
    private readonly INavigationService? _navigationService;
    
    [ObservableProperty] private string? _title;
    [ObservableProperty] private decimal? _periodValue;
    [ObservableProperty] private bool? _dayChecked = true;
    [ObservableProperty] private bool? _weekChecked = false;
    [ObservableProperty] private bool? _monthChecked = false;
    [ObservableProperty] private bool? _isAtMost;
    [ObservableProperty] private bool? _isValue;
    [ObservableProperty] private decimal? _targetValue;
    [ObservableProperty] private string? _targetUnit;
    [ObservableProperty] private DateTimeOffset? _targetEndDate;
    
    public HabitEditViewModel(IHabitStore? habitStore, INavigationService navigationService, HabitModel? habitModel = null)
    {
        _habitStore = habitStore;
        _habitModel = habitModel;
        _navigationService = navigationService;
        Title = habitModel?.Title ?? string.Empty;
        PeriodValue = habitModel?.PeriodValue ?? 1;
        DayChecked = habitModel == null || habitModel.Period == PeriodType.Daily;
        WeekChecked = habitModel is { Period: PeriodType.Weekly };
        MonthChecked = habitModel is { Period: PeriodType.Monthly };
        IsAtMost = habitModel?.IsAtMost ?? false;
        IsValue = habitModel?.TargetValue != null;
        TargetValue = habitModel?.TargetValue ?? 0;
        TargetUnit = habitModel?.Unit ?? string.Empty;
        TargetEndDate = habitModel?.TargetEndDate;
        
    }
    
    
    [RelayCommand]
    private void ClearDate()
    {
        TargetEndDate = null;
    }

    [RelayCommand]
    private void Save()
    {
        if (_habitStore == null) return;
        var isNew = false;
        if (_habitModel == null)
        {
            _habitModel = new HabitModel()
            {
                CreateDate = DateTime.Now
            }; 
            isNew = true;
        }
        else
        {
            _habitModel.TargetEndDate = DateTime.Now;
        }
        Task.Run(async () =>
        {
            if (Title is null) return;
            
            _habitModel.Title = Title;
            _habitModel.TargetEndDate = TargetEndDate?.Date;
            _habitModel.IsAtMost = IsAtMost ?? false;
            _habitModel.TargetValue = TargetValue;
            _habitModel.Period = 
                DayChecked ?? true ? PeriodType.Daily : 
                WeekChecked ?? false ? PeriodType.Weekly : 
                MonthChecked ?? false ? PeriodType.Monthly : 
                PeriodType.Daily;
            _habitModel.PeriodValue = PeriodValue ?? 1;
            _habitModel.Unit = TargetUnit ?? string.Empty;
            
            if (isNew)
            {
                await _habitStore.AddHabitAsync(_habitModel);
                _navigationService?.NavigateToSub<HabitEditViewModel>();
            }
            else
            {
                await _habitStore.UpdateAsync(_habitModel);
            }
            WeakReferenceMessenger.Default.Send(new DrawerChangedMessage(false));
        });
    }
}