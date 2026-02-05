using System;
using System.Globalization;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Styling;
using Material.Colors;
using Material.Styles.Themes;
using Material.Styles.Themes.Base;
using ZincirApp.Assets;
using ZincirApp.Models;

namespace ZincirApp.Services;

public interface ISettingsService
{
    Task<AppSettings> GetSettings();
    Task SaveSettings(AppSettings settings);
    void ApplySettings(AppSettings settings);
}

public class SettingsService(IStorageService storage, IAesService aesService) : ISettingsService
{
    private AppSettings? _appSettings;
    private const string SettingsFileName  = "user_settings";
    public async Task<AppSettings> GetSettings()
    {
        if (_appSettings != null) return _appSettings;
        var defaultSettings = new AppSettings
        {
            UserId = Keys.UserId
        };
        var result = await storage.LoadText(SettingsFileName);
        if (!result.IsSuccess || result.Value is not { } rawJson || string.IsNullOrWhiteSpace(rawJson))
        {
            await SaveSettings(defaultSettings);
            return defaultSettings;
        }
        try
        {
            string decryptedJson = await aesService.Decrypt(rawJson, Keys.SettingsId, Keys.Salt, Keys.Iterations);
            var loadedSettings = JsonSerializer.Deserialize<AppSettings>(
                decryptedJson, AppJsonContext.Default.AppSettings) ?? defaultSettings;
            if (!ValidateHash(loadedSettings))
            {
                await SaveSettings(defaultSettings);
                return defaultSettings;
            }
            if (IsSyncRequired(decryptedJson, loadedSettings))
                await SaveSettings(loadedSettings);
            _appSettings = loadedSettings;
            return _appSettings;
        }
        catch
        {
            await SaveSettings(defaultSettings);
            return defaultSettings;
        }
    }

    private bool IsSyncRequired(string originalJson, AppSettings loadedSettings)
    {
        string? currentHash = loadedSettings.HashSignature;
        loadedSettings.HashSignature = null;
        string serializedCurrent = JsonSerializer.Serialize(loadedSettings, AppJsonContext.Default.AppSettings);
        loadedSettings.HashSignature = currentHash;
        return !NormalizeJson(originalJson).Equals(serializedCurrent);
    }

    public async Task SaveSettings(AppSettings settings)
    {
        settings.HashSignature = null; 
        string jsonWithoutHash = JsonSerializer.Serialize(settings, AppJsonContext.Default.AppSettings);
        settings.HashSignature = ComputeHash(jsonWithoutHash);
        string finalJson = JsonSerializer.Serialize(settings, AppJsonContext.Default.AppSettings);
        string encryptedJson = await aesService.Encrypt(finalJson, Keys.SettingsId, Keys.Salt, Keys.Iterations);
        await storage.SaveText(SettingsFileName, encryptedJson);
    }

    public void ApplySettings(AppSettings settings)
    {
        if (Application.Current != null)
        {
            var newVariant = settings.Theme == "Dark" ? ThemeVariant.Dark : ThemeVariant.Light;
            Application.Current.RequestedThemeVariant = newVariant;
            var materialTheme = Application.Current.Styles.OfType<MaterialTheme>().FirstOrDefault();
            if (materialTheme != null)
            {
                materialTheme.BaseTheme = newVariant == ThemeVariant.Dark 
                    ? BaseThemeMode.Dark 
                    : BaseThemeMode.Light;
                if (Enum.TryParse(settings.PrimaryColor, out PrimaryColor primaryColor))
                {
                    materialTheme.PrimaryColor = primaryColor;
                }
                if (Enum.TryParse(settings.SecondaryColor, out SecondaryColor secondaryColor))
                {
                    materialTheme.SecondaryColor = secondaryColor;
                }
            }
        }

        // Dil
        Locale.UiTexts.Culture = new CultureInfo(settings.Language);
        Thread.CurrentThread.CurrentCulture = new CultureInfo(settings.Language);
        Thread.CurrentThread.CurrentUICulture = new CultureInfo(settings.Language);
    }

    private string ComputeHash(string text)
    {
        using var sha256 = SHA256.Create();
        byte[] bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(text + Keys.SettingsId));
        return Convert.ToBase64String(bytes);
    }

    private bool ValidateHash(AppSettings settings)
    {
        try
        {
            string? storedHash = settings.HashSignature; 
            if (string.IsNullOrWhiteSpace(storedHash)) return false;
            settings.HashSignature = null; 
            string jsonWithoutHash = JsonSerializer.Serialize(settings, AppJsonContext.Default.AppSettings);
            settings.HashSignature = storedHash;
            string recomputedHash = ComputeHash(jsonWithoutHash);
            return storedHash == recomputedHash;
        }
        catch { return false; }
    }
    
    private string NormalizeJson(string json)
    {
        try
        {
            using var doc = JsonDocument.Parse(json);
            return JsonSerializer.Serialize(doc, AppJsonContext.Default.AppSettings);
        }
        catch
        {
            return json;
        }
    }
}