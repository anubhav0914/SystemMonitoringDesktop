using System.Collections.Generic;
using System.Diagnostics;
using System.IO;

namespace SystemMonitoring
{
    public static class ScheduledTaskService
    {
        private static HashSet<string> maliciousHashes = new HashSet<string>
        {
            "aaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaa"
        };

        public static List<string> CheckScheduledTasksAndHandle()
        {
            var results = new List<string>();

            ProcessStartInfo psi = new ProcessStartInfo("schtasks.exe", "/query /fo CSV /v")
            {
                RedirectStandardOutput = true,
                UseShellExecute = false,
                CreateNoWindow = true
            };

            using (Process p = Process.Start(psi))
            {
                string output = p.StandardOutput.ReadToEnd();
                p.WaitForExit();

                var lines = output.Replace("\r\n", "\n").Split('\n');
                if (lines.Length < 2) return results;

                var headers = CsvHelper.ParseCsvLine(lines[0]);
                int taskToRunIdx = System.Array.IndexOf(headers, "Task To Run");
                if (taskToRunIdx < 0) return results;

                for (int i = 1; i < lines.Length; i++)
                {
                    if (string.IsNullOrWhiteSpace(lines[i])) continue;
                    var fields = CsvHelper.ParseCsvLine(lines[i]);
                    if (taskToRunIdx >= fields.Length) continue;

                    string taskRun = fields[taskToRunIdx].Trim();
                    if (string.IsNullOrEmpty(taskRun) || taskRun == "N/A") continue;

                    string exePath = CsvHelper.ExtractExePath(taskRun);
                    if (!File.Exists(exePath)) continue;

                    string sha256 = FileHashHelper.ComputeSHA256OfFile(exePath);
                    results.Add($"Task: {exePath} SHA256={sha256}");

                    if (maliciousHashes.Contains(sha256.ToLowerInvariant()))
                    {
                        string quarantine = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "quarantine");
                        Directory.CreateDirectory(quarantine);
                        string dest = Path.Combine(quarantine, Path.GetFileName(exePath));
                        File.Move(exePath, dest);
                        results.Add($"Malicious file moved to quarantine: {dest}");
                    }
                }
            }

            return results;
        }
    }
}
