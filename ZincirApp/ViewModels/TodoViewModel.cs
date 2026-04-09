using System;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.Input;
using ZincirApp.Models;
using ZincirApp.Services;
using ZincirApp.Stores;

namespace ZincirApp.ViewModels;

public partial class TodoViewModel : ViewModelBase
{
    public ITodoStore Store { get; }
    private readonly INavigationService _navService;
    public TodoViewModel(ITodoStore store, INavigationService navigationService)
    {
        Store = store;
        _navService = navigationService;
        Task.Run(async () =>
        {
            await Store.LoadAsync();
        });
    }

    [RelayCommand]
    private void DeleteTodo(TodoItemViewModel? model)
    {
        Task.Run(async () =>
        {
            if (model is { Model: not null }) 
                await Store.DeleteAsync(model.Model.Uuid);
        });
    }

    [RelayCommand]
    private void ShowTodoDetails(TodoItemViewModel? model)
    {
        _navService.NavigateToSub<TodoEditViewModel>(model is { Model: not null }
            ? new TodoEditViewModel(Store, model.Model)
            : new TodoEditViewModel(Store));
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