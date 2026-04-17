using System.Threading.Tasks;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using ZincirApp.Messages;
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
    private void ShowHabitDetailView(HabitItemViewModel? model)
    {
        _navService.NavigateToSub(model is not null
            ? new HabitEditViewModel(Store, _navService, model.Model)
            : new HabitEditViewModel(Store, _navService));
        WeakReferenceMessenger.Default.Send(new DrawerChangedMessage(true));
    }
}