using CommunityToolkit.Mvvm.ComponentModel;
using ZincirApp.Assets;

namespace ZincirApp.ViewModels;

public partial class MainViewModel : ObservableObject
{
    [ObservableProperty] private string _greeting = UiTexts.Greeting;
}