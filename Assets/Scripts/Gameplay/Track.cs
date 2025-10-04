using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// This script sets up the note highway / track.
/// </summary>
public class Track : MonoBehaviour {
    public string midiFile;
    public AudioSource songAudio;

    /// <summary>
    /// The time a note takes to hit the keys in seconds.
    /// This effects how fast notes go down the highway.
    /// </summary>
    public float approachTime = 15;

    /// <summary>
    /// Maps MIDI note number to a key's coordinates.
    /// </summary>
    /// 
    /// <remarks>
    ///  Doing so prevents unnecessary GetComponent() calls, improving performance. 
    /// </remarks>
    public static Dictionary<int, Transform> KeyPositions = new Dictionary<int, Transform>();

    private void Start() {
        // Build dictionary from in game keys //
        Transform keyboard = transform.Find("Keyboard");
        if (keyboard == null) {
            Debug.LogError("No keyboard object found under NoteTrack.");
        }

        foreach (Transform octave in keyboard) {
            foreach (Transform key in octave) {
                if (int.TryParse(key.name, out int midiNote)) {
                    KeyPositions[midiNote] = key.transform;
                }
            }
        }
    }
}