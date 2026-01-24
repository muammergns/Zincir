using System.Diagnostics;

namespace ZincirApp.Services;

public interface ITimerService
{
    void StartTimer(uint seconds);
}

public class TimerService : ITimerService
{

    public void StartTimer(uint seconds)
    {
        Debug.WriteLine($@"Starting timer: {seconds}");
    }
}