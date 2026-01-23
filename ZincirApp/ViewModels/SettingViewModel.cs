using System;
using ZincirApp.Services;

namespace ZincirApp.ViewModels;

public class SettingViewModel : ViewModelBase
{

    public SettingViewModel(IServiceProvider serviceProvider) :base(serviceProvider)
    {
        Console.WriteLine(@"Setting ViewModel");
    }
}