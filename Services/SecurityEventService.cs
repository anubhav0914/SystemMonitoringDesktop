using System.Collections.Generic;
using System.Diagnostics;

namespace SystemMonitoring
{
    public static class SecurityEventService
    {
        public static List<string> CheckSecurityEvents()
        {
            var messages = new List<string>();
            try
            {
                EventLog securityLog = new EventLog("Security");
                int start = System.Math.Max(0, securityLog.Entries.Count - 10);

                for (int i = start; i < securityLog.Entries.Count; i++)
                {
                    var entry = securityLog.Entries[i];
                    messages.Add($"Security Event: {entry.EntryType} - {entry.Message.Substring(0, System.Math.Min(10, entry.Message.Length))}...");
                }
            }
            catch (System.Exception ex)
            {
                messages.Add("Security events check failed: " + ex.Message);
            }
            return messages;
        }
    }
}
