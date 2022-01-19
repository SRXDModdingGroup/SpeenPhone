using HarmonyLib;
using UnityEngine;

namespace SpeenPhone
{
    internal class AudioPatches
    {
        private static AudioClip clip;

        public static bool LoadAudio(string path)
        {
#pragma warning disable 0618
            using (WWW www = new WWW(BepInEx.Utility.ConvertToWWWFormat(path)))
#pragma warning restore 0618
            {
                try
                {
                    clip = www.GetAudioClip();
                    while (clip.loadState != AudioDataLoadState.Loaded) { }
                }
                catch
                {
                    Main.LogError($"Error while loading {path}");
                    Main.LogError(www.error);
                }
                Main.LogInfo($"Loaded audio file at {path}");
                return true;
            }
        }

        [HarmonyPatch(typeof(NoteSoundPlayer), nameof(NoteSoundPlayer.GetSoundEffectForNoteSoundType))]
        [HarmonyPostfix]
        private static void PlayNote(NoteSoundPlayer.NoteSoundType noteSoundType, SoundEffectAssets soundEffectAssets, ref SoundEffect __result)
        {
            for (int i = 0; i < __result.clips.Length; i++)
            {
                __result.clips[i] = clip;
            }
        }
    }
}
