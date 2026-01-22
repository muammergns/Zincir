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

    private PaneState _currentState;
    private readonly DispatcherTimer? _debounceTimer;
    private double _resizeWidth;
    private readonly HabitListView _habitList = new HabitListView();
    private readonly PomodoroView _pomodoro = new PomodoroView();
    private readonly SettingListView _setting  = new SettingListView();
    private readonly TodoListView _todoList  = new TodoListView();
    private readonly NavigationView _navigation  = new NavigationView();
    private readonly TodayView _today  = new TodayView();
    private readonly DatabaseSelectView _databaseSelect = new DatabaseSelectView();
    public MainView()
    {
        InitializeComponent();
        LeftContent.Content = _navigation;
        ShowView(_today);
        _navigation.SetTodayViewButton();
        _navigation.TodayViewButtonClicked += () =>
        {
            ShowView(_today);
            _navigation.SetTodayViewButton();
        };
        _navigation.TodoViewButtonClicked += () =>
        {
            ShowView(_todoList);
            _navigation.SetTodoViewButton();
        };
        _navigation.HabitViewButtonClicked += () =>
        {
            ShowView(_habitList);
            _navigation.SetHabitViewButton();
        };
        _navigation.PomodoroViewButtonClicked += () =>
        {
            ShowView(_pomodoro);
            _navigation.SetPomodoroViewButton();
        };
        _navigation.SettingViewButtonClicked += () =>
        {
            ShowView(_setting);
            _navigation.SetSettingViewButton();
        };
        _navigation.DatabaseSelectButtonClicked += () =>
        {
            ShowView(_databaseSelect);
            _navigation.SetDatabaseSelectButton();
        };
        this.SizeChanged += OnSizeChanged;
        _debounceTimer = new DispatcherTimer
        {
            Interval = TimeSpan.FromMilliseconds(50)
        };
        _debounceTimer.Tick += ResizeTimer_Tick;
        _debounceTimer.Start();
        _navigation.PaneCloseButton.Click += (_, _) =>
        {
            LeftDrawer.IsPaneOpen = false;
        };
        LeftPaneToggleButton.Content = _navigation.PaneCloseButton.Content = "<";
        LeftPaneToggleButton.Click += (_, _) =>
        {
            LeftDrawer.IsPaneOpen = !LeftDrawer.IsPaneOpen;
            LeftPaneToggleButton.Content = LeftDrawer.IsPaneOpen ? "<" : ">";
        };
        LeftDrawer.PaneClosing += PanelClosing;
        CenterDate.Text = _navigation.CenterDate.Text = DateTime.Now.ToLongDateString();
    }

    private void PanelClosing(object? sender, CancelRoutedEventArgs e)
    {
        if (e.Source is not SplitView splitView) return;
        if (splitView.Equals(LeftDrawer)) return;
        if (!LeftDrawer.IsPaneOpen) return;
        if (!LeftDrawer.DisplayMode.Equals(SplitViewDisplayMode.Overlay)) return;
        e.Cancel = true;
        LeftDrawer.IsPaneOpen = false;
    }
    
    private void ShowView(UserControl control)
    {
        CenterContent.Content = control;
        _navigation.UnCheckAllButtons();
        if (_currentState is not PaneState.Small) return;
        LeftDrawer.IsPaneOpen = false;
    }
    private void ResizeTimer_Tick(object? sender, EventArgs e)
    {
        UpdateLayoutState();
        _debounceTimer?.Stop();
    }

    private void UpdateLayoutState()
    {
        _currentState = 
            _resizeWidth < 700 ? PaneState.Small : 
            _resizeWidth < 1070 ? PaneState.Medium : 
            PaneState.Large;
        LeftDrawer.DisplayMode = _currentState is PaneState.Large or PaneState.Medium ? 
            SplitViewDisplayMode.Inline :  SplitViewDisplayMode.Overlay;
        LeftDrawer.IsPaneOpen = _currentState is PaneState.Large or PaneState.Medium;
        LeftPaneToggleButton.Content = LeftDrawer.IsPaneOpen ? "<" : ">";
        LeftDrawer.OpenPaneLength = _currentState is PaneState.Small ? _resizeWidth : 250;
        _navigation.HeaderDock.IsVisible = _currentState is PaneState.Small;
    }
    private void OnSizeChanged(object? sender, SizeChangedEventArgs e)
    {
        //int o = Convert.ToInt32(e.PreviousSize.Width);
        //int l = Convert.ToInt32(_resizeWidth);
        //int n = Convert.ToInt32(e.NewSize.Width);
        //Console.WriteLine($@"Prev:{o} Log:{l} New:{n}");
        _resizeWidth = e.NewSize.Width;
        _debounceTimer?.Stop();
        _debounceTimer?.Start();
    }

    

}