using System;
using System.Reflection;
using Harmony;
using Newtonsoft.Json;
using UnityEngine;

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
        }
    }

    [HarmonyPatch(typeof(HBS.Logging.Logger), "HandleUnityLog", MethodType.Normal)]
    static class LogAttacher
    {
        static void Prefix(string logString, string stackTrace, LogType type)
        {
            for (var i = 0; i < Core.ModSettings.PrefixesToIgnore.Length; i++)
            {
                var prefix = Core.ModSettings.PrefixesToIgnore[i];
                if (logString.StartsWith(prefix)) return;
            }

            Logger.Debug($"{type.ToString()} - {logString}");
            if ((type == LogType.Error || type == LogType.Exception) &&
                !string.IsNullOrEmpty(stackTrace))
            {
                Logger.Debug(stackTrace);
            }
        }
    }
}