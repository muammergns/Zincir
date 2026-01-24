using System;
using System.Threading.Tasks;

namespace ZincirApp.Services;

public interface INotificationService
{
    void RequestPermission();
    bool CheckPermission();
    void ScheduleNotification(string title, string message, TimeSpan delay);
    void ShowNotification(string title, string message);
}

public class NotificationService(IMessageService messageService) : INotificationService
{

    public void RequestPermission()
    {
        
    }

    public bool CheckPermission()
    {
        return true;
    }

    public void ScheduleNotification(string title, string message, TimeSpan delay)
    {
        
    }

    public void ShowNotification(string title, string message)
    {
        messageService.ShowAlert(title, message);
    }
}