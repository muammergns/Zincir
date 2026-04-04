using System;
using System.Threading.Tasks;
using Avalonia.Threading;
using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.Extensions.DependencyInjection;
using ZincirApp.Assets;
using ZincirApp.Models;
using ZincirApp.Services;

namespace ZincirApp.ViewModels;

public partial class ViewModelBase : ObservableObject
{
    protected INavigationService NavService { get; }
    protected ZincirDbService DbService { get; }

    protected ViewModelBase(IServiceProvider serviceProvider)
    {
        NavService ??= serviceProvider.GetRequiredService<INavigationService>();
        DbService ??= serviceProvider.GetRequiredService<ZincirDbService>();
        try
        {
            var storageService = serviceProvider.GetRequiredService<IStorageService>();
            DbService.SetPath(storageService.GetPath("zincir.db"));
            DbService.SetSalt(Keys.DatabaseId);
            DbService.SetPin("1234");
            if (!DbService.IsInitialized)
            {
                Task.Run(async () =>
                {
                    await DbService.EnsureInitializedAsync();
                });
            }
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
        }
    }
}