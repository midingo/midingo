using System;
using System.Collections;
using Melanchall.DryWetMidi.Interaction;
using UnityEngine;

public class Metronome : MonoBehaviour {
    private bool _enabled;
    public TempoMap TMap;
    public double dspStartTime;


    /// <summary>
    /// Event fired every metronome tick.
    /// The DSP time of this tick will be sent
    /// </summary>
    public event Action<double> OnTick; 
    
    /// <summary>
    /// Number of ticks for each beat.
    /// 1 for quarter notes, 4 for sixteenth notes 
    /// </summary>
    public int ticksPerBeat = 1;

    private int _currentBeat;
    private double _beatInterval;
    private double _nextTick;

    
    public void StartMetronome(TempoMap tempoMap) {
        
    }
}
