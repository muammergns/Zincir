using System;
using System.Collections.Generic;

namespace ZincirApp.Models
{
    public class HabitModel
    {
        public Guid Uuid { get; set; } = Guid.NewGuid();
        public string Title { get; set; } = string.Empty;
        public DateTime CreateDate { get; set; } = DateTime.UtcNow;
        public DateTime? TargetEndDate { get; set; }
        public bool IsAtMost { get; set; }
        public double TargetValue { get; set; }
        public TimeSpan Period { get; set; }
        public string Unit { get; set; } = string.Empty;
        public virtual ICollection<HabitLogModel> HabitLogs { get; set; } = new List<HabitLogModel>();
        public virtual ICollection<PomodoroModel> Pomodoros { get; set; } = new List<PomodoroModel>();
    }
}
