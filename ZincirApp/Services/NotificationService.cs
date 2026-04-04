using System;
using System.Threading.Tasks;
using Avalonia.Threading;

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
    private readonly DispatcherTimer _timer = new();
    private string _title = string.Empty;
    private string _message = string.Empty;
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
        _timer.Interval = delay;
        _title = title;
        _message = message;
        _timer.Tick += Tick;
        _timer.Start();
    }

    private void Tick(object? sender, EventArgs e)
    {
        _timer.Stop();
        _timer.Tick -= Tick;
        ShowNotification(_title, _message);
    }

    public void ShowNotification(string title, string message)
    {
        messageService.ShowAlert(message, title);
    }
}