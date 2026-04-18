using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using ZincirApp.Locale;
using ZincirApp.Messages;
using ZincirApp.Models;
using ZincirApp.Services;
using ZincirApp.Stores;

namespace ZincirApp.ViewModels;

public partial class TodoViewModel : ViewModelBase
{
    public ITodoStore Store { get; }
    private readonly INavigationService _navService;
    private readonly List<TodoModel?> _parentTodoModels = [];
    [ObservableProperty] private string? _parentTodoTitle;
    [ObservableProperty] private int _tabSelectionIndex;
    public TodoViewModel(ITodoStore store, INavigationService navigationService)
    {
        Store = store;
        _navService = navigationService;
        Task.Run(async () =>
        {
            await Store.LoadAsync();
            _parentTodoModels.Clear();
            ParentTodoTitle = "Görevler";
            _navService.NavigateToSub<TodoEditViewModel>();
        });
    }

    partial void OnTabSelectionIndexChanged(int value)
    {
        var lastModel = _parentTodoModels.LastOrDefault();
        if (lastModel is null) 
        {
            Task.Run(async () =>
            {
                await Store.LoadAsync(value == 1);
                _parentTodoModels.Clear();
                ParentTodoTitle = "Görevler";
                _navService.NavigateToSub<TodoEditViewModel>();
            });
        }
        else
        {
            _navService.NavigateToSub(
                new TodoEditViewModel(Store, _navService, null, lastModel));
            ParentTodoTitle = "Görevler";
            foreach (var parentTodoModel in _parentTodoModels)
            {
                ParentTodoTitle += $@" / {parentTodoModel?.Title ?? "-"}";
            }
            Task.Run(async () => {
                await Store.LoadAsync(lastModel.Id, value == 1);
            });
        }
    }

    [RelayCommand]
    private void DeleteTodo(TodoItemViewModel? model)
    {
        Task.Run(async () =>
        {
            if (model is { Model: not null }) 
                await Store.DeleteAsync(model.Model.Id);
        });
    }

    [RelayCommand]
    private void ChangePinnedTodo(TodoItemViewModel? model)
    {
        Task.Run(async () =>
        {
            if (model is { Model: not null })
            {
                model.Model.IsPinned = !model.Model.IsPinned;
                await Store.UpdateAsync(model.Model);
            }
        });
    }

    [RelayCommand]
    private void ShowSubTodoList(TodoItemViewModel? model)
    {
        if (model is not { Model: not null }) return;
        _parentTodoModels.Add(model.Model);
        ParentTodoTitle = "Görevler";
        foreach (var parentTodoModel in _parentTodoModels)
        {
            ParentTodoTitle += $@" / {parentTodoModel?.Title ?? "-"}";
        }
        _navService.NavigateToSub(
            new TodoEditViewModel(Store, _navService, null, model.Model));
        Task.Run(async () => {
            await Store.LoadAsync(model.Model.Id);
        });
    }

    [RelayCommand]
    private void ShowTodoDetails(TodoItemViewModel? model)
    {
        _navService.NavigateToSub(model is { Model: not null }
            ? new TodoEditViewModel(Store, _navService, model.Model, _parentTodoModels.LastOrDefault())
            : new TodoEditViewModel(Store, _navService, null, _parentTodoModels.LastOrDefault()));
        WeakReferenceMessenger.Default.Send(new DrawerChangedMessage(true));
    }

    [RelayCommand]
    private void ShowParentTodoList()
    {
        if (_parentTodoModels.Count == 0) return;
        _parentTodoModels.Remove(_parentTodoModels.LastOrDefault());
        var lastModel = _parentTodoModels.LastOrDefault();
        if (lastModel is null) 
        {
            Task.Run(async () =>
            {
                await Store.LoadAsync();
                _parentTodoModels.Clear();
                ParentTodoTitle = "Görevler";
                _navService.NavigateToSub<TodoEditViewModel>();
            });
        }
        else
        {
            _navService.NavigateToSub(
                new TodoEditViewModel(Store, _navService, null, lastModel));
            ParentTodoTitle = "Görevler";
            foreach (var parentTodoModel in _parentTodoModels)
            {
                ParentTodoTitle += $@" / {parentTodoModel?.Title ?? "-"}";
            }
            Task.Run(async () => {
                await Store.LoadAsync(lastModel.Id);
            });
        }
    }
    
    [RelayCommand]
    private void ShowMainTodoList()
    {
        if (_parentTodoModels.Count == 0) return;
        Task.Run(async () =>
        {
            await Store.LoadAsync();
            _parentTodoModels.Clear();
            ParentTodoTitle = "Görevler";
            _navService.NavigateToSub<TodoEditViewModel>();
        });
    }

    [RelayCommand]
    private void ChangeCheckedTodo(TodoItemViewModel? model)
    {
        Task.Run(async () =>
        {
            if (model is { Model: not null })
            {
                model.Model.IsCompleted = model.IsCompleted;
                await Store.UpdateAsync(model.Model);
            }
        });
    }
}