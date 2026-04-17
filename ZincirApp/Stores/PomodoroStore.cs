using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using ZincirApp.Models;
using ZincirApp.Services;
using ZincirApp.ViewModels;

namespace ZincirApp.Stores;


public interface IPomodoroStore
{
    ObservableCollection<PomodoroHistoryItemViewModel> Items { get; }
    Task LoadAsync();
    PomodoroHistoryItemViewModel? GetById(Guid id);
    Task AddAsync(PomodoroModel model);
    Task UpdateAsync(PomodoroModel model);
    Task DeleteAsync(Guid id);
}

public class PomodoroStore(IDatabaseService db) : IPomodoroStore
{
    public ObservableCollection<PomodoroHistoryItemViewModel> Items { get; } = [];
    
    public async Task LoadAsync()
    {
        var data = await db.GetAllAsync<PomodoroModel>();
        
        _cache.Clear();
        Items.Clear();

        foreach (var p in data)
        {
            var vm = new PomodoroHistoryItemViewModel(p);
            _cache[p.Id] = vm;
            Items.Add(vm);
        }
    }

    public PomodoroHistoryItemViewModel? GetById(Guid id)=> 
        _cache.TryGetValue(id, out var vm) ? vm : null;

    public async Task AddAsync(PomodoroModel model)
    {
        await db.InsertAsync<PomodoroModel>(model);
        
        var vm = new PomodoroHistoryItemViewModel(model);
        _cache[model.Id] = vm;
        Items.Add(vm);
    }

    public async Task UpdateAsync(PomodoroModel model)
    {
        await db.UpdateAsync(model);
        
        if (_cache.TryGetValue(model.Id, out var vm))
        {
            vm.Update(model);
        }
    }

    public async Task DeleteAsync(Guid id)
    {
        await db.DeleteAsync<PomodoroModel>(id);
        
        if (_cache.TryGetValue(id, out var vm))
        {
            Items.Remove(vm);
            _cache.Remove(id);
        }
    }

    private readonly Dictionary<Guid, PomodoroHistoryItemViewModel> _cache = new();


}