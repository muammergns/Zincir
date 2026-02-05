using System;
using System.Threading;
using Android.Provider;
using Android.App;
using Android.Content;
using ZincirApp.Assets;
using ZincirApp.Services;

namespace ZincirApp.Android;

public class AndroidDeviceIdService(Context context) : IDeviceIdService
{
    private static readonly Lock Lock = new Lock();
    private const string PrefFileName = "zincir_prefs";
    private const string GuidKey = "app_instance_id";
    private const string NotFound = "NOT_FOUND";
    public string GetDeviceId()
    {
        lock (Lock)
        {
            string? androidId = Settings.Secure.GetString(context.ContentResolver, Settings.Secure.AndroidId);
            if (!string.IsNullOrEmpty(androidId)) return androidId;
            string? savedId = null;
            var prefs = context.GetSharedPreferences(PrefFileName, FileCreationMode.Private);
            if (prefs != null)
            {
                savedId = prefs.GetString(GuidKey, NotFound);
                if (savedId == NotFound)
                {
                    var editor = prefs.Edit();
                    if (editor != null)
                    {
                        savedId = Guid.NewGuid().ToString();
                        editor.PutString(GuidKey, savedId);
                        editor.Apply();
                    }
                }
            }
            return savedId ?? Keys.DeviceId;
        }
    }
}