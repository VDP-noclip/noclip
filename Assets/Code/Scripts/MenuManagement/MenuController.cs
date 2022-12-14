using System.Collections;
using System.Collections.Generic;
using POLIMIGameCollective;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.Rendering.PostProcessing;
using TMPro;
using UnityEditor;
using UnityEngine.Audio;
using UnityEngine.Rendering;

/// <summary>
/// This is the Menu Controller: a centralized structure where every menu element resides.
/// This implies sliders, buttons, popups and text. It also applies eventual modified settings (es.: SetResolution).
/// 
/// Important note: brightness has been temporarily ditched. PostProcessing effects will be
/// evaluated in the future. Keep an eye on the repo's issues.
/// </summary>
public class MenuController : MonoBehaviour
{
    [Header("Objects to fade")] 
    [SerializeField] private GameObject mainCanvas;
    [SerializeField] private GameObject mainGradient;
    [SerializeField] private GameObject bottomGradient;
    [SerializeField] private GameObject noclipLogo;
    [SerializeField] private GameObject logoBlur;
    [SerializeField] private GameObject enterButton;
    [SerializeField] private TMP_Text _resumeText;
    [SerializeField] private GameObject settingsButton;
    [SerializeField] private TMP_Text settingsText;
    [SerializeField] private GameObject exitButton;
    [SerializeField] private GameObject feedbackButton;
    [SerializeField] private GameObject controlsButton;
    [SerializeField] private GameObject pressedExit;
    [SerializeField] private GameObject pressedOptions;
    [SerializeField] private GameObject popoutContainer;
    [SerializeField] private GameObject exitContainer;
    [SerializeField] private GameObject feedbackContainer;
    [SerializeField] private GameObject controlsContainer;

    [Header("Audio To Play")] 
    [SerializeField] private AudioSource noclipEcho;

    [Header("Gameplay Settings")]
    [SerializeField] private Slider controllerSensitivitySlider = null;
    [SerializeField] private Slider controllerFovSlider = null;

    [Header("Graphics Settings")]
    [SerializeField] private TMP_Dropdown qualityDropdown;
    [SerializeField] private Toggle fullScreenToggle;

    [Header("Utils")] 
    [SerializeField] private GameObject animations;

    private bool _isStartPressed = false;
    private bool _isSettingsPressed = false;
    private bool _isQuitPressed = false;
    
    [Header("Pause Settings")]
    [SerializeField] private bool _isPauseMenu = false;
    [SerializeField] private AudioSource _pause;
    [SerializeField] private AudioSource _resume;

    [Header("Volume Settings")]
    [SerializeField] private Slider globalVolumeSlider = null;
    [SerializeField] private Slider soundVolumeSlider = null;
    [SerializeField] private Slider effectsVolumeSlider = null;
    [SerializeField] private AudioMixer audioMixer;

    [Header("Confirmation")] 
    [SerializeField] private GameObject confirmationPrompt = null;
    
    [Header("Levels To Load")]
    public string _newGameLevel;

    [Header("Resolution Dropdowns")] 
    public TMP_Dropdown resolutionDropdown;

    private Resolution[] resolutions;

    private bool _fadeIsRunning;
    private bool _isPaused;

    // When the Menu starts the game will iterate through various available resolutions.
    // When done, it'll set the settings' dropdown menu (graphics) to whatever resolution has been found.
    
    private void Start()
    {
        if (!_isPauseMenu)
        {
            animations.SetActive(true);
        }
        
        SetFOV(PlayerPrefs.GetFloat("cameraFov"));
        SetEffectsVolume(PlayerPrefs.GetFloat("effectsVolume"));
        SetGlobalVolume(PlayerPrefs.GetFloat("globalVolume"));
        SetSoundVolume(PlayerPrefs.GetFloat("soundtrackVolume"));
        SetFullScreen(PlayerPrefs.GetInt("masterFullscreen") == 1);
        SetQuality(PlayerPrefs.GetInt("masterQuality"));
        
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
    
    private void Update()
    {
        if (Input.GetButtonDown("Pause") && _isPauseMenu && !_fadeIsRunning)
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
        StartCoroutine(_FadeInUI());

        _isStartPressed = false;
        _isSettingsPressed = false;
        _isQuitPressed = false;
        
        Time.timeScale = 0;
        AudioListener.pause = false;
        EventManager.TriggerEvent("PauseTimeConstraintsTimer");

        _pause.ignoreListenerPause=true;
        _resume.ignoreListenerPause=true;
        _pause.Play();
        
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        _isPaused = true;
    }

    public void DeactivateMenu()
    {
        Time.timeScale = 1;
        AudioListener.pause = false;
        EventManager.TriggerEvent("ResumeTimeConstraintsTimer");

        _resume.Play();

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        pressedExit.SetActive(false);
        pressedOptions.SetActive(false);
        popoutContainer.SetActive(false);
        exitContainer.SetActive(false);
        feedbackContainer.SetActive(false);
        controlsContainer.SetActive(false);
        
        StartCoroutine(FadeOutUI());
        
        ChangeColor(settingsText);
        ChangeColor(_resumeText);
        
        _isPaused = false;
    }
    
    
    
    public void ReturnToMenu()
    {
        Time.timeScale = 1;
        AudioListener.pause = false;

        _isPaused = false;
        
        SceneManager.LoadScene("Menu_0");
    }
    
    // Starts a new game. _newGameLevel will be the first level passed to the SceneManager.
    public void StartGameDialogYes()
    {
        SceneManager.LoadScene(_newGameLevel);
    }

    /* // Loads a previously saved level.
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
    */
    
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

    // Sets volume in Mixer
    public void SetSoundVolume(float volume)
    {
        audioMixer.SetFloat("soundtrackVolume", Mathf.Log(volume) * 20);
        PlayerPrefs.SetFloat("soundtrackVolume", volume);

        soundVolumeSlider.value = volume;

        StartCoroutine(ConfirmationBox());
    }
    
    public void SetGlobalVolume(float volume)
    {
        audioMixer.SetFloat("globalVolume", Mathf.Log(volume) * 20);
        PlayerPrefs.SetFloat("globalVolume", volume);

        globalVolumeSlider.value = volume;
        Debug.Log("YOOOOOOOOOOOOO");

        StartCoroutine(ConfirmationBox());
    }
    
    public void SetEffectsVolume(float volume)
    {
        audioMixer.SetFloat("effectsVolume", Mathf.Log(volume) * 20);
        PlayerPrefs.SetFloat("effectsVolume", volume);

        effectsVolumeSlider.value = volume;

        StartCoroutine(ConfirmationBox());
    }
    public void SetControllerSensitivity(float sensitivity)
    {
        PlayerPrefs.SetFloat("masterSensitivity", Mathf.RoundToInt(sensitivity));
        EventManager.TriggerEvent("setSensitivity", sensitivity.ToString());

        controllerSensitivitySlider.value = sensitivity;
        
        StartCoroutine(ConfirmationBox());
    }

    public void SetFOV(float fov)
    {
        PlayerPrefs.SetFloat("cameraFov", fov);
        EventManager.TriggerEvent("UpdateFovFromPlayerPrefs");

        controllerFovSlider.value = fov;
        
        StartCoroutine(ConfirmationBox());

    }
    
    // Applies changes. These actually save the information.
    public void GraphicsApply()
    {
        SetQuality(qualityDropdown.value);
        SetFullScreen(fullScreenToggle.isOn);
        StartCoroutine(ConfirmationBox());
    }
    
    public void SetFullScreen(bool isFullscreen)
    {
        PlayerPrefs.SetInt("masterFullscreen", isFullscreen ? 1 : 0);
        Screen.fullScreen = isFullscreen;
        fullScreenToggle.isOn = isFullscreen;
    }
    
    public void SetQuality(int qualityIndex)
    {
        PlayerPrefs.SetInt("masterQuality", qualityIndex);
        QualitySettings.SetQualityLevel(qualityIndex);
        qualityDropdown.value = qualityIndex;
    }

    // When prompted, the player can reset various settings' values.
    // This button changes based on the menutype it's given.
    /*
    public void ResetButton(string MenuType)
    {
        if (MenuType == "Audio")
        {
            audioMixer.SetFloat("soundtrackVolume", Mathf.Log(defaultVolume) * 20);
            audioMixer.SetFloat("globalVolume", Mathf.Log(defaultVolume) * 20);
            audioMixer.SetFloat("effectsVolume", Mathf.Log(defaultVolume) * 20);
            
            globalVolumeSlider.value = defaultVolume;
            effectsVolumeSlider.value = defaultVolume;
            soundVolumeSlider.value = defaultVolume;
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
    */

    public void ChangeStartColor(TMP_Text buttonText)
    {
        if (!_isStartPressed)
        {
            exitButton.GetComponent<TMP_Text>().color = Color.white;
            settingsButton.GetComponent<TMP_Text>().color = Color.white;
            buttonText.color = Color.black;
        }
        else
        {
            buttonText.color = Color.white;
        }
    }
    
    public void ChangeSettingsColor(TMP_Text buttonText)
    {
        if (!_isSettingsPressed)
        {
            exitButton.GetComponent<TMP_Text>().color = Color.white;
            enterButton.GetComponent<TMP_Text>().color = Color.white;
            buttonText.color = Color.black;
        }
        else
        {
            buttonText.color = Color.white;
        }
    }
    
    public void ChangeQuitColor(TMP_Text buttonText)
    {
        if (!_isQuitPressed)
        {
            enterButton.GetComponent<TMP_Text>().color = Color.white;
            settingsButton.GetComponent<TMP_Text>().color = Color.white;
            buttonText.color = Color.black;
        }
        else
        {
            buttonText.color = Color.white;
        }
    }

    public void ChangeColor(TMP_Text buttonText)
    {
        buttonText.color = Color.white;
    }
    
    // Returns an image on the bottom-left.
    // Lets the player know settings have changed.
    // A chance for a cool animation to take place, perhaps? | 2023 edit: no.
    public IEnumerator ConfirmationBox()
    {
        confirmationPrompt.SetActive(true);
        yield return StartCoroutine(WaitForRealSeconds(2f));
        confirmationPrompt.SetActive(false);
    }
    
        private IEnumerator _FadeInUI()
        {
            _fadeIsRunning = true;
            StartCoroutine(FadeInAndOutCoroutine(mainCanvas, true, 0.1f));
            yield return StartCoroutine(WaitForRealSeconds(0.07f));
            StartCoroutine(FadeInAndOutCoroutine(mainGradient, true, 0.1f));
            StartCoroutine(FadeInAndOutCoroutine(bottomGradient, true, 0.1f));
            yield return StartCoroutine(WaitForRealSeconds(0.07f));
            StartCoroutine(FadeInAndOutCoroutine(enterButton, true, 0.1f));
            yield return StartCoroutine(WaitForRealSeconds(0.07f));
            StartCoroutine(FadeInAndOutCoroutine(settingsButton, true, 0.1f));
            yield return StartCoroutine(WaitForRealSeconds(0.07f));
            StartCoroutine(FadeInAndOutCoroutine(exitButton, true, 0.1f));
            yield return StartCoroutine(WaitForRealSeconds(0.07f));
            
            ChangeColor(settingsText);
            ChangeColor(_resumeText);
            
            StartCoroutine(FadeInAndOutCoroutine(feedbackButton, true, 0.1f));
            yield return StartCoroutine(WaitForRealSeconds(0.07f));
            StartCoroutine(FadeInAndOutCoroutine(controlsButton, true, 0.1f));
            yield return StartCoroutine(WaitForRealSeconds(0.1f));
            StartCoroutine(FadeInAndOutCoroutine(logoBlur, true, 0.1f));
            noclipEcho.Play();
            yield return StartCoroutine(WaitForRealSeconds(0.15f));
            StartCoroutine(FadeInAndOutCoroutine(noclipLogo, true, 0.1f));
            _fadeIsRunning = false;
        }
        
        private IEnumerator FadeOutUI()
        {
            _fadeIsRunning = true;
            ChangeColor(settingsText);
            ChangeColor(_resumeText);
            
            StartCoroutine(FadeInAndOutCoroutine(exitButton, false, 0.05f));
            yield return new WaitForSecondsRealtime(0.05f);
            StartCoroutine(FadeInAndOutCoroutine(settingsButton, false, 0.05f));
            yield return new WaitForSecondsRealtime(0.05f);
            StartCoroutine(FadeInAndOutCoroutine(enterButton, false, 0.05f));
            yield return new WaitForSecondsRealtime(0.05f);
            StartCoroutine(FadeInAndOutCoroutine(controlsButton, false, 0.05f));
            yield return new WaitForSecondsRealtime(0.05f);
            StartCoroutine(FadeInAndOutCoroutine(feedbackButton, false, 0.05f));
            yield return new WaitForSecondsRealtime(0.05f);
            StartCoroutine(FadeInAndOutCoroutine(noclipLogo, false, 0.05f));
            yield return new WaitForSecondsRealtime(0.05f);
            StartCoroutine(FadeInAndOutCoroutine(logoBlur, false, 0.05f));
            yield return new WaitForSecondsRealtime(0.05f);
            StartCoroutine(FadeInAndOutCoroutine(mainGradient, false, 0.05f));
            yield return new WaitForSecondsRealtime(0.05f);
            StartCoroutine(FadeInAndOutCoroutine(bottomGradient, false, 0.05f));
            yield return new WaitForSecondsRealtime(0.05f);
            StartCoroutine(FadeInAndOutCoroutine(mainCanvas, false, 0.05f));
            _fadeIsRunning = false;
        }
        
        public static IEnumerator WaitForRealSeconds(float time)
        {
            float start = Time.realtimeSinceStartup;
            while (Time.realtimeSinceStartup < start + time)
            {
                yield return null;
            }
        }
    private IEnumerator FadeInAndOutCoroutine(GameObject objectToFade, bool fadeIn, float duration)
        {
                
                float counter = 0f;

                //Set Values depending on if fadeIn or fadeOut
                float a, b;
                if (fadeIn)
                {
                    a = 0;
                    b = 1;
                }
                else
                {
                    a = 1;
                    b = 0;
                }

                int mode = 0;
                Color currentColor = Color.clear;
                
                SpriteRenderer tempSPRenderer = objectToFade.GetComponent<SpriteRenderer>();
                Image tempImage = objectToFade.GetComponent<Image>();
                RawImage tempRawImage = objectToFade.GetComponent<RawImage>();
                MeshRenderer tempRenderer = objectToFade.GetComponent<MeshRenderer>();
                TMP_Text tempText = objectToFade.GetComponent<TMP_Text>();

                //Check if this is a Sprite
                if (tempSPRenderer != null)
                {
                    currentColor = tempSPRenderer.color;
                    mode = 0;
                }
                //Check if Image
                else if (tempImage != null)
                {
                    currentColor = tempImage.color;
                    mode = 1;
                }
                //Check if RawImage
                else if (tempRawImage != null)
                {
                    currentColor = tempRawImage.color;
                    mode = 2;
                }
                //Check if Text 
                else if (tempText != null)
                {
                    currentColor = tempText.color;
                    mode = 3;
                }

                //Check if 3D Object
                else if (tempRenderer != null)
                {
                    currentColor = tempRenderer.material.color;
                    mode = 4;

                    //ENABLE FADE Mode on the material if not done already
                    tempRenderer.material.SetFloat("_Mode", 2);
                    tempRenderer.material.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
                    tempRenderer.material.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
                    tempRenderer.material.SetInt("_ZWrite", 0);
                    tempRenderer.material.DisableKeyword("_ALPHATEST_ON");
                    tempRenderer.material.EnableKeyword("_ALPHABLEND_ON");
                    tempRenderer.material.DisableKeyword("_ALPHAPREMULTIPLY_ON");
                    tempRenderer.material.renderQueue = 3000;
                }
                else
                {
                    yield break;
                }

                while (counter < duration)
                {
                    counter += Time.unscaledDeltaTime;
                    float alpha = Mathf.Lerp(a, b, counter / duration);

                    switch (mode)
                    {
                        case 0:
                            tempSPRenderer.color = new Color(currentColor.r, currentColor.g, currentColor.b, alpha);
                            break;
                        case 1:
                            tempImage.color = new Color(currentColor.r, currentColor.g, currentColor.b, alpha);
                            break;
                        case 2:
                            tempRawImage.color = new Color(currentColor.r, currentColor.g, currentColor.b, alpha);
                            break;
                        case 3:
                            tempText.color = new Color(currentColor.r, currentColor.g, currentColor.b, alpha);
                            break;
                        case 4:
                            tempRenderer.material.color = new Color(currentColor.r, currentColor.g, currentColor.b, alpha);
                            break;
                    }
                    
                    
                    yield return null;
                }
        }
}
