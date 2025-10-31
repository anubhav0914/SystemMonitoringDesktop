using System;
using System.Collections.Generic;
using System.IO;

namespace SystemMonitoring
{
    public static class CsvHelper
    {
        public static string[] ParseCsvLine(string line)
        {
            var fields = new List<string>();
            bool inQuote = false;
            var sb = new System.Text.StringBuilder();

            for (int i = 0; i < line.Length; i++)
            {
                char c = line[i];
                if (c == '"')
                {
                    if (inQuote && i + 1 < line.Length && line[i + 1] == '"')
                    {
                        sb.Append('"');
                        i++;
                    }
                    else
                        inQuote = !inQuote;
                }
                else if (c == ',' && !inQuote)
                {
                    fields.Add(sb.ToString());
                    sb.Clear();
                }
                else
                {
                    sb.Append(c);
                }
            }

            fields.Add(sb.ToString());
            return fields.ToArray();
        }

        public static string ExtractExePath(string command)
        {
            command = command.Trim();
            command = Environment.ExpandEnvironmentVariables(command);

            if (command.StartsWith("\""))
            {
                int idx = command.IndexOf('"', 1);
                if (idx > 1) return command.Substring(1, idx - 1);
            }
            else
            {
                var parts = command.Split(new[] { ' ' }, 2);
                if (File.Exists(parts[0])) return parts[0];
            }

            return command;
        }
    }
}
