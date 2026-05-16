using System;
using System.Collections.Generic;
using System.Linq;
using Avalonia.Media;
using CommunityToolkit.Mvvm.ComponentModel;
using Material.Icons;
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
        switch (typeIndex)
        {
            case 0:
                ColumnLeftValue = GetMaxStrike(periodDates, model.HabitLogs).ToString();
                ColumnLeftUnit = model.PeriodValue > 1 ? "dönem" : periodUnit;
                ColumnCenterValue = model.HabitLogs.Count.ToString();
                ColumnCenterUnit = "kez";
                break;
            case 1:
                ColumnLeftValue = GetTotalLogValue(model).ToString();
                ColumnLeftUnit = UnitText;
                ColumnCenterValue = ((periodDates.Count - 1) * (model.TargetValue ?? 0)).ToString("F0"); 
                ColumnCenterUnit = UnitText;
                break;
            case 2:
                ColumnLeftValue = GetMaxStrike(periodDates, model.HabitLogs).ToString();
                ColumnLeftUnit = model.PeriodValue > 1 ? "dönem" : periodUnit;
                ColumnCenterValue = model.HabitLogs.Count.ToString();
                ColumnCenterUnit = "kez";
                break;
            case 3:
                ColumnLeftValue = GetTotalLogValue(model).ToString();
                ColumnLeftUnit = UnitText;
                ColumnCenterValue = ((periodDates.Count - 1) * (model.TargetValue ?? 0)).ToString("F0"); 
                ColumnCenterUnit = UnitText;
                break;
            case 4:
                ColumnLeftValue = model.HabitLogs.Count(l => (int)l.Value == 1).ToString();
                ColumnLeftUnit = "kez";
                ColumnCenterValue = model.HabitLogs.Count(l => (int)l.Value == 0).ToString();
                ColumnCenterUnit = "kez";
                break;
            case 5:
                ColumnLeftValue = GetMedianValue(model).ToString();
                ColumnLeftUnit = UnitText;
                ColumnCenterValue = model.HabitLogs.Count(l => (int)l.Value > (model.TargetValue ?? 0)).ToString();
                ColumnCenterUnit = "kez";
                break;
            case 6:
                ColumnLeftValue = model.HabitLogs.Count(l => (int)l.Value == 1).ToString();
                ColumnLeftUnit = "kez";
                ColumnCenterValue = model.HabitLogs.Count(l => (int)l.Value == 0).ToString();
                ColumnCenterUnit = "kez";
                break;
            case 7:
                ColumnLeftValue = GetMedianValue(model).ToString();
                ColumnLeftUnit = UnitText;
                ColumnCenterValue = model.HabitLogs.Count(l => (int)l.Value > (model.TargetValue ?? 0)).ToString();
                ColumnCenterUnit = "kez";
                break;
            default: 
                ColumnLeftValue = string.Empty;
                ColumnLeftUnit = string.Empty;
                ColumnCenterValue = string.Empty;
                ColumnCenterUnit = string.Empty;
                ColumnRightValue = string.Empty;
                ColumnRightUnit = string.Empty;
                break;
        }
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

    private static int GetMedianValue(HabitModel model)
    {
        var totalValue = GetTotalLogValue(model);
        var medianValue = model.HabitLogs.Count > 0 ? totalValue / model.HabitLogs.Count : 0;
        return medianValue;
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

    private static int GetTotalLogValue(HabitModel model)
    {
        var totalValue = model.HabitLogs.Sum(logModel => logModel.Value);
        return Convert.ToInt32(totalValue);
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
    
    private static bool HasLogInDateRange(DateTime rangeStartDate, DateTime rangeEndDate, ICollection<HabitLogModel> habitLogs)
    {
        return habitLogs.Any(logDate => logDate.Date >= rangeStartDate && logDate.Date < rangeEndDate);
    }

    private static int GetMaxStrike(IReadOnlyList<DateTime> periodDates, ICollection<HabitLogModel> habitLogs)
    {
        ArgumentNullException.ThrowIfNull(periodDates);
        if (periodDates.Count < 2) throw new ArgumentException(null, nameof(periodDates));

        var maxConsecutiveStreak = 0;
        var currentConsecutiveStreak = 0;

        for (var periodIndex = 0; periodIndex < periodDates.Count - 1; periodIndex++)
        {
            if (HasLogInDateRange(periodDates[periodIndex], periodDates[periodIndex + 1], habitLogs))
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

    

}

