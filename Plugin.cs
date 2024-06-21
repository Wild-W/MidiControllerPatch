using BepInEx;
using BepInEx.Logging;
using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
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
        public ManualLogSource logger;

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate void UnityInitNativePluginDelegate(IntPtr hModule);

        [DllImport("kernel32.dll", SetLastError = true)]
        static private extern IntPtr LoadLibrary(string lpFileName);

        public AudioMixer midiMixer;

        public const uint LOAD_LIBRARY_SEARCH_DEFAULT_DIRS = 0x00001000;

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

            string location = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            logger = BepInEx.Logging.Logger.CreateLogSource(modGUID);

            // Tell UnityPlayer to load the native library
            IntPtr unityPlayerAddress = GetModuleBaseAddress(Process.GetCurrentProcess(), "UnityPlayer.dll");
            IntPtr functionAddress = (IntPtr)((long)unityPlayerAddress + 0x5b3d50);
            var initNativePlugin = (UnityInitNativePluginDelegate)
                Marshal.GetDelegateForFunctionPointer(functionAddress, typeof(UnityInitNativePluginDelegate));

            IntPtr fluidSynth = LoadLibrary(Path.Combine(location, "audioplugin-fluidsynth-3.dll"));
            logger.LogInfo("fluidsynth = " + fluidSynth.ToString());
            initNativePlugin(fluidSynth);

            // Load the audio mixer
            AssetBundle mixerBundle = AssetBundle.LoadFromFile(Path.Combine(location, "midiaudiomixer.lethalbundle"));
            midiMixer = mixerBundle.LoadAsset<AudioMixer>("MidiDiagetic");
            mixerBundle.Unload(false);

            ApplyPatches();

            logger.LogInfo($"{modName}:{modVersion} successfully loaded.");
        }

        private void ApplyPatches()
        {
            Minis.MidiSystemWrangler.Initialize();
            Patches.PlayerControllerBPatch.Init();
        }

        private static IntPtr GetModuleBaseAddress(Process process, string moduleName)
        {
            foreach (ProcessModule module in process.Modules)
            {
                if (module.ModuleName.Equals(moduleName, StringComparison.OrdinalIgnoreCase))
                {
                    return module.BaseAddress;
                }
            }
            return IntPtr.Zero;
        }
    }
}
