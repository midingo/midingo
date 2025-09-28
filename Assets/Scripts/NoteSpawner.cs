using System.Collections.Generic;
using Melanchall.DryWetMidi.Core;
using Melanchall.DryWetMidi.Interaction;
using UnityEngine;

/**
 * This class spawns notes in a scene, determined by MIDI note numbers
 */
public class NoteSpawner : MonoBehaviour {
    public string midiFilePath;
    public GameObject notePrefab;
    public AudioSource backingTrack;
    public float approachTime = 3;

    private List<ScheduledNote> _notes = new List<ScheduledNote>();
    private int _nextNoteIndx;
    private double _gameStartTime; // Track when the game actually started
    
    // MIDI note -> note coords
    private Dictionary<int, Transform> _keyLookup = new Dictionary<int, Transform>();
    
    /**
     * This class stores relevant information for spawning notes
     */
    public class ScheduledNote {
        public float SpawnTime; // time this note should spawn (relative to song start)
        public float HitTime; // when this note is to be hit (relative to song start)
        public int NoteID; // MIDI note number
        public float NoteDuration;
    }
    
    void Start() {
        // Build dictionary //
        foreach (Transform octave in transform) {
            foreach (Transform key in octave) {
                if (int.TryParse(key.name, out int midiNote)) {
                    _keyLookup[midiNote] = key.transform;
                }
            }
        }
        
        // SETTING UP MIDI FILE //
        // Load midi file
        var mf = MidiFile.Read(midiFilePath);
        var tempoMap = mf.GetTempoMap();

        // Read file for notes
        foreach (var note in mf.GetNotes()) {
            var hitTime = (float)TimeConverter.ConvertTo<MetricTimeSpan>(note.Time, tempoMap).TotalSeconds; // When this note should hit the keys
            float spawnTime = hitTime - approachTime; // Offset note spawning to give time to scroll down
            spawnTime = Mathf.Max(spawnTime, 0f); // No negative spawn times
            float duration = (float)TimeConverter.ConvertTo<MetricTimeSpan>(note.Length, tempoMap).TotalSeconds;
            
            _notes.Add(new ScheduledNote {
                HitTime = hitTime,
                SpawnTime = spawnTime,
                NoteID = note.NoteNumber,
                NoteDuration = duration
            });
        }
        
        // Sort notes by spawn time
        _notes.Sort((a, b) => a.SpawnTime.CompareTo(b.SpawnTime));
        
        // Schedule playback with delay
        _gameStartTime = AudioSettings.dspTime + .5;
        backingTrack.PlayScheduled(_gameStartTime);
    }

    private void Update() {
        // Calculate elapsed time since the game started
        double elapsedTime = AudioSettings.dspTime - _gameStartTime;
        
        // Only start spawning after the game has actually started
        if (elapsedTime < 0) return;

        while (_nextNoteIndx < _notes.Count && _notes[_nextNoteIndx].SpawnTime <= elapsedTime) {
            int targetNote = _notes[_nextNoteIndx].NoteID;

            if (_keyLookup.TryGetValue(targetNote, out Transform keyTransform)) {
                Vector3 noteOffset = new Vector3(0f, 0f, 100f);
                Vector3 spawnPos = keyTransform.position + noteOffset;

                GameObject noteObject = Instantiate(notePrefab, spawnPos, Quaternion.identity);
                FallingNote fn = noteObject.GetComponent<FallingNote>();

                if (fn) {
                    double spawnDSP = _notes[_nextNoteIndx].SpawnTime + _gameStartTime;
                    double hitDSP = _notes[_nextNoteIndx].HitTime + _gameStartTime;
                    fn.Init(spawnPos, keyTransform.position, spawnDSP, hitDSP);
                }

            } else {
                Debug.LogWarning($"Can't find note: {_notes[_nextNoteIndx].NoteID}");
            }
            
            _nextNoteIndx++;
        }
    }
}