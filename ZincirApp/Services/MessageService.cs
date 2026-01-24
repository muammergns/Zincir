using Avalonia;

using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Threading;
using MsBox.Avalonia;

namespace ZincirApp.Services;

public interface IMessageService
{
    void ShowAlert(string message, string title = "Alert");
}

public class DesktopMessageBoxService() : IMessageService
{
    public void ShowAlert(string message, string title = "Alert")
    {
        var mainWindow = (Application.Current?.ApplicationLifetime as IClassicDesktopStyleApplicationLifetime)?.MainWindow;
        if (mainWindow == null) return;
        Dispatcher.UIThread.InvokeAsync(async () =>
        {
            var box = MessageBoxManager.GetMessageBoxStandard(title, message);
            await box.ShowAsPopupAsync(mainWindow);
        });
    }
}

public class SingleViewMessageBoxService : IMessageService
{
    public void ShowAlert(string message, string title = "Alert")
    {
        Dispatcher.UIThread.InvokeAsync(async () =>
        {
            var box = MessageBoxManager.GetMessageBoxStandard(title, message);
            await box.ShowAsync();
        });
    }
}
