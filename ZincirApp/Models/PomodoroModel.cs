using System;

namespace ZincirApp.Models;

public class PomodoroModel
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public DateTime CreateDate { get; set; } = DateTime.Now;
    public int SessionQuality { get; set; }
    public TimeSpan SessionTimeSpan { get; set; }
    public Guid? HabitId { get; set; }
    public virtual HabitModel? Habit { get; set; }
    public Guid? TodoId { get; set; }
    public virtual TodoModel? Todo { get; set; }
}

