using ZincirApp.ViewModels;

namespace ZincirApp.Messages;

public record AddPlusHabitLogMessage(HabitItemViewModel? Model);
public record AddMinusHabitLogMessage(HabitItemViewModel? Model);
public record AddValueHabitLogMessage(HabitItemViewModel? Model);