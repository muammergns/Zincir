using Avalonia.Media;
using Avalonia.Media.Immutable;
using CommunityToolkit.Mvvm.ComponentModel;
using Material.Icons;
using ZincirApp.Models;

namespace ZincirApp.ViewModels;

public partial class TodoItemViewModel(TodoModel? todoModel) : ObservableObject
{
    public TodoModel? Model { get; private set; } = todoModel;
    [ObservableProperty] private string _title = todoModel?.Title ?? string.Empty;
    [ObservableProperty] private bool _isCompleted = todoModel?.IsCompleted ?? false;
    [ObservableProperty] private bool _isPinned = todoModel?.IsPinned ?? false;
    [ObservableProperty] private bool _isUrgent = todoModel?.IsUrgent ?? false;
    [ObservableProperty] private bool _isImportant = todoModel?.IsImportant ?? false;
    [ObservableProperty] private string _description = todoModel?.Description ?? string.Empty;
    [ObservableProperty] private string _createDate = 
        @$"{todoModel?.CreateDate.ToLongDateString() ?? string.Empty} {todoModel?.CreateDate.ToLongTimeString() ?? string.Empty}";
    [ObservableProperty] private MaterialIconKind _pinKind = todoModel?.IsPinned ?? false ? MaterialIconKind.Pin : MaterialIconKind.PinOff;

    [ObservableProperty] private IBrush _priorityColor = (todoModel?.IsImportant, todoModel?.IsUrgent) switch
    {
        (true, true) => Brush.Parse("#F44336"),    // Acil ve Önemli (Kırmızı) #F44336
        (true, false) => Brush.Parse("#2196F3"),   // Önemli ama Acil Değil (Mavi)#2196F3
        (false, true) => Brush.Parse("#FFC107"),   // Acil ama Önemli Değil (Sarı/Turuncu)#FFC107
        _ => Brush.Parse("#9E9E9E")                // İkisi de Değil (Gri)#9E9E9E
    };

    partial void OnIsPinnedChanged(bool value)
    {
        PinKind = value ? MaterialIconKind.Pin : MaterialIconKind.PinOff;
    }


    public void Update(TodoModel? model)
    {
        Model = model;
        
    }
    
}