using Microsoft.Win32;
using System;

namespace SystemMonitoring.Services
{
    public static class UsbService
    {
        public static bool DisableUsbMassStorage()
        {
            const string keyPath = @"SYSTEM\CurrentControlSet\Services\UsbStor";
            using (var key = Registry.LocalMachine.OpenSubKey(keyPath, writable: true))
            {
                if (key == null)
                    throw new InvalidOperationException("UsbStor service key not found.");

                object v = key.GetValue("Start") ?? 3;
                int startValue = Convert.ToInt32(v);

                if (startValue == 4) return true;

                key.SetValue("Start", 4, RegistryValueKind.DWord);
                return true;
            }
        }
    }
}
