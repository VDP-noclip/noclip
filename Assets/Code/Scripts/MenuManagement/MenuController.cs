using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.Rendering.PostProcessing;
using TMPro;
using UnityEditor;
using UnityEngine.Audio;

/// <summary>
/// This is the Menu Controller: a centralized structure where every menu element resides.
/// This implies sliders, buttons, popups and text. It also applies eventual modified settings (es.: SetResolution).
/// 
/// Important note: brightness has been temporarily ditched. PostProcessing effects will be
/// evaluated in the future. Keep an eye on the repo's issues.
/// </summary>
public class MenuController : MonoBehaviour
{
    [Header("Gameplay Settings")] 
    [SerializeField] private TMP_Text controllerSensitivityTextValue = null;
    [SerializeField] private Slider controllerSensitivitySlider = null;
    [SerializeField] private int defaultSensitivity = 4;
    
    public int mainControllerSensitivity = 4;

    [Header("Toggle Settings")] [SerializeField]
    private Toggle invertYToggle = null;

    [Header("Graphics Settings")]
    [SerializeField] private TMP_Dropdown qualityDropdown;
    [SerializeField] private Toggle fullScreenToggle;
    
    private int _qualityLevel;
    private bool _isFullScreen;
    private float _brightnessLevel;

    [Header("Volume Settings")] 
    [SerializeField] private TMP_Text volumeTextValue = null;
    [SerializeField] private Slider volumeSlider = null;
    [SerializeField] private float defaultVolume = 1.0f;
    [SerializeField] private AudioMixer audioMixer;

    private float currentVolume;

    [Header("Confirmation")] 
    [SerializeField] private GameObject confirmationPrompt = null;
    
    [Header("Levels To Load")]
    [SerializeField] private GameObject noSavedGameDialog = null;
    public string _newGameLevel;
    private string levelToLoad;

    [Header("Resolution Dropdowns")] 
    public TMP_Dropdown resolutionDropdown;

    private Resolution[] resolutions;

    // When the Menu starts the game will iterate through various available resolutions.
    // When done, it'll set the settings' dropdown menu (graphics) to whatever resolution has been found.
    private void Start()
    {
        resolutions = Screen.resolutions;
        resolutionDropdown.ClearOptions();

        List<string> options = new List<string>();

        int currentResolutionIndex = 0;

        for (int i = 0; i < resolutions.Length; i++)
        {
            string option = resolutions[i].width + " x " + resolutions[i].height;
            options.Add(option);

            if (resolutions[i].width == Screen.width && resolutions[i].height == Screen.height)
            {
                currentResolutionIndex = i;
            }
        }

        resolutionDropdown.AddOptions(options);
        resolutionDropdown.value = currentResolutionIndex;
        resolutionDropdown.RefreshShownValue();
    }
    
    // Starts a new game. _newGameLevel will be the first level passed to the SceneManager.
    public void StartGameDialogYes()
    {
        SceneManager.LoadScene(_newGameLevel);
    }

    // Loads a previously saved level.
    public void ResumeGameDialogYes()
    {
        // From a design perspective, when a saved level is found the resume button should become more visible. This will be dealt with when polishing.
        if (PlayerPrefs.HasKey("SavedLevel"))
        {
            levelToLoad = PlayerPrefs.GetString("SavedLevel");
            
            if (Application.CanStreamedLevelBeLoaded(levelToLoad))
            {
                SceneManager.LoadScene(levelToLoad);
            }
            else
            {
                noSavedGameDialog.SetActive(true);
            }
        }
        else
        {
            noSavedGameDialog.SetActive(true);
        }
    }

    // Quits the game.
    public void QuitGameButton()
    {
        Application.Quit();
    }

    // Various setters; these change the visual side of the menu, for example
    // the amount of brightness shown in numbers or if fullscreen is checked or not.
    public void SetResolution(int resolutionIndex)
    {
        Resolution resolution = resolutions[resolutionIndex];
        Screen.SetResolution(resolution.width, resolution.height, Screen.fullScreen);
    }
    public void SetFullScreen(bool isFullscreen)
    {
        _isFullScreen = isFullscreen;
    }
    public void SetQuality(int qualityIndex)
    {
        _qualityLevel = qualityIndex;
    }
    
    // TODO: For Stefano: this works! Look at VolumeApply.
    public void SetVolume(float volume)
    {
        currentVolume = volume;
        audioMixer.SetFloat("soundtrackVolume", Mathf.Log(currentVolume) * 20);
        volumeTextValue.text = volume.ToString("0.0");
    }
    public void SetControllerSensitivity(float sensitivity)
    {
        mainControllerSensitivity = Mathf.RoundToInt(sensitivity);
        controllerSensitivityTextValue.text = sensitivity.ToString("0");
    }
    
    // Applies changes. These actually save the information.
    public void GraphicsApply()
    {
        PlayerPrefs.SetInt("masterQuality", _qualityLevel);
        QualitySettings.SetQualityLevel(_qualityLevel);
        
        PlayerPrefs.SetInt("masterFullscreen", (_isFullScreen ? 1 : 0));
        Screen.fullScreen = _isFullScreen;

        StartCoroutine(ConfirmationBox());
    }
    
    // TODO: Doesn't work. At least, it doesn't load this in LoadPrefs
    public void VolumeApply()
    {
        PlayerPrefs.SetFloat("soundtrackVolume", currentVolume);
        StartCoroutine(ConfirmationBox());
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

        PlayerPrefs.SetFloat("masterSensitivity", mainControllerSensitivity);

        StartCoroutine(ConfirmationBox());
    }

    // When prompted, the player can reset various settings' values.
    // This button changes based on the menutype it's given.
    public void ResetButton(string MenuType)
    {
        if (MenuType == "Audio")
        {
            audioMixer.SetFloat("soundtrackVolume", Mathf.Log(defaultVolume) * 20);
            volumeSlider.value = defaultVolume;
            volumeTextValue.text = defaultVolume.ToString("0.0");
            
            VolumeApply();
        }

        if (MenuType == "Gameplay")
        {
            controllerSensitivityTextValue.text = defaultSensitivity.ToString("0");
            controllerSensitivitySlider.value = defaultSensitivity;
            mainControllerSensitivity = defaultSensitivity;
            invertYToggle.isOn = false;
            
            GameplayApply();
        }

        if (MenuType == "Graphics")
        {
            qualityDropdown.value = 1;
            QualitySettings.SetQualityLevel((1));

            fullScreenToggle.isOn = false;
            Screen.fullScreen = false;

            Resolution currentResolution = Screen.currentResolution;
            Screen.SetResolution(currentResolution.width, currentResolution.height, Screen.fullScreen);
            resolutionDropdown.value = resolutions.Length;
            
            GraphicsApply();
        }
    }
    
    // Returns an image on the bottom-left.
    // Lets the player know settings have changed.
    // A chance for a cool animation to take place, perhaps?
    public IEnumerator ConfirmationBox()
    {
        confirmationPrompt.SetActive(true);
        yield return new WaitForSeconds(2);
        confirmationPrompt.SetActive(false);
    }
}
