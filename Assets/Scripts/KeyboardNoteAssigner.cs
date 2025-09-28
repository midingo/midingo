using System.Collections.Generic;
using UnityEngine;

public class KeyboardNoteAssigner : MonoBehaviour {
    // Semitone offsets from C
    private Dictionary<string, int> _noteOffsets = new Dictionary<string, int>() {
        { "C", 0 }, { "C#", 1 }, { "D", 2 }, { "D#", 3 },
        { "E", 4 }, { "F", 5 }, { "F#", 6 }, { "G", 7 },
        { "G#", 8 }, { "A", 9 }, { "A#", 10 }, { "B", 11 }
    };
    
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
    }
}
