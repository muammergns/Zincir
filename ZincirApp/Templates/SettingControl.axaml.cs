using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;

namespace ZincirApp.Templates;

public class SettingControl : TemplatedControl
{
    public static readonly StyledProperty<string> HeaderProperty = AvaloniaProperty.Register<SettingControl, string>(
        nameof(Header), defaultValue: "Lorem ipsum dolor sit amet, consectetur adipiscing elit.");

    public string Header
    {
        get => GetValue(HeaderProperty);
        set => SetValue(HeaderProperty, value);
    }

    public static readonly StyledProperty<string> DescriptionProperty = AvaloniaProperty.Register<SettingControl, string>(
        nameof(Description), defaultValue: "Lorem ipsum dolor sit amet, consectetur adipiscing elit. Vestibulum ut fringilla felis. Proin porttitor felis lectus, non venenatis neque maximus eget. Morbi pulvinar ac tortor semper pellentesque. In semper, justo vel accumsan sagittis, mauris sem feugiat justo, ac ullamcorper dui sapien ac nibh. Mauris id mollis magna. Sed ac placerat ipsum. Suspendisse eget mauris sed eros semper tempus. Sed ex urna, imperdiet vitae mauris eu, iaculis imperdiet ligula. Phasellus auctor, neque vel dapibus finibus, purus sapien suscipit neque, eget interdum neque odio ac turpis. Duis vulputate erat tristique ipsum vestibulum, eu pharetra felis iaculis. Integer scelerisque aliquam ultrices. Sed vel ante eu leo hendrerit fringilla. Phasellus pellentesque facilisis lorem vel commodo. Ut gravida mauris eu nunc aliquet bibendum. Aliquam scelerisque hendrerit nulla, vel sollicitudin nulla sollicitudin vel.");

    public string Description
    {
        get => GetValue(DescriptionProperty);
        set => SetValue(DescriptionProperty, value);
    }
    
    public static readonly StyledProperty<Control?> SettingContentProperty =
        AvaloniaProperty.Register<SettingControl, Control?>(nameof(SettingContent));

    public Control? SettingContent
    {
        get => GetValue(SettingContentProperty);
        set => SetValue(SettingContentProperty, value);
    }

    public static readonly StyledProperty<bool> IsDescriptionVisibleProperty = AvaloniaProperty.Register<SettingControl, bool>(
        nameof(IsDescriptionVisible), defaultValue: true);

    public bool IsDescriptionVisible
    {
        get => GetValue(IsDescriptionVisibleProperty);
        set => SetValue(IsDescriptionVisibleProperty, value);
    }
}