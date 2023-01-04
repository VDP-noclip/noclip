using System.Collections;
using System.Collections.Generic;
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

    [SerializeField] private MenuController menuController;

    [Header("Volume Settings")]
    [SerializeField] private Slider globalVolumeSlider = null;
    [SerializeField] private Slider effectsVolumeSlider = null;
    [SerializeField] private Slider soundtrackVolumeSlider = null;
    [SerializeField] private AudioMixer audioMixer;

    [Header("Quality Level Settings")]
    [SerializeField] private TMP_Dropdown qualityDropdown;
    
    [Header("Fullscreen Settings")]
    [SerializeField] private Toggle fullScreenToggle;
    
    [Header("Sensitivity Settings")]
    [SerializeField] private Slider controllerSensitivitySlider = null;
    private void Start()
    {
        if (canUse)
        {
            // Changes soundtrack volume based on stored value
            if (PlayerPrefs.HasKey("soundtrackVolume"))
            {
                
                float localSoundVolume = PlayerPrefs.GetFloat("soundtrackVolume");
                Debug.Log("local volume menu " + localSoundVolume);
                
                soundtrackVolumeSlider.value = localSoundVolume;
                audioMixer.SetFloat("soundtrackVolume", Mathf.Log(localSoundVolume) * 20);
            }
            

            if (PlayerPrefs.HasKey("effectsVolume"))
            {
                
                float localEffectsVolume = PlayerPrefs.GetFloat("effectsVolume");
                
                effectsVolumeSlider.value = localEffectsVolume;
                audioMixer.SetFloat("effectsVolume", Mathf.Log(localEffectsVolume) * 20);
            }

            
            if (PlayerPrefs.HasKey("globalVolume"))
            {
                
                float localGlobalVolume = PlayerPrefs.GetFloat("globalVolume");

                globalVolumeSlider.value = localGlobalVolume;
                audioMixer.SetFloat("globalVolume", Mathf.Log(localGlobalVolume) * 20);
            }
            

            if (PlayerPrefs.HasKey("masterQuality"))
            {
                int localQuality = PlayerPrefs.GetInt("masterQuality");
                qualityDropdown.value = localQuality;
                QualitySettings.SetQualityLevel(localQuality);
            }

            if (PlayerPrefs.HasKey("masterFullscreen"))
            {
                int localFullscreen = PlayerPrefs.GetInt("masterFullscreen");

                if (localFullscreen == 1)
                {
                    Screen.fullScreen = true;
                    fullScreenToggle.isOn = true;
                }
                else
                {
                    Screen.fullScreen = false;
                    fullScreenToggle.isOn = false;
                }
            }
            
            // More infos about these two checks in MouseLook.cs.
            if (PlayerPrefs.HasKey("masterSensitivity"))
            {
                float localSensitivity = PlayerPrefs.GetFloat("masterSensitivity");

                controllerSensitivitySlider.value = localSensitivity;
            }
        }
    }
}
