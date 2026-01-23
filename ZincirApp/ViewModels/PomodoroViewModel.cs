using System;
using ZincirApp.Services;

namespace ZincirApp.ViewModels;

public class PomodoroViewModel : ViewModelBase
{

    public PomodoroViewModel(IServiceProvider serviceProvider) :base(serviceProvider)
    {
        Console.WriteLine(@"PomodorViewModel ctor");
    }
}