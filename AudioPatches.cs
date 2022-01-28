using HarmonyLib;
using System.Collections.Generic;
using UnityEngine;

namespace SpeenPhone
{
    public class AudioPatches
    {
        public static AudioClip[] HitSounds;
        public static AudioClip[] MissSounds;
        public static AudioClip[] DeathSounds;
        public static AudioClip[] WinSounds;

        private static bool disableHitsounds = false;

        [HarmonyPatch(typeof(SoundEffectPlayer), nameof(SoundEffectPlayer.PlayOneShot))]
        [HarmonyPrefix]
        private static void Hitsound(ref SoundEffect soundEffect)
        {
            if (disableHitsounds) return;
            var sfe = SoundEffectAssets.Instance;
            if (DeathSounds != null && DeathSounds.Length > 0 && soundEffect.clips == sfe.trackFailedSound.clips)
            {
                
                soundEffect.clips = DeathSounds;
                return;
            }
            if (WinSounds != null && WinSounds.Length > 0 && soundEffect.clips == sfe.trackCompleteSound.clips)
            {
                soundEffect.clips = WinSounds;
                return;
            }
            if (MissSounds != null && MissSounds.Length > 0 && soundEffect.clips == sfe.loseHealthSound.clips)
            {
                soundEffect.clips = MissSounds;
                return;
            }
        }

        [HarmonyPatch(typeof(NoteSoundPlayer), nameof(NoteSoundPlayer.GetSoundEffectForNoteSoundType))]
        [HarmonyPostfix]
        private static void PlayNote(NoteSoundPlayer.NoteSoundType noteSoundType, SoundEffectAssets soundEffectAssets, ref SoundEffect __result)
        {
            if (disableHitsounds) return;
            if (HitSounds == null || HitSounds.Length == 0) return;
            __result.clips = HitSounds;
        }

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
    }
}
