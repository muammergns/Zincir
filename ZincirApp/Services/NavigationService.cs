using System;
using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.Extensions.DependencyInjection;
using ZincirApp.ViewModels;

namespace ZincirApp.Services;

public interface INavigationService
{
    ViewModelBase? CurrentView { get; }
    ViewModelBase? SubView { get; }
    void NavigateTo<T>() where T : ViewModelBase;
    void NavigateToSub<T>() where T : ViewModelBase;
}

public class NavigationService(IServiceProvider serviceProvider) : ObservableObject, INavigationService
{
    private ViewModelBase? _currentView;

    public ViewModelBase? CurrentView
    {
        get => _currentView;
        private set => SetProperty(ref _currentView, value);
    }
    private ViewModelBase? _subView;

    public ViewModelBase? SubView
    {
        get => _subView;
        private set => SetProperty(ref _subView, value);
    }
    public void NavigateTo<T>() where T : ViewModelBase
    {
        try
        {
            var viewModel = serviceProvider.GetRequiredService<T>();
            CurrentView = viewModel;
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
        }
    }
    public void NavigateToSub<T>() where T : ViewModelBase
    {
        try
        {
            var viewModel = serviceProvider.GetRequiredService<T>();
            SubView = viewModel;
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
        }
    }
}