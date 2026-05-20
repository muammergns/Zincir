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
    Task ReLoadAsync();
    PomodoroHistoryItemViewModel? GetById(Guid id);
    Task AddAsync(PomodoroModel model);
    Task UpdateAsync(PomodoroModel model);
    Task DeleteAsync(Guid id);
    void AddUpdateCache(PomodoroModel model);
}

public class PomodoroStore: IPomodoroStore
{
    public ObservableCollection<PomodoroHistoryItemViewModel> Items { get; } = [];
    private readonly IDatabaseService _db;
    private bool _isInitialized;

    public PomodoroStore(IDatabaseService db)
    {
        _db = db;
        _isInitialized = false;
        Task.Run(async () => await LoadAsync());
    }
    
    public async Task LoadAsync()
    {
        if (_isInitialized) return;
        _isInitialized = true;
        var data = await _db.GetPomodorosAsync();
        _cache.Clear();
        Items.Clear();
        foreach (var p in data)
        {
            var vm = new PomodoroHistoryItemViewModel(p);
            _cache[p.Id] = vm;
            Items.Add(vm);
        }
    }
    
    public async Task ReLoadAsync()
    {
        var data = await _db.GetPomodorosAsync();
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
        await _db.InsertAsync<PomodoroModel>(model);
        
        var vm = new PomodoroHistoryItemViewModel(model);
        _cache[model.Id] = vm;
        Items.Insert(0,vm);
    }

    public async Task UpdateAsync(PomodoroModel model)
    {
        await _db.UpdateAsync(model);
        
        if (_cache.TryGetValue(model.Id, out var vm))
        {
            vm.Update(model);
        }
    }

    public async Task DeleteAsync(Guid id)
    {
        await _db.DeleteAsync<PomodoroModel>(id);
        
        if (_cache.TryGetValue(id, out var vm))
        {
            Items.Remove(vm);
            _cache.Remove(id);
        }
    }

    public void AddUpdateCache(PomodoroModel model)
    {
        var vm = new PomodoroHistoryItemViewModel(model);
        _cache[model.Id] = vm;
        Items.Insert(0,vm);
    }

    private readonly Dictionary<Guid, PomodoroHistoryItemViewModel> _cache = new();


}