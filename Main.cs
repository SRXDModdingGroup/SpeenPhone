using BepInEx;
using BepInEx.Logging;
using HarmonyLib;
using MewsToolbox;
using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace SpeenPhone
{
    [BepInPlugin(PluginInfo.GUID, PluginInfo.NAME, PluginInfo.VERSION)]
    public class Main : BaseUnityPlugin
    {
        private static ManualLogSource logger;
        public static string ConfigPath { get; } = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "Speen Mods");
        public static string ConfigFileName { get; } = Path.Combine(ConfigPath, "SpeenPhoneConfig.ini");

        public new static IniFile Config { get; private set; }

        void Awake()
        {
            logger = Logger;

            if (!Directory.Exists(ConfigPath))
                Directory.CreateDirectory(ConfigPath);
            if (!File.Exists(ConfigFileName))
                File.Create(ConfigFileName).Close();
            Config = new IniFile(ConfigFileName);

            LoadHitsounds();
            Harmony harmony = new Harmony(PluginInfo.GUID);
            harmony.PatchAll<AudioPatches>();
        }

        void LoadHitsounds()
        {
            string hitsoundsPath = Config.GetValueOrDefaultTo("SFX", "HitsoundsPath", "/path/to/hitsounds/folder");
            if (hitsoundsPath == "/path/to/hitsounds/folder")
            {
                LogWarning("This is your first time running the mod (or update 1.1.0). Go to Documents/SpeenMods/SpeenPhoneConfig.ini to change the hitsounds folder path");
                LogWarning("More informations on the GitHub page: https://github.com/SRXDModdingGroup/SpeenPhone#hitsound-folder-info");
                return;
            }
            if (!Directory.Exists(hitsoundsPath))
            {
                LogError("The folder specified in the configuration file does not exist or is inaccessible.");
                return;
            }

            if (Directory.Exists(Path.Combine(hitsoundsPath, "Fail")))
            {
                List<AudioClip> fail = new List<AudioClip>();
                foreach (string path in Directory.EnumerateFiles(Path.Combine(hitsoundsPath, "Fail")))
                {
                    fail.Add(LoadAudio(path));
                }
                AudioPatches.DeathSounds = fail.ToArray();
            }
            if (Directory.Exists(Path.Combine(hitsoundsPath, "Win")))
            {
                List<AudioClip> win = new List<AudioClip>();
                foreach (string path in Directory.EnumerateFiles(Path.Combine(hitsoundsPath, "Win")))
                {
                    win.Add(LoadAudio(path));
                }
                AudioPatches.WinSounds = win.ToArray();
            }
            if (Directory.Exists(Path.Combine(hitsoundsPath, "Miss")))
            {
                List<AudioClip> miss = new List<AudioClip>();
                foreach (string path in Directory.EnumerateFiles(Path.Combine(hitsoundsPath, "Miss")))
                {
                    miss.Add(LoadAudio(path));
                }
                AudioPatches.MissSounds = miss.ToArray();
            }
            if (Directory.Exists(Path.Combine(hitsoundsPath, "Hit")))
            {
                List<AudioClip> hit = new List<AudioClip>();
                foreach (string path in Directory.EnumerateFiles(Path.Combine(hitsoundsPath, "Hit")))
                {
                    hit.Add(LoadAudio(path));
                }
                AudioPatches.HitSounds = hit.ToArray();
            }
        }

        public static AudioClip LoadAudio(string path)
        {
#pragma warning disable 0618
            using (WWW www = new WWW(BepInEx.Utility.ConvertToWWWFormat(path)))
#pragma warning restore 0618
            {
                AudioClip clip = null;
                try
                {
                    clip = www.GetAudioClip();
                    while (clip.loadState != AudioDataLoadState.Loaded) { }
                    Main.LogInfo($"Loaded audio file at {path}");
                }
                catch
                {
                    Main.LogError($"Error while loading {path}");
                    Main.LogError(www.error);
                }
                return clip;
            }
        }

        public static void Log(LogLevel level, object msg) => logger.Log(level, msg);
        public static void LogDebug(object msg) => Log(LogLevel.Debug, msg);
        public static void LogInfo(object msg) => Log(LogLevel.Info, msg);
        public static void LogMessage(object msg) => Log(LogLevel.Message, msg);
        public static void LogWarning(object msg) => Log(LogLevel.Warning, msg);
        public static void LogError(object msg) => Log(LogLevel.Error, msg);
    }
}
