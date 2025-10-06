using System.Collections.Generic;
using Melanchall.DryWetMidi.Core;
using Melanchall.DryWetMidi.Interaction;
using UnityEngine;

/// <summary>
/// Handles setup for the note highway.
/// </summary>
public class Track : MonoBehaviour {
    // Gameplay Data //
    /// <summary>
    /// The time a note takes to hit the keys in seconds.
    /// This effects how fast notes go down the highway.
    /// </summary>
    public float approachTime = 15;

    public Material[] noteColors;
    public GameObject notePrefabNatural;
    public GameObject notePrefabAccidental;
    
    /// <summary>
    /// Maps MIDI note number to a key's coordinates.
    /// </summary>
    /// 
    /// <remarks>
    ///  Doing so prevents unnecessary GetComponent() calls, improving performance. 
    /// </remarks>
    public static Dictionary<int, Transform> KeyPositions = new Dictionary<int, Transform>();
    public static double StartTime;
    public static double ElapsedTime;
    
    // MIDI data //
    public string midiFilePath;
    public MidiFile Chart;
    public TempoMap Tempo;
    
    // Audio data //
    public AudioSource songAudio;
    
    private void Start() {
        // Build dictionary from in game keys
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

        
        // Reading MIDI file
        Chart = MidiFile.Read(midiFilePath);
        Tempo = Chart.GetTempoMap();
        
        // Start spawning notes
        NoteSpawner ns = keyboard.gameObject.AddComponent<NoteSpawner>();
        ns.track = this;
        
        
        // Start audio playback
        StartTime = AudioSettings.dspTime + 1;
        songAudio.PlayScheduled(StartTime);
    }

    private void Update() {
        ElapsedTime = AudioSettings.dspTime - StartTime;
    }
}