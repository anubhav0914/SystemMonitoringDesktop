using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SystemMonitoring.Persistence
{
    public class LogEntry
    {
        public DateTime Timestamp { get; set; }
        public bool? UsbDisabled { get; set; }
        public string WifiCurrent { get; set; }
        public List<string> Messages { get; set; } = new List<string>();
    }
}
