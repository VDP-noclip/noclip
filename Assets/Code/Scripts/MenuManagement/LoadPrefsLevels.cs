using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Audio;

/// <summary>
/// LoadPrefs checks whether user settings are locally saved. This script is basically a big list of checks.
/// If you've ever programmed for Android, PlayerPrefs is the equivalent of SharedPreferences.
/// 
/// You may see PlayerPrefs scattered around other scripts; that's because the checks in LoadPrefs are
/// mostly made to change the menu's state.
/// </summary>
public class LoadPrefsLevels : MonoBehaviour
{
    [Header("General Settings")] 
    [SerializeField] private bool canUse = false;

    [SerializeField] private PauseMenuController pauseMenuController;

    [Header("Mixer")] 
    [SerializeField] private AudioMixer _audioMixer;

    [Header("Volume Settings")]
    [SerializeField] private Slider _volumeSoundtrackSlider;
    [SerializeField] private Slider _volumeEffectsSlider;
    [SerializeField] private Slider _volumeGlobalSlider;
    [SerializeField] private float _defaultVolume = 1.0f;
    
    [Header("Sensitivity Settings")]
    [SerializeField] private Slider controllerSensitivitySlider = null;
    
    [Header("Invert Y Settings")]
    [SerializeField] private Toggle invertYToggle = null;
    

    private void Start()
    {
        if (canUse)
        {
            if (PlayerPrefs.HasKey("soundtrackVolume"))
            {
                float _localSoundtrackVolume = PlayerPrefs.GetFloat("soundtrackVolume");
                
                _volumeSoundtrackSlider.value = _localSoundtrackVolume;
                _audioMixer.SetFloat("soundtrackVolume", Mathf.Log(_localSoundtrackVolume) * 20);
            }
            
            if (PlayerPrefs.HasKey("effectsVolume"))
            {
                float _localEffectsVolume = PlayerPrefs.GetFloat("effectsVolume");
                
                _volumeEffectsSlider.value = _localEffectsVolume;
                _audioMixer.SetFloat("effectsVolume", Mathf.Log(_localEffectsVolume) * 20);
            }
            
            if (PlayerPrefs.HasKey("globalVolume"))
            {
                float _localGlobalVolume = PlayerPrefs.GetFloat("globalVolume");
                
                _volumeGlobalSlider.value = _localGlobalVolume;
                _audioMixer.SetFloat("globalVolume", Mathf.Log(_localGlobalVolume * 20));
            }

            // More infos about these two checks in MouseLook.cs.
            if (PlayerPrefs.HasKey("masterSensitivity"))
            {
                float localSensitivity = PlayerPrefs.GetFloat("masterSensitivity");
                
                controllerSensitivitySlider.value = localSensitivity;
                pauseMenuController.mainControllerSensitivity = Mathf.RoundToInt(localSensitivity);
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
