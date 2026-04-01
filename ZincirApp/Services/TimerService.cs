using System;
using Avalonia.Threading;


namespace ZincirApp.Services;

public interface ITimerService
{
    event EventHandler<TimeSpan>? Tick;
    string Title { get; set; }
    DateTime? CurrentSessionStartTime { get; set; }
    TimeSpan AccumulatedTime { get; set; }
    bool IsRunning { get; }
    TimeSpan ElapsedTime { get; }
    void Start();
    void Pause();
    void Reset();
    void LoadState(TimeSpan accumulatedTime, DateTime? currentSessionStartTime);
}


public class TimerService : ITimerService
{
    private readonly DispatcherTimer _timer;
    
    public event EventHandler<TimeSpan>? Tick;
    
    public string Title { get; set; }

    public DateTime? CurrentSessionStartTime { get; set; }
    public TimeSpan AccumulatedTime { get; set; } = TimeSpan.Zero;

    public bool IsRunning => _timer.IsEnabled;

    public TimeSpan ElapsedTime
    {
        get
        {
            if (!CurrentSessionStartTime.HasValue)
            {
                return AccumulatedTime;
            }

            return AccumulatedTime + (DateTime.Now - CurrentSessionStartTime.Value);
        }
    }

    public TimerService()
    {
        _timer = new DispatcherTimer
        {
            Interval = TimeSpan.FromMilliseconds(200)
        };
        _timer.Tick += (_, _) => RaiseTick();
    }

    public void LoadState(TimeSpan accumulatedTime, DateTime? currentSessionStartTime)
    {
        AccumulatedTime = accumulatedTime;
        CurrentSessionStartTime = currentSessionStartTime;

        if (CurrentSessionStartTime.HasValue)
        {
            _timer.Start();
        }
        RaiseTick();
    }

    public void Start()
    {
        if (IsRunning) return;

        CurrentSessionStartTime = DateTime.Now;
        _timer.Start();
        RaiseTick();
    }

    public void Pause()
    {
        if (!IsRunning) return;

        AccumulatedTime += (DateTime.Now - CurrentSessionStartTime!.Value);
        CurrentSessionStartTime = null;
        _timer.Stop();
        RaiseTick();
    }

    public void Reset()
    {
        _timer.Stop();
        CurrentSessionStartTime = null;
        AccumulatedTime = TimeSpan.Zero;
        RaiseTick();
    }

    private void RaiseTick()
    {
        Tick?.Invoke(this, ElapsedTime);
    }

    
}