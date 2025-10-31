using System;
using System.Windows.Forms;
using System.Threading;
using SystemMonitoring.Utils;

namespace SystemMonitoring
{
    public partial class Form1 : Form
    {
        private System.Threading.Timer monitorTimer;
        private const int IntervalMs = 20000; // 20 seconds
        private SystemMonitor monitor;

        public Form1()
        {
            InitializeComponent();
            this.Load += Form1_Load;
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            monitor = new SystemMonitor();

            if (!monitor.IsRunningAsAdmin())
            {
                LogHelper.AppendLog("Program must run as Administrator. Exiting.");
                Application.Exit();
                return;
            }

            monitorTimer = new System.Threading.Timer(
                callback: MonitorCallback,
                state: null,
                dueTime: 0,
                period: IntervalMs
            );
        }

        private void MonitorCallback(object state)
        {
            var logEntry = monitor.RunAllChecks();
            monitor.AppendLogToUI(logEntry, lvLogs); // lvLogs is your ListView
        }
    }
}
