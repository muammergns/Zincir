using System;
using System.Collections.Generic;

namespace ZincirApp.Models
{
    public class TodoModel
    {
        public Guid Uuid { get; set; } = Guid.NewGuid();
        public string Title { get; set; } = string.Empty;
        public DateTime CreateDate { get; set; } = DateTime.UtcNow;
        public bool IsCompleted { get; set; }
        public bool IsUrgent { get; set; }
        public bool IsImportant { get; set; }
        public bool IsPinned { get; set; }
        public string Description { get; set; } = string.Empty;
        public Guid? ParentTodoUuid { get; set; }
        public virtual TodoModel? ParentTodo { get; set; }
        public virtual ICollection<TodoModel> SubTodos { get; set; } = new List<TodoModel>();
        public virtual ICollection<PomodoroModel> Pomodoros { get; set; } = new List<PomodoroModel>();
    }
}
