using System;
using System.Runtime.InteropServices.JavaScript;
using System.Threading.Tasks;
using ZincirApp.Models;
using ZincirApp.Services;

namespace ZincirApp.Browser;

public partial class BrowserStorageService : IStorageService
{
    public string Storage { get; } = "ZincirApp";
    public string GetPath(string fileName)
    {
        return $"{Storage}/{fileName}";
    }
    [JSImport("setItem", "StorageModule")]
    private static partial string SetItemJs(string key, string value);

    [JSImport("getItem", "StorageModule")]
    private static partial string? GetItemJs(string key);
    private static bool _isModuleLoaded;
    
    private async Task EnsureModuleLoaded()
    {
        if (!_isModuleLoaded)
        {
            await JSHost.ImportAsync("StorageModule", "/storage.js");
            _isModuleLoaded = true;
        }
    }

    public async Task<StorageResult> SaveText(string fileName, string data)
    {
        return await SaveToLocalStorage($"{Storage}/{fileName}", data);
    }

    public async Task<StorageResult> SaveText(string fileName, string folderName, string data)
    {
        return await SaveToLocalStorage($"{Storage}/{folderName}/{fileName}", data);
    }

    public async Task<StorageResult<string>> LoadText(string fileName)
    {
        return await LoadFromLocalStorage($"{Storage}/{fileName}");
    }

    public async Task<StorageResult<string>> LoadText(string fileName, string folderName)
    {
        return await LoadFromLocalStorage($"{Storage}/{folderName}/{fileName}");
    }

    private async Task<StorageResult> SaveToLocalStorage(string key, string data)
    {
        await Task.Yield();
        Console.WriteLine($@"Saving {data} to {key}");
        try
        {
            await EnsureModuleLoaded();
            string result = SetItemJs(key, data);
            return StorageResult<string>.Success(result);
        }
        catch (JSException ex) when (ex.Message.Contains("QuotaExceededError"))
        {
            return StorageResult.Failure(StorageError.DiskFull);
        }
        catch (Exception ex)
        {
            return StorageResult.Failure(StorageError.IoError, ex.Message);
        }
    }

    private async Task<StorageResult<string>> LoadFromLocalStorage(string key)
    {
        Console.WriteLine($@"Loading {key}");
        await Task.Yield();
        try
        {
            await EnsureModuleLoaded();
            string? data = GetItemJs(key);
            Console.WriteLine($@"Loaded {data}");
            return data==null ? StorageResult<string>.Failure(StorageError.NotFound) : StorageResult<string>.Success(data);
        }
        catch (Exception ex)
        {
            return StorageResult<string>.Failure(StorageError.Unknown, ex.Message);
        }
    }
}