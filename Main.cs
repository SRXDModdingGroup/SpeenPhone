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
        
        public static string SoundsPath { get; private set; }

        private void Awake()
        {
            logger = Logger;

            if (!Directory.Exists(ConfigPath))
                Directory.CreateDirectory(ConfigPath);
            if (!File.Exists(ConfigFileName))
                File.Create(ConfigFileName).Close();
            Config = new IniFile(ConfigFileName);

            GetSoundsPath();
            Harmony harmony = new Harmony(PluginInfo.GUID);
            harmony.PatchAll<AudioPatches>();
        }

        private static void GetSoundsPath()
        {
            SoundsPath = Config.GetValueOrDefaultTo("SFX", "HitsoundsPath", string.Empty);
            
            if (string.IsNullOrWhiteSpace(SoundsPath))
            {
                LogWarning("This is your first time running the mod (or update 1.1.0). Go to Documents/SpeenMods/SpeenPhoneConfig.ini to change the hitsounds folder path");
                LogWarning("More informations on the GitHub page: https://github.com/SRXDModdingGroup/SpeenPhone#hitsound-folder-info");

                return;
            }
            
            if (!Directory.Exists(SoundsPath))
            {
                LogError("The folder specified in the configuration file does not exist or is inaccessible.");
            }
        }

        public static bool TryLoadClips(string directory, out AudioClip[] clips) {
            directory = Path.Combine(SoundsPath, directory);
            
            if (!Directory.Exists(directory)) {
                clips = null;
                
                return false;
            }

            var clipsList = new List<AudioClip>();

            foreach (string path in Directory.EnumerateFiles(directory)) {
                if (TryLoadClip(path, out var clip))
                    clipsList.Add(clip);
            }

            if (clipsList.Count == 0) {
                clips = null;

                return false;
            }

            clips = clipsList.ToArray();

            return true;
        }

        private static bool TryLoadClip(string path, out AudioClip clip) {
            clip = null;

            if (!File.Exists(path))
                return false;
            
#pragma warning disable 0618
            using (WWW www = new WWW(BepInEx.Utility.ConvertToWWWFormat(path)))
#pragma warning restore 0618
            {
                try
                {
                    clip = www.GetAudioClip();
                    
                    while (clip.loadState == AudioDataLoadState.Loading) { }

                    if (clip.loadState == AudioDataLoadState.Loaded)
                        LogInfo($"Loaded audio file at {path}");
                    else {
                        clip = null;
                        LogInfo($"Failed to load audio file at {path}");
                    }
                    
                }
                catch
                {
                    LogError($"Error while loading {path}");
                    LogError(www.error);
                }
                
                return clip != null;
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
