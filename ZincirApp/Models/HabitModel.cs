using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ZincirApp.Models;

public enum PeriodType
{
    Daily,
    Weekly,
    Monthly
}

public class HabitModel
{
    public Guid Id { get; init; } = Guid.NewGuid();
    [MaxLength(200)]
    public string Title { get; set; } = string.Empty;
    public DateTime CreateDate { get; init; } = DateTime.Now;
    public DateTime? TargetEndDate { get; set; }
    public bool IsAtMost { get; set; }
    public decimal? TargetValue { get; set; }
    public PeriodType Period { get; set; }
    public decimal PeriodValue { get; set; }
    [MaxLength(200)]
    public string Unit { get; set; } = string.Empty;
    public virtual ICollection<HabitLogModel> HabitLogs { get; init; } = new List<HabitLogModel>();
    public virtual ICollection<PomodoroModel> Pomodoros { get; init; } = new List<PomodoroModel>();
}

