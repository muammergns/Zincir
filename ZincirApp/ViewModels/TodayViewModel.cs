using System;
using Avalonia.Threading;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using Microsoft.Extensions.DependencyInjection;
using ZincirApp.Messages;
using ZincirApp.Services;

namespace ZincirApp.ViewModels;

public partial class TodayViewModel : ViewModelBase
{
    private readonly INotificationService? _notificationService;
    public TodayViewModel(IServiceProvider serviceProvider) :base(serviceProvider)
    {
        _notificationService = serviceProvider.GetService<INotificationService>();
        Dispatcher.UIThread.InvokeAsync(async () =>
        {
            if (_notificationService != null)
            {
                bool result = await _notificationService.CheckPermission();
                if (result)
                {
                    
                    _notificationService.ShowNotification("Title", "Message");
                }
                else
                {
                    await _notificationService.RequestPermission();
                }
            }
        },  DispatcherPriority.Background);
        
        
        //service?.ScheduleNotification("Today","Deneme" , new TimeSpan(0, 0, 30));
        //var service = serviceProvider.GetService<ITimerService>();
        //service?.StartTimer(60);
    }

    [RelayCommand] private void OpenPane()
    {
        WeakReferenceMessenger.Default.Send(new DrawerChangedMessage(true));
        _notificationService?.ScheduleNotification("Title", "Message",  TimeSpan.FromSeconds(10));
    }
    
}