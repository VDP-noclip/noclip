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
    [SerializeField] private float _coolDownSeconds = 5f;

    private List<BaseNoclipObjectController> _noclipObjControllers;
    private ObjectMaterialSwitcher[] _objectMaterialSwitchers;
    private CameraManager _cameraManager;
    
    private bool _playerInsideNoclipEnabler;
    private bool _insideNoclipAreaZoneIsPlaying;
    private NoclipState _noclipState;
    
    private GameObject _noclipCamera;
    private GameObject _realityCamera;
    private GameObject _postprocessReality;
    private GameObject _postprocessNoclip;
    
    private NoclipMovement _noclipMovement;

    private enum NoclipState
    {
        RealityCannotEnableNoclip,
        RealityCooldown,
        RealityCanEnableNoclip,
        NoclipEnabled,
        GoingBackToBody
    }

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
        _noclipState = NoclipState.RealityCannotEnableNoclip;
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
    public void SetPlayerIsInsideNoclipEnabler(bool playerInsideNoclipEnabler)
    {
        Debug.Log("SetPlayerIsInsideNoclipEnabler: " + playerInsideNoclipEnabler);
        _playerInsideNoclipEnabler = playerInsideNoclipEnabler;
        if (_noclipState == NoclipState.RealityCooldown || _noclipState == NoclipState.GoingBackToBody)
        {
            Debug.Log("Transitioning...");
            return;
        }
        
        if (playerInsideNoclipEnabler && _noclipState == NoclipState.RealityCannotEnableNoclip)
        {
            EventManager.TriggerEvent("DisplayHint", _noclipOptions.howToActivateNoclip);
            _noclipState = NoclipState.RealityCanEnableNoclip;
            return;
        }
        
        if (!playerInsideNoclipEnabler && _noclipState == NoclipState.RealityCanEnableNoclip)
        {
            EventManager.TriggerEvent("ClearHints");
            _noclipState = NoclipState.RealityCannotEnableNoclip;
        }

    }
    
    public bool IsNoclipEnabled()
    {
        return _noclipState == NoclipState.NoclipEnabled;
    }
    

    /// <summary>
    /// Activate the noclip mode to all the objects and switch camera to the noclip one.
    /// </summary>
    private IEnumerator EnableNoclip()
    {
        EventManager.TriggerEvent("ClearHints");
        Debug.Log("Enablenoclip");
        _noclipState = NoclipState.NoclipEnabled;
        _postprocessReality.SetActive(false);
        _postprocessNoclip.SetActive(true);
        
        _effectsAudioSource.PlayOneShot(_audioTracks.enableNoclip);
        _noclipObjControllers.ForEach(obj => {
            try {
                obj.ActivateNoclip();
            } catch{
                //log error problem with a noclip object controller
                Debug.LogError("Error with a noclip object controller");
            }
        });
        _cameraManager.SwitchCamera();
        RenderNoclipMode();
        Debug.Log("Enablenoclip finished. Current state: " + _noclipState);
        yield return null;
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
        _noclipObjControllers.ForEach(obj => {
            try {
                obj.DisableNoclip();
            } catch{
                //log error problem with a noclip object controller
                Debug.LogError("Error with a noclip object controller");
            }
        });
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
            if (_noclipState == NoclipState.RealityCanEnableNoclip)
                StartCoroutine(EnableNoclip());
            else if (_noclipState == NoclipState.RealityCooldown)
                EventManager.TriggerEvent("DisplayHint", "Wait for cooldown...");
            else 
                EventManager.TriggerEvent("DisplayHint", _noclipOptions.tryToActivateNoclipOutside);
        }

        // When releasing
        if (Input.GetButtonUp("Noclip") && _noclipState == NoclipState.NoclipEnabled)
        {
            Debug.Log("call GoBackToBodyCoroutine");
            StartCoroutine(GoBackToBodyCoroutine());
        }
        
    }

    public void NoclipRespawnSequence()
    {
        Debug.Log("NoclipRespawnSequence TODO, with Tizio!");
    }

    private IEnumerator GoBackToBodyCoroutine()
    {
        _noclipState = NoclipState.GoingBackToBody;
        _noclipMovement.SetEnableMovement(false);
        while (!IsBackToBody())
        {
            _noclipCamera.transform.position = Vector3.Lerp(_noclipCamera.transform.position, _realityCamera.transform.position, 0.1f);
            _noclipCamera.transform.rotation = Quaternion.Lerp(_noclipCamera.transform.rotation, _realityCamera.transform.rotation, 0.1f);
            yield return new WaitForSeconds(0.01f);
        }
        yield return DisableNoclip();
        
        // Cooldown phase
        _noclipState = NoclipState.RealityCooldown;
        yield return new WaitForSecondsRealtime(_coolDownSeconds);

        if (_playerInsideNoclipEnabler)
        {
            EventManager.TriggerEvent("ClearHints");
            _noclipState = NoclipState.RealityCanEnableNoclip;
        }
        else
            _noclipState = NoclipState.RealityCannotEnableNoclip;
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
            obj => {
            BaseNoclipObjectController objCtrl = obj.GetComponent<BaseNoclipObjectController>();
            //if null log error name of the object
            if (objCtrl == null){
                Debug.LogError(obj.name + " has no BaseNoclipObjectController " + obj);
            }
            return objCtrl;
        }).ToList();
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
            /*
            else if (obj.CompareTag("RealityObject"))
                noclipMaterials = _noclipOptions.noClipMaterialsForRealityObjects;
            */
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
        if (_playerInsideNoclipEnabler && !_insideNoclipAreaZoneIsPlaying)
        {
            Debug.Log("Start NoclipZoneSound");
            // Disabled clip since we're not using it.
            // _noclipZoneAudioSource.PlayOneShot(_audioTracks.noclipZoneSound); 
            _insideNoclipAreaZoneIsPlaying = true;
        }

        if (_insideNoclipAreaZoneIsPlaying && (!_playerInsideNoclipEnabler || _noclipState != NoclipState.NoclipEnabled))
        {
            _noclipZoneAudioSource.Stop();
            _insideNoclipAreaZoneIsPlaying = false;
        }

        yield return null;
    }
}