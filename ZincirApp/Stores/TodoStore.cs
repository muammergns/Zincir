using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Concurrency;
using System.Reactive.Disposables;
using System.Reactive.Disposables.Fluent;
using System.Reactive.Linq;
using System.Threading;
using System.Threading.Tasks;
using DynamicData;
using DynamicData.Binding;
using DynamicData.Kernel;
using ZincirApp.Models;
using ZincirApp.Services;
using ZincirApp.ViewModels;

namespace ZincirApp.Stores;

public interface ITodoStore
{
    ObservableCollectionExtended<TodoItemViewModel> Items { get; }
    
    Task LoadAsync(
        bool isPinned, 
        bool isImportant,
        bool isUrgent, 
        bool isCompleted);
    Task LoadAsync(
        Guid parentTodoId, 
        bool isPinned, 
        bool isImportant,
        bool isUrgent, 
        bool isCompleted);
    Task<TodoModel?> GetById(Guid id);
    Task AddAsync(TodoModel model);
    Task UpdateAsync(TodoModel model);
    Task DeleteAsync(Guid id);
    Task AddPomodoroAsync(PomodoroModel model);
    Task UpdatePomodoroAsync(PomodoroModel model);
    Task DeletePomodoroAsync(Guid todoId, Guid pomodoroId);
}

public class TodoStore : ITodoStore, IDisposable
{
    private readonly IDatabaseService _db;
    private readonly SourceCache<TodoModel, Guid> _taskSource = new(t => t.Id);
    private readonly IObservableCache<TodoItemViewModel, Guid> _vmCache;
    private readonly CompositeDisposable _disposables = new();
    
    public ObservableCollectionExtended<TodoItemViewModel> Items { get; } = [];
    
    public TodoStore(IDatabaseService db)
    {
        _db = db;
        
        _vmCache = _taskSource.Connect()
            .Transform(model => new TodoItemViewModel(model))
            .AsObservableCache()
            .DisposeWith(_disposables);

        var uiContext = SynchronizationContext.Current;
        
        _vmCache.Connect()
            .SortAndBind(Items, new TodoItemComparer())
            .ObserveOn(uiContext != null ? new SynchronizationContextScheduler(uiContext) : Scheduler.Default) 
            .DisposeMany()
            .Subscribe()
            .DisposeWith(_disposables);
        
        Task.Run(async () => await LoadAsync());
    }
    
    private void UpdateCacheWithLog(Guid todoId, Action<TodoModel> updateAction)
    {
        var todoOpt = _taskSource.Lookup(todoId);
        if (!todoOpt.HasValue) return;
        var todo = todoOpt.Value;
        updateAction(todo);
        _taskSource.AddOrUpdate(todo);
    }

    public async Task LoadAsync(
        bool isPinned = false, 
        bool isImportant = false,
        bool isUrgent = false, 
        bool isCompleted = false)
    {
        var data = await _db.GetParentTodosAsync(isPinned, isImportant, isUrgent, isCompleted);
        
        _taskSource.Edit(inner =>
        {
            inner.Clear();
            inner.AddOrUpdate(data);
        });
    }
    
    public async Task LoadAsync(
        Guid parentTodoId, 
        bool isPinned = false, 
        bool isImportant = false,
        bool isUrgent = false, 
        bool isCompleted = false)
    {
        var data = await _db.GetSubTodosAsync(parentTodoId, isPinned, isImportant, isUrgent, isCompleted);
        
        _taskSource.Edit(inner =>
        {
            inner.Clear();
            inner.AddOrUpdate(data);
        });
    }

    public async Task<TodoModel?> GetById(Guid id)
    {
        return await _db.GetTodoByIdAsync(id);
    }

    public async Task AddAsync(TodoModel model)
    {
        await _db.InsertAsync(model);
        _taskSource.AddOrUpdate(model);
    }

    public async Task UpdateAsync(TodoModel model)
    {
        await _db.UpdateAsync(model);
        _taskSource.AddOrUpdate(model);
    }

    public async Task DeleteAsync(Guid id)
    {
        await _db.DeleteAsync<TodoModel>(id);
        _taskSource.Remove(id);
    }

    public async Task AddPomodoroAsync(PomodoroModel model)
    {
        if (model.TodoId is null) return;
        await _db.InsertAsync(model);
        UpdateCacheWithLog((Guid)model.TodoId, todo => todo.Pomodoros.Add(model));
    }

    public async Task UpdatePomodoroAsync(PomodoroModel model)
    {
        if (model.TodoId is null) return;
        await _db.UpdateAsync(model);
        UpdateCacheWithLog((Guid)model.TodoId, todo =>
        {
            var existing = todo.Pomodoros.FirstOrDefault(l => l.Id == model.Id);
            if (existing == null) return;
            todo.Pomodoros.Remove(existing);
            todo.Pomodoros.Add(model);
        });
    }

    public async Task DeletePomodoroAsync(Guid todoId, Guid pomodoroId)
    {
        await _db.DeleteAsync<PomodoroModel>(pomodoroId);
        UpdateCacheWithLog(todoId, todo =>
        {
            var existing = todo.Pomodoros.FirstOrDefault(l => l.Id == pomodoroId);
            if (existing != null) todo.Pomodoros.Remove(existing);
        });
    }

    public void Dispose() => _disposables.Dispose();

    private class TodoItemComparer : IComparer<TodoItemViewModel>
    {
        public int Compare(TodoItemViewModel? x, TodoItemViewModel? y)
        {
            if (ReferenceEquals(x, y)) return 0;
            if (x?.Model == null) return 1;
            if (y?.Model == null) return -1;
            var c1 = x.Model.IsCompleted.CompareTo(y.Model.IsCompleted);
            if (c1 != 0) return c1;
            var c2 = y.IsPinned.CompareTo(x.IsPinned);
            if (c2 != 0) return c2;
            var c3 = GetMatrixScore(x).CompareTo(GetMatrixScore(y));
            if (c3 != 0) return c3;
            var c4 = y.Model.TargetDate.HasValue.CompareTo(x.Model.TargetDate.HasValue);
            if (c4 != 0) return c4;
            if (x.Model.TargetDate.HasValue && y.Model.TargetDate.HasValue)
            {
                var c5 = x.Model.TargetDate.Value.CompareTo(y.Model.TargetDate.Value);
                if (c5 != 0) return c5;
            }
            var c6 = y.Model.CreateDate.CompareTo(x.Model.CreateDate);
            if (c6 != 0) return c6;
            return 0;
        }

        private int GetMatrixScore(TodoItemViewModel item)
        {
            return item.IsUrgent switch
            {
                true when item.IsImportant => 0,
                false when item.IsImportant => 1,
                true when !item.IsImportant => 2,
                _ => 3
            };
        }
    }
}