using System;
using UnityEngine;

public class FallingNote : MonoBehaviour {
    private Vector3 _spawnPos;
    private Vector3 _targetPos;

    private double _spawnTime;
    private double _hitTime;
    private bool _initialized;

    public void Init(Vector3 spawnPosition, Vector3 targetPosition, double spawnTime, double hitTime) {
        _spawnPos = spawnPosition;
        _targetPos = targetPosition;
        _spawnTime = spawnTime;
        _hitTime = hitTime;
        _initialized = true;
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
