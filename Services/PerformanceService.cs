using System;
using System.Diagnostics;
using System.Threading;

namespace SystemMonitoring
{
    public static class PerformanceService
    {
        public static string GetCpuAndMemoryUsage()
        {
            try
            {
                var process = Process.GetCurrentProcess();
                float usedMemMB = process.WorkingSet64 / (1024f * 1024f);

                TimeSpan startCpuTime = process.TotalProcessorTime;
                DateTime startTime = DateTime.UtcNow;

                Thread.Sleep(500);

                TimeSpan endCpuTime = process.TotalProcessorTime;
                DateTime endTime = DateTime.UtcNow;

                double cpuUsedMs = (endCpuTime - startCpuTime).TotalMilliseconds;
                double totalMsPassed = (endTime - startTime).TotalMilliseconds;

                int processorCount = Environment.ProcessorCount;
                double cpuPercent = (cpuUsedMs / (totalMsPassed * processorCount)) * 100.0;

                return $"CPU (this program): {cpuPercent:F1}% | Memory (this program): {usedMemMB:F1} MB";
            }
            catch { return "Failed to get program CPU/memory usage"; }
        }
    }
}
