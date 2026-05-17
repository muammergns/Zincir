using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.VisualTree;

namespace ZincirApp.Views;

public partial class TodoEditView : UserControl
{
    public TodoEditView()
    {
        InitializeComponent();
        TodoEditDatePicker.SelectedDateChanged += (_, _) =>
        {
            TodoEditDatePicker.FindDescendantOfType<Popup>()?.Close();
        };
    }
}