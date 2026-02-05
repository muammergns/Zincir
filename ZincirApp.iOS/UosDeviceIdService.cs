using System;
using Foundation;
using Security;
using UIKit;
using ZincirApp.Services;

namespace ZincirApp.iOS;

public class UosDeviceIdService : IDeviceIdService
{
    private const string KeychainServiceId = "com.zincirapp.service";
    private const string KeychainAccountId = "device_id";

    public string GetDeviceId()
    {
        string? existingId = GetIdFromKeychain();
        if (!string.IsNullOrEmpty(existingId)) return existingId;
        string? idfv = UIDevice.CurrentDevice.IdentifierForVendor?.AsString();
        string finalId = !string.IsNullOrEmpty(idfv) ? idfv : Guid.NewGuid().ToString();
        SaveIdToKeychain(finalId);

        return finalId;
    }

    private string? GetIdFromKeychain()
    {
        var query = new SecRecord(SecKind.GenericPassword)
        {
            Service = KeychainServiceId,
            Account = KeychainAccountId
        };

        var match = SecKeyChain.QueryAsRecord(query, out SecStatusCode code);
        return code == SecStatusCode.Success ? match?.ValueData?.ToString() : null;
    }

    private void SaveIdToKeychain(string id)
    {
        var record = new SecRecord(SecKind.GenericPassword)
        {
            Service = KeychainServiceId,
            Account = KeychainAccountId,
            ValueData = NSData.FromString(id)
        };

        SecKeyChain.Add(record);
    }
}