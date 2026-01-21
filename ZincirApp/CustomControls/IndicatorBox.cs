using Avalonia;
using Avalonia.Controls;
using Avalonia.Media;

namespace ZincirApp.CustomControls;

public class IndicatorBox : CheckBox
{
    public static readonly StyledProperty<IBrush?> IndicatorBrushProperty =
        AvaloniaProperty.Register<IndicatorBox, IBrush?>(nameof(IndicatorBrush),
            defaultValue: Brushes.Transparent);

    public IBrush? IndicatorBrush
    {
        get => GetValue(IndicatorBrushProperty);
        set => SetValue(IndicatorBrushProperty, value);
    }
    
    public static readonly StyledProperty<double> IndicatorSizeProperty =
        AvaloniaProperty.Register<IndicatorBox, double>(nameof(IndicatorSize),
            defaultValue: 24);

    public double IndicatorSize
    {
        get => GetValue(IndicatorSizeProperty);
        set => SetValue(IndicatorSizeProperty, value);
    }
    
}