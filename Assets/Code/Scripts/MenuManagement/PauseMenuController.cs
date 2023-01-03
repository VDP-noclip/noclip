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
    [SerializeField] private GameObject _feedbackUI;
    [SerializeField] private GameObject _controlsUI;

    [Header("Pause Status")]
    [SerializeField] private bool _isPaused;
    
    [Header("Audio")]
    [SerializeField] private AudioSource _menuPress;
    [SerializeField] private AudioMixer _audioMixer;

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
        _settingsMenuUI.SetActive(false);
        _feedbackUI.SetActive(false);
        _controlsUI.SetActive(false);
    }

    public void ReturnToMenu()
    {
        Time.timeScale = 1;
        AudioListener.pause = false;

        _isPaused = false;
        
        SceneManager.LoadScene("Menu_0");
    }
    
    public void SetControllerSensitivity(float sensitivity)
    {
        EventManager.TriggerEvent("setSensitivity", sensitivity.ToString());
        PlayerPrefs.SetFloat("masterSensitivity", Mathf.RoundToInt(sensitivity));

        _controllerSensitivitySlider.value = sensitivity;
        
        StartCoroutine(ConfirmationBox());
    }
    public void SetSoundtrackVolume(float volume)
    {
        _audioMixer.SetFloat("soundtrackVolume", Mathf.Log(volume) * 20);
        PlayerPrefs.SetFloat("soundtrackVolume", volume);
        
        _volumeSoundtrackSlider.value = volume;
        
        StartCoroutine(ConfirmationBox());
    }
    public void SetEffectsVolume(float volume)
    {
        _audioMixer.SetFloat("effectsVolume", Mathf.Log(volume) * 20);
        PlayerPrefs.SetFloat("effectsVolume", volume);
        
        _volumeEffectsSlider.value = volume;
        
        StartCoroutine(ConfirmationBox());
    }
    public void SetGlobalVolume(float volume)
    {
        _audioMixer.SetFloat("globalVolume", Mathf.Log(volume) * 20);
        PlayerPrefs.SetFloat("globalVolume", volume);
        
        _volumeGlobalSlider.value = volume;
        
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
