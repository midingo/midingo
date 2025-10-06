using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class KeyboardNoteAssigner : MonoBehaviour {
    // Top of keys
    public static Vector3 NaturalTop;
    public static Vector3 AccidentalTop;
    
    // Semitone offsets from C
    private Dictionary<string, int> _noteOffsets = new Dictionary<string, int>() {
        { "C", 0 }, { "C#", 1 }, { "D", 2 }, { "D#", 3 },
        { "E", 4 }, { "F", 5 }, { "F#", 6 }, { "G", 7 },
        { "G#", 8 }, { "A", 9 }, { "A#", 10 }, { "B", 11 }
    };
    
    /// <summary>
    /// Determines if given key is a natural or accidental
    /// </summary>
    /// <param name="midiNote">MIDI note #</param>
    /// <returns></returns>
    public static bool IsAccidental(int midiNote) {
        int noteInOctave = midiNote % 12;

        // Accidentals correspond to these semitone positions in the octave
        return noteInOctave == 1 || noteInOctave == 3 || noteInOctave == 6 || noteInOctave == 8 || noteInOctave == 10;
    }
    
    private void Start() {
        foreach (Transform octave in transform) {
            // Assumes that octaves are named like C0, C1, C2, etc...
            string octaveName = octave.name;
            int octaveNumber = int.Parse(octaveName.Substring(1));

            foreach (Transform key in octave) {
                if (_noteOffsets.TryGetValue(key.name, out int semitone)) { // valid semitone / note name
                    int midiName = ((octaveNumber + 2) * 12) + semitone;
                    key.name = midiName.ToString();
                }
                else {
                    Debug.LogWarning($"Key {key.name} isn't recognized in {octave.name}.");
                }
            }
        }
        
        // Fetches the top. This assumes that C3 exists!
        Transform sampleNatural = transform.Find("60"); // C3
        Transform sampleAccidental = transform.Find("61"); //C#3

        if (sampleNatural && sampleNatural.TryGetComponent<Renderer>(out var natRenderer)) {
            NaturalTop = natRenderer.bounds.max;
        }

        if (sampleAccidental && sampleAccidental.TryGetComponent<Renderer>(out var accRenderer)) {
            AccidentalTop = accRenderer.bounds.max;
        }
    }
}
