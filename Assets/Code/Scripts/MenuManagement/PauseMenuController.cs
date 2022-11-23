using System.Collections;
using System.Collections.Generic;
using System.Security;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PauseMenuController : MonoBehaviour
{
    [SerializeField] private GameObject pauseMenuUI;
    [SerializeField] private GameObject settingsMenuUI;
    [SerializeField] private GameObject audioMenuUI;
    [SerializeField] private GameObject gameplayMenuUI;
    
    [SerializeField] private Button resume;
    [SerializeField] private Button settings;
    [SerializeField] private Button exit;

    [SerializeField] private bool isPaused;
    [SerializeField] private AudioSource menuPress;
    
    [Header("Sensitivity Settings")]
    [SerializeField] private TMP_Text controllerSensitivityTextValue = null;
    [SerializeField] private Slider controllerSensitivitySlider = null;
    
    public int mainControllerSensitivity = 4;
    
    [Header("Volume Settings")] 
    [SerializeField] private TMP_Text volumeTextValue = null;
    [SerializeField] private Slider volumeSlider = null;
    [SerializeField] private float defaultVolume = 1.0f;
    
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.L))
        {
            if (!isPaused)
            {
                ActivateMenu();
            }
            else
            {
                DeactivateMenu();
            }
        }
    }

    public void ActivateMenu()
    {
        Time.timeScale = 0;
        AudioListener.pause = true;
        
        menuPress.ignoreListenerPause=true;
        menuPress.Play();
        
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        pauseMenuUI.SetActive(true);
        isPaused = true;
    }

    public void DeactivateMenu()
    {
        Time.timeScale = 1;
        AudioListener.pause = false;
        
        menuPress.Play();
        
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        pauseMenuUI.SetActive(false);
        audioMenuUI.SetActive(false);
        settingsMenuUI.SetActive(false);
        gameplayMenuUI.SetActive(false);
        
        resume.enabled = true;
        settings.enabled = true;
        exit.enabled = true;
        
        isPaused = false;
    }
    
    public void SetControllerSensitivity(float sensitivity)
    {
        mainControllerSensitivity = Mathf.RoundToInt(sensitivity);
        controllerSensitivityTextValue.text = sensitivity.ToString("0");
    }
    public void SetVolume(float volume)
    {
        AudioListener.volume = volume;
        volumeTextValue.text = volume.ToString("0.0");
        volumeSlider.value = volume;
    }
    
    public void GameplayApply()
    {
        PlayerPrefs.SetFloat("masterSensitivity", mainControllerSensitivity);
    }
    
    public void VolumeApply()
    {
        PlayerPrefs.SetFloat("masterVolume", AudioListener.volume);
    }
}
