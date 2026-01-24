using System;
using System.Globalization;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading;
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
    AppSettings GetSettings();
    void SaveSettings(AppSettings settings);
    void ApplySettings(AppSettings settings);
}

public class SettingsService(IStorageService storage) : ISettingsService
{
    public AppSettings GetSettings()
    {
        var defaultSettings = new AppSettings();
        string? rawJson = storage.Load(KeyTexts.SettingsKey);
        if (string.IsNullOrWhiteSpace(rawJson))
        {
            SaveSettings(defaultSettings);
            return defaultSettings;
        }
        try
        {
            if (!ValidateHash(rawJson))
            {
                SaveSettings(defaultSettings);
                return defaultSettings;
            }
            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true,
                PreferredObjectCreationHandling = JsonObjectCreationHandling.Populate
            };
            var loadedSettings = JsonSerializer.Deserialize<AppSettings>(rawJson, options) ?? defaultSettings;
            if (!IsSyncRequired(rawJson, loadedSettings)) return loadedSettings;
            SaveSettings(loadedSettings);
            return loadedSettings;
        }
        catch
        {
            SaveSettings(defaultSettings);
            return defaultSettings;
        }
    }

    private bool IsSyncRequired(string originalJson, AppSettings loadedSettings)
    {
        string? currentHash = loadedSettings.HashSignature;
        loadedSettings.HashSignature = null;
        string serializedCurrent = JsonSerializer.Serialize(loadedSettings);
        loadedSettings.HashSignature = currentHash;
        return !NormalizeJson(originalJson).Equals(serializedCurrent);
    }

    public void SaveSettings(AppSettings settings)
    {
        settings.HashSignature = null; 
        var jsonWithoutHash = JsonSerializer.Serialize(settings);
        settings.HashSignature = ComputeHash(jsonWithoutHash);
        var finalJson = JsonSerializer.Serialize(settings);
        storage.Save(KeyTexts.SettingsKey, finalJson);
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
        var bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(text + KeyTexts.Salt));
        return Convert.ToBase64String(bytes);
    }

    private bool ValidateHash(string fullJson)
    {
        try
        {
            using var doc = JsonDocument.Parse(fullJson);
            if (!doc.RootElement.TryGetProperty(nameof(AppSettings.HashSignature), out var hashProp))
                return false;
            string? storedHash = hashProp.GetString();
            var options = new JsonSerializerOptions { WriteIndented = false };
            var tempObj = JsonSerializer.Deserialize<AppSettings>(fullJson, options);
            if (tempObj == null) return false;
            tempObj.HashSignature = null;
            string recomputedHash = ComputeHash(JsonSerializer.Serialize(tempObj));

            return storedHash == recomputedHash;
        }
        catch { return false; }
    }
    
    private string NormalizeJson(string json)
    {
        try
        {
            using var doc = JsonDocument.Parse(json);
            return JsonSerializer.Serialize(doc, new JsonSerializerOptions { WriteIndented = false });
        }
        catch
        {
            return json;
        }
    }
}