using System;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using ZincirApp.Assets;
using ZincirApp.Services;

namespace ZincirApp.Desktop;

public class DesktopDeviceIdService : IDeviceIdService
{
    public string GetDeviceId()
    {
        try
        {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                return GetWindowsId();
            }
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            {
                return GetLinuxId();
            }
            if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
            {
                return GetMacId();
            }
        }
        catch (Exception)
        {
            //Ignore
        }

        return Keys.DeviceId;
    }

    private string GetWindowsId()
    {
        var process = new Process
        {
            StartInfo = new ProcessStartInfo
            {
                FileName = "wmic",
                Arguments = "bios get serialnumber",
                UseShellExecute = false,
                RedirectStandardOutput = true,
                CreateNoWindow = true
            }
        };
        process.Start();
        string output = process.StandardOutput.ReadToEnd();
        process.WaitForExit();
        
        string result = output.Replace("SerialNumber", "").Trim();
        return string.IsNullOrEmpty(result) ? Keys.DeviceId : result;
    }

    private string GetLinuxId()
    {
        try
        {
            if (File.Exists("/proc/cpuinfo"))
            {
                string[] lines = File.ReadAllLines("/proc/cpuinfo");
                foreach (string line in lines)
                {
                    if (!line.StartsWith("Serial", StringComparison.OrdinalIgnoreCase)) continue;
                    string[] parts = line.Split(':');
                    if (parts.Length <= 1) continue;
                    string serial = parts[1].Trim();
                    if (!string.IsNullOrEmpty(serial) && serial != "0000000000000000")
                    {
                        return serial;
                    }
                }
            }
        }
        catch
        {
            // Ignore
        }
        
        try
        {
            if (File.Exists("/etc/machine-id"))
            {
                return File.ReadAllText("/etc/machine-id").Trim();
            }
            if (File.Exists("/var/lib/dbus/machine-id"))
            {
                return File.ReadAllText("/var/lib/dbus/machine-id").Trim();
            }
        }
        catch
        {
            // Ignore
        }

        return Keys.DeviceId;
    }

    private string GetMacId()
    {
        var process = new Process
        {
            StartInfo = new ProcessStartInfo
            {
                FileName = "ioreg",
                Arguments = "-rd1 -c IOPlatformExpertDevice",
                UseShellExecute = false,
                RedirectStandardOutput = true,
                CreateNoWindow = true
            }
        };
        process.Start();
        string output = process.StandardOutput.ReadToEnd();
        process.WaitForExit();
        int index = output.IndexOf("IOPlatformUUID", StringComparison.Ordinal);
        if (index < 0) return Keys.DeviceId;
        string result = output.Substring(index).Split('"')[3];
        return result;
    }
}