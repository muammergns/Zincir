using System;
using Avalonia.Media;
using CommunityToolkit.Mvvm.ComponentModel;
using Material.Icons;
using ZincirApp.Models;

namespace ZincirApp.ViewModels;

public partial class TodoItemViewModel : ObservableObject
{
    public TodoModel? Model { get; private set; }
    [ObservableProperty] private string _title;
    [ObservableProperty] private bool _isCompleted;
    [ObservableProperty] private bool _isPinned;
    [ObservableProperty] private bool _isUrgent;
    [ObservableProperty] private bool _isImportant;
    [ObservableProperty] private string _description;
    [ObservableProperty] private string _createDate;
    [ObservableProperty] private string _updateDate;
    [ObservableProperty] private string _targetDate;
    [ObservableProperty] private MaterialIconKind _pinKind;
    [ObservableProperty] private IBrush _priorityColor;
    [ObservableProperty] private IBrush _priorityBackground;
    [ObservableProperty] private bool _isUpdateVisible;
    [ObservableProperty] private bool _isTargetVisible;

    public TodoItemViewModel(TodoModel? todoModel)
    {
        Model = todoModel;
        Title = todoModel?.Title ?? string.Empty;
        IsCompleted = todoModel?.IsCompleted ?? false;
        IsPinned = todoModel?.IsPinned ?? false;
        IsUrgent = todoModel?.IsUrgent ?? false;
        IsImportant = todoModel?.IsImportant ?? false;
        Description = todoModel?.Description ?? string.Empty;
        CreateDate = @$"{todoModel?.CreateDate.ToLongDateString() ?? string.Empty} {todoModel?.CreateDate.ToLongTimeString() ?? string.Empty}";
        UpdateDate = @$"{todoModel?.UpdateDate?.ToLongDateString() ?? string.Empty} {todoModel?.UpdateDate?.ToLongTimeString() ?? string.Empty}";
        TargetDate = @$"{todoModel?.TargetDate?.ToLongDateString() ?? string.Empty}";
        PinKind = todoModel?.IsPinned ?? false ? MaterialIconKind.Pin : MaterialIconKind.PinOff;
        PriorityColor = (todoModel?.IsImportant, todoModel?.IsUrgent) switch
        {
            (true, true) => GetBrush("F44336"),
            (true, false) => GetBrush("2196F3"),
            (false, true) => GetBrush("FFC107"),
            _ => GetBrush("9E9E9E")
        };
        PriorityBackground = (todoModel?.IsImportant, todoModel?.IsUrgent) switch
        {
            (true, true) => GetBrush("F44336", 0.025),
            (true, false) => GetBrush("2196F3", 0.025),
            (false, true) => GetBrush("FFC107", 0.025),
            _ => GetBrush("9E9E9E", 0.025)
        };
        IsUpdateVisible = todoModel?.UpdateDate is not null;
        IsTargetVisible = todoModel?.TargetDate is not null;
    }

    partial void OnIsPinnedChanged(bool value)
    {
        PinKind = value ? MaterialIconKind.Pin : MaterialIconKind.PinOff;
    }
    
    private static IBrush GetBrush(string color, double alpha = 1)
    {
        if (alpha is > 1 or < 0) alpha = 1;
        var alphaString = Convert.ToInt32(alpha*255).ToString("X2");
        return Brush.Parse($@"#{alphaString}{color}");
    }
    
}