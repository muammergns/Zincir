using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
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
    [ObservableProperty] private bool _isPinnedList;
    [ObservableProperty] private bool _isUrgentList;
    [ObservableProperty] private bool _isImportantList;
    [ObservableProperty] private bool _isCompletedList;
    public TodoViewModel(ITodoStore store, INavigationService navigationService)
    {
        Store = store;
        _navService = navigationService;
        _parentTodoModels.Clear();
        ParentTodoTitle = "Görevler";
    }

    partial void OnIsPinnedListChanged(bool value)
    {
        Console.WriteLine($@"Pinned list: {value}");
        RefreshList();
    }

    partial void OnIsUrgentListChanged(bool value)
    {
        Console.WriteLine($@"Urgent list: {value}");
        RefreshList();
    }

    partial void OnIsImportantListChanged(bool value)
    {
        Console.WriteLine($@"Important list: {value}");
        RefreshList();
    }

    partial void OnIsCompletedListChanged(bool value)
    {
        Console.WriteLine($@"Completed list: {value}");
        RefreshList();
    }

    private void RefreshList()
    {
        var lastModel = _parentTodoModels.LastOrDefault();
        if (lastModel is null) 
        {
            Task.Run(async () =>
            {
                await Store.LoadAsync(IsPinnedList, IsImportantList, IsUrgentList, IsCompletedList);
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
                await Store.LoadAsync(lastModel.Id, IsPinnedList, IsImportantList, IsUrgentList, IsCompletedList);
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
                model.Model.UpdateDate = DateTime.Now;
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
            await Store.LoadAsync(model.Model.Id, IsPinnedList, IsImportantList, IsUrgentList, IsCompletedList);
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
                await Store.LoadAsync(IsPinnedList, IsImportantList, IsUrgentList, IsCompletedList);
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
                await Store.LoadAsync(lastModel.Id, IsPinnedList, IsImportantList, IsUrgentList, IsCompletedList);
            });
        }
    }
    
    [RelayCommand]
    private void ShowMainTodoList()
    {
        if (_parentTodoModels.Count == 0) return;
        Task.Run(async () =>
        {
            await Store.LoadAsync(IsPinnedList, IsImportantList, IsUrgentList, IsCompletedList);
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
                model.Model.UpdateDate = DateTime.Now;
                await Store.UpdateAsync(model.Model);
            }
        });
    }
}