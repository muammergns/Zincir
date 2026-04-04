using System;

namespace ZincirApp.Models;
    
public class HabitLogModel
{
    public Guid Uuid { get; set; } = Guid.NewGuid();
    public Guid HabitUuid { get; set; }
    public DateTime Date { get; set; } = DateTime.UtcNow;
    public double Value { get; set; }
    public TimeSpan? Duration { get; set; }
    public virtual HabitModel? Habit { get; set; }
}
