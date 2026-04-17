using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ZincirApp.Models
{
    public class TodoModel
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        [MaxLength(200)]
        public string Title { get; set; } = string.Empty;
        public DateTime CreateDate { get; set; } = DateTime.Now;
        public DateTime? TargetDate { get; set; }
        public DateTime? UpdateDate { get; set; }
        public bool IsCompleted { get; set; }
        public bool IsUrgent { get; set; }
        public bool IsImportant { get; set; }
        public bool IsPinned { get; set; }
        [MaxLength(1000)]
        public string Description { get; set; } = string.Empty;
        public Guid? ParentTodoId { get; set; }
        public virtual TodoModel? ParentTodo { get; set; }
        public virtual ICollection<TodoModel> SubTodos { get; set; } = new List<TodoModel>();
        public virtual ICollection<PomodoroModel> Pomodoros { get; set; } = new List<PomodoroModel>();
    }
}
