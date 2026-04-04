using System.Collections.Generic;
using ZincirApp.Models;
using ZincirApp.Services;

namespace ZincirApp.Messages;

public record PomodoroListUpdated(List<PomodoroModel> Pomodoros, DbState ChangeType);