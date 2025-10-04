using System.Collections.Generic;
using Melanchall.DryWetMidi.Interaction;
using UnityEngine;

/**
 * This class spawns notes in a scene, determined by MIDI note numbers
 */
public class NoteSpawner : MonoBehaviour {
    public Track track;

    private List<ScheduledNote> _notes = new List<ScheduledNote>();
    private int _nextNoteIndex;
    
    /// <summary>
    /// Stores information about a falling note
    /// </summary>
    struct ScheduledNote {
        public float SpawnTime; // time this note should spawn (relative to song start)
        public float HitTime; // When this note should hit the keys (relative to song start)
        public int NoteID; // MIDI note number
        public float NoteDuration;
    }
    
    void Start() {
        var chart = track.Chart;
        var tempoMap = track.Tempo;

        // Read file for notes
        foreach (var note in chart.GetNotes()) {
            var hitTime = (float)TimeConverter.ConvertTo<MetricTimeSpan>(note.Time, tempoMap).TotalSeconds; 
            float spawnTime = hitTime - track.approachTime; // Offset note spawning to give time to scroll down
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
    }

    private void Update() {
        while (_nextNoteIndex < _notes.Count && _notes[_nextNoteIndex].SpawnTime <= Track.ElapsedTime) {
            int targetNote = _notes[_nextNoteIndex].NoteID;

            if (Track.KeyPositions.TryGetValue(targetNote, out Transform keyTransform)) {
                Vector3 keyOffset = new Vector3(0f, 3.43f, 0f);
                Vector3 targetPos = keyTransform.position + keyOffset;
                Vector3 noteOffset = new Vector3(0f, 125f, 0f);
                Vector3 spawnPos = targetPos + noteOffset;

                GameObject noteObject = Instantiate(track.notePrefab, spawnPos, Quaternion.identity);
                FallingNote fn = noteObject.GetComponent<FallingNote>();

                if (fn) {
                    double spawnDspTime = _notes[_nextNoteIndex].SpawnTime + Track.StartTime;
                    double hitDspTime = _notes[_nextNoteIndex].HitTime + Track.StartTime;
                    fn.Init(spawnPos, targetPos, spawnDspTime, hitDspTime, _notes[_nextNoteIndex].NoteDuration);
                }
            } else {
                Debug.LogWarning($"Can't find note: {_notes[_nextNoteIndex].NoteID}");
            }
            
            _nextNoteIndex++;
        }
    }
}