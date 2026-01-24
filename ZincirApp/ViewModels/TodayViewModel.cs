using System;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using Microsoft.Extensions.DependencyInjection;
using ZincirApp.Messages;
using ZincirApp.Services;

namespace ZincirApp.ViewModels;

public partial class TodayViewModel : ViewModelBase
{
    public TodayViewModel(IServiceProvider serviceProvider) :base(serviceProvider)
    {
        Console.WriteLine(@"Today ViewModel");
    }

    [RelayCommand] private void OpenPane()
    {
        WeakReferenceMessenger.Default.Send(new DrawerChangedMessage(true));
    }
    
}