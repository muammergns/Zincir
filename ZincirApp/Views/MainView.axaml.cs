using System;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Threading;

namespace ZincirApp.Views;

public partial class MainView : UserControl
{
    public enum PaneState{
        Small,
        Medium,
        Large
    }

    private PaneState _windowState, _panelState;
    private readonly DispatcherTimer _leftDebounceTimer = new DispatcherTimer{ Interval = TimeSpan.FromMilliseconds(100) };
    private readonly DispatcherTimer _rightDebounceTimer = new DispatcherTimer{ Interval = TimeSpan.FromMilliseconds(100) };
    private double _resizeWindowWidth, _resizePanelWidth;
    private bool _isRightFocus;
    public MainView()
    {
        InitializeComponent();
        this.SizeChanged += OnWindowSizeChanged;
        RightDrawer.SizeChanged += OnPanelSizeChanged;
        _leftDebounceTimer.Tick += LeftDebounceTimer_Tick;
        _leftDebounceTimer.Start();
        _rightDebounceTimer.Tick += RightDebounceTimer_Tick;
        _rightDebounceTimer.Start();
        LeftPaneCloseButton.Click += (_, _) =>
        {
            LeftDrawer.IsPaneOpen = false;
        };
        RightPaneCloseButton.Click += (_, _) =>
        {
            RightDrawer.IsPaneOpen = false;
            _isRightFocus = false;
        };
        LeftPaneToggleButton.Content = LeftPaneCloseButton.Content = "<";
        RightPaneCloseButton.Content = ">";
        LeftPaneToggleButton.Click += (_, _) =>
        {
            LeftDrawer.IsPaneOpen = !LeftDrawer.IsPaneOpen;
            LeftPaneToggleButton.Content = LeftDrawer.IsPaneOpen ? "<" : ">";
        };
        LeftDrawer.PaneClosing += PanelClosing;
        CenterDate.Text = LeftDate.Text = DateTime.Now.ToLongDateString();
    }

    private void PanelClosing(object? sender, CancelRoutedEventArgs e)
    {
        if (e.Source is not SplitView splitView) return;
        if (splitView.Equals(LeftDrawer)) return;
        if (!LeftDrawer.IsPaneOpen)
        { if (splitView.Equals(RightDrawer)) _isRightFocus = false; return; }
        if (!LeftDrawer.DisplayMode.Equals(SplitViewDisplayMode.Overlay)) return;
        e.Cancel = true;
        LeftDrawer.IsPaneOpen = false;
    }
    
    private void LeftDebounceTimer_Tick(object? sender, EventArgs e)
    {
        UpdateWindowLayoutState();
        _leftDebounceTimer.Stop();
    }
    private void RightDebounceTimer_Tick(object? sender, EventArgs e)
    {
        UpdatePanelLayoutState();
        _rightDebounceTimer.Stop();
    }

    private void UpdateWindowLayoutState()
    {
        _windowState = 
            _resizeWindowWidth < 700 ? PaneState.Small : 
            _resizeWindowWidth < 1070 ? PaneState.Medium : 
            PaneState.Large;
        LeftDrawer.DisplayMode = _windowState is PaneState.Large or PaneState.Medium ? 
            SplitViewDisplayMode.Inline :  SplitViewDisplayMode.Overlay;
        LeftDrawer.IsPaneOpen = _windowState is PaneState.Large or PaneState.Medium;
        LeftPaneToggleButton.Content = LeftDrawer.IsPaneOpen ? "<" : ">";
        LeftDrawer.OpenPaneLength = _windowState is PaneState.Small ? _resizeWindowWidth : 250;
        LeftHeaderDock.IsVisible = _windowState is PaneState.Small;
    }
    private void UpdatePanelLayoutState()
    {
        _panelState = 
            _resizePanelWidth < 450 ? PaneState.Small : 
            _resizePanelWidth < 700 ? PaneState.Medium : 
            PaneState.Large;
        RightDrawer.DisplayMode = _panelState is PaneState.Large ? 
            SplitViewDisplayMode.Inline :  SplitViewDisplayMode.Overlay;
        RightDrawer.IsPaneOpen = _panelState is PaneState.Large || _isRightFocus;
        RightPaneCloseButton.IsVisible = _panelState is not PaneState.Large;
        RightDrawer.OpenPaneLength = 
            _panelState is PaneState.Small or PaneState.Medium ? 
                _resizePanelWidth : _resizePanelWidth * 0.5;
    }
    private void OnWindowSizeChanged(object? sender, SizeChangedEventArgs e)
    {
        _resizeWindowWidth = e.NewSize.Width;
        _leftDebounceTimer.Stop();
        _leftDebounceTimer.Start();
    }
    
    private void OnPanelSizeChanged(object? sender, SizeChangedEventArgs e)
    {
        _resizePanelWidth = e.NewSize.Width;
        _rightDebounceTimer.Stop();
        _rightDebounceTimer.Start();
    }

    private void RightDrawer_OnGotFocus(object? sender, RoutedEventArgs e)
    {
        _isRightFocus = true;
    }

    private void RightDrawer_OnLostFocus(object? sender, RoutedEventArgs e)
    {
        _isRightFocus = false;
    }
    

}