using System.Threading.Tasks;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using ZincirApp.Messages;
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
            _navService.NavigateToSub<TodoEditViewModel>();
        });
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
    private void ShowTodoDetails(TodoItemViewModel? model)
    {
        _navService.NavigateToSub(model is { Model: not null }
            ? new TodoEditViewModel(Store, _navService, model.Model)
            : new TodoEditViewModel(Store, _navService));
        WeakReferenceMessenger.Default.Send(new DrawerChangedMessage(true));
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