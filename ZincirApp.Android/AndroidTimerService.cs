using Android.App;
using Android.Content;
using Android.Provider;

using ZincirApp.Services;

namespace ZincirApp.Android;

public class AndroidTimerService : ITimerService
{

    public void StartTimer(uint seconds)
    {
        var intent = new Intent(AlarmClock.ActionSetTimer);
        intent.PutExtra(AlarmClock.ExtraLength, 60);
        intent.PutExtra(AlarmClock.ExtraSkipUi, true);
        intent.AddFlags(ActivityFlags.NewTask);
        Application.Context.StartActivity(intent);
    }
}