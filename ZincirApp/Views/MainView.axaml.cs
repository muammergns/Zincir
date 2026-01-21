using System;
using System.Threading;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Threading;
using ZincirApp.Core;

namespace ZincirApp.Views;

public partial class MainView : UserControl
{
    private enum PaneState{
        Small,
        Medium,
        Large
    }
    private PaneState _paneState = PaneState.Large;
    private CancellationTokenSource? _sizeChangedCancellation;
    private readonly NavigationCoord _navigationCoord;
    public MainView()
    {
        InitializeComponent();
        _navigationCoord = new NavigationCoord(this);
        this.SizeChanged += OnSizeChanged;
        UpdateLayoutState(Width, false);
        LeftPaneToggleButton.Content = "<";
        LeftPaneToggleButton.Click += (_, _) =>
        {
            LeftDrawer.IsPaneOpen = !LeftDrawer.IsPaneOpen;
        };
        LeftDrawer.PaneClosing += (_, _) =>
        {
            LeftPaneToggleButton.Content = ">";
        };
        LeftDrawer.PaneOpening += (_, _) =>
        {
            LeftPaneToggleButton.Content = "<";
        };
        CenterDate.Text = DateTime.Now.ToLongDateString();
    }

    public void SetCenterContent(Control content)
    {
        CenterContent.Content = content;
    }

    public void SetLeftContent(Control content)
    {
        LeftContent.Content = content;
    }

    public void CloseLeftPane()
    {
        LeftDrawer.IsPaneOpen = false;
    }
    public void CloseLeftPaneIfPageSmall()
    {
        if (_paneState is not PaneState.Small) return;
        LeftDrawer.IsPaneOpen = false;
    }

    public void OpenLeftPane()
    {
        LeftDrawer.IsPaneOpen = true;
    }
    
    private void OnSizeChanged(object? sender, SizeChangedEventArgs e)
    {
        _sizeChangedCancellation?.Cancel();
        _sizeChangedCancellation = new CancellationTokenSource();
        var token = _sizeChangedCancellation.Token;
        double newWidth = e.NewSize.Width;
        Task.Delay(16, token).ContinueWith(t => 
        {
            if (t.IsCanceled) return;
            Dispatcher.UIThread.Post(() => 
            {
                UpdateLayoutState(newWidth);
            }, DispatcherPriority.Background);
        },
        token);
        
    }
    
    private void UpdateLayoutState(double w, bool init=true)
    {
        var lastState = _paneState;
        _paneState = 
            w < 700 ? PaneState.Small : 
            w < 1070 ? PaneState.Medium : 
            PaneState.Large;
        if (lastState == _paneState && init) return;
        LeftDrawer.DisplayMode = _paneState is PaneState.Large or PaneState.Medium ? 
            SplitViewDisplayMode.Inline :  SplitViewDisplayMode.Overlay;
        LeftDrawer.IsPaneOpen = _paneState is PaneState.Large or PaneState.Medium;
        
    }
    

}