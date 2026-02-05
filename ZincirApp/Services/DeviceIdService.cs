using System;
using ZincirApp.Assets;

namespace ZincirApp.Services;

public interface IDeviceIdService
{
    string GetDeviceId();
}

public class DeviceIdService : IDeviceIdService
{

    public string GetDeviceId()
    {
        return Keys.DeviceId;
    }
}