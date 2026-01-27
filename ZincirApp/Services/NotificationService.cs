using System;
using System.Threading.Tasks;

namespace ZincirApp.Services;

public interface INotificationService
{
    Task<bool> RequestPermission();
    Task<bool> CheckPermission();
    void ScheduleNotification(string title, string message, TimeSpan delay);
    void ShowNotification(string title, string message);
}

public class NotificationService(IMessageService messageService) : INotificationService
{

    public Task<bool> RequestPermission()
    {
        return Task.FromResult(true);
    }

    public Task<bool> CheckPermission()
    {
        return Task.FromResult(true);
    }

    public void ScheduleNotification(string title, string message, TimeSpan delay)
    {
        
    }

    public void ShowNotification(string title, string message)
    {
        messageService.ShowAlert(title, message);
    }
}