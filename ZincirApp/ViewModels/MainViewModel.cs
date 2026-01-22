using CommunityToolkit.Mvvm.ComponentModel;
using ZincirApp.Locale;

namespace ZincirApp.ViewModels;

public partial class MainViewModel : ObservableObject
{
    [ObservableProperty] private string _greeting = UiTexts.Greeting;
}