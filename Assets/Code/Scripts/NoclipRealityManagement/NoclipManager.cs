using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Code.ScriptableObjects;
using Code.Scripts.Utils;
using POLIMIGameCollective;

/// <summary>
/// This class manages the switch between noclip and normal mode. It correctly renders the noclip objects and
/// changes the camera from the noclip to the normal one, only when certain conditions are met.
/// </summary>
public class NoclipManager : MonoBehaviour
{
    [SerializeField] private NoclipOptions _noclipOptions;
    [SerializeField] private AudioSource _effectsAudioSource;
    [SerializeField] private AudioSource _noclipZoneAudioSource;
    [SerializeField] private AudioTracks _audioTracks;
    [SerializeField] private GameObject _noclipCamera;
    [SerializeField] private GameObject _realityCamera;

    private List<BaseNoclipObjectController> _noclipObjControllers;
    private ObjectMaterialSwitcher[] _objectMaterialSwitchers;
    private CameraManager _cameraManager;
    
    private bool _playerInsideNoclipEnabler;
    private bool _insideNoclipAreaZoneIsPlaying;
    private bool _acceptUserInput = true;
    private NoclipState _noclipState;
    
    private GameObject _postprocessReality;
    private GameObject _postprocessNoclip;
    
    private NoclipMovement _noclipMovement;
    private float _endCooldownAbsoluteTime;

    private float _animationSlowdownFactor;
    private bool _animatedObjectsPresent = false;
    //gameobject list animated objects
    private List<GameObject> _animatedObjects = new List<GameObject>();
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
        GetReadyForPuzzle();
        _noclipZoneAudioSource.volume = _audioTracks.noClipSoundVolumeMultiplier;
        _noclipMovement = _noclipCamera.GetComponent<NoclipMovement>();
        _noclipState = NoclipState.RealityCannotEnableNoclip;
    }

    private void Start()
    {
        GameObject environment = GameObject.Find("Environment");
        _postprocessReality = environment.transform.Find("PostProcessingReality").gameObject;
        _postprocessNoclip = environment.transform.Find("PostProcessingNoclip").gameObject;

        AnimatedMaterialsSetup();
    }

    private void AnimatedMaterialsSetup()
    {
        //find AnimatedMaterialsHolder
        GameObject animatedObjectsHolder = GameObject.Find("AnimatedObjectsHolder");
        if (animatedObjectsHolder != null)
        {
            _animatedObjectsPresent = true;
            AnimatedMaterials animatedMaterials = animatedObjectsHolder.GetComponent<AnimatedMaterials>();
            _animationSlowdownFactor = animatedMaterials.GetNoclipSlowdownFactor();
            //add all children to _animatedObjects
            foreach (Transform child in animatedObjectsHolder.transform)
            {
                _animatedObjects.Add(child.gameObject);
            }
        }
        else
        {
            _animatedObjectsPresent = false;
            //log no animated objects found
            Debug.Log("No animated objects found");
        }
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
    /// Tells the noclip manager if the player is inside the noclip enabler. If needed, the noclipState changes.
    /// </summary>
    public void SetPlayerIsInsideNoclipEnabler(bool playerInsideNoclipEnabler)
    {
        _playerInsideNoclipEnabler = playerInsideNoclipEnabler;
        if (_noclipState is NoclipState.RealityCooldown or NoclipState.GoingBackToBody or NoclipState.NoclipEnabled)
        {
            // Transitioning, we don't have to do anything!
            return;
        }
        
        if (playerInsideNoclipEnabler && _noclipState == NoclipState.RealityCannotEnableNoclip)
        {
            if (_acceptUserInput)
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

    public bool RealityPlayerCanMove()
    {
        return _noclipState is NoclipState.RealityCooldown or NoclipState.RealityCanEnableNoclip
            or NoclipState.RealityCannotEnableNoclip;
    }

    public void SetAcceptUserInput(bool acceptUserInput)
    {
        _acceptUserInput = acceptUserInput;
    }

    /// <summary>
    /// Activate the noclip mode to all the objects and switch camera to the noclip one.
    /// </summary>
    private IEnumerator EnableNoclip()
    {
        SetAnimationSpeed("slow");

        EventManager.TriggerEvent("ClearHints");
        _noclipState = NoclipState.NoclipEnabled;
        EventManager.TriggerEvent("PauseTimeConstraintsTimer");
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
        yield return null;
    }

    /// <summary>
    /// Deactivate the noclip mode to all the objects and switch camera to the normal one.
    /// </summary>
    private IEnumerator DisableNoclip()
    {
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
        EventManager.TriggerEvent("ResumeTimeConstraintsTimer");
        yield return null;
    }

    private void SetAnimationSpeed(string how){
        if(_animatedObjectsPresent){
            //for each object in _animatedObjects
            foreach (var obj in _animatedObjects)
            {
                try {
                    //get material
                    Material material = obj.GetComponent<Renderer>().material;
                    //get scroll speed
                    float scrollSpeed = material.GetFloat("_ScrollSpeed");
                    //set new scroll speed
                    if(how == "slow"){
                        material.SetFloat("_ScrollSpeed", scrollSpeed/_animationSlowdownFactor);
                    } else if(how == "normal"){
                        material.SetFloat("_ScrollSpeed", scrollSpeed*_animationSlowdownFactor);
                    }
                    else
                    {
                        throw new Exception($"Invalid value. Got '{how}'");
                    }
                } catch {
                    Debug.LogError("Invalid animated object " + obj.name);
                }
            }
        }
    }
    
    private void Update()
    {
        //StartCoroutine(StartOrStopNoclipZoneSound());

        // When pressing
        if (Input.GetButtonDown("Noclip") && _acceptUserInput)
        {
            switch (_noclipState)
            {
                case NoclipState.RealityCanEnableNoclip:
                    StartCoroutine(EnableNoclip());
                    break;
                case NoclipState.RealityCooldown:
                    int secondsToWait = Mathf.CeilToInt(_endCooldownAbsoluteTime - Time.time);
                    EventManager.TriggerEvent("DisplayHint", $"Noclip cooldown, wait {secondsToWait}s!");
                    break;
                default:
                    EventManager.TriggerEvent("DisplayHint", _noclipOptions.tryToActivateNoclipOutside);
                    break;
            }
        }

        // When releasing
        if (Input.GetButtonUp("Noclip") && _noclipState == NoclipState.NoclipEnabled && _acceptUserInput)
        {
            StartCoroutine(GoBackToBodyCoroutine());
        }
        
    }

    private IEnumerator GoBackToBodyCoroutine()
    {
        _noclipState = NoclipState.GoingBackToBody;
        _noclipMovement.SetEnableMovement(false);
        
        // Go back to body animation
        float timeElapsed = 0;
        Vector3 startPosition = _noclipCamera.transform.position;
        Quaternion startAngle = _noclipCamera.transform.rotation;
        while (!IsBackToBody())
        {
            timeElapsed += Time.deltaTime;
            float t = timeElapsed / _noclipOptions.backToBodyAnimationDuration;
            _noclipCamera.transform.position = Vector3.Lerp(startPosition, _realityCamera.transform.position, t);
            _noclipCamera.transform.rotation = Quaternion.Lerp(startAngle, _realityCamera.transform.rotation, t);
            yield return new WaitForEndOfFrame();
        }
        
        yield return DisableNoclip();
        
        // Cooldown phase
        if (_noclipOptions.cooldownSeconds > 0)
            _noclipState = NoclipState.RealityCooldown;
            _endCooldownAbsoluteTime = Time.time + _noclipOptions.cooldownSeconds;
            yield return new WaitForSecondsRealtime(_noclipOptions.cooldownSeconds);
        
        // Cooldown is over, update noclipState!
        if (_playerInsideNoclipEnabler)
        {
            _noclipState = NoclipState.RealityCanEnableNoclip;
            EventManager.TriggerEvent("DisplayHint", _noclipOptions.howToActivateNoclip);
        }
        else
        {
            _noclipState = NoclipState.RealityCannotEnableNoclip;
            EventManager.TriggerEvent("ClearHints");
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
        EventManager.TriggerEvent("SetNoclipSkybox");
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
        EventManager.TriggerEvent("SetRealitySkybox");
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