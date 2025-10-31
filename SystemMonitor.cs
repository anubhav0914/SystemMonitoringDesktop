using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Security.Cryptography;
using System.Security.Principal;
using System.Text;
using System.Text.Json;
using Microsoft.Win32;
using System.Windows.Forms;
using SystemMonitoring.Persistence;
using SystemMonitoring.Utils;
using SystemMonitoring.Services;

namespace SystemMonitoring
{
    public class SystemMonitor
    {
        private string logPath = "monitor_log.json";
        private HashSet<string> maliciousHashes = new HashSet<string>
        {
            "aaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaa"
        };

        public bool IsRunningAsAdmin()
        {
            using (WindowsIdentity id = WindowsIdentity.GetCurrent())
            {
                WindowsPrincipal p = new WindowsPrincipal(id);
                return p.IsInRole(WindowsBuiltInRole.Administrator);
            }
        }

        public LogEntry RunAllChecks()
        {
            var logEntry = new LogEntry { Timestamp = DateTime.UtcNow };

            try { logEntry.UsbDisabled = UsbService.DisableUsbMassStorage(); logEntry.Messages.Add($"USB storage disabled: {logEntry.UsbDisabled}"); }
            catch (Exception ex) { logEntry.Messages.Add("USB check failed: " + ex.Message); }

            try { logEntry.Messages.AddRange(ScheduledTaskService.CheckScheduledTasksAndHandle()); }
            catch (Exception ex) { logEntry.Messages.Add("Scheduled tasks check failed: " + ex.Message); }

            try
            {
                string allowed = "Bheek Le Lo";
                bool ok = WifiService.EnsureConnectedWifiIs(allowed, out string current, out string wifiMsg);
                logEntry.WifiCurrent = current;
                logEntry.Messages.Add(wifiMsg);
            }
            catch (Exception ex) { logEntry.Messages.Add("Wi-Fi check failed: " + ex.Message); }

            logEntry.Messages.AddRange(SecurityEventService.CheckSecurityEvents());
            logEntry.Messages.Add(PerformanceService.GetCpuAndMemoryUsage());

            return logEntry;
        }

        // --- Move all helper functions here ---
        // DisableUsbMassStorage(), CheckScheduledTasksAndHandle(), ComputeSHA256OfFile(), 
        // EnsureConnectedWifiIs(), GetCpuAndMemoryUsage(), ParseCsvLine(), ExtractExePath(), etc.

        public void AppendLogToUI(LogEntry entry, ListView lvLogs)
        {
            List<LogEntry> list = new List<LogEntry>();
            if (File.Exists(logPath))
            {
                try { list = JsonSerializer.Deserialize<List<LogEntry>>(File.ReadAllText(logPath)) ?? new List<LogEntry>(); }
                catch { list = new List<LogEntry>(); }
            }

            if (list.Count > 0)
            {
                var lastEntry = list[list.Count - 1];
                entry.Messages.RemoveAll(m => lastEntry.Messages.Contains(m));
            }

            if (entry.Messages.Count > 0)
            {
                list.Add(entry);
                File.WriteAllText(logPath, JsonSerializer.Serialize(list, new JsonSerializerOptions { WriteIndented = true }));

                foreach (var msg in entry.Messages)
                {
                    string type = "Info";
                    if (msg.Contains("not found") || msg.Contains("failed") || msg.Contains("error")) type = "Error";
                    else if (msg.Contains("Connected")) type = "Success";

                    if (lvLogs.InvokeRequired)
                        lvLogs.Invoke(new Action(() => AddToListView(lvLogs, type, msg)));
                    else
                        AddToListView(lvLogs, type, msg);
                }
            }
        }

        private void AddToListView(ListView lv, string type, string msg)
        {
            var item = new ListViewItem(type);
            item.SubItems.Add(msg);
            if (type == "Error") item.ForeColor = System.Drawing.Color.Red;
            else if (type == "Success") item.ForeColor = System.Drawing.Color.Green;
            lv.Items.Add(item);
            lv.EnsureVisible(lv.Items.Count - 1);
        }
    }
}
