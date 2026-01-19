namespace ZincirApp.Settings;

public interface ISettingsStorage
{
    void Save(string key, string value);
    string? Load(string key);
}