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
    
    [Header("Buttons")]
    [SerializeField] private Button _resume;
    [SerializeField] private TMP_Text _resumeText;
    
    [SerializeField] private Button _settings;
    [SerializeField] private TMP_Text _settingsText;
        
    [SerializeField] private Button _exit;
    [SerializeField] private TMP_Text _exitText;
        
    [SerializeField] private Button _audio;
    [SerializeField] private TMP_Text _audioText;
        
    [SerializeField] private Button _gameplay;
    [SerializeField] private TMP_Text _gameplayText;
        
    [SerializeField] private Button _return;
    [SerializeField] private TMP_Text _returnText;

    [Header("Pause Status")]
    [SerializeField] private bool _isPaused;
    
    [Header("Audio")]
    [SerializeField] private AudioSource _menuPress;
    [SerializeField] private AudioMixer _audioMixer;
    [SerializeField] private float _mufflingQuantity;

    private float _currentGlobalVolume;
    private float _currentEffectsVolume;
    private float _currentSoundVolume;
    
    [Header("Gameplay Settings")]
    [SerializeField] private Slider _controllerSensitivitySlider = null;
    [SerializeField] private int _defaultSensitivity = 4;
    [SerializeField] private Toggle invertYToggle = null;
    
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
        SetVolumeEffects(PlayerPrefs.GetFloat("effectsVolume"));
        SetVolumeGlobal(PlayerPrefs.GetFloat("globalVolume"));
        SetVolumeSoundtrack(PlayerPrefs.GetFloat("soundtrackVolume"));
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
        SetVolumeEffects(PlayerPrefs.GetFloat("effectsVolume"));
        SetVolumeGlobal(PlayerPrefs.GetFloat("globalVolume"));
        SetVolumeSoundtrack(PlayerPrefs.GetFloat("soundtrackVolume"));
        

        Time.timeScale = 0;
        AudioListener.pause = false;
        
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
        SetVolumeGlobal(PlayerPrefs.GetFloat("soundtrackVolume"));
        Time.timeScale = 1;
        AudioListener.pause = false;

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
        
        _resume.enabled = true;
        _resumeText.alpha = 1;
        
        _settings.enabled = true;
        _settingsText.alpha = 1;
        
        _exit.enabled = true;
        _exitText.alpha = 1;

        _audio.enabled = true;
        _audioText.alpha = 1;
        
        _gameplay.enabled = true;
        _gameplayText.alpha = 1;
        
        _return.enabled = true;
        _returnText.alpha = 1;
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
    public void SetVolumeSoundtrack(float volume)
    {
        _currentSoundVolume = volume;
        _audioMixer.SetFloat("soundtrackVolume", Mathf.Log(_currentSoundVolume) * 20);
        _volumeSoundtrackSlider.value = volume;
    }
    public void SetVolumeEffects(float volume)
    {
        _currentEffectsVolume = volume;
        _audioMixer.SetFloat("effectsVolume", Mathf.Log(_currentEffectsVolume) * 20);
        _volumeEffectsSlider.value = volume;
    }
    public void SetVolumeGlobal(float volume)
    {
        _currentGlobalVolume = volume;
        _audioMixer.SetFloat("globalVolume", Mathf.Log(_currentGlobalVolume) * 20);
        _volumeGlobalSlider.value = volume;
    }
    public void GameplayApply()
    {
        if (invertYToggle.isOn)
        {
            PlayerPrefs.SetInt("masterInvertY", 1);
            
        }
        else
        {
            PlayerPrefs.SetInt("masterInvertY", 0);
        }

        Debug.Log("Setting sensitivity in LoadPrefs: " + mainControllerSensitivity);
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
    
    
    public void ResetButton(string MenuType)
    {
        if (MenuType == "Audio")
        {
            _currentSoundVolume = _defaultVolume;
            _currentEffectsVolume = _defaultVolume;
            _currentGlobalVolume = _defaultVolume;
            
            _volumeEffectsSlider.value = _defaultVolume;
            _volumeSoundtrackSlider.value = _defaultVolume;
            _volumeGlobalSlider.value = _defaultVolume;

            VolumeApply();
        }

        if (MenuType == "Gameplay")
        {
            _controllerSensitivitySlider.value = _defaultSensitivity;
            mainControllerSensitivity = _defaultSensitivity;
            invertYToggle.isOn = false;
            
            GameplayApply();
        }
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
