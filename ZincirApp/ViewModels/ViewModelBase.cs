using System;
using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.Extensions.DependencyInjection;
using ZincirApp.Services;

namespace ZincirApp.ViewModels;

public partial class ViewModelBase(IServiceProvider serviceProvider) : ObservableObject
{
    protected INavigationService NavService { get; } = serviceProvider.GetRequiredService<INavigationService>();

}