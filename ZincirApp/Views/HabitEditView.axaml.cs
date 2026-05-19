using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

namespace ZincirApp.Views;

public partial class HabitEditView : UserControl
{
    public HabitEditView()
    {
        InitializeComponent();
    }

    private void CloseDateButton_OnClick(object? sender, RoutedEventArgs e)
    {
        SelectDateButton.Flyout?.Hide();
    }
}