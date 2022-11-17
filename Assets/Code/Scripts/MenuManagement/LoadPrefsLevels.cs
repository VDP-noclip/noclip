using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

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

    [Header("Volume Settings")]
    [SerializeField] private TMP_Text volumeTextValue = null;
    [SerializeField] private Slider volumeSlider = null;
    
    [Header("Sensitivity Settings")]
    [SerializeField] private TMP_Text controllerSensitivityTextValue = null;
    [SerializeField] private Slider controllerSensitivitySlider = null;
    

    private void Update()
    {
        if (canUse)
        {
            if (PlayerPrefs.HasKey("masterVolume"))
            {
                float localVolume = PlayerPrefs.GetFloat("masterVolume");

                volumeTextValue.text = localVolume.ToString("0.0");
                volumeSlider.value = localVolume;
                AudioListener.volume = localVolume;
            }

            // More infos about these two checks in MouseLook.cs.
            if (PlayerPrefs.HasKey("masterSensitivity"))
            {
                float localSensitivity = PlayerPrefs.GetFloat("masterSensitivity");

                controllerSensitivityTextValue.text = localSensitivity.ToString("0");
                controllerSensitivitySlider.value = localSensitivity;
                pauseMenuController.mainControllerSensitivity = Mathf.RoundToInt(localSensitivity);
            }
            
        }
    }
}
