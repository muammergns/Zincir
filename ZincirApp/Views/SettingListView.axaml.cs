using System;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Threading;
using ZincirApp.Locale;
using ZincirApp.Settings.SettingsViews;

namespace ZincirApp.Views;

public partial class SettingListView : UserControl
{
    public enum PaneState{
        Small,
        Medium,
        Large
    }
    private readonly AppearanceSettingsView  _appearanceSettingsView = new AppearanceSettingsView();
    private readonly DispatcherTimer? _debounceTimer;
    private double _resizeWidth;
    private PaneState _currentState;
    private bool _isRightFocus;
    public SettingListView()
    {
        InitializeComponent();
        RightContent.Content = _appearanceSettingsView;
        ClosePaneButton.Content = ">";
        Header.Text = UiTexts.Greeting;
        AppearanceSettingsButton.Click += (_, _) =>
        {
            RightContent.Content = _appearanceSettingsView;
            Header.Text = UiTexts.Greeting;
            RightDrawer.IsPaneOpen = true;
            _isRightFocus = true;
        };
        ClosePaneButton.Click += (_, _) =>
        {
            RightDrawer.IsPaneOpen = false;
            _isRightFocus = false;
        };
        RightDrawer.SizeChanged += (_, args) =>
        {
            _resizeWidth = args.NewSize.Width;
            _debounceTimer?.Stop();
            _debounceTimer?.Start();
        };
        _debounceTimer = new DispatcherTimer
        {
            Interval = TimeSpan.FromMilliseconds(100)
        };
        _debounceTimer.Tick += ResizeTimer_Tick;
        _debounceTimer.Start();
    }
    
    private void ResizeTimer_Tick(object? sender, EventArgs e)
    {
        UpdateLayoutState();
        _debounceTimer?.Stop();
    }
    private void UpdateLayoutState()
    {
        _currentState = 
            _resizeWidth < 450 ? PaneState.Small : 
            _resizeWidth < 700 ? PaneState.Medium : 
            PaneState.Large;
        RightDrawer.DisplayMode = _currentState is PaneState.Large ? 
            SplitViewDisplayMode.Inline :  SplitViewDisplayMode.Overlay;
        RightDrawer.IsPaneOpen = _currentState is PaneState.Large || _isRightFocus;
        ClosePaneButton.IsVisible = _currentState is not PaneState.Large;
        RightDrawer.OpenPaneLength = _currentState is PaneState.Small or PaneState.Medium ? _resizeWidth : _resizeWidth * 0.5;
        
    }

    private void RightDrawer_OnGotFocus(object? sender, GotFocusEventArgs e)
    {
        _isRightFocus = true;
    }

    private void RightDrawer_OnLostFocus(object? sender, RoutedEventArgs e)
    {
        _isRightFocus = false;
    }
}