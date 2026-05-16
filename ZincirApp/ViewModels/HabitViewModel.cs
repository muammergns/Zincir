using System;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using ZincirApp.Messages;
using ZincirApp.Models;
using ZincirApp.Services;
using ZincirApp.Stores;

namespace ZincirApp.ViewModels;

public partial class HabitViewModel : ViewModelBase
{
    public IHabitStore Store { get; }
    private readonly INavigationService _navService;
    public HabitViewModel(IHabitStore store, INavigationService navigationService)
    {
        Store = store;
        _navService = navigationService;
        Task.Run(async () =>
        {
            await store.LoadAsync();
            _navService.NavigateToSub<HabitEditViewModel>();
        });
    }

    [RelayCommand]
    private void ShowHabitEditView(HabitItemViewModel? model)
    {
        _navService.NavigateToSub(model is not null
            ? new HabitEditViewModel(Store, _navService, model.Model)
            : new HabitEditViewModel(Store, _navService));
        WeakReferenceMessenger.Default.Send(new DrawerChangedMessage(true));
    }

    [RelayCommand]
    private void ShowHabitDetailView(HabitItemViewModel? model)
    {
        if (model is null) return;
        //_navService.NavigateToSub(model);
        Console.WriteLine($@"TODO - habit detay view oluşturmayı unutma: {model.Title}");
    }
    
    [RelayCommand]
    private void ShowAddHabitLogView(HabitItemViewModel? model)
    {
        if (model is null) return;
        //_navService.NavigateToSub(model);
        Console.WriteLine($@"TODO - habit add log view oluşturmayı unutma: {model.Title}");
        Task.Run(async () =>
        {
            await Store.AddLogAsync(new HabitLogModel
            {
                Date = DateTime.Now,
                HabitId = model.Model.Id,
                Value = 1
            });
            _navService.NavigateToSub<HabitEditViewModel>();
        });
    }
}