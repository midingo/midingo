using System;
using System.Collections.Generic;
using System.Linq;
using Melanchall.DryWetMidi.Common;
using Melanchall.DryWetMidi.Core;
using Melanchall.DryWetMidi.Multimedia;
using UnityEngine;

public class ReadMIDITest : MonoBehaviour {
    public string midiFilePath;
    public GameObject notePrefab;
    public Transform spawnPoint;

    private Playback _pb;
    private Queue<SevenBitNumber> _toSpawn = new Queue<SevenBitNumber>();

    void Start() {
        // init a virtual output
        var output = OutputDevice.GetAll().FirstOrDefault();

        // Load in file
        var mapdata = MidiFile.Read(midiFilePath, new ReadingSettings() {
                UnknownChunkIdPolicy = UnknownChunkIdPolicy.Abort
            }
        );

        // Create playback
        _pb = mapdata.GetPlayback(output);

        // Subscribe to note start
        _pb.NotesPlaybackStarted += OnNoteStarted;

        // Start playback
        _pb.Start();
    }

    private void Update() {
        while (_toSpawn.Count > 0) {
            var note = _toSpawn.Dequeue();
            Instantiate(notePrefab, spawnPoint.position, Quaternion.identity);
        }
    }

    private void OnNoteStarted(object sender, NotesEventArgs e) {
        foreach (var note in e.Notes) {
            Debug.Log($"Note: {note.NoteName} / #{note.NoteNumber} started @ {note.Time}");

            _toSpawn.Enqueue(note.NoteNumber);
        }
    }

    // cleanup after exit
    void OnDestroy() {
        if (_pb != null) {
            _pb.NotesPlaybackStarted -= OnNoteStarted;
            _pb.Dispose();
            _pb = null;
        }
    }
}