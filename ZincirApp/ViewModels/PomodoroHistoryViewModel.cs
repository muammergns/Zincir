using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using ZincirApp.Messages;
using ZincirApp.Models;
using ZincirApp.Services;

namespace ZincirApp.ViewModels;

public partial class PomodoroHistoryViewModel : ViewModelBase
{
    [ObservableProperty]private ObservableCollection<PomodoroHistoryModel> _pomodoroHistory = [];
    public PomodoroHistoryViewModel(IServiceProvider serviceProvider) : base(serviceProvider)
    {
        WeakReferenceMessenger.Default.Register<PomodoroListUpdated>(this, (obj, args) =>
        {
            switch (args.ChangeType)
            {
                case DbState.Insert:
                    foreach (var item in args.Pomodoros)
                    {
                        PomodoroHistory.Add(GetHistoryModel(item));
                    }
                    break;
                case DbState.Update:
                    foreach (var item in args.Pomodoros)
                    {
                        Console.WriteLine(item.Uuid);
                    }
                    break;
                case DbState.Delete:
                    var idsToRemove = args.Pomodoros.Select(p => p.Uuid).ToHashSet();
                    for (var i = PomodoroHistory.Count - 1; i >= 0; i--)
                    {
                        if (idsToRemove.Contains(PomodoroHistory[i].SessionId))
                        {
                            PomodoroHistory.RemoveAt(i);
                        }
                    }
                    break;
                case DbState.Get:
                    PomodoroHistory.Clear();
                    foreach (var item in args.Pomodoros)
                    {
                        PomodoroHistory.Add(GetHistoryModel(item));
                    }
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
            
        });
        
        
    }
    
    [RelayCommand]
    private async Task DeleteItemAsync(PomodoroHistoryModel item)
    {
        PomodoroHistory.Remove(item);
        await DbService.DeleteAsync<PomodoroModel>(item.SessionId);
        WeakReferenceMessenger.Default.Send(new PomodoroListUpdated(
            [new PomodoroModel { Uuid = item.SessionId }], 
            DbState.Delete));
    }

    private PomodoroHistoryModel GetHistoryModel(PomodoroModel item)
    {
        var ts = item.SessionTimeSpan;
        var date = item.CreateDate;

        var longDate = date.ToLongDateString();
        var longTime = date.ToLongTimeString();
    
        var hasParent = item.Habit is not null || item.Todo is not null;

        return new PomodoroHistoryModel()
        {

            CreateDateText = hasParent ? $"{longDate} {longTime}" : longTime,
            SessionDurationText = ts.ToString(@"hh\:mm\:ss"),
            TitleText = item.Habit?.Title ?? item.Todo?.Title ?? longDate,
            SessionId = item.Uuid
        };
    }
}

public partial class PomodoroHistoryModel : ObservableObject
{
    [ObservableProperty] private string _titleText = string.Empty;
    [ObservableProperty] private string _createDateText = string.Empty;
    [ObservableProperty] private string _sessionDurationText = string.Empty;
    public Guid SessionId { get; init; }
}