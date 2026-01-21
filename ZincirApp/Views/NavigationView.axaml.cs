using System;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace ZincirApp.Views;

public partial class NavigationView : UserControl
{
    public Action? TodayViewButtonClicked { get; set; }
    public Action? TodoViewButtonClicked  { get; set; }
    public Action? HabitViewButtonClicked  { get; set; }
    public Action? PomodoroViewButtonClicked  { get; set; }
    public Action? SettingViewButtonClicked  { get; set; }
    public Action? DatabaseSelectButtonClicked  { get; set; }
    
    public NavigationView()
    {
        InitializeComponent();
        TodayViewButton.Click += (_, _) => TodayViewButtonClicked?.Invoke();
        TodoViewButton.Click += (_, _) => TodoViewButtonClicked?.Invoke();
        HabitViewButton.Click += (_, _) => HabitViewButtonClicked?.Invoke();
        PomodoroViewButton.Click += (_, _) => PomodoroViewButtonClicked?.Invoke();
        SettingViewButton.Click += (_, _) => SettingViewButtonClicked?.Invoke();
        DatabaseSelectButton.Click += (_, _) => DatabaseSelectButtonClicked?.Invoke();
    }

    public void SetTodayViewButton(bool isOn = true)
    {
        TodayViewButton.IsChecked = isOn;
    }
    public void SetTodoViewButton(bool isOn = true)
    {
        TodoViewButton.IsChecked = isOn;
    }
    public void SetHabitViewButton(bool isOn = true)
    {
        HabitViewButton.IsChecked = isOn;
    }
    public void SetPomodoroViewButton(bool isOn = true)
    {
        PomodoroViewButton.IsChecked = isOn;
    }
    public void SetSettingViewButton(bool isOn = true)
    {
        SettingViewButton.IsChecked = isOn;
    }
    public void SetDatabaseSelectButton(bool isOn = true)
    {
        DatabaseSelectButton.IsChecked = isOn;
    }

    public void UnCheckAllButtons()
    {
        TodayViewButton.IsChecked = false;
        TodoViewButton.IsChecked = false;
        HabitViewButton.IsChecked = false;
        PomodoroViewButton.IsChecked = false;
        SettingViewButton.IsChecked = false;
        DatabaseSelectButton.IsChecked = false;
    }
}