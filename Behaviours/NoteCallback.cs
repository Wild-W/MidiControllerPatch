using UnityEngine.InputSystem;
using UnityEngine;
using FluidMidi;
using System.IO;
using System.Reflection;

namespace MidiControllerPatch.Behaviours
{
    public class NoteCallback : MonoBehaviour
    {
        Synthesizer synthesizer;

        void Start()
        {
            synthesizer = gameObject.GetComponent<Synthesizer>();
            synthesizer.soundFontPath = Path.Combine(Path.GetDirectoryName(
                Assembly.GetExecutingAssembly().Location), "Yamaha_C3_Grand_Piano.sf2");

            InputSystem.onDeviceChange += (device, change) =>
            {
                Debug.Log($"Device change: {device.description.product}, Type: {device.GetType()}");
                if (change != InputDeviceChange.Added) return;

                Minis.MidiDevice midiDevice = device as Minis.MidiDevice;
                if (midiDevice == null) return;

                midiDevice.onWillNoteOn += (note, velocity) => {
                    Debug.Log(string.Format(
                        "Note On #{0} ({1}) vel:{2:0.00} ch:{3} dev:'{4}'",
                        note.noteNumber,
                        note.shortDisplayName,
                        velocity,
                        (note.device as Minis.MidiDevice)?.channel,
                        note.device.description.product
                    ));

                    synthesizer.PlayNote(note.noteNumber, (int)(velocity * 100));
                };

                midiDevice.onWillNoteOff += (note) => {
                    Debug.Log(string.Format(
                        "Note Off #{0} ({1}) ch:{2} dev:'{3}'",
                        note.noteNumber,
                        note.shortDisplayName,
                        (note.device as Minis.MidiDevice)?.channel,
                        note.device.description.product
                    ));

                    synthesizer.PlayNote(note.noteNumber, 0);
                };
            };
        }
    }
}
