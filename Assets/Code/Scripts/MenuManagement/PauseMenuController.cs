using System.Collections;
using System.Collections.Generic;
using System.Security;
using Cinemachine;
using POLIMIGameCollective;
using TMPro;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PauseMenuController : MonoBehaviour
{
    [Header("Containers")]
    [SerializeField] private GameObject _pauseMenuUI;
    [SerializeField] private GameObject _settingsMenuUI;
    [SerializeField] private GameObject _audioMenuUI;
    [SerializeField] private GameObject _gameplayMenuUI;
    [SerializeField] private GameObject _feedbackUI;
    [SerializeField] private GameObject _controlsUI;

    [Header("Pause Status")]
    [SerializeField] private bool _isPaused;
    
    [Header("Audio")]
    [SerializeField] private AudioSource _menuPress;
    [SerializeField] private AudioMixer _audioMixer;

    private float _currentGlobalVolume;
    private float _currentEffectsVolume;
    private float _currentSoundVolume;
    
    [Header("Gameplay Settings")]
    [SerializeField] private Slider _controllerSensitivitySlider = null;
    [SerializeField] private int _defaultSensitivity = 4;

    public int mainControllerSensitivity = 4;
    
    [Header("Volume Settings")]
    [SerializeField] private Slider _volumeSoundtrackSlider;
    [SerializeField] private Slider _volumeEffectsSlider;
    [SerializeField] private Slider _volumeGlobalSlider;
    [SerializeField] private float _defaultVolume = 1.0f;
    
    [Header("Confirmation")] 
    [SerializeField] private GameObject confirmationPrompt = null;

    private void Start()
    {
        SetEffectsVolume(PlayerPrefs.GetFloat("effectsVolume"));
        SetGlobalVolume(PlayerPrefs.GetFloat("globalVolume"));
        SetSoundtrackVolume(PlayerPrefs.GetFloat("soundtrackVolume"));
    }
    private void Update()
    {
        if (Input.GetButtonDown("Pause"))
        {
            if (!_isPaused)
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
        SetEffectsVolume(PlayerPrefs.GetFloat("effectsVolume"));
        SetGlobalVolume(PlayerPrefs.GetFloat("globalVolume"));
        SetSoundtrackVolume(PlayerPrefs.GetFloat("soundtrackVolume"));
     
        Time.timeScale = 0;
        AudioListener.pause = false;
        EventManager.TriggerEvent("PauseTimeConstraintsTimer");
        
        _menuPress.ignoreListenerPause=true;
        _menuPress.Play();
        
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        
        _controlsUI.SetActive(true);
        _pauseMenuUI.SetActive(true);
        _isPaused = true;
    }

    public void DeactivateMenu()
    {
        SetGlobalVolume(PlayerPrefs.GetFloat("soundtrackVolume"));
        Time.timeScale = 1;
        AudioListener.pause = false;
        EventManager.TriggerEvent("ResumeTimeConstraintsTimer");

        _menuPress.Play();
        
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        OverlayUpdate();

        _isPaused = false;
    }

    public void OverlayUpdate()
    {
        _pauseMenuUI.SetActive(false);
        _audioMenuUI.SetActive(false);
        _settingsMenuUI.SetActive(false);
        _gameplayMenuUI.SetActive(false);
        _feedbackUI.SetActive(false);
        _controlsUI.SetActive(false);
    }

    public void ReturnToMenu()
    {
        Time.timeScale = 1;
        AudioListener.pause = false;
        
        VolumeApply();
        
        _isPaused = false;
        
        SceneManager.LoadScene("Menu_0");
    }
    
    public void SetControllerSensitivity(float sensitivity)
    {
        EventManager.TriggerEvent("setSensitivity", sensitivity.ToString());
        mainControllerSensitivity = Mathf.RoundToInt(sensitivity);
    }
    public void SetSoundtrackVolume(float volume)
    {
        _currentSoundVolume = volume;
        _audioMixer.SetFloat("soundtrackVolume", Mathf.Log(_currentSoundVolume) * 20);
        PlayerPrefs.SetFloat("soundtrackVolume", _currentSoundVolume);
    }
    public void SetEffectsVolume(float volume)
    {
        _currentEffectsVolume = volume;
        _audioMixer.SetFloat("effectsVolume", Mathf.Log(_currentEffectsVolume) * 20);
        PlayerPrefs.SetFloat("effectsVolume", _currentEffectsVolume);
    }
    public void SetGlobalVolume(float volume)
    {
        _currentGlobalVolume = volume;
        _audioMixer.SetFloat("globalVolume", Mathf.Log(_currentGlobalVolume) * 20);
        PlayerPrefs.SetFloat("globalVolume", _currentGlobalVolume);
    }
    public void GameplayApply()
    {
        PlayerPrefs.SetFloat("masterSensitivity", mainControllerSensitivity);
        
        StartCoroutine(ConfirmationBox());
    }
    
    public void VolumeApply()
    {
        PlayerPrefs.SetFloat("soundtrackVolume", _currentSoundVolume);
        PlayerPrefs.SetFloat("effectsVolume", _currentEffectsVolume);
        PlayerPrefs.SetFloat("globalVolume", _currentGlobalVolume);
            
        StartCoroutine(ConfirmationBox());
    }
  
    public IEnumerator ConfirmationBox()
    {
        confirmationPrompt.SetActive(true);
        yield return StartCoroutine(WaitForRealSeconds(2f));
        confirmationPrompt.SetActive(false);
    }
    
    public static IEnumerator WaitForRealSeconds(float time)
    {
        float start = Time.realtimeSinceStartup;
        while (Time.realtimeSinceStartup < start + time)
        {
            yield return null;
        }
    }
}
