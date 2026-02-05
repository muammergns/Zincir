using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace ZincirApp.Services;

public interface IAesService
{
    Task<string> Encrypt(string plainText, string password, string salt, int iterations);
    Task<string> Decrypt(string cipherText, string password, string salt, int iterations);
}

public class AesService : IAesService
{
    private const int KeySizeInBytes = 32;

    public async Task<string> Encrypt(string plainText, string password, string salt, int iterations)
    {
        if (string.IsNullOrEmpty(plainText)) return plainText;
        byte[] plainBytes = Encoding.UTF8.GetBytes(plainText);
    
        using var aes = Aes.Create();
        aes.Key = Rfc2898DeriveBytes.Pbkdf2(
            password,
            Encoding.UTF8.GetBytes(salt),
            iterations,
            HashAlgorithmName.SHA256,
            KeySizeInBytes);
        aes.GenerateIV();
        byte[] iv = aes.IV;

        using var ms = new MemoryStream();
        ms.Write(iv, 0, iv.Length);

        using (var encryptor = aes.CreateEncryptor(aes.Key, iv))
        await using (var cs = new CryptoStream(ms, encryptor, CryptoStreamMode.Write))
        {
            cs.Write(plainBytes, 0, plainBytes.Length);
            await cs.FlushFinalBlockAsync();
        }

        return Convert.ToBase64String(ms.ToArray());
    }

    public async Task<string> Decrypt(string cipherText, string password, string salt, int iterations)
    {
        if (string.IsNullOrEmpty(cipherText)) return cipherText;

        byte[] fullCipher = Convert.FromBase64String(cipherText);

        using var aes = Aes.Create();
        aes.Key = Rfc2898DeriveBytes.Pbkdf2(
            password,
            Encoding.UTF8.GetBytes(salt),
            iterations,
            HashAlgorithmName.SHA256,
            KeySizeInBytes);

        byte[] iv = new byte[aes.BlockSize / 8];
        byte[] cipher = new byte[fullCipher.Length - iv.Length];

        Buffer.BlockCopy(fullCipher, 0, iv, 0, iv.Length);
        Buffer.BlockCopy(fullCipher, iv.Length, cipher, 0, cipher.Length);

        using var decryptor = aes.CreateDecryptor(aes.Key, iv);
        using var ms = new MemoryStream(cipher);
        await using var cs = new CryptoStream(ms, decryptor, CryptoStreamMode.Read);
        using var sr = new StreamReader(cs);

        return await sr.ReadToEndAsync();
    }
}