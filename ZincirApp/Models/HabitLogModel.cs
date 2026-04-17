using System;

namespace ZincirApp.Models;
    
public class HabitLogModel
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid HabitId { get; set; }
    public DateTime Date { get; set; } = DateTime.Now;
    public double Value { get; set; }
    public TimeSpan? Duration { get; set; }
    public virtual HabitModel? Habit { get; set; }
}
