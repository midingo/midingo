using System.Linq;
using Melanchall.DryWetMidi.Core;
using Melanchall.DryWetMidi.Multimedia;
using UnityEngine;

public class MIDIHandler : MonoBehaviour {
    private InputDevice _midiDevice;

    void Start() {
        // List available MIDI devices
        foreach (var midiDevice in InputDevice.GetAll()) {
            Debug.Log($"Midi device found: {midiDevice.Name}");
        }

        // Selects default
        _midiDevice = InputDevice.GetAll().FirstOrDefault();
        if (_midiDevice == null) {
            Debug.LogError("Can't recognize any MIDI devices!");
            return;
        }

        // Subscribe to MIDI events
        _midiDevice.EventReceived += OnMidiEvent;
        _midiDevice.StartEventsListening();
    }

    private void OnMidiEvent(object sender, MidiEventReceivedEventArgs e) {
        // This runs whenever a MIDI event arrives
        var midiEvent = e.Event;

        if (midiEvent is NoteOnEvent noteOn) {
            Debug.Log($"NOTE ON: {noteOn.NoteNumber} velocity {noteOn.Velocity}");
        } else if (midiEvent is NoteOffEvent noteOff) {
            Debug.Log($"NOTE OFF: {noteOff.NoteNumber}");
        }
    }

    void OnDestroy() {
        if (_midiDevice != null) {
            _midiDevice.EventReceived -= OnMidiEvent;
            _midiDevice.StopEventsListening();
            _midiDevice.Dispose();
        }
    }
}