using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class GameOptionsManager : MonoBehaviour
{
    private GameOptions _gameOptions;

    public GameOptions Options => _gameOptions;
    

    public void Start()
    {
        Load();
    }


    public void Save()
    {
        PlayerPrefs.SetFloat("_xLookSens", _gameOptions.XLookSens);
        PlayerPrefs.SetFloat("_yLookSens", _gameOptions.YLookSens);
        PlayerPrefs.SetInt("_invertAim", _gameOptions.InvertAim == true ? 1 : 0);
        PlayerPrefs.SetFloat("_masterSoundVol", _gameOptions.MasterSoundVol);
        PlayerPrefs.SetFloat("_sfxSoundVol", _gameOptions.SFXSoundVol);
        PlayerPrefs.SetFloat("_musicSoundVol", _gameOptions.MusicSoundVol);
        PlayerPrefs.SetFloat("_voiceSoundVol", _gameOptions.VoiceSoundVol);
        PlayerPrefs.SetFloat("_cameraFOV", _gameOptions.CameraFOV);
        PlayerPrefs.Save();
    }

    public void Load()
    {
        _gameOptions.XLookSens = PlayerPrefs.GetFloat("_xLookSens", _gameOptions.XLookSens);
        _gameOptions.YLookSens = PlayerPrefs.GetFloat("_yLookSens", _gameOptions.YLookSens);
        _gameOptions.InvertAim = PlayerPrefs.GetInt("_invertAim", _gameOptions.InvertAim == true ? 1 : 0) > 0;
        _gameOptions.MasterSoundVol = PlayerPrefs.GetFloat("_masterSoundVol", _gameOptions.MasterSoundVol);
        _gameOptions.SFXSoundVol = PlayerPrefs.GetFloat("_sfxSoundVol", _gameOptions.SFXSoundVol);
        _gameOptions.MusicSoundVol = PlayerPrefs.GetFloat("_musicSoundVol", _gameOptions.MusicSoundVol);
        _gameOptions.VoiceSoundVol = PlayerPrefs.GetFloat("_voiceSoundVol", _gameOptions.VoiceSoundVol);
        _gameOptions.CameraFOV = PlayerPrefs.GetFloat("_cameraFOV", _gameOptions.CameraFOV);
    }
}
