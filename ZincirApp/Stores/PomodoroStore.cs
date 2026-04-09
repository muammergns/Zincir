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
    PomodoroHistoryItemViewModel? GetById(Guid uuid);
    Task AddAsync(PomodoroModel model);
    Task UpdateAsync(PomodoroModel model);
    Task DeleteAsync(Guid uuid);
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
            _cache[p.Uuid] = vm;
            Items.Add(vm);
        }
    }

    public PomodoroHistoryItemViewModel? GetById(Guid uuid)=> 
        _cache.TryGetValue(uuid, out var vm) ? vm : null;

    public async Task AddAsync(PomodoroModel model)
    {
        await db.InsertAsync<PomodoroModel>(model);
        
        var vm = new PomodoroHistoryItemViewModel(model);
        _cache[model.Uuid] = vm;
        Items.Add(vm);
    }

    public async Task UpdateAsync(PomodoroModel model)
    {
        await db.UpdateAsync(model);
        
        if (_cache.TryGetValue(model.Uuid, out var vm))
        {
            vm.Update(model);
        }
    }

    public async Task DeleteAsync(Guid uuid)
    {
        await db.DeleteAsync<PomodoroModel>(uuid);
        
        if (_cache.TryGetValue(uuid, out var vm))
        {
            Items.Remove(vm);
            _cache.Remove(uuid);
        }
    }

    private readonly Dictionary<Guid, PomodoroHistoryItemViewModel> _cache = new();


}