using System;
using ZincirApp.Services;

namespace ZincirApp.ViewModels;

public class TodoViewModel : ViewModelBase
{

    public TodoViewModel(IServiceProvider serviceProvider) :base(serviceProvider)
    {
        Console.WriteLine(@"TodoViewModel ctor");
    }
}