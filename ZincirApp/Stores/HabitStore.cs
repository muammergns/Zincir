using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Concurrency;
using System.Reactive.Disposables;
using System.Reactive.Disposables.Fluent;
using System.Reactive.Linq;
using System.Threading;
using System.Threading.Tasks;
using Avalonia.Threading;
using DynamicData;
using DynamicData.Binding;
using DynamicData.Kernel;
using ZincirApp.Models;
using ZincirApp.Services;
using ZincirApp.ViewModels;

namespace ZincirApp.Stores;

public interface IHabitStore
{
    ObservableCollectionExtended<HabitItemViewModel> Items { get; }
    Task LoadAsync();
    HabitItemViewModel? GetById(Guid id);
    Task AddHabitAsync(HabitModel model);
    Task UpdateAsync(HabitModel model);
    Task DeleteAsync(Guid id);
    Task AddLogAsync(HabitLogModel log);
    Task UpdateLogAsync(HabitLogModel log);
    Task DeleteLogAsync(Guid habitId, Guid logId);
}

public class HabitStore : IHabitStore, IDisposable
{
    private readonly IDatabaseService _db;
    private readonly SourceCache<HabitModel, Guid> _habitSource = new(h => h.Id);
    private readonly IObservableCache<HabitItemViewModel, Guid> _vmCache;
    private readonly CompositeDisposable _disposables = new();
    private readonly DispatcherTimer _timer =  new()
    {
        Interval = TimeSpan.FromSeconds(1)
    };
    
    public ObservableCollectionExtended<HabitItemViewModel> Items { get; } = [];

    [Obsolete("Obsolete")]
    public HabitStore(IDatabaseService db)
    {
        _db = db;
        var uiContext = SynchronizationContext.Current;
        _vmCache = _habitSource.Connect()
            .Transform(model => new HabitItemViewModel(model, GetPeriodDates(model)))
            .AsObservableCache()
            .DisposeWith(_disposables);

        _vmCache.Connect()
            .Sort(new HabitItemComparer())
            .ObserveOn(uiContext != null ? new SynchronizationContextScheduler(uiContext) : Scheduler.Default)
            .Bind(Items)
            .DisposeMany()
            .Subscribe()
            .DisposeWith(_disposables);

        _timer.Tick += Tick;
        _timer.Start();
    }

    private void Tick(object? sender, EventArgs e)
    {
        foreach (var item in Items)
        {
            item.Tick();
        }
    }
    
    public async Task LoadAsync()
    {
        var models = await _db.GetHabitsWithLogsAsync();
        _habitSource.Edit(inner =>
        {
            inner.Clear();
            inner.AddOrUpdate(models);
        });
    }

    public HabitItemViewModel? GetById(Guid id) => _vmCache.Lookup(id).ValueOrDefault();

    public async Task AddHabitAsync(HabitModel model)
    {
        await _db.InsertAsync(model);
        _habitSource.AddOrUpdate(model);
    }

    public async Task UpdateAsync(HabitModel model)
    {
        await _db.UpdateAsync(model);
        _habitSource.AddOrUpdate(model);
    }

    public async Task DeleteAsync(Guid id)
    {
        var habitOpt = _habitSource.Lookup(id);
        if (habitOpt.HasValue)
        {
            foreach (var log in habitOpt.Value.HabitLogs.ToList())
            {
                await _db.DeleteAsync<HabitLogModel>(log.Id);
            }
        }
        await _db.DeleteAsync<HabitModel>(id);
        _habitSource.Remove(id);
    }
    
    public async Task AddLogAsync(HabitLogModel log)
    {
        await _db.InsertAsync(log);
        UpdateCacheWithLog(log.HabitId, habit => habit.HabitLogs.Add(log));
    }

    public async Task UpdateLogAsync(HabitLogModel log)
    {
        await _db.UpdateAsync(log);
        UpdateCacheWithLog(log.HabitId, habit =>
        {
            var existing = habit.HabitLogs.FirstOrDefault(l => l.Id == log.Id);
            if (existing == null) return;
            habit.HabitLogs.Remove(existing);
            habit.HabitLogs.Add(log);
        });
    }

    public async Task DeleteLogAsync(Guid habitId, Guid logId)
    {
        await _db.DeleteAsync<HabitLogModel>(logId);
        UpdateCacheWithLog(habitId, habit =>
        {
            var existing = habit.HabitLogs.FirstOrDefault(l => l.Id == logId);
            if (existing != null) habit.HabitLogs.Remove(existing);
        });
    }

    private void UpdateCacheWithLog(Guid habitId, Action<HabitModel> updateAction)
    {
        var habitOpt = _habitSource.Lookup(habitId);
        if (!habitOpt.HasValue) return;
        var habit = habitOpt.Value;
        updateAction(habit);
        _habitSource.AddOrUpdate(habit);
    }

    public void Dispose()
    {
        _disposables.Dispose();
        _timer.Stop();
        _timer.Tick -= Tick;
    } 

    private class HabitItemComparer : IComparer<HabitItemViewModel>
    {
        public int Compare(HabitItemViewModel? x, HabitItemViewModel? y)
        {
            if (ReferenceEquals(x, y)) return 0;
            if (x == null) return 1;
            if (y == null) return -1;
            var c1 = y.IsCurrentPeriodCompleted.CompareTo(x.IsCurrentPeriodCompleted);
            if (c1 != 0) return c1;
            var c2 = x.CurrentPeriodEndDate.CompareTo(y.CurrentPeriodEndDate);
            if (c2 != 0) return c2;
            var c3 = y.Model.CreateDate.CompareTo(x.Model.CreateDate);
            if (c3 != 0) return c3;
            return 0;
        }
    }

    private static List<DateTime> GetPeriodDates(HabitModel model, DayOfWeek firstDayOfWeek = DayOfWeek.Monday)
    {
        var now = DateTime.Now;
        var createDate = model.CreateDate;
        var list = new List<DateTime>();
        switch (model.Period)
        {
            case PeriodType.Daily:
                var periodDateOfDay = createDate.Date;
                list.Add(periodDateOfDay);
                while (periodDateOfDay < now)
                {
                    periodDateOfDay = periodDateOfDay.AddDays((double)model.PeriodValue);
                    list.Add(periodDateOfDay);
                }
                break;
            case PeriodType.Weekly:
                var periodDateOfWeek = createDate.Date.AddDays(-(createDate.DayOfWeek - firstDayOfWeek + 7) % 7);
                if (periodDateOfWeek > createDate.Date) periodDateOfWeek = periodDateOfWeek.AddDays(-7);
                list.Add(periodDateOfWeek);
                while (periodDateOfWeek < now)
                {
                    periodDateOfWeek = periodDateOfWeek.AddDays((double)model.PeriodValue * 7);
                    list.Add(periodDateOfWeek);
                }
                break;
            case PeriodType.Monthly:
                var periodDateOfMonth = new DateTime(createDate.Year, createDate.Month, 1);
                if (periodDateOfMonth > createDate.Date) periodDateOfMonth = periodDateOfMonth.AddMonths(-1);
                list.Add(periodDateOfMonth);
                while (periodDateOfMonth < now)
                {
                    periodDateOfMonth = periodDateOfMonth.AddMonths((int)model.PeriodValue);
                    list.Add(periodDateOfMonth);
                }
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
        return list;
    }
    
    private static (DateTime,DateTime,int) GetCurrentPeriodDates(HabitModel model, DayOfWeek firstDayOfWeek = DayOfWeek.Monday)
    {
        var now = DateTime.Now.Date;
        var createDate = model.CreateDate;
        DateTime currentPeriodEndDate;
        DateTime currentPeriodStartDate;
        int currentPeriodIndex;
        var periodValue = (int)model.PeriodValue;

        switch (model.Period)
        {
            case PeriodType.Daily:
                var daysPassed = (int)(now - createDate.Date).TotalDays;
                currentPeriodIndex = daysPassed / periodValue;
                currentPeriodEndDate = createDate.Date.AddDays((currentPeriodIndex + 1) * periodValue);
                currentPeriodStartDate = currentPeriodEndDate.AddDays(-periodValue);
                break;
            case PeriodType.Weekly:
                var firstDayOfFirstWeek = createDate.Date.AddDays(-(createDate.DayOfWeek - firstDayOfWeek + 7) % 7);
                if (firstDayOfFirstWeek > createDate.Date) firstDayOfFirstWeek = firstDayOfFirstWeek.AddDays(-7);
                var weeksPassed = (int)((now - firstDayOfFirstWeek).TotalDays / 7);
                currentPeriodIndex = weeksPassed / periodValue;
                currentPeriodEndDate = firstDayOfFirstWeek.AddDays((currentPeriodIndex + 1) * periodValue * 7);
                currentPeriodStartDate = currentPeriodEndDate.AddDays(-(periodValue * 7));
                break;
            case PeriodType.Monthly:
                var firstDayOfMonth = new DateTime(createDate.Year, createDate.Month, 1);
                if (firstDayOfMonth > createDate.Date) firstDayOfMonth = firstDayOfMonth.AddMonths(-1);
                var monthsPassed = (now.Year - firstDayOfMonth.Year) * 12 + now.Month - firstDayOfMonth.Month;
                currentPeriodIndex = monthsPassed / periodValue;
                currentPeriodEndDate = firstDayOfMonth.AddMonths((currentPeriodIndex + 1) * periodValue).AddDays(-1);
                currentPeriodStartDate = currentPeriodEndDate.AddMonths(-periodValue);
                break;
            default:
                var daysPassedDefault = (int)(now - createDate.Date).TotalDays;
                currentPeriodIndex = daysPassedDefault / periodValue;
                currentPeriodEndDate = createDate.Date.AddDays((currentPeriodIndex + 1) * periodValue);
                currentPeriodStartDate = currentPeriodEndDate.AddDays(-periodValue);
                break;
        }
        return (currentPeriodStartDate, currentPeriodEndDate, currentPeriodIndex);
    }
}