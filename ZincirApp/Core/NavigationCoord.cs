using Avalonia.Controls;
using ZincirApp.Views;

namespace ZincirApp.Core;

public class NavigationCoord
{
    public NavigationCoord(MainView mainView)
    {
        _mainView = mainView;
        _mainView.SetLeftContent(_navigation);
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
    }

    private readonly MainView _mainView;
    private readonly HabitListView _habitList = new HabitListView();
    private readonly PomodoroView _pomodoro = new PomodoroView();
    private readonly SettingListView _setting  = new SettingListView();
    private readonly TodoListView _todoList  = new TodoListView();
    private readonly NavigationView _navigation  = new NavigationView();
    private readonly TodayView _today  = new TodayView();
    private readonly DatabaseSelectView _databaseSelect = new DatabaseSelectView();

    private void ShowView(Control control)
    {
        _mainView.SetCenterContent(control);
        _mainView.CloseLeftPaneIfPageSmall();
        _navigation.UnCheckAllButtons();
    }
    
    
}