using System;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using ZincirApp.Messages;
using ZincirApp.Models;
using ZincirApp.Services;
using ZincirApp.Stores;

namespace ZincirApp.ViewModels;

public partial class TodoEditViewModel(ITodoStore? todoStore, INavigationService navigationService, TodoModel? todoModel = null) : ViewModelBase
{
    [ObservableProperty] private bool _isCompleted = todoModel?.IsCompleted ?? false;
    [ObservableProperty] private bool _isPinned = todoModel?.IsPinned ?? false;
    [ObservableProperty] private bool _isImportant = todoModel?.IsImportant ?? false;
    [ObservableProperty] private bool _isUrgent = todoModel?.IsUrgent ?? false;
    [ObservableProperty] private string _title = todoModel?.Title ?? string.Empty;
    [ObservableProperty] private string _description = todoModel?.Description ?? string.Empty;
    [ObservableProperty] private DateTimeOffset? _targetDate = todoModel?.TargetDate;
    [ObservableProperty] private bool _isModelNotNull = todoModel is not null;
    private TodoModel? _todoModel = todoModel;

    [RelayCommand]
    private void ClearDate()
    {
        if (TargetDate is null)
        {
            Console.WriteLine(@"TargetDate is null");
            return;
        }
        Console.WriteLine(TargetDate.Value.Date.ToLongDateString());
        TargetDate = null;
    }

    [RelayCommand]
    private void Delete()
    {
        if (_todoModel == null || todoStore == null) return;
        Task.Run(async () =>
        {
            await todoStore.DeleteAsync(_todoModel.Id);
            navigationService.NavigateToSub<TodoEditViewModel>();
        });
    }

    [RelayCommand]
    private void Save()
    {
        if (todoStore == null) return;
        var isNew = false;
        if (_todoModel == null)
        {
            _todoModel = new TodoModel
            {
                CreateDate = DateTime.Now
            }; 
            isNew = true;
        }
        else
        {
            _todoModel.UpdateDate = DateTime.Now;
        }
        Task.Run(async () =>
        {
            _todoModel.IsCompleted = IsCompleted;
            _todoModel.Title = Title;
            _todoModel.Description = Description;
            _todoModel.IsPinned = IsPinned;
            _todoModel.IsImportant = IsImportant;
            _todoModel.IsUrgent = IsUrgent;
            _todoModel.TargetDate = TargetDate?.Date;

            if (isNew)
            {
                await todoStore.AddAsync(_todoModel);
                navigationService.NavigateToSub<TodoEditViewModel>();
            }
            else
            {
                await todoStore.UpdateAsync(_todoModel);
            }
            WeakReferenceMessenger.Default.Send(new DrawerChangedMessage(false));
        });
    }
}