﻿using System;
using System.Reflection;
using Harmony;
using HBS.Logging;
using Newtonsoft.Json;
using UnityEngine;
using System.Collections.Generic;

namespace FuckYouLogShit
{
    public class Core
    {
        public const string ModName = "FuckYouLogShit";
        public const string ModId   = "com.joelmeador.FuckYouLogShit";

        internal static Settings ModSettings = new Settings();
        internal static string ModDirectory;

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
            Logger.InitDebugFile();
            var harmony = HarmonyInstance.Create(ModId);
            harmony.PatchAll(Assembly.GetExecutingAssembly());
            var fuck = AccessTools.Inner(typeof(HBS.Logging.Logger), "LogImpl");
            var you = AccessTools.Method(fuck, "LogAtLevel", new Type[]
            {
                typeof(HBS.Logging.LogLevel),
                typeof(object),
                typeof(UnityEngine.Object),
                typeof(Exception),
                typeof(HBS.Logging.IStackTrace)
            });
            var man = typeof(LogAtLevelAttacher).GetMethod(nameof(LogAtLevelAttacher.Prefix));
            harmony.Patch(you, new HarmonyMethod(man));
        }
    }

//    [HarmonyPatch(typeof(HBS.Logging.Logger.LogImpl), "LogAtLevel", new Type[]
//    {
//        typeof(HBS.Logging.LogLevel),
//        typeof(object),
//        typeof(UnityEngine.Object),
//        typeof(Exception),
//        typeof(HBS.Logging.IStackTrace)
//    })]
    static class LogAtLevelAttacher
    {
        private static bool HaveLoggedDebugMessage = false;
        private static FormatHelper FormatHelper = new FormatHelper();

        public static bool Prefix(HBS.Logging.LogLevel level, object message, UnityEngine.Object context, Exception exception, HBS.Logging.IStackTrace location)
        {
            if (!HaveLoggedDebugMessage)
            {
                Logger.Debug("Logging from LogAtLevel");
                HaveLoggedDebugMessage = true;
            }

            var logString = FormatHelper.FormatMessage("FYLS", level, message, exception, location);
            for (var i = 0; i < Core.ModSettings.PrefixesToIgnore.Length; i++)
            {
                var prefix = Core.ModSettings.PrefixesToIgnore[i];
                if (logString.StartsWith(prefix)) return false;
            }

            Logger.Debug(logString);
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
        //private static bool HaveLoggedDebugMessage = false;

        static bool Prefix(string logString, string stackTrace, LogType type)
        {
//            for (var i = 0; i < Core.ModSettings.PrefixesToIgnore.Length; i++)
//            {
//                var prefix = Core.ModSettings.PrefixesToIgnore[i];
//                if (logString.StartsWith(prefix)) return false;
//            }
//
//            Logger.Debug($"{type.ToString()} - {logString}");
//            if ((type == LogType.Error || type == LogType.Exception) &&
//                !string.IsNullOrEmpty(stackTrace))
//            {
//                Logger.Debug(stackTrace);
//            }

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
            { LogType.Log, LogLevel.Log},
            { LogType.Assert, LogLevel.Log },
            { LogType.Warning, LogLevel.Warning },
            { LogType.Error, LogLevel.Error },
            { LogType.Exception, LogLevel.Error }
        };
        public void LogFormat(LogType logType, UnityEngine.Object context, string format, params object[] args)
        {
            hbslog.LogAtLevel(lmap[logType], string.Format(format, args), context);
        }
    }
}