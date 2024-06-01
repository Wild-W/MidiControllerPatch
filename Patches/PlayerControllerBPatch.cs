using MidiControllerPatch.Behaviours;
using UnityEngine;
using FluidMidi;
using UnityEngine.Audio;
using System.Linq;

namespace MidiControllerPatch.Patches
{
    internal class PlayerControllerBPatch
    {
        public static void Init(AudioMixer mixer)
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
                audioSource.outputAudioMixerGroup = mixer.FindMatchingGroups("Master").FirstOrDefault();
                audioSource.spatialBlend = 1;
            };
        }
    }
}
