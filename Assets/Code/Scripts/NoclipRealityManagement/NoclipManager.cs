using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Code.ScriptableObjects;
using Code.Scripts.Utils;
using POLIMIGameCollective;


/// <summary>
/// This class manages the switch between noclip and normal mode. It calls Noclip() of all the noclip objects and
/// changes the camera from the noclip to the normal one, only when certain conditions are met.
/// </summary>
public class NoclipManager : MonoBehaviour
{
    [SerializeField] private NoclipOptions _noclipOptions;
    [SerializeField] private AudioSource _audioSource;
    [SerializeField] private AudioTracks _audioTracks;

    private List<BaseNoclipObjectController> _noclipObjControllers;
    private ObjectMaterialSwitcher[] _objectMaterialSwitchers;
    private CameraManager _cameraManager;

    private bool _noclipEnabled;
    private bool _playerCanSwitchMode;
    
    private bool _goingBackToBody;
    private GameObject _noclipCamera;
    private GameObject _realityCamera;
    
    void Awake()
    {
        _cameraManager = FindObjectOfType<CameraManager>();
        RenderSettings.skybox = _noclipOptions.realitySkyboxMaterial;
        GetReadyForPuzzle();
    }

    private void Start()
    {
        GameObject allplayer = GameObject.Find("AllPlayer");
        GameObject noclipplayer = allplayer.transform.Find("NoclipPlayer").gameObject;
        GameObject realityplayer = allplayer.transform.Find("RealityPlayer").gameObject;
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
            EventManager.TriggerEvent("DisplayHint", _noclipOptions.howToDeactivateNoclip);
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
        _audioSource.PlayOneShot(_audioTracks.EnableNoclip);
        _noclipObjControllers.ForEach(obj => obj.ActivateNoclip());
        _noclipEnabled = true;
        _goingBackToBody = false;
        _cameraManager.SwitchCamera();
        RenderNoclipMode();
        yield return null;
    }

    /// <summary>
    /// Deactivate the noclip mode to all the objects and switch camera to the normal one.
    /// </summary>
    private IEnumerator DisableNoclip()
    {
        Debug.Log("Disablenoclip");
        _audioSource.PlayOneShot(_audioTracks.DisableNoclip);
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
        // When pressing
        if (Input.GetKeyDown(_noclipOptions.noclipKey))
        {
            if (_playerCanSwitchMode)
                SwitchMode();
            else if (!_noclipEnabled)
                EventManager.TriggerEvent("DisplayHint", _noclipOptions.tryToActivateNoclipOutside); 
            else if (_noclipEnabled)
                EventManager.TriggerEvent("DisplayHint", _noclipOptions.tryToDeactivateNoclipOutside);
        }
        
        // When releasing
        if (Input.GetKeyUp(_noclipOptions.noclipKey)){
            _goingBackToBody = true;
            //_noclipMouseLook.CopyRotationCoordinates(_realityMouseLook);  // Add a method that slowly changes
        }
        
    }

    private void FixedUpdate()
    {
        //if _goingBackToBody slowly move noclipcamera to realitycamera position
        if (_goingBackToBody){
            _noclipCamera.transform.position = Vector3.Lerp(_noclipCamera.transform.position, _realityCamera.transform.position, 0.1f);
            _noclipCamera.transform.rotation = Quaternion.Lerp(_noclipCamera.transform.rotation, _realityCamera.transform.rotation, 0.1f);
            //if noclipcamera is close to realitycamera, NoClipReturnedToBody
            //this is extra but if you don't leave the body with noclip it won't trigger enter
            if (Vector3.Distance(_noclipCamera.transform.position, _realityCamera.transform.position) < 0.1f){
                NoClipReturnedToBody();
            }
        }
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
        GameObject[] backgroundObjects = GameObject.FindGameObjectsWithTag("Background");
        List<GameObject> gameObjectsToChange = new();
        gameObjectsToChange.AddRange(realityObjects);
        gameObjectsToChange.AddRange(backgroundObjects);

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
            else
                noclipMaterials = _noclipOptions.noClipMaterialsForRealityObjects;

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
            objectMaterialSwitcher.SetNoclipMaterials();
        }
    }
    
    private void RenderRealityMode()
    {
        RenderSettings.skybox = _noclipOptions.realitySkyboxMaterial;
        foreach (var objectMaterialSwitcher in _objectMaterialSwitchers)
        {
            objectMaterialSwitcher.SetOriginalMaterials();
        }
    }
}