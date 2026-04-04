using System;
using Avalonia.Threading;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using Microsoft.Extensions.DependencyInjection;
using ZincirApp.Assets;
using ZincirApp.Messages;
using ZincirApp.Models;
using ZincirApp.Services;

namespace ZincirApp.ViewModels;

public partial class TodayViewModel : ViewModelBase
{
    
    public TodayViewModel(IServiceProvider serviceProvider) :base(serviceProvider)
    {
        //ListDb();
        
        
        //service?.ScheduleNotification("Today","Deneme" , new TimeSpan(0, 0, 30));
        //var service = serviceProvider.GetService<ITimerService>();
        //service?.StartTimer(60);
    }

    [RelayCommand] private void OpenPane()
    {
        WeakReferenceMessenger.Default.Send(new DrawerChangedMessage(true));
        Dispatcher.UIThread.InvokeAsync(async () =>
        {
            try
            {
                await DbService.InsertAsync(new TodoModel()
                {
                    CreateDate = DateTime.Now,
                    Description = "",
                    Title = ""
                });
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
            ListDb();
        }, DispatcherPriority.Background);
    }

    private void ListDb()
    {
        Dispatcher.UIThread.InvokeAsync(async () =>
        {
            try
            {
                var list = await DbService.GetAllAsync<TodoModel>();
                foreach (var item in list)
                {
                    Console.WriteLine($@"{item.Title},{item.CreateDate.ToShortDateString()}");
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }, DispatcherPriority.Background);
    }
    
}