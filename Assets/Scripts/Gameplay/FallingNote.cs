using System;
using UnityEngine;

public class FallingNote : MonoBehaviour {
    private Vector3 _spawnPos;
    private Vector3 _targetPos;

    private double _spawnTime;
    private double _hitTime;
    private double _duration;
    private bool _initialized;

    public void Init(Vector3 spawnPosition, Vector3 targetPosition, double spawnTime, double hitTime, double duration, int midiNoteNumber, Track track) {
        _spawnPos = spawnPosition;
        _targetPos = targetPosition;
        _spawnTime = spawnTime;
        _hitTime = hitTime;
        _duration = duration;
        _initialized = true;
        
        // Replace material for all instances of GlossyWhite (placeholder material)
        Material color = GetNoteColor(track.noteColors, midiNoteNumber);
        
        Renderer[] renderers = GetComponentsInChildren<Renderer>();
        foreach (Renderer r in renderers) {
            if (r.material != null && r.material.name.StartsWith("GlossyWhite")) {
                r.material = color;
            }
        }
    }
    
    private static Material GetNoteColor(Material[] colors, int midiNoteNumber) {
        
        if (KeyboardNoteAssigner.IsAccidental(midiNoteNumber)) { // TODO: Handle accidental colors better
            return colors[0];
        }
        
        int semitone = midiNoteNumber % 12;
        // Match up index
        int matIndex;
        switch (semitone) {
            case 0: matIndex = 0; break; // C
            case 2: matIndex = 1; break; // D
            case 4: matIndex = 2; break; // E
            case 5: matIndex = 3; break; // F
            case 7: matIndex = 4; break; // G
            case 9: matIndex = 5; break; // A
            case 11: matIndex = 6; break; // B
            default:
                Debug.LogWarning("Invalid note semitone: " + semitone);
                matIndex = 0;
                break;
        }
        
        return colors[matIndex];
    }

    private void Update() {
        if (!_initialized) return;

        double currTime = AudioSettings.dspTime;
        
        // Normalized progress between spawn and hit
        double t = (currTime - _spawnTime) / (_hitTime - _spawnTime);
        t = Mathf.Clamp01((float)t);
        
        // Moves note
        transform.position = Vector3.Lerp(_spawnPos, _targetPos, (float)t);
    }
}
