using Avalonia.Controls;
using Avalonia.Interactivity;

namespace ZincirApp.Views;

public partial class TodoEditView : UserControl
{
    public TodoEditView()
    {
        InitializeComponent();
    }

    private void CloseDateButton_OnClick(object? sender, RoutedEventArgs e)
    {
        SelectDateButton.Flyout?.Hide();
    }
}