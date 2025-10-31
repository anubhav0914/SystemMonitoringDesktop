using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using SystemMonitoring.Persistence;

namespace SystemMonitoring.Utils
{
    internal  class LogHelper
    {
        private static string logPath = "monitor_log.json";

        public static void AppendLogToUI(LogEntry entry, ListView lvLogs)
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

        private static void AddToListView(ListView lv, string type, string msg)
        {
            var item = new ListViewItem(type);
            item.SubItems.Add(msg);
            if (type == "Error") item.ForeColor = System.Drawing.Color.Red;
            else if (type == "Success") item.ForeColor = System.Drawing.Color.Green;
            lv.Items.Add(item);
            lv.EnsureVisible(lv.Items.Count - 1);
        }

        public static void AppendLog(string message)
        {
            AppendLogToUI(new LogEntry { Timestamp = DateTime.UtcNow, Messages = new List<string> { message } }, new ListView());
        }
    }
}
