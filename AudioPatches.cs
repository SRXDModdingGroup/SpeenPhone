using HarmonyLib;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

namespace SpeenPhone
{
    public class AudioPatches
    {
        private static bool disableHitsounds = false;

        [HarmonyPatch(typeof(Track), nameof(Track.Update))]
        [HarmonyPostfix]
        private static void ToggleHitsounds()
        {
            if (Input.GetKeyDown(KeyCode.F12))
            {
                disableHitsounds = !disableHitsounds;
                NotificationSystemGUI.AddMessage("Custom hitsounds are " + (disableHitsounds ? "OFF" : "ON"));
            }
        }

        [HarmonyPatch(typeof(SoundEffectAssets), "CreateSoundEffectMappings"), HarmonyPrefix]
        private static void SoundEffectAssets_CreateSoundEffectMappings_Prefix(SoundEffectAssets __instance, Dictionary<string, SoundEffect> ____soundEffectMapping) {
            foreach (var field in typeof(SoundEffectAssets).GetFields(BindingFlags.Instance | BindingFlags.Public)) {
                if (field.FieldType != typeof(SoundEffect) || !Main.TryGetClips(field.Name, out var clips))
                    continue;

                var soundEffect = (SoundEffect) field.GetValue(__instance);
                    
                soundEffect.clips = clips;
                field.SetValue(__instance, soundEffect);
            }
        }
    }
}
