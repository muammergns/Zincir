using System;
using ZincirApp.Services;

namespace ZincirApp.ViewModels;

public class TodayViewModel : ViewModelBase
{

    public TodayViewModel(IServiceProvider serviceProvider) :base(serviceProvider)
    {
        Console.WriteLine(@"Today ViewModel");
    }
}