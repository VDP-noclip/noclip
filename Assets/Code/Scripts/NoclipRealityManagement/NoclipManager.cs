using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Code.ScriptableObjects;
using Code.Scripts.Utils;
using POLIMIGameCollective;
using UnityEngine.Audio;


/// <summary>
/// This class manages the switch between noclip and normal mode. It calls Noclip() of all the noclip objects and
/// changes the camera from the noclip to the normal one, only when certain conditions are met.
/// </summary>
public class NoclipManager : MonoBehaviour
{
    [SerializeField] private NoclipOptions _noclipOptions;
    [SerializeField] private AudioSource _effectsAudioSource;
    [SerializeField] private AudioSource _noclipZoneAudioSource;
    [SerializeField] private AudioTracks _audioTracks;

    private List<BaseNoclipObjectController> _noclipObjControllers;
    private ObjectMaterialSwitcher[] _objectMaterialSwitchers;
    private CameraManager _cameraManager;

    private bool _noclipEnabled;
    private bool _playerCanSwitchMode;
    private bool _insideNoclipAreaZoneIsPlaying;
    
    private bool _goingBackToBody;
    private GameObject _noclipCamera;
    private GameObject _realityCamera;
    private GameObject _postprocessReality;
    private GameObject _postprocessNoclip;
    
    private NoclipMovement _noclipMovement;

    void Awake()
    {
        _cameraManager = FindObjectOfType<CameraManager>();
        RenderSettings.skybox = _noclipOptions.realitySkyboxMaterial;
        GetReadyForPuzzle();
        _noclipZoneAudioSource.volume = _audioTracks.noClipSoundVolumeMultiplier;
        //find NoclipCamera
        _noclipCamera = GameObject.Find("NoclipCamera");
        //NoclipMovement
        _noclipMovement = _noclipCamera.GetComponent<NoclipMovement>();
    }

    private void Start()
    {
        GameObject allplayer = GameObject.Find("AllPlayer");
        GameObject noclipplayer = allplayer.transform.Find("NoclipPlayer").gameObject;
        GameObject realityplayer = allplayer.transform.Find("RealityPlayer").gameObject;

        GameObject environment = GameObject.Find("Environment");
        _postprocessReality = environment.transform.Find("PostProcessingReality").gameObject;
        _postprocessNoclip = environment.transform.Find("PostProcessingNoclip").gameObject;
        
        _noclipCamera = noclipplayer.transform.Find("NoclipCamera").gameObject;
        _realityCamera = realityplayer.transform.Find("RealityCamera").gameObject;
    }

    /// <summary>
    /// To be called at the beginning of each puzzle and in the awake method. It gets the reference to all the objects
    /// needed by the noclip manager.
    /// </summary>
    public void GetReadyForPuzzle()
    {
        StartCoroutine(GetNoClipObjControllers());
        StartCoroutine(GetObjectMaterialSwitchers());
    }

    /// <summary>
    /// Set this to true if you want the player to be able to switch mode
    /// </summary>
    public void SetPlayerCanSwitchMode(bool playerCanSwitchMode)
    {
        _playerCanSwitchMode = playerCanSwitchMode;
        if (!playerCanSwitchMode)
            EventManager.TriggerEvent("ClearHints");
        else if (_noclipEnabled)
            Debug.Log("Ciaooo");
        //    EventManager.TriggerEvent("DisplayHint", _noclipOptions.howToDeactivateNoclip); // disabled because useless
        else
            EventManager.TriggerEvent("DisplayHint", _noclipOptions.howToActivateNoclip);
    }
    
    
    /// <summary>
    /// Allow the player to disable no clip, or disable noclip automatically
    /// </summary>
    public void NoClipReturnedToBody()
    {
        _playerCanSwitchMode = true;
        if (_goingBackToBody && _noclipEnabled)
            StartCoroutine(DisableNoclip());
    }
    
    /// <summary>
    /// Inform noclip manager that the noclip camera is outside the body and block the option to disable noclip mode
    /// </summary>
    public void NoClipExitedToBody()
    {
        _playerCanSwitchMode = false;
        EventManager.TriggerEvent("ClearHints");
    }

    public bool IsNoclipEnabled()
    {
        return _noclipEnabled;
    }

    private void SwitchMode()
    {
        if (_noclipEnabled)
            StartCoroutine(DisableNoclip());
        else
            StartCoroutine(EnableNoclip());
    }

    /// <summary>
    /// Activate the noclip mode to all the objects and switch camera to the noclip one.
    /// </summary>
    private IEnumerator EnableNoclip()
    {
        Debug.Log("Enablenoclip");
        
       
        _postprocessReality.SetActive(false);
        _postprocessNoclip.SetActive(true);
        
        _effectsAudioSource.PlayOneShot(_audioTracks.enableNoclip);
        _noclipObjControllers.ForEach(obj => obj.ActivateNoclip());
        _noclipEnabled = true;
        _goingBackToBody = false;
        _cameraManager.SwitchCamera();
        RenderNoclipMode();
        yield return null;
    }
    
    private void EnableNoclipNow()
    {
        Debug.Log("Enablenoclip");
        
       
        _postprocessReality.SetActive(false);
        _postprocessNoclip.SetActive(true);
        
        _effectsAudioSource.PlayOneShot(_audioTracks.enableNoclip);
        _noclipObjControllers.ForEach(obj => obj.ActivateNoclip());
        _noclipEnabled = true;
        _goingBackToBody = false;
        _cameraManager.SwitchCamera();
        RenderNoclipMode();
    }

    /// <summary>
    /// Deactivate the noclip mode to all the objects and switch camera to the normal one.
    /// </summary>
    private IEnumerator DisableNoclip()
    {
        Debug.Log("Disablenoclip");
        
        _postprocessNoclip.SetActive(false);
        _postprocessReality.SetActive(true);
        
        _noclipMovement.SetEnableMovement(true);
        _effectsAudioSource.PlayOneShot(_audioTracks.disableNoclip);
        _noclipObjControllers.ForEach(obj => obj.DisableNoclip());
        _noclipEnabled = false;
        _goingBackToBody = false;
        _cameraManager.SwitchCamera();
        RenderRealityMode();
        yield return null;
    }


    /// <summary>
    /// Allows the player to enable/disable noclip mode.
    /// </summary>
    private void Update()
    {
        StartCoroutine(StartOrStopNoclipZoneSound());

        // When pressing
        if (Input.GetButtonDown("Noclip"))
        {
            if (_playerCanSwitchMode)
                SwitchMode();
            else if (!_noclipEnabled)
                EventManager.TriggerEvent("DisplayHint", _noclipOptions.tryToActivateNoclipOutside); 
            else if (_noclipEnabled)
                Debug.Log("There was an event here");
                // EventManager.TriggerEvent("DisplayHint", _noclipOptions.tryToDeactivateNoclipOutside);
        }
        
        // When releasing
        if (Input.GetButtonUp("Noclip") && _noclipEnabled){
            _goingBackToBody = true;
            _noclipMovement.SetEnableMovement(false);
            //_noclipMouseLook.CopyRotationCoordinates(_realityMouseLook);  // Add a method that slowly changes
        }
    }

    public void NoclipRespawnSequence(){
        Debug.Log("NoclipRespawnSequence");
        EnableNoclipNow();
        _goingBackToBody = true;
        _noclipMovement.SetEnableMovement(false);
    }

    private void FixedUpdate()
    {
        //if _goingBackToBody slowly move noclipcamera to realitycamera position
        if (_goingBackToBody){
            _noclipCamera.transform.position = Vector3.Lerp(_noclipCamera.transform.position, _realityCamera.transform.position, 0.1f);
            _noclipCamera.transform.rotation = Quaternion.Lerp(_noclipCamera.transform.rotation, _realityCamera.transform.rotation, 0.1f);
            if (IsBackToBody())
                NoClipReturnedToBody();
        }
        //copy noclipcamera rotation to realitycamera rotation
        else{
            if(!_noclipEnabled)
                _noclipCamera.transform.rotation = _realityCamera.transform.rotation;
        }
    }
    
    private bool IsBackToBody()
    {
        float distThreshold = 0.1f;
        int degreesThreshold = 1;
        bool backToBody = Vector3.Distance(_noclipCamera.transform.position, _realityCamera.transform.position) < distThreshold;
        bool sameRealCameraOrientation = Quaternion.Angle(_noclipCamera.transform.rotation, _realityCamera.transform.rotation) < degreesThreshold;
        return backToBody && sameRealCameraOrientation;
    }

    private IEnumerator GetNoClipObjControllers()
    {
        GameObject[] noclipObjects = GameObject.FindGameObjectsWithTag("NoclipObject");
        _noclipObjControllers = noclipObjects.Select(
            obj => obj.GetComponent<BaseNoclipObjectController>()).ToList();
        yield return null;
    }
    
    private IEnumerator GetObjectMaterialSwitchers()
    {
        GameObject[] realityObjects = GameObject.FindGameObjectsWithTag("RealityObject");
        GameObject[] noclipObjects = GameObject.FindGameObjectsWithTag("NoclipObject");
        GameObject[] backgroundObjects = GameObject.FindGameObjectsWithTag("Background");
        List<GameObject> gameObjectsToChange = new();
        gameObjectsToChange.AddRange(realityObjects);
        gameObjectsToChange.AddRange(backgroundObjects);
        gameObjectsToChange.AddRange(noclipObjects);

        ObjectMaterialSwitcher[] objectMaterialSwitchers = new ObjectMaterialSwitcher[gameObjectsToChange.Count];
        
        for (int i = 0; i < gameObjectsToChange.Count; i++)
        {
            Material[] noclipMaterials;
            GameObject obj = gameObjectsToChange[i];
            NoclipMaterialHolder noclipMaterialHolder = obj.GetComponent<NoclipMaterialHolder>();
            
            if (noclipMaterialHolder)
                noclipMaterials = noclipMaterialHolder.GetNoclipMaterials();
            else if (obj.CompareTag("Background"))
                noclipMaterials = _noclipOptions.noClipMaterialsForBackgroundObjects;
            else if (obj.CompareTag("RealityObject"))
                noclipMaterials = _noclipOptions.noClipMaterialsForRealityObjects;
            else
                continue;

            ObjectMaterialSwitcher objectMaterialSwitcher = new ObjectMaterialSwitcher(obj, noclipMaterials);
            objectMaterialSwitchers[i] = objectMaterialSwitcher;
        }
        _objectMaterialSwitchers = objectMaterialSwitchers;
        
        yield return null;
    }

    private void RenderNoclipMode()
    {
        RenderSettings.skybox = _noclipOptions.noClipSkyboxMaterial;
        foreach (var objectMaterialSwitcher in _objectMaterialSwitchers)
        {
            if(objectMaterialSwitcher != null)
            {
                objectMaterialSwitcher.SetNoclipMaterials();
            }
        }
    }
    
    private void RenderRealityMode()
    {
        RenderSettings.skybox = _noclipOptions.realitySkyboxMaterial;
        foreach (var objectMaterialSwitcher in _objectMaterialSwitchers)
        {
            if(objectMaterialSwitcher != null)
            {
                objectMaterialSwitcher.SetOriginalMaterials();
            }
        }
    }

    private IEnumerator StartOrStopNoclipZoneSound()
    {
        if (_playerCanSwitchMode && !_insideNoclipAreaZoneIsPlaying)
        {
            Debug.Log("Start NoclipZoneSound");
            // Disabled clip since we're not using it.
            // _noclipZoneAudioSource.PlayOneShot(_audioTracks.noclipZoneSound); 
            _insideNoclipAreaZoneIsPlaying = true;
        }

        if (_insideNoclipAreaZoneIsPlaying && (!_playerCanSwitchMode || _noclipEnabled))
        {
            _noclipZoneAudioSource.Stop();
            _insideNoclipAreaZoneIsPlaying = false;
        }

        yield return null;
    }
}