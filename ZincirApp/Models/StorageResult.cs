namespace ZincirApp.Models;

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