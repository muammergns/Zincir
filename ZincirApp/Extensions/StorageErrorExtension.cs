using ZincirApp.Models;

namespace ZincirApp.Extensions;

public static class StorageErrorExtension
{
    public static string ToUserMessage(this StorageError error)
    {
        return error switch
        {
            StorageError.None => string.Empty,
            StorageError.NotFound => "Belirtilen dosya veya klasör bulunamadı.",
            StorageError.AccessDenied => "Erişim reddedildi. Dosya başka bir program tarafından kullanılıyor olabilir veya yazma yetkiniz yok.",
            StorageError.DiskFull => "Cihazda yeterli alan yok. Lütfen depolama alanını kontrol edin.",
            StorageError.IoError => "Dosya sistemiyle ilgili bir hata oluştu (Giriş/Çıkış hatası).",
            StorageError.Unknown => "Beklenmedik bir hata meydana geldi.",
            _ => "Tanımlanamayan bir hata oluştu."
        };
    }
}