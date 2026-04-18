using System;
using System.Collections.Generic;
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
    
    Task LoadAsync(bool isCompleted = false);
    Task LoadAsync(Guid parentTodoId, bool isCompleted = false);
    TodoItemViewModel? GetById(Guid id);
    Task AddAsync(TodoModel model);
    Task UpdateAsync(TodoModel model);
    Task DeleteAsync(Guid id);
}

public class TodoStore : ITodoStore, IDisposable
{
    private readonly IDatabaseService _db;
    private readonly SourceCache<TodoModel, Guid> _taskSource = new(t => t.Id);
    private readonly IObservableCache<TodoItemViewModel, Guid> _vmCache;
    private readonly CompositeDisposable _disposables = new();
    
    public ObservableCollectionExtended<TodoItemViewModel> Items { get; } = [];

    [Obsolete("Obsolete")]
    public TodoStore(IDatabaseService db)
    {
        _db = db;
        
        _vmCache = _taskSource.Connect()
            .Transform(model => new TodoItemViewModel(model))
            .AsObservableCache()
            .DisposeWith(_disposables);

        var uiContext = SynchronizationContext.Current;
        
        _vmCache.Connect()
            .Sort(new TodoItemComparer())
            .ObserveOn(uiContext != null ? new SynchronizationContextScheduler(uiContext) : Scheduler.Default) 
            .Bind(Items)
            .DisposeMany()
            .Subscribe()
            .DisposeWith(_disposables);
    }

    public async Task LoadAsync(bool isCompleted = false)
    {
        var data = await _db.GetParentTodosAsync(isCompleted);
        
        _taskSource.Edit(inner =>
        {
            inner.Clear();
            inner.AddOrUpdate(data);
        });
    }
    
    public async Task LoadAsync(Guid parentTodoId, bool isCompleted = false)
    {
        var data = await _db.GetSubTodosAsync(parentTodoId, isCompleted);
        
        _taskSource.Edit(inner =>
        {
            inner.Clear();
            inner.AddOrUpdate(data);
        });
    }

    public TodoItemViewModel? GetById(Guid id) => _vmCache.Lookup(id).ValueOrDefault();

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

    public void Dispose() => _disposables.Dispose();

    private class TodoItemComparer : IComparer<TodoItemViewModel>
    {
        public int Compare(TodoItemViewModel? x, TodoItemViewModel? y)
        {
            if (ReferenceEquals(x, y)) return 0;
            if (x?.Model == null) return 1;
            if (y?.Model == null) return -1;
            if (x.Model.IsCompleted != y.Model.IsCompleted) return x.Model.IsCompleted.CompareTo(y.Model.IsCompleted);
            
            var result = GetGroupScore(x).CompareTo(GetGroupScore(y));
            if (result != 0) return result;

            result = GetMatrixScore(x).CompareTo(GetMatrixScore(y));
            return result != 0 ? result : string.Compare(y.CreateDate, x.CreateDate, StringComparison.Ordinal);
        }

        private int GetGroupScore(TodoItemViewModel item)
        {
            if (item.IsPinned) return 0;
            if (item.Model?.TargetDate == null) return 2;
            
            return item.Model.TargetDate.Value.Date <= DateTime.Today ? 1 : 2;
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