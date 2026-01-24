using System;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using Microsoft.Extensions.DependencyInjection;
using ZincirApp.Messages;
using ZincirApp.Services;

namespace ZincirApp.ViewModels;

public partial class TodayViewModel : ViewModelBase
{
    public TodayViewModel(IServiceProvider serviceProvider) :base(serviceProvider)
    {
        var service = serviceProvider.GetService<INotificationService>();
        if (service != null)
        {
            if (service.CheckPermission())
            {
                service?.ShowNotification("Title", "Message");
            }
            else
            {
                service.RequestPermission();
            }
        }
        
        //service?.ScheduleNotification("Today","Deneme" , new TimeSpan(0, 0, 30));
        //var service = serviceProvider.GetService<ITimerService>();
        //service?.StartTimer(60);
    }

    [RelayCommand] private void OpenPane()
    {
        WeakReferenceMessenger.Default.Send(new DrawerChangedMessage(true));
    }
    
}