using MidiControllerPatch.Behaviours;
using UnityEngine;
using FluidMidi;
using UnityEngine.Audio;
using System.Linq;

namespace MidiControllerPatch.Patches
{
    internal class PlayerControllerBPatch
    {
        public static void Init()
        {
            On.GameNetcodeStuff.PlayerControllerB.Awake +=
                (On.GameNetcodeStuff.PlayerControllerB.orig_Awake orig, GameNetcodeStuff.PlayerControllerB self) =>
            {
                orig(self);
                GameObject midiObject = new GameObject("MidiController");
                midiObject.transform.SetParent(self.gameObject.transform, false);
                Synthesizer synthesizer = midiObject.AddComponent<Synthesizer>();
                midiObject.AddComponent<NoteCallback>();
                AudioSource audioSource = midiObject.AddComponent<AudioSource>();
                audioSource.loop = false;
                audioSource.outputAudioMixerGroup = Plugin.Instance.midiMixer.FindMatchingGroups("Master").FirstOrDefault();
                audioSource.spatialBlend = 1;
            };
        }
    }
}
