using BepInEx;
using BepInEx.Logging;
using HarmonyLib;
using MewsToolbox;
using System;
using System.IO;

namespace SpeenPhone
{
    [BepInPlugin(PluginInfo.GUID, PluginInfo.NAME, PluginInfo.VERSION)]
    public class Main : BaseUnityPlugin
    {
        private static ManualLogSource logger;
        public static string ConfigPath { get; } = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "Speen Mods");
        public static string ConfigFileName { get; } = Path.Combine(ConfigPath, "SpeenPhoneConfig.ini");
        public static string NoteHitSoundFile { get; private set; }

        public new static IniFile Config { get; private set; }

        void Awake()
        {
            logger = Logger;

            if (!File.Exists(ConfigFileName))
                File.Create(ConfigFileName).Close();
            Config = new IniFile(ConfigFileName);
            NoteHitSoundFile = Config.GetValueOrDefaultTo("SFX", "NoteHitSoundPath", "/path/to/sound/file");
            if (NoteHitSoundFile == "/path/to/sound/file")
            {
                LogWarning("This is your first time using this mod. Please set a path to the sound file you would like to use.");
                LogInfo("This mod will not load.");
                return;
            }

            if (!File.Exists(NoteHitSoundFile))
            {
                LogError("The file specified in the config does not exist.");
                LogInfo("This mod will not load.");
                return;
            }

            //if (!NoteHitSoundFile.EndsWith(".ogg"))
            //{
            //    LogWarning("The specified audio file is not a Vorbis file! (.ogg)");
            //    return;
            //}

            if (!AudioPatches.LoadAudio(NoteHitSoundFile))
                return;

            Harmony harmony = new Harmony(PluginInfo.GUID);
            harmony.PatchAll<AudioPatches>();
        }

        public static void Log(LogLevel level, object msg) => logger.Log(level, msg);
        public static void LogDebug(object msg) => Log(LogLevel.Debug, msg);
        public static void LogInfo(object msg) => Log(LogLevel.Info, msg);
        public static void LogMessage(object msg) => Log(LogLevel.Message, msg);
        public static void LogWarning(object msg) => Log(LogLevel.Warning, msg);
        public static void LogError(object msg) => Log(LogLevel.Error, msg);
    }
}
