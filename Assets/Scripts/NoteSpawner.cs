using System;
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
    public Transform rootSpawnPoint;
    public AudioSource backingTrack;
    public float approachTime = 3;

    private List<ScheduledNote> _notes = new List<ScheduledNote>();
    private int _nextNoteIndx = 0;
    private double _gameStartTime; // Track when the game actually started
    
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
        // SETTING UP MIDI FILE //
        // Load midi file
        var mf = MidiFile.Read(midiFilePath);
        var tempoMap = mf.GetTempoMap();

        // Read file for notes
        foreach (var note in mf.GetNotes()) {
            var hitTime = (float)TimeConverter.ConvertTo<MetricTimeSpan>(note.Time, tempoMap).TotalSeconds;
            float spawnTime = hitTime - approachTime;
            spawnTime = Mathf.Max(spawnTime, 0f); // No negative spawn times
            
            _notes.Add(new ScheduledNote {
                HitTime = hitTime,
                SpawnTime = spawnTime,
                NoteID = note.NoteNumber
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
            Vector3 noteOffset = new Vector3(0f + _notes[_nextNoteIndx].NoteID, 0f, 0f);
            Vector3 spawnPos = rootSpawnPoint.position + noteOffset;
            Instantiate(notePrefab, spawnPos, Quaternion.identity);
            _nextNoteIndx++;
        }
    }
}