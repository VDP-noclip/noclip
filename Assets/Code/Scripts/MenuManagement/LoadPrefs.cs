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
        PlayerPrefs.DeleteAll();
        
        if (!canUse) return;
        
        // localSoundVolume
        float localSoundVolume;
        if (PlayerPrefs.HasKey("soundtrackVolume"))
            localSoundVolume = PlayerPrefs.GetFloat("soundtrackVolume");
        else
        {
            localSoundVolume = _defaultPlayerPrefs.soundTracksVolumeDecibel;
            PlayerPrefs.SetFloat("soundtrackVolume", localSoundVolume);
        }
        Debug.Log("localSoundVolume:" + localSoundVolume);

        // effectsVolume
        float effectsVolume;
        if (PlayerPrefs.HasKey("effectsVolume"))
            effectsVolume = PlayerPrefs.GetFloat("effectsVolume");
        else
        {
            effectsVolume = _defaultPlayerPrefs.effectsVolumeDecibel;
            PlayerPrefs.SetFloat("effectsVolume", effectsVolume);
        }

        // globalVolume
        float globalVolume;
        if (PlayerPrefs.HasKey("globalVolume"))
            globalVolume = PlayerPrefs.GetFloat("globalVolume");
        else
        {
            globalVolume = _defaultPlayerPrefs.globalVolumeDecibel;
            PlayerPrefs.SetFloat("globalVolume", globalVolume);
        }

        // cameraFov
        float cameraFov;
        if (PlayerPrefs.HasKey("cameraFov"))
            cameraFov = PlayerPrefs.GetFloat("cameraFov");
        else
        {
            cameraFov = _defaultPlayerPrefs.defaultFov;
            PlayerPrefs.SetFloat("cameraFov", cameraFov);
        }

        // masterQuality
        int masterQuality;
        if (PlayerPrefs.HasKey("masterQuality"))
            masterQuality = PlayerPrefs.GetInt("masterQuality");
        else
        {
            masterQuality = _defaultPlayerPrefs.masterQuality;
            PlayerPrefs.SetInt("masterQuality", masterQuality);
        }
        QualitySettings.SetQualityLevel(masterQuality);

        // masterSensitivity
        float masterSensitivity;
        if (PlayerPrefs.HasKey("masterSensitivity"))
            masterSensitivity = PlayerPrefs.GetFloat("masterSensitivity");
        else
        {
            masterSensitivity = _defaultPlayerPrefs.masterSensitivity;
            PlayerPrefs.SetFloat("masterSensitivity", masterSensitivity);
        }

        // full screen
        int defaultMasterFullScreen;
        if (PlayerPrefs.HasKey("masterFullscreen"))
            defaultMasterFullScreen = PlayerPrefs.GetInt("masterFullscreen");
        else
        {
            defaultMasterFullScreen = _defaultPlayerPrefs.masterFullScreen ? 1 : 0;
            PlayerPrefs.SetInt("masterFullscreen", defaultMasterFullScreen);
        }
        if (defaultMasterFullScreen == 1)
        {
            Screen.fullScreen = true;
        }
        else
        {
            Screen.fullScreen = false;
        }
    }
    
}
