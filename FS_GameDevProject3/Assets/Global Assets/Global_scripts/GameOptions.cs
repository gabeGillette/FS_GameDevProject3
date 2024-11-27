/// Authors: Gabriel Gillette
/// Desc: Game Options Object

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]

public class GameOptions : ScriptableObject
{
    /*---------------------------------------------- SERIALIZED */

    [Tooltip("X-Axis Sensitivity (normalized)")]
    [SerializeField] [Range(0, 1)] float _xLookSens;

    [Tooltip("Y-Axis Sensitivity (normalized)")]
    [SerializeField] [Range(0, 1)] float _yLookSens;

    [Tooltip("Invert look")]
    [SerializeField] bool _invertAim;

    [Tooltip("Master Sound volume (normalized)")]
    [SerializeField] [Range(0, 1)] float _masterSoundVol;
    [Tooltip("Sound effects volume (normalized)")]
    [SerializeField] [Range(0, 1)] float _sfxVolume;
    [Tooltip("Music volume (normalized)")]
    [SerializeField] [Range(0, 1)] float _musicVolume;
    [Tooltip("Voice volume (normalized)")]
    [SerializeField] [Range(0, 1)] float _voiceVolume;

    [Tooltip("Camera FOV in degrees")]
    [SerializeField, Range(60, 100)] float _CameraFOV;


    public float XLookSens { get { return _xLookSens; } set { _xLookSens = value; } }
    public float YLookSens { get { return _yLookSens; } set { _yLookSens = value; } }
    public bool InvertAim { get { return _invertAim; } set { _invertAim = value; } }
    public float MasterSoundVol { get { return _masterSoundVol; } set { _masterSoundVol = value; } }
    public float SFXSoundVol { get { return _sfxVolume; } set { _sfxVolume = value; } }
    public float MusicSoundVol { get { return _musicVolume; } set { _musicVolume = value; } }
    public float VoiceSoundVol { get { return _voiceVolume; } set { _voiceVolume = value; } }
    public float CameraFOV { get { return _CameraFOV; } set { _CameraFOV = value; } }

}
