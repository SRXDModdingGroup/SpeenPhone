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

        private static Dictionary<string, string> mappings;
        private static Dictionary<string, AudioClip[]> newClips;

        private void Awake()
        {
            logger = Logger;

            if (!Directory.Exists(ConfigPath))
                Directory.CreateDirectory(ConfigPath);
            if (!File.Exists(ConfigFileName))
                File.Create(ConfigFileName).Close();
            Config = new IniFile(ConfigFileName);

            GetMappings();
            Harmony harmony = new Harmony(PluginInfo.GUID);
            harmony.PatchAll<AudioPatches>();
        }

        public static bool TryGetClips(string name, out AudioClip[] clips) {
            if (mappings.TryGetValue(name, out string mapping) && newClips.TryGetValue(mapping, out clips))
                return true;

            clips = null;

            return false;
        }

        private static void GetMappings()
        {
            string soundsPath = Config.GetValueOrDefaultTo("SFX", "HitsoundsPath", string.Empty);

            if (string.IsNullOrWhiteSpace(soundsPath))
            {
                LogWarning("This is your first time running the mod (or update 1.1.0). Go to Documents/SpeenMods/SpeenPhoneConfig.ini to change the hitsounds folder path");
                LogWarning("More informations on the GitHub page: https://github.com/SRXDModdingGroup/SpeenPhone#hitsound-folder-info");

                return;
            }
            
            if (!Directory.Exists(soundsPath))
            {
                LogError("The folder specified in the configuration file does not exist or is inaccessible.");
            }

            mappings = new Dictionary<string, string>();

            using (var reader = new StreamReader(Path.Combine(soundsPath, "Mappings.txt"))) {
                while (!reader.EndOfStream) {
                    string line = reader.ReadLine();

                    if (string.IsNullOrWhiteSpace(line))
                        continue;
                
                    string[] split = line.Split('=');
                    
                    if (split.Length < 2)
                        continue;
                    
                    mappings.Add(split[0].Trim(), split[1].Trim());
                }
            }

            newClips = new Dictionary<string, AudioClip[]>();

            foreach (string directory in Directory.EnumerateDirectories(soundsPath)) {
                string name = Path.GetFileName(directory);
                
                if (string.IsNullOrWhiteSpace(name) || !mappings.ContainsValue(name) || !TryLoadClips(directory, out var clips)) {
                    LogWarning($"Could not get clips for {name}");
                    
                    continue;
                }
                
                newClips.Add(name, clips);
            }
        }

        private static bool TryLoadClips(string directory, out AudioClip[] clips) {
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
                    
                    LogInfo($"Loaded audio file at {path}");
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
