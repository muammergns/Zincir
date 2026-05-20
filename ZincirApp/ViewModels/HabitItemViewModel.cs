using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Avalonia.Controls;
using Avalonia.Media;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using Material.Icons;
using ZincirApp.Messages;
using ZincirApp.Models;

namespace ZincirApp.ViewModels;

public sealed partial class HabitItemViewModel : ObservableObject
{
    public HabitModel Model { get; }
    [ObservableProperty] private string _title;
    [ObservableProperty] private IBrush? _baseColor;
    [ObservableProperty] private string _targetValueText;
    [ObservableProperty] private string _unitText;
    [ObservableProperty] private string _currentProgressValueText;
    [ObservableProperty] private IBrush? _checkButtonBorderColor;
    [ObservableProperty] private IBrush? _checkButtonBackgroundColor;
    [ObservableProperty] private IBrush? _checkButtonForegroundColor;
    [ObservableProperty] private MaterialIconKind? _checkButtonKind;
    [ObservableProperty] private bool _isValueType;
    [ObservableProperty] private bool _isTickType;
    [ObservableProperty] private string _periodLengthValueText;
    [ObservableProperty] private string _taskLastDateText;
    [ObservableProperty] private MaterialIconKind? _taskLastDateKind;
    [ObservableProperty] private string _pomodoroSpendTimeText; 
    [ObservableProperty] private string _periodRemainTimeText; 
    [ObservableProperty] private bool _isCurrentPeriodCompleted; 
    [ObservableProperty] private string _columnLeftEmoji;
    [ObservableProperty] private string _columnCenterEmoji;
    [ObservableProperty] private string _columnRightEmoji;
    [ObservableProperty] private string _columnLeftDescription;
    [ObservableProperty] private string _columnCenterDescription;
    [ObservableProperty] private string _columnRightDescription;
    [ObservableProperty] private string _columnLeftValue;
    [ObservableProperty] private string _columnCenterValue;
    [ObservableProperty] private string _columnRightValue;
    [ObservableProperty] private string _columnLeftUnit;
    [ObservableProperty] private string _columnCenterUnit;
    [ObservableProperty] private string _columnRightUnit;
    [ObservableProperty] private double _addHabitLogValue;
    
    public DateTime CurrentPeriodEndDate { get; }
    
    private static readonly string[] BaseColors = 
    [
        "22c55e", // 1. Yeşil edinme·onay·hedefsiz
        "3b82f6", // 2. Mavi edinme·değer·hedefsiz
        "14b8a6", // 3. Turkuaz edinme·onay·hedefli
        "8b5cf6", // 4. Mor edinme·değer·hedefli
        "64748b", // 5. Gri-Mavi/Slate kurtulma·onay·hedefsiz
        "f97316", // 6. Turuncu kurtulma·değer·hedefsiz
        "ec4899", // 7. Pembe kurtulma·onay·hedefli
        "ef4444", // 8. Kırmızı kurtulma·değer·hedefli
    ];

    private static readonly string[] ColumnLeftEmojis =
    [
        "🏆", "⬆️", "🏆", "⬆️", "🛡️", "📊", "🛡️", "📊"
    ];
    private static readonly string[] ColumnCenterEmojis =
    [
        "✅", "🎯", "✅", "🎯", "❌", "❌", "❌", "❌"
    ];
    private static readonly string[] ColumnRightEmojis =
    [
        "🔢", "🔢", "⏳", "⏳", "🔢", "🔢", "⏳", "⏳"
    ];

    private static readonly string[] ColumnLeftDescriptions =
    [
        "En uzun seri",
        "Toplam birikim",
        "En uzun seri",
        "Toplam birikim",
        "Kurtulma",
        "Ortalama",
        "Kurtulma",
        "Ortalama"
    ];
    private static readonly string[] ColumnCenterDescriptions =
    [
        "Toplam onay",
        "Olması gereken",
        "Toplam onay",
        "Olması gereken",
        "Kaçırma",
        "Kaçırma",
        "Kaçırma",
        "Kaçırma"
    ];
    private static readonly string[] ColumnRightDescriptions =
    [
        "Toplam periyot",
        "Toplam periyot",
        "Hedefe kalan",
        "Hedefe kalan",
        "Toplam periyot",
        "Toplam periyot",
        "Hedefe kalan",
        "Hedefe kalan"
    ];

    

    public HabitItemViewModel(HabitModel model, List<DateTime> periodDates)
    {
        Model = model;
        Title = model.Title;
        var typeIndex = GetTypeIndex(model);
        var periodTotalValue = GetPeriodTotalValue(model, periodDates);
        var periodUnit = GetPeriodUnitText(model);
        IsCurrentPeriodCompleted = GetIsCurrentPeriodCompleted(model, periodTotalValue);
        CurrentPeriodEndDate = periodDates.LastOrDefault();
        BaseColor = GetBrush(BaseColors[typeIndex], 0.2);
        TargetValueText = model.TargetValue?.ToString("F0") ?? string.Empty;
        AddHabitLogValue = Convert.ToDouble(model.TargetValue ?? 0);
        UnitText = model.Unit;
        CurrentProgressValueText = model.TargetValue is not null ?  periodTotalValue.ToString("F0") : string.Empty;
        CheckButtonBackgroundColor = GetBrush(BaseColors[typeIndex], 0.2);
        CheckButtonBorderColor = GetBrush(BaseColors[typeIndex]);
        CheckButtonForegroundColor = GetBrush(BaseColors[typeIndex]);
        CheckButtonKind = IsCurrentPeriodCompleted ? MaterialIconKind.Tick : model.IsAtMost ? MaterialIconKind.Minus : MaterialIconKind.Plus;
        IsValueType = model.TargetValue is not null;
        IsTickType = model.TargetValue is null;
        PeriodLengthValueText = $@"{(model.PeriodValue > 1 ? $@"{model.PeriodValue:F0}" : "Her")} {periodUnit}";
        PomodoroSpendTimeText = GetPomodoroSpendTimeText(model, periodDates);
        TaskLastDateText = model.TargetEndDate?.ToLongDateString() ?? "Süre sonu yok";
        TaskLastDateKind = model.TargetEndDate?.ToLongDateString() is not null ? MaterialIconKind.DateRange : MaterialIconKind.Infinity;
        PeriodRemainTimeText = GetRemainPeriodText(periodDates.LastOrDefault());
        ColumnLeftEmoji = ColumnLeftEmojis[typeIndex];
        ColumnCenterEmoji = ColumnCenterEmojis[typeIndex];
        ColumnRightEmoji = ColumnRightEmojis[typeIndex];
        ColumnLeftDescription =  ColumnLeftDescriptions[typeIndex];
        ColumnCenterDescription =  ColumnCenterDescriptions[typeIndex];
        ColumnRightDescription =  ColumnRightDescriptions[typeIndex];
        ColumnRightUnit = model.PeriodValue > 1 ? "dönem" : periodUnit;
        ColumnRightValue = model.TargetEndDate is not null ? 
            GetLastPeriodCount(CurrentPeriodEndDate, model).ToString() : 
            (periodDates.Count - 1).ToString();
        ColumnCenterUnit = typeIndex is 1 or 3 ? UnitText : "kez";
        ColumnLeftUnit = typeIndex switch
        {
            0 or 2 => model.PeriodValue > 1 ? "dönem" : periodUnit,
            1 or 3 or 5 or 7 => UnitText,
            _ => "kez"
        };
        ColumnLeftValue = typeIndex switch
        {
            0 or 2 => GetMaxStrike(periodDates, model.HabitLogs, h => (int)h.Value==1).ToString(),
            1 or 3 => model.HabitLogs.Sum(logModel => logModel.Value).ToString("F0"),
            4 or 6 => GetTotalStrike(periodDates, model.HabitLogs, h => (int)h.Value==1).ToString(),
            5 or 7 => GetMedianValue(periodDates, model.HabitLogs).ToString(),
            _ => string.Empty
        };
        ColumnCenterValue = typeIndex switch
        {
            0 or 2 => GetTotalStrike(periodDates, model.HabitLogs).ToString(),
            1 or 3 => ((periodDates.Count - 1) * (model.TargetValue ?? 0)).ToString("F0"),
            4 or 6 => GetTotalStrike(periodDates, model.HabitLogs, h => (int)h.Value==0).ToString(),
            5 or 7 => GetTotalStrike(periodDates, model.HabitLogs, l => (int)l.Value > (model.TargetValue ?? 0)).ToString(),
            _ => string.Empty
        };
    }

    private static string GetPomodoroSpendTimeText(HabitModel model, List<DateTime> periodDates)
    {
        var totalDuration = TimeSpan.FromTicks(model.Pomodoros
            .Where(p => p.CreateDate >= periodDates.FirstOrDefault() && p.CreateDate <= periodDates.LastOrDefault())
            .Sum(pomodoroModel => pomodoroModel.SessionTimeSpan.Ticks));
        return $@"{(int)totalDuration.TotalDays}.{totalDuration:hh\:mm\:ss}";
    }

    private static string GetPeriodUnitText(HabitModel model)
    {
        return model.Period switch
        {
            PeriodType.Daily => "gün",
            PeriodType.Weekly => "hafta",
            PeriodType.Monthly => "ay",
            _ => ""
        };
    }
    

    private static int GetPeriodTotalValue(HabitModel model, List<DateTime> periodDates)
    {
        var periodTotalValue = model.HabitLogs
            .Where(l => l.Date >= periodDates.FirstOrDefault() && l.Date <= periodDates.LastOrDefault())
            .Sum(l => l.Value);
        return Convert.ToInt32(periodTotalValue);
    }

    private static bool GetIsCurrentPeriodCompleted(HabitModel model, int periodTotalValue)
    {
        var isCompleted = model.TargetValue is not null ? periodTotalValue >= (double)model.TargetValue : periodTotalValue > 0;
        return isCompleted;
    }

    private static int GetTypeIndex(HabitModel model)
    {
        var typeIndex = 0;
        if (model.TargetValue is not null)
            typeIndex |= 1 << 0;
        if (model.TargetEndDate is not null)
            typeIndex |= 1 << 1;
        if (model.IsAtMost)
            typeIndex |= 1 << 2;
        return typeIndex;
    }

    private static List<HabitLogModel> GetLogsInDateRange(DateTime rangeStartDate, DateTime rangeEndDate,
        ICollection<HabitLogModel> habitLogs)
    {
        return habitLogs.Where(h => h.Date >= rangeStartDate && h.Date <= rangeEndDate).ToList();
    }
    
    
    private static int GetMedianValue(IReadOnlyList<DateTime> periodDates, ICollection<HabitLogModel> habitLogs,
        Expression<Func<HabitLogModel, double>>? expression = null)
    {
        ArgumentNullException.ThrowIfNull(periodDates);
        if (periodDates.Count < 2) throw new ArgumentException(null, nameof(periodDates));
        expression ??= item => item.Value;
        var compiledExpression = expression.Compile();
        var totalValue = habitLogs.Sum(compiledExpression);
        return Convert.ToInt32(totalValue / (periodDates.Count - 1));
    }

    private static int GetTotalStrike(IReadOnlyList<DateTime> periodDates, ICollection<HabitLogModel> habitLogs,
        Expression<Func<HabitLogModel, bool>>? expression = null)
    {
        ArgumentNullException.ThrowIfNull(periodDates);
        if (periodDates.Count < 2) throw new ArgumentException(null, nameof(periodDates));
        var totalStrike = 0;
        expression ??= item => true;
        var compiledExpression = expression.Compile();
        for (var periodIndex = 0; periodIndex < periodDates.Count - 1; periodIndex++)
        {
            var logs = GetLogsInDateRange(periodDates[periodIndex], periodDates[periodIndex + 1], habitLogs);
            if (logs.Any(compiledExpression))
            {
                totalStrike++;
            }
        }
        return totalStrike;
    }

    private static int GetMaxStrike(IReadOnlyList<DateTime> periodDates, ICollection<HabitLogModel> habitLogs, 
        Expression<Func<HabitLogModel, bool>>? expression = null)
    {
        ArgumentNullException.ThrowIfNull(periodDates);
        if (periodDates.Count < 2) throw new ArgumentException(null, nameof(periodDates));
        expression ??= item => true;
        var compiledExpression = expression.Compile();
        var maxConsecutiveStreak = 0;
        var currentConsecutiveStreak = 0;

        for (var periodIndex = 0; periodIndex < periodDates.Count - 1; periodIndex++)
        {
            var logs = GetLogsInDateRange(periodDates[periodIndex], periodDates[periodIndex + 1], habitLogs);
            if (logs.Any(compiledExpression))
            {
                currentConsecutiveStreak++;
                maxConsecutiveStreak = Math.Max(maxConsecutiveStreak, currentConsecutiveStreak);
            }
            else
            {
                currentConsecutiveStreak = 0;
            }
        }

        return maxConsecutiveStreak;
    }

    private static int GetLastPeriodCount(DateTime endDate, HabitModel model)
    {
        if (model.TargetEndDate is null) return 0;
        var count = 0;
        var bufferDate = endDate;
        while (bufferDate < model.TargetEndDate)
        {
            bufferDate = model.Period switch
            {
                PeriodType.Daily => bufferDate.AddDays((double)model.PeriodValue),
                PeriodType.Weekly => bufferDate.AddDays((double)(model.PeriodValue * 7)),
                PeriodType.Monthly => bufferDate.AddMonths((int)model.PeriodValue),
                _ => DateTime.MaxValue
            };
            count++;
        }
        return count;
    }

    private static IBrush GetBrush(string color, double alpha = 1)
    {
        if (alpha is > 1 or < 0) alpha = 1;
        var alphaString = Convert.ToInt32(alpha*255).ToString("X2");
        return Brush.Parse($@"#{alphaString}{color}");
    }

    public void Tick()
    {
        PeriodRemainTimeText = GetRemainPeriodText(CurrentPeriodEndDate);
    }
    
    private static string GetRemainPeriodText(DateTime currentPeriodEndDate)
    {
        var remainingTime = currentPeriodEndDate - DateTime.Now;

        if (remainingTime.TotalDays >= 1)
        {
            return $"{(int)remainingTime.TotalDays} gün {remainingTime.Hours} sa";
        }
        return remainingTime.TotalHours >= 1 ? $"{remainingTime.Hours} sa {remainingTime.Minutes} dk" : 
            $"{remainingTime.Minutes:D2} dk {remainingTime.Seconds:D2} sn";
    }

    [RelayCommand]
    private void AddPlusHabitLog(object? sender)
    {
        if (sender is Button button)
        {
            button.Flyout?.Hide();
        }
        WeakReferenceMessenger.Default.Send(new AddPlusHabitLogMessage(this));
    }
    
    [RelayCommand]
    private void AddMinusHabitLog(object? sender)
    {
        if (sender is Button button)
        {
            button.Flyout?.Hide();
        }
        WeakReferenceMessenger.Default.Send(new AddMinusHabitLogMessage(this));
    }
    
    [RelayCommand]
    private void AddValueHabitLog(object? sender)
    {
        if (sender is Button button)
        {
            button.Flyout?.Hide();
        }
        WeakReferenceMessenger.Default.Send(new AddValueHabitLogMessage(this));
    }

}

