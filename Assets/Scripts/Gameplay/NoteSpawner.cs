using System.Collections.Generic;
using Melanchall.DryWetMidi.Interaction;
using UnityEngine;

/**
 * This class spawns notes in a scene, determined by MIDI note numbers
 */
public class NoteSpawner : MonoBehaviour {
    public Track track;

    public List<ScheduledNote> NoteQueue = new List<ScheduledNote>();
    public int nextNoteIndex;
    private Vector3 noteOffset = new Vector3(0f, 125f, 0f);

    /// <summary>
    /// Stores information about a falling note
    /// </summary>
    public struct ScheduledNote {
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

            NoteQueue.Add(new ScheduledNote {
                HitTime = hitTime,
                SpawnTime = spawnTime,
                NoteID = note.NoteNumber,
                NoteDuration = duration
            });
        }

        // Sort notes by spawn time
        NoteQueue.Sort((a, b) => a.SpawnTime.CompareTo(b.SpawnTime));
    }

    private void Update() {
        while (nextNoteIndex < NoteQueue.Count && NoteQueue[nextNoteIndex].SpawnTime <= Track.ElapsedTime) {
            int targetNote = NoteQueue[nextNoteIndex].NoteID;

            if (Track.KeyComponents.TryGetValue(targetNote, out GameObject keyObject)) {
                Transform keyTransform = keyObject.transform;
                GameObject noteObject;
                Vector3 targetPos;
                Vector3 spawnPos;

                if (KeyboardNoteAssigner.IsAccidental(targetNote)) {
                    targetPos = new Vector3(
                        keyTransform.position.x,
                        KeyboardNoteAssigner.AccidentalTop.y + 3.5f,
                        keyTransform.position.z
                    );
                    spawnPos = targetPos + noteOffset;
                    noteObject = Instantiate(track.notePrefabAccidental, spawnPos, Quaternion.Euler(0, 90, 0));
                } else {
                    targetPos = new Vector3(
                        keyTransform.position.x,
                        KeyboardNoteAssigner.NaturalTop.y + 3.5f,
                        keyTransform.position.z
                    );
                    spawnPos = targetPos + noteOffset;
                    noteObject = Instantiate(track.notePrefabNatural, spawnPos, Quaternion.Euler(0, 90, 0));
                }

                FallingNote fn = noteObject.GetComponent<FallingNote>();

                if (fn) {
                    // Spawn note
                    double spawnDspTime = NoteQueue[nextNoteIndex].SpawnTime + Track.StartTime;
                    double hitDspTime = NoteQueue[nextNoteIndex].HitTime + Track.StartTime;
                    fn.Init(spawnPos, targetPos, 
                        spawnDspTime, hitDspTime, NoteQueue[nextNoteIndex].NoteDuration, 
                        NoteQueue[nextNoteIndex].NoteID,
                        track);
                }
            } else {
                Debug.LogWarning($"Can't find note: {NoteQueue[nextNoteIndex].NoteID}");
            }

            nextNoteIndex++;
        }
    }
}