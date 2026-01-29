using System;
using System.IO;
using System.Threading.Tasks;

namespace ZincirApp.Services;

public interface IStorageService
{
    Task<StorageResult> SaveText(string fileName, string data);
    Task<StorageResult> SaveText(string fileName, string folderName, string data);
    Task<StorageResult<string>> LoadText(string fileName);
    Task<StorageResult<string>> LoadText(string fileName, string folderName);
    string GetPath(string fileName);
    string Storage {get; }
}

public enum StorageError
{
    None,           // İşlem başarılı
    NotFound,       // Dosya veya klasör yok
    AccessDenied,   // Yetki hatası (Dosya kullanımda olabilir)
    DiskFull,       // Depolama alanı dolu (WASM'da QuotaExceeded)
    IoError,        // Genel giriş/çıkış hatası
    Unknown         // Beklenmedik hata
}

public class StorageResult
{
    public bool IsSuccess { get; }
    public StorageError Error { get; }
    public string? Message { get; }

    protected StorageResult(bool isSuccess, StorageError error,  string? message = null)
    {
        IsSuccess = isSuccess;
        Error = error;
        Message = message;
    }

    public static StorageResult Success() => new StorageResult(true, StorageError.None);
    public static StorageResult Failure(StorageError error)
        => new StorageResult(false, error);
    public static StorageResult Failure(StorageError error, string message)
        => new StorageResult(false, error, message);
}

public class StorageResult<T> : StorageResult
{
    public T? Value { get; }

    private StorageResult(bool isSuccess, T? value, StorageError error, string? message = null) 
        : base(isSuccess, error, message)
    {
        Value = value;
    }

    public static StorageResult<T> Success(T value)
        => new StorageResult<T>(true, value, StorageError.None);
    public new static StorageResult<T> Failure(StorageError error)
        => new StorageResult<T>(false, default, error);
    public new static StorageResult<T> Failure(StorageError error, string message)
        => new StorageResult<T>(false, default, error, message);
}

public class StorageService : IStorageService
{
    public async Task<StorageResult> SaveText(string fileName, string data)
    {
        string fullPath = Path.Combine(Storage, fileName);
        
        if (!CreateFolder(fullPath))
        {
            return StorageResult.Failure(StorageError.AccessDenied);
        }

        try
        {
            await File.WriteAllTextAsync(fullPath, data);
            return StorageResult.Success();
        }
        catch (UnauthorizedAccessException ex)
        {
            return StorageResult.Failure(StorageError.AccessDenied, ex.Message);
        }
        catch (IOException ex) when (IsDiskFull(ex))
        {
            return StorageResult.Failure(StorageError.DiskFull);
        }
        catch (IOException ex)
        {
            return StorageResult.Failure(StorageError.IoError, ex.Message);
        }
        catch (Exception ex)
        {
            return StorageResult.Failure(StorageError.Unknown, ex.Message);
        }
    }

    public async Task<StorageResult> SaveText(string fileName, string folderName, string data)
    {
        string fullPath = Path.Combine(Storage, folderName, fileName);
        
        if (!CreateFolder(fullPath))
        {
            return StorageResult.Failure(StorageError.AccessDenied);
        }

        try
        {
            await File.WriteAllTextAsync(fullPath, data);
            return StorageResult.Success();
        }
        catch (UnauthorizedAccessException ex)
        {
            return StorageResult.Failure(StorageError.AccessDenied, ex.Message);
        }
        catch (IOException ex) when (IsDiskFull(ex))
        {
            return StorageResult.Failure(StorageError.DiskFull);
        }
        catch (IOException ex)
        {
            return StorageResult.Failure(StorageError.IoError, ex.Message);
        }
        catch (Exception ex)
        {
            return StorageResult.Failure(StorageError.Unknown, ex.Message);
        }
    }
    private bool IsDiskFull(IOException ex)
    {
        const int errorDıskFull = 0x70; // 112
        const int errorHandleDıskFull = 0x27; // 39
        int errorCode = ex.HResult & 0xFFFF;
        return errorCode == errorDıskFull || errorCode == errorHandleDıskFull;
    }
    public async Task<StorageResult<string>> LoadText(string fileName)
    {
        string fullPath = Path.Combine(Storage, fileName);

        if (!CheckFile(fullPath))
        {
            return StorageResult<string>.Failure(StorageError.NotFound);
        }

        try
        {
            string content = await File.ReadAllTextAsync(fullPath);
            return StorageResult<string>.Success(content);
        }
        catch (UnauthorizedAccessException ex)
        {
            return StorageResult<string>.Failure(StorageError.AccessDenied, ex.Message);
        }
        catch (IOException ex)
        {
            return StorageResult<string>.Failure(StorageError.IoError, ex.Message);
        }
        catch (Exception ex)
        {
            return StorageResult<string>.Failure(StorageError.Unknown, ex.Message);
        }
    }

    public async Task<StorageResult<string>> LoadText(string fileName, string folderName)
    {
        string fullPath = Path.Combine(Storage, folderName, fileName);

        if (!CheckFile(fullPath))
        {
            return StorageResult<string>.Failure(StorageError.NotFound);
        }

        try
        {
            string content = await File.ReadAllTextAsync(fullPath);
            return StorageResult<string>.Success(content);
        }
        catch (UnauthorizedAccessException ex)
        {
            return StorageResult<string>.Failure(StorageError.AccessDenied, ex.Message);
        }
        catch (IOException ex)
        {
            return StorageResult<string>.Failure(StorageError.IoError, ex.Message);
        }
        catch (Exception ex)
        {
            return StorageResult<string>.Failure(StorageError.Unknown, ex.Message);
        }
    }

    public string GetPath(string fileName)
    {
        return Path.Combine(Storage, fileName);
    }

    private bool CreateFolder(string folderPath)
    {
        string? directory = Path.GetDirectoryName(folderPath);
        if (string.IsNullOrWhiteSpace(directory))
        {
            return false;
        }
        if (Directory.Exists(directory))
        {
            return true;
        }
        try
        {
            var info = Directory.CreateDirectory(directory);
            return info.Exists;
        }
        catch (Exception)
        {
            return false;
        }
    }
    private bool CheckFile(string folderPath)
    {
        return !string.IsNullOrWhiteSpace(folderPath) && File.Exists(folderPath);
    }
    public string Storage {get;} = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "ZincirApp");
}