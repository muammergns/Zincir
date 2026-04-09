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
    void NavigateTo<T>(T vm) where T : ViewModelBase;
    void NavigateToSub<T>() where T : ViewModelBase;
    void NavigateToSub<T>(T vm) where T : ViewModelBase;
    bool DrawerState { get; set; }
}

public class NavigationService(IServiceProvider serviceProvider) : ObservableObject, INavigationService
{
    public ViewModelBase? CurrentView
    {
        get;
        private set => SetProperty(ref field, value);
    }

    public ViewModelBase? SubView
    {
        get;
        private set => SetProperty(ref field, value);
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

    public void NavigateTo<T>(T vm) where T : ViewModelBase
    {
        try
        {
            CurrentView = vm;
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

    public void NavigateToSub<T>(T vm) where T : ViewModelBase
    {
        try
        {
            SubView = vm;
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
        }
    }

    public bool DrawerState { get; set; }
}