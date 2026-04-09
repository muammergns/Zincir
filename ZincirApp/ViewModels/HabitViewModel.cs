using System;
using CommunityToolkit.Mvvm.ComponentModel;
using ZincirApp.Services;

namespace ZincirApp.ViewModels;

public class HabitViewModel : ViewModelBase
{

    public HabitViewModel()
    {
        Console.WriteLine(@"HabitViewModel ctor");
    }
}