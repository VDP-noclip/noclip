using System.Collections;
using System.Collections.Generic;
using Code.ScriptableObjects;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Audio;

/// <summary>
/// LoadPref checks whether user settings are locally saved. This script is basically a big list of checks.
/// If you've ever programmed for Android, PlayerPrefs is the equivalent of SharedPreferences.
/// 
/// You may see PlayerPrefs scattered around other scripts; that's because the checks in LoadPrefs are
/// mostly made to change the menu's state.
/// </summary>
public class LoadPrefs : MonoBehaviour
{
    [Header("General Settings")] 
    [SerializeField] private bool canUse = false;
    [SerializeField] private DefaultPlayerPrefs _defaultPlayerPrefs;

    private void Awake()
    {
        // PlayerPrefs.DeleteAll();
        
        if (!canUse) return;
        
        if (! PlayerPrefs.HasKey("soundtrackVolume"))
            PlayerPrefs.SetFloat("soundtrackVolume", _defaultPlayerPrefs.soundTracksVolumeDecibel);
        
        if (! PlayerPrefs.HasKey("effectsVolume"))
            PlayerPrefs.SetFloat("effectsVolume", _defaultPlayerPrefs.effectsVolumeDecibel);
        
        if (!PlayerPrefs.HasKey("globalVolume"))
            PlayerPrefs.SetFloat("globalVolume", _defaultPlayerPrefs.globalVolumeDecibel);
        
        if (!PlayerPrefs.HasKey("cameraFov"))
            PlayerPrefs.SetFloat("cameraFov", _defaultPlayerPrefs.defaultFov);

        if (!PlayerPrefs.HasKey("masterQuality"))
            PlayerPrefs.SetInt("masterQuality", _defaultPlayerPrefs.masterQuality);
        QualitySettings.SetQualityLevel(PlayerPrefs.GetInt("masterQuality"));
        
        if (!PlayerPrefs.HasKey("masterSensitivity"))
            PlayerPrefs.SetFloat("masterSensitivity", _defaultPlayerPrefs.masterSensitivity);

        // full screen
        if (!PlayerPrefs.HasKey("masterFullscreen"))
        {
            int defaultMasterFullScreen;
            defaultMasterFullScreen = _defaultPlayerPrefs.masterFullScreen ? 1 : 0;
            PlayerPrefs.SetInt("masterFullscreen", defaultMasterFullScreen);
        }
        if (PlayerPrefs.GetInt("masterFullscreen") == 1)
        {
            Screen.fullScreen = true;
        }
        else
        {
            Screen.fullScreen = false;
        }
    }
    
}
