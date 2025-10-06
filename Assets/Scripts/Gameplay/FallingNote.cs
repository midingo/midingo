using System;
using UnityEngine;

public class FallingNote : MonoBehaviour {
    private Vector3 _spawnPos;
    private Vector3 _targetPos;

    private double _spawnTime;
    private double _hitTime;
    private double _duration;
    private bool _initialized;

    public void Init(Vector3 spawnPosition, Vector3 targetPosition, double spawnTime, double hitTime, double duration, Material color) {
        _spawnPos = spawnPosition;
        _targetPos = targetPosition;
        _spawnTime = spawnTime;
        _hitTime = hitTime;
        _duration = duration;
        _initialized = true;
        
        // Replace material for all renderers in this prefab instance
        Renderer[] renderers = GetComponentsInChildren<Renderer>();
        foreach (Renderer r in renderers) {
            if (r.material != null && r.material.name.StartsWith("GlossyWhite")) {
                r.material = color;
            }
        }
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
