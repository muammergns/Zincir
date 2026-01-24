using System;
using System.Globalization;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Media;
using Avalonia.Threading;
using Material.Colors;
using Material.Styles.Themes;

namespace ZincirApp.Extensions;

public static class UiUtils
{
    public static void SetMaterialBackground(Control? control, string resourceKey)
    {
        if (control == null) return;

        var app = Application.Current;
        if (app == null) return;

        if (app.TryGetResource(resourceKey, app.ActualThemeVariant, out object? res) && res is IBrush brush)
        {
            Dispatcher.UIThread.InvokeAsync(() =>
            {
                control.SetValue(TemplatedControl.BackgroundProperty, brush);
            });
        }
    }
    public static void SetMaterialBackground(Control? control, PrimaryColor color)
    {
        if (control == null) return;

        var app = Application.Current;
        if (app == null) return;
        Dispatcher.UIThread.InvokeAsync(() =>
        {
            control.SetValue(TemplatedControl.BackgroundProperty, MaterialColorBrush(color));
        });
    }
    
    public static IBrush MaterialColorBrush(PrimaryColor primaryColor)
    {
        return new SolidColorBrush(SwatchHelper.Lookup[(MaterialColor)primaryColor]);
    }

    public static IBrush MaterialColorBrush(SecondaryColor secondaryColor)
    {
        return new SolidColorBrush(SwatchHelper.Lookup[(MaterialColor)secondaryColor]) ;
    }
    
    public static IBrush MaterialColorBrush(MaterialColor materialColor)
    {
        return new SolidColorBrush(SwatchHelper.Lookup[materialColor]) ;
    }
    
    public static string ObjToString(object? value, int decimalPlaces = 2)
    {
        switch (value)
        {
            case null:
                return string.Empty;
            case IConvertible:
                try
                {
                    double numericValue = Convert.ToDouble(value, CultureInfo.InvariantCulture);
                    string format = $"N{decimalPlaces}";
                    return numericValue.ToString(format, CultureInfo.CurrentCulture);
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                }
                break;
        }
        try
        {
            return value.ToString() ?? string.Empty;
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            return string.Empty;
        }
    }
}