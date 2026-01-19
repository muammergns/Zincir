using System;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using ZincirApp.Assets;

namespace ZincirApp.Settings;

public class SettingsService(ISettingsStorage storage)
{
    public AppSettings Load()
    {
        var defaultSettings = new AppSettings();
        string? rawJson = storage.Load(KeyTexts.SettingsKey);
        if (string.IsNullOrWhiteSpace(rawJson))
        {
            Save(defaultSettings);
            return defaultSettings;
        }
        try
        {
            if (!ValidateHash(rawJson))
            {
                Save(defaultSettings);
                return defaultSettings;
            }
            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true,
                PreferredObjectCreationHandling = JsonObjectCreationHandling.Populate
            };
            var loadedSettings = JsonSerializer.Deserialize<AppSettings>(rawJson, options) ?? defaultSettings;
            if (!IsSyncRequired(rawJson, loadedSettings)) return loadedSettings;
            Save(loadedSettings);
            return loadedSettings;
        }
        catch
        {
            Save(defaultSettings);
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

    public void Save(AppSettings settings)
    {
        settings.HashSignature = null; 
        var jsonWithoutHash = JsonSerializer.Serialize(settings);
        settings.HashSignature = ComputeHash(jsonWithoutHash);
        var finalJson = JsonSerializer.Serialize(settings);
        storage.Save(KeyTexts.SettingsKey, finalJson);
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