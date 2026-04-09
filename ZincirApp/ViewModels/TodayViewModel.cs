using System;
using Avalonia.Threading;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using Microsoft.Extensions.DependencyInjection;
using ZincirApp.Assets;
using ZincirApp.Messages;
using ZincirApp.Models;
using ZincirApp.Services;

namespace ZincirApp.ViewModels;

public partial class TodayViewModel : ViewModelBase
{
    public TodayViewModel()
    {
        
    }

    [RelayCommand] private void OpenPane()
    {
        WeakReferenceMessenger.Default.Send(new DrawerChangedMessage(true));
    }
}