using System.Threading.Tasks;
using CommunityToolkit.Mvvm.Input;
using ZincirApp.Stores;

namespace ZincirApp.ViewModels;

public partial class PomodoroHistoryViewModel : ViewModelBase
{

    public IPomodoroStore Store { get; }
    public PomodoroHistoryViewModel(IPomodoroStore pomodoroStore)
    {
        Store = pomodoroStore;
        Task.Run(async () =>
        {
            await Store.LoadAsync();
        });
    }

    [RelayCommand]
    private void DeletePomodoro(PomodoroHistoryItemViewModel? model)
    {
        Task.Run(async () =>
        {
            if (model is { Model: not null }) await Store.DeleteAsync(model.Model.Id);
        });
    }
    
}

