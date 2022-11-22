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
    [SerializeField] private TMP_Text volumeTextValue = null;
    [SerializeField] private Slider volumeSlider = null;
    [SerializeField] private AudioMixer audioMixer;

    [Header("Quality Level Settings")]
    [SerializeField] private TMP_Dropdown qualityDropdown;
    
    [Header("Fullscreen Settings")]
    [SerializeField] private Toggle fullScreenToggle;
    
    [Header("Sensitivity Settings")]
    [SerializeField] private TMP_Text controllerSensitivityTextValue = null;
    [SerializeField] private Slider controllerSensitivitySlider = null;
    
    [Header("Invert Y Settings")]
    [SerializeField] private Toggle invertYToggle = null;

    private void Awake()
    {
        if (canUse)
        {
            // TODO: Doesn't work...
            if (PlayerPrefs.HasKey("soundtrackVolume"))
            {
                
                float localVolume = PlayerPrefs.GetFloat("soundtrackVolume");
                Debug.Log("volume locale: " + localVolume);
                
                volumeTextValue.text = localVolume.ToString("0.0");
                volumeSlider.value = localVolume;
                audioMixer.SetFloat("soundtrackVolume", Mathf.Log(localVolume) * 20);
            }
            else
            {
                menuController.ResetButton("Audio");
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

                controllerSensitivityTextValue.text = localSensitivity.ToString("0");
                controllerSensitivitySlider.value = localSensitivity;
                menuController.mainControllerSensitivity = Mathf.RoundToInt(localSensitivity);
            }
            
            if (PlayerPrefs.HasKey("masterInvertY"))
            {
                if (PlayerPrefs.GetInt("masterInvertY") == 1)
                {
                    invertYToggle.isOn = true;
                }

                else
                {
                    invertYToggle.isOn = false;
                }
            }
        }
    }
}
