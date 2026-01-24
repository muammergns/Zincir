namespace ZincirApp.Services;

public interface IStorageService
{
    void Save(string key, string value);
    string? Load(string key);
}

public class StorageService : IStorageService
{

    public void Save(string key, string value)
    {
        
    }

    public string? Load(string key)
    {
        return null;
    }
}