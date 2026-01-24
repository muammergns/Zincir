using Avalonia.Controls;
using Material.Colors;
using ZincirApp.Extensions;

namespace ZincirApp.Views;

public partial class TodayView : UserControl
{
    public TodayView()
    {
        InitializeComponent();
        UiUtils.SetMaterialBackground(TodayButton, PrimaryColor.Blue);
    }
}