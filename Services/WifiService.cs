using System;
using System.Diagnostics;
using System.IO;

namespace SystemMonitoring
{
    public static class WifiService
    {
        public static bool EnsureConnectedWifiIs(string allowedSsid, out string currentSsid, out string message)
        {
            currentSsid = "";
            message = "";

            var psi = new ProcessStartInfo("netsh", "wlan show interfaces")
            {
                RedirectStandardOutput = true,
                UseShellExecute = false,
                CreateNoWindow = true
            };

            using (var p = Process.Start(psi))
            {
                string output = p.StandardOutput.ReadToEnd();
                p.WaitForExit();

                using (StringReader sr = new StringReader(output))
                {
                    string line;
                    while ((line = sr.ReadLine()) != null)
                    {
                        if (line.Contains("SSID") && line.Contains(":"))
                        {
                            string val = line.Split(':')[1].Trim();
                            if (!val.Equals("0"))
                            {
                                currentSsid = val;
                                break;
                            }
                        }
                    }
                }
            }

            if (string.IsNullOrEmpty(currentSsid))
            {
                message = "No Wi-Fi connected.";
                return false;
            }

            if (!string.Equals(currentSsid, allowedSsid, StringComparison.OrdinalIgnoreCase))
            {
                var psi2 = new ProcessStartInfo("netsh", "wlan disconnect")
                {
                    RedirectStandardOutput = true,
                    UseShellExecute = false,
                    CreateNoWindow = true
                };
                using (var p2 = Process.Start(psi2))
                {
                    p2.StandardOutput.ReadToEnd();
                    p2.WaitForExit();
                }
                message = $"Disconnected from {currentSsid} because allowed is {allowedSsid}.";
                return false;
            }

            message = $"Connected to allowed Wi-Fi: {currentSsid}.";
            return true;
        }
    }
}
