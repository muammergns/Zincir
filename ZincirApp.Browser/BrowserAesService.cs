using System;
using System.Runtime.InteropServices.JavaScript;
using System.Threading.Tasks;
using ZincirApp.Services;

namespace ZincirApp.Browser;

public partial class BrowserAesService : IAesService
{
    private bool _moduleLoaded;

    private async Task EnsureModuleLoaded()
    {
        if (!_moduleLoaded)
        {
            await JSHost.ImportAsync("AesInterop", "/aesInterop.js");
            _moduleLoaded = true;
        }
    }

    [JSImport("aesEncrypt", "AesInterop")]
    private static partial Task<string> JS_Encrypt(string plainText, string password, string salt, int iterations);

    [JSImport("aesDecrypt", "AesInterop")]
    private static partial Task<string> JS_Decrypt(string cipherText, string password, string salt, int iterations);

    public async Task<string> Encrypt(string plainText, string password, string salt, int iterations)
    {
        if (string.IsNullOrEmpty(plainText)) return string.Empty;

        try
        {
            await EnsureModuleLoaded();
            return await JS_Encrypt(plainText, password, salt, iterations);
        }
        catch (Exception)
        {
            return string.Empty;
        }
    }

    public async Task<string> Decrypt(string cipherText, string password, string salt, int iterations)
    {
        if (string.IsNullOrEmpty(cipherText)) return string.Empty;

        try
        {
            await EnsureModuleLoaded();
            return await JS_Decrypt(cipherText, password, salt, iterations);
        }
        catch (Exception)
        {
            return string.Empty;
        }
    }
}
