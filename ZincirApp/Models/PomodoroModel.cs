using System;

namespace ZincirApp.Models
{
    public class PomodoroModel
    {
        public Guid Uuid { get; set; } = Guid.NewGuid();
        public DateTime CreateDate { get; set; } = DateTime.UtcNow;
        public TimeSpan SessionTimeSpan { get; set; }
        public Guid? HabitUuid { get; set; }
        public virtual HabitModel? Habit { get; set; }
        public Guid? TodoUuid { get; set; }
        public virtual TodoModel? Todo { get; set; }
    }
}
