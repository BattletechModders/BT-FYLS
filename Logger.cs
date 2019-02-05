using System;
using System.IO;
using Harmony;

namespace BattletechLogCleaner
{
    public static class Logger
    {
        private static string CleanLogFilePath => $"{Core.ModDirectory}/../cleaned_output_log.txt";
        private static string FullLogFilePath  => $"{Core.ModDirectory}/../output_log.txt";

        public static StreamWriter CleanWriter;
        public static StreamWriter FullWriter;

        public static void InitDebugFiles()
        {
            CleanWriter = new StreamWriter(CleanLogFilePath, false);
            CleanWriter.AutoFlush = true;
            CleanWriter.WriteLine($"{DateTime.Now} Cleaned Log");
            CleanWriter.WriteLine(new string(c: '-', count: 80));
            CleanWriter.WriteLine(VersionInfo.GetFormattedInfo());
            if (Core.ModSettings.preserveFullLog)
            {
                FullWriter = new StreamWriter(FullLogFilePath, false);
                FullWriter.AutoFlush = true;
                FullWriter.WriteLine($"{DateTime.Now} Full Log");
                FullWriter.WriteLine(new string(c: '-', count: 80));
                FullWriter.WriteLine(VersionInfo.GetFormattedInfo());
            }
        }

        public static void Error(Exception ex)
        {
            CleanWriter.WriteLine($"Message: {ex.Message}");
            CleanWriter.WriteLine($"StackTrace: {ex.StackTrace}");
            CleanWriter.WriteLine($"Date: {DateTime.Now}");
            CleanWriter.WriteLine(new string(c: '-', count: 80));
        }

        public static void Full(String line)
        {
            FullWriter.WriteLine($"{DateTime.Now.ToString("s")} {line}");
        }

        public static void Debug(String line)
        {
            CleanWriter.WriteLine($"{DateTime.Now.ToString("s")} {line}");
        }
    }
}
