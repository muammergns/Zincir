using System;
using CommunityToolkit.Mvvm.ComponentModel;
using ZincirApp.Models;

namespace ZincirApp.ViewModels;

public sealed partial class HabitItemViewModel : ObservableObject
{

    public HabitModel Model { get; }
    [ObservableProperty] private string _title;
    [ObservableProperty] private string _remainPeriodText = string.Empty;
    [ObservableProperty] private string _targetDateText;
    private readonly DateTime _currentPeriodEndDate;
    

    public HabitItemViewModel(HabitModel model)
    {
        Model = model;
        Title = model.Title;
        TargetDateText = model.TargetEndDate?.ToLongDateString() ?? "Süre sonu yok";
        _currentPeriodEndDate = GetCurrentPeriodEndDate(
            createDate: model.CreateDate,
            dayChecked: model.Period is PeriodType.Daily,
            weekChecked: model.Period is PeriodType.Weekly,
            monthChecked: model.Period is PeriodType.Monthly,
            periodValue: (int)model.PeriodValue,
            firstDayOfWeek: DayOfWeek.Monday);
    }


    public void Tick()
    {
        RemainPeriodText = GetRemainPeriodText(_currentPeriodEndDate);
    }
    
    private static string GetRemainPeriodText(DateTime currentPeriodEndDate)
    {
        var remainingTime = currentPeriodEndDate - DateTime.Now;

        if (remainingTime.TotalDays >= 1)
        {
            return $"{(int)remainingTime.TotalDays} gün {remainingTime.Hours} saat";
        }
        return remainingTime.TotalHours >= 1 ? $"{remainingTime.Hours} saat {remainingTime.Minutes} dakika" : 
            $"{remainingTime.Minutes:D2} dakika {remainingTime.Seconds:D2} saniye";
    }

    private static DateTime GetCurrentPeriodEndDate(DateTime createDate, bool dayChecked, bool weekChecked, bool monthChecked, int periodValue, DayOfWeek firstDayOfWeek)
    {
        var now = DateTime.Now.Date;
        DateTime currentPeriodEndDate;

        if (dayChecked)
        {
            var daysPassed = (int)(now - createDate.Date).TotalDays;
            var currentPeriodIndex = daysPassed / periodValue;
            currentPeriodEndDate = createDate.Date.AddDays((currentPeriodIndex + 1) * periodValue);
        }
        else if (weekChecked)
        {
            var firstDayOfFirstWeek = createDate.Date.AddDays(-(createDate.DayOfWeek - firstDayOfWeek + 7) % 7);
            if (firstDayOfFirstWeek > createDate.Date) firstDayOfFirstWeek = firstDayOfFirstWeek.AddDays(-7);

            var weeksPassed = (int)((now - firstDayOfFirstWeek).TotalDays / 7);
            var currentPeriodIndex = weeksPassed / periodValue;
            currentPeriodEndDate = firstDayOfFirstWeek.AddDays((currentPeriodIndex + 1) * periodValue * 7);
        }
        else if (monthChecked)
        {
            var firstDayOfMonth = new DateTime(createDate.Year, createDate.Month, 1);
            if (firstDayOfMonth > createDate.Date) firstDayOfMonth = firstDayOfMonth.AddMonths(-1);

            var monthsPassed = (now.Year - firstDayOfMonth.Year) * 12 + now.Month - firstDayOfMonth.Month;
            var currentPeriodIndex = monthsPassed / periodValue;
            currentPeriodEndDate = firstDayOfMonth.AddMonths((currentPeriodIndex + 1) * periodValue).AddDays(-1);
        }
        else
        {
            var daysPassed = (int)(now - createDate.Date).TotalDays;
            var currentPeriodIndex = daysPassed / periodValue;
            currentPeriodEndDate = createDate.Date.AddDays((currentPeriodIndex + 1) * periodValue);
        }

        return currentPeriodEndDate;
    }

}