using HarmonyLib;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

namespace SpeenPhone
{
    public class AudioPatches
    {
        private static bool enableCustomSounds = true;

        private static List<FieldInfo> fields;
        private static Dictionary<string, SoundEffect> defaultSoundEffects;
        private static Dictionary<string, SoundEffect> customSoundEffects;

        private static void SetClips() {
            var instance = SoundEffectAssets.Instance;
            
            foreach (var field in fields) {
                string name = field.Name;
                SoundEffect soundEffect;

                if (enableCustomSounds)
                    soundEffect = customSoundEffects[name];
                else
                    soundEffect = defaultSoundEffects[name];
                
                field.SetValue(instance, soundEffect);
            }
        }

        [HarmonyPatch(typeof(Game), nameof(Game.Update)), HarmonyPostfix]
        private static void Game_Update_Postfix()
        {
            if (Input.GetKeyDown(KeyCode.F12))
            {
                enableCustomSounds = !enableCustomSounds;
                NotificationSystemGUI.AddMessage("Custom sounds are " + (enableCustomSounds ? "ON" : "OFF"));
                SetClips();
            }
        }

        [HarmonyPatch(typeof(SoundEffectAssets), "CreateSoundEffectMappings"), HarmonyPrefix]
        private static void SoundEffectAssets_CreateSoundEffectMappings_Prefix(SoundEffectAssets __instance, Dictionary<string, SoundEffect> ____soundEffectMapping) {
            fields = new List<FieldInfo>();
            defaultSoundEffects = new Dictionary<string, SoundEffect>();
            customSoundEffects = new Dictionary<string, SoundEffect>();
            
            foreach (var field in typeof(SoundEffectAssets).GetFields(BindingFlags.Instance | BindingFlags.Public)) {
                string name = field.Name;
                    
                if (field.FieldType != typeof(SoundEffect) || !Main.TryGetClips(name, out var clips))
                    continue;
                
                var soundEffect = (SoundEffect) field.GetValue(__instance);

                fields.Add(field);
                defaultSoundEffects.Add(name, soundEffect);
                soundEffect.clips = clips;
                customSoundEffects.Add(name, soundEffect);
            }
            
            SetClips();
        }
    }
}
