using System;
using System.IO;
using Harmony;

namespace FuckYouLogShit
{
    public static class Logger
    {
        private static string LogFilePath    => $"{Core.ModDirectory}/../cleaned_output_log.txt";
        private static string ModLogFilePath => $"{Core.ModDirectory}/{Core.ModName}.log";

        public static StreamWriter writer;
        public static void InitDebugFile()
        {
            writer = new StreamWriter(LogFilePath, false);
            writer.AutoFlush = true;
            writer.WriteLine($"{DateTime.Now} Cleaned Log");
            writer.WriteLine(new string(c: '-', count: 80));
            writer.WriteLine(VersionInfo.GetFormattedInfo());
        }

        public static void Error(Exception ex)
        {
            writer.WriteLine($"Message: {ex.Message}");
            writer.WriteLine($"StackTrace: {ex.StackTrace}");
            writer.WriteLine($"Date: {DateTime.Now}");
            writer.WriteLine(new string(c: '-', count: 80));
        }

        public static void Debug(String line)
        {
            writer.WriteLine($"{DateTime.Now.ToString("s")} - {line}");
        }
    }
}