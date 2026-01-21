using Avalonia;
using Avalonia.Controls;
using Material.Icons;
using Material.Icons.Avalonia;

namespace ZincirApp.CustomControls;

public class IconButton : Button
{
    public static readonly StyledProperty<bool> IconVisibleProperty =
        AvaloniaProperty.Register<IconButton, bool>(nameof(IconVisible),
            defaultValue: true);

    public bool IconVisible
    {
        get => GetValue(IconVisibleProperty);
        set => SetValue(IconVisibleProperty, value);
    }
    
    public static readonly StyledProperty<ushort> IconSizeProperty =
        AvaloniaProperty.Register<IconButton, ushort>(nameof(IconVisible),
            defaultValue: 25);

    public ushort IconSize
    {
        get => GetValue(IconSizeProperty);
        set => SetValue(IconSizeProperty, value);
    }
    
    public static readonly StyledProperty<bool> IsCheckedProperty =
        AvaloniaProperty.Register<IconButton, bool>(nameof(IsChecked),
            defaultValue: false);

    public bool IsChecked
    {
        get => GetValue(IsCheckedProperty);
        set => SetValue(IsCheckedProperty, value);
    }
    
    public static readonly StyledProperty<MaterialIconKind> KindProperty = 
        AvaloniaProperty.Register<MaterialIcon, MaterialIconKind>(nameof (Kind), defaultValue: MaterialIconKind.ArrowUp);
    
    public MaterialIconKind Kind
    {
        get => this.GetValue<MaterialIconKind>(KindProperty);
        set => this.SetValue<MaterialIconKind>(KindProperty, value);
    }
}