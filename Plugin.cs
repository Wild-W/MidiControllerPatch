using BepInEx;
using BepInEx.Logging;
using System.IO;
using System.Reflection;
using UnityEngine;
using UnityEngine.Audio;

namespace MidiControllerPatch
{
    [BepInPlugin(modGUID, modName, modVersion)]
    public class Plugin : BaseUnityPlugin
    {
        internal const string modGUID = "wildw.midikeyboard";
        internal const string modName = "Midi Keyboard";
        internal const string modVersion = "1.0.0";

        internal static Plugin Instance;
        internal static ManualLogSource logger;

        AudioMixer midiMixer;

        void Awake ()
        {
            if (Instance == null)
            {
                Instance = this;
            }
            else
            {
                return;
            }

            

            logger = BepInEx.Logging.Logger.CreateLogSource(modGUID);

            AssetBundle mixerBundle = AssetBundle.LoadFromFile(
                Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location),
                "midiaudiomixer.lethalbundle"));
            midiMixer = mixerBundle.LoadAsset<AudioMixer>("MidiDiagetic");
            mixerBundle.Unload(false);

            ApplyPatches();

            logger.LogInfo($"{modName}:{modVersion} successfully loaded.");
        }

        private void ApplyPatches()
        {
            Minis.MidiSystemWrangler.Initialize();
            Patches.PlayerControllerBPatch.Init(midiMixer);
        }
    }
}
