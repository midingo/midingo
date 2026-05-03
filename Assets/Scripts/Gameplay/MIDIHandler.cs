using System;
using System.Linq;
using Melanchall.DryWetMidi.Core;
using Melanchall.DryWetMidi.Multimedia;
using UnityEngine;

public class MIDIHandler : MonoBehaviour {
    private InputDevice _midiDevice;
    private Color _originalColor;

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

            if (Track.KeyComponents.TryGetValue(noteOn.NoteNumber, out GameObject keyObject)) {
                Renderer r = keyObject.GetComponent<Renderer>();
                
                if (r != null) {
                    _originalColor = r.material.color;
                    r.material.color = Color.forestGreen;
                } else {
                    Debug.LogError($"Renderer component can't be found for {noteOn.NoteNumber}");
                }
            } else {
                Debug.LogError($"GameObject can't be found for {noteOn.NoteNumber}");
            }} else if (midiEvent is NoteOffEvent noteOff) {
            Debug.Log($"NOTE OFF: {noteOff.NoteNumber}");

            if (Track.KeyComponents.TryGetValue(noteOff.NoteNumber, out GameObject keyObject)) {
                Renderer r = keyObject.GetComponent<Renderer>();

                if (r != null) {
                    r.material.color = _originalColor;
                } else {
                    Debug.LogError($"Renderer component can't be found for {noteOff.NoteNumber}");
                }
            } else {
                Debug.LogError($"GameObject can't be found for {noteOff.NoteNumber}");
            }
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