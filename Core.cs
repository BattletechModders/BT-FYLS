using System;
using System.Reflection;
using Harmony;
using HBS.Logging;
using Newtonsoft.Json;
using UnityEngine;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace FYLS
{
    public class Core
    {
        public const string ModName = "FYLS";
        public const string ModId   = "com.joelmeador.FYLS";

        internal static Settings ModSettings = new Settings();
        internal static string ModDirectory;
        internal static Regex LogPrefixesMatcher;

        public static void Init(string directory, string settingsJson)
        {
            try
            {
                ModSettings = JsonConvert.DeserializeObject<Settings>(settingsJson);
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
                ModSettings = new Settings();
            }
            ModDirectory = directory;
            var escapedIgnoredPrefixes = ModSettings.PrefixesToIgnore.Select(x => Regex.Escape(x));
            var ignoredPrefixesPattern = $"^(?:{string.Join("|", escapedIgnoredPrefixes.ToArray())})";
            LogPrefixesMatcher = new Regex(ignoredPrefixesPattern);

            Logger.InitDebugFiles();

            var harmony = HarmonyInstance.Create(ModId);
            harmony.PatchAll(Assembly.GetExecutingAssembly());

            // patch manually because of internal classes
            var logImplemntation = AccessTools.Inner(typeof(HBS.Logging.Logger), "LogImpl");
            var original = AccessTools.Method(logImplemntation, "LogAtLevel", new Type[]
            {
                typeof(HBS.Logging.LogLevel),
                typeof(object),
                typeof(UnityEngine.Object),
                typeof(Exception),
                typeof(HBS.Logging.IStackTrace)
            });
            var prefixMethod = typeof(LogAtLevelAttacher).GetMethod(nameof(LogAtLevelAttacher.Prefix));
            harmony.Patch(original, new HarmonyMethod(prefixMethod));
        }
    }

    public static class LogAtLevelAttacher
    {
        private static FormatHelper FormatHelper = new FormatHelper();
        private static String loggerName = "FYLS";
        public static bool Prefix(HBS.Logging.LogLevel level, object message, UnityEngine.Object context, Exception exception, HBS.Logging.IStackTrace location)
        {
            var logString = FormatHelper.FormatMessage(loggerName, level, message, exception, location);
            if (Core.ModSettings.preserveFullLog) Logger.Full(logString);

            if (!Core.LogPrefixesMatcher.IsMatch(logString)) {
               Logger.Debug(logString);
            }
            return false;
        }
    }

    [HarmonyPatch(typeof(UnityEngine.Debug), "logger", MethodType.Getter)]
    public static class DebugLoggerAttacher
    {
        public static ILogger DebugLog;
        public static bool Prefix(ref ILogger __result)
        {
            if (DebugLog == null) DebugLog = new UnityEngine.Logger(new LoggerProxy(HBS.Logging.Logger.GetLogger("UnityEngine.Debug")));
            __result = DebugLog;
            return false;
        }
    }
    
    [HarmonyPatch(typeof(HBS.Logging.Logger), "HandleUnityLog", MethodType.Normal)]
    public static class LogAttacher
    {
        public static bool Prefix(string logString, string stackTrace, LogType type)
        {
            return false;
        }
    }

    class LoggerProxy : ILogHandler
    {
        ILog hbslog;
        public LoggerProxy(ILog hbslog)
        {
            this.hbslog = hbslog;
        }

        public void LogException(Exception exception, UnityEngine.Object context)
        {
            hbslog.LogException(exception, context);
        }

        static Dictionary<LogType, LogLevel> lmap = new Dictionary<LogType, LogLevel> {
            {LogType.Log, LogLevel.Log},
            {LogType.Assert, LogLevel.Log},
            {LogType.Warning, LogLevel.Warning},
            {LogType.Error, LogLevel.Error},
            {LogType.Exception, LogLevel.Error}
        };
        public void LogFormat(LogType logType, UnityEngine.Object context, string format, params object[] args)
        {
            hbslog.LogAtLevel(lmap[logType], string.Format(format, args), context);
        }
    }
}
