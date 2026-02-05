using System;
using System.IO;
using System.Threading.Tasks;
using ZincirApp.Models;

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