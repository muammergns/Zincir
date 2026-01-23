using System;
using ZincirApp.Services;

namespace ZincirApp.ViewModels;

public class HabitViewModel : ViewModelBase
{

    public HabitViewModel(IServiceProvider serviceProvider) :base(serviceProvider)
    {
        Console.WriteLine(@"HabitViewModel ctor");
    }
}