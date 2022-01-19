using BepInEx;
using BepInEx.Logging;

namespace SpeenPhone
{
    [BepInPlugin(PluginInfo.GUID, PluginInfo.NAME, PluginInfo.VERSION)]
    public class Main : BaseUnityPlugin
    {
        private static ManualLogSource logger;

        void Awake()
        {
            logger = Logger;
            LogMessage("Hi!");
        }

        public static void Log(LogLevel level, object msg) => logger.Log(level, msg);
        public static void LogDebug(object msg) => Log(LogLevel.Debug, msg);
        public static void LogInfo(object msg) => Log(LogLevel.Info, msg);
        public static void LogMessage(object msg) => Log(LogLevel.Message, msg);
        public static void LogWarning(object msg) => Log(LogLevel.Warning, msg);
        public static void LogError(object msg) => Log(LogLevel.Error, msg);
    }
}
