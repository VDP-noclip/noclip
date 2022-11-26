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

    private List<BaseNoclipObjectController> _noclipObjControllers;
    private ObjectMaterialSwitcher[] _objectMaterialSwitchers;
    private CameraManager _cameraManager;

    private bool _noclipEnabled;   
    private bool _playerCanEnableNoclip;
    private bool _playerCanDisableNoclip;

    void Awake()
    {
        _cameraManager = FindObjectOfType<CameraManager>();
        RenderSettings.skybox = _noclipOptions.realitySkyboxMaterial;
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
    /// Set this to true when the player is in the noclipEnabler with its body.
    /// </summary>
    public void SetPlayerCanEnableNoClip(bool playerCanEnableNoclip)
    {
        _playerCanEnableNoclip = playerCanEnableNoclip;
        if (playerCanEnableNoclip)
            EventManager.TriggerEvent("DisplayHint", "PRESS P TO NOCLIP");
        else
            EventManager.TriggerEvent("ClearHints");
    }

    /// <summary>
    /// Set this to true when the player moves the noclip camera close enough to the real body.
    /// </summary>
    public void SetPlayerCanDisableNoclip(bool playerCanDisableNoclip)
    {
        _playerCanDisableNoclip = playerCanDisableNoclip;
    }
    
    /// <summary>
    /// Allow the player to disable no clip, or disable noclip automatically
    /// </summary>
    public void NoClipReturnedToBody()
    {
        _playerCanDisableNoclip = true;
        if (_noclipOptions.automaticReturnToBody)
            StartCoroutine(DisableNoclip());
    }
    
    /// <summary>
    /// Inform noclip manager that the noclip camera is outside the body and block the option to disable noclip mode
    /// </summary>
    public void NoClipExitedToBody()
    {
        _playerCanDisableNoclip = false;
    }

    public bool IsNoclipEnabled()
    {
        return _noclipEnabled;
    }

    /// <summary>
    /// Activate the noclip mode to all the objects and switch camera to the noclip one.
    /// </summary>
    private IEnumerator EnableNoclip()
    {
        _playerCanEnableNoclip = false;
        _noclipObjControllers.ForEach(obj => obj.ActivateNoclip());
        _noclipEnabled = true;
        _cameraManager.SwitchCamera();
        RenderNoclipMode();
        yield return null;
    }

    /// <summary>
    /// Deactivate the noclip mode to all the objects and switch camera to the normal one.
    /// </summary>
    private IEnumerator DisableNoclip()
    {
        _playerCanDisableNoclip = false;
         _noclipObjControllers.ForEach(obj => obj.DisableNoclip());
        _noclipEnabled = false;
        _cameraManager.SwitchCamera();
        // The camera/bodies are still in the correct area, this should be set to false when the player exits
        // the platform
        _playerCanEnableNoclip = true;
        RenderRealityMode();
        yield return null;
    }


    /// <summary>
    /// Allows the player to enable/disable noclip mode.
    /// </summary>
    private void Update()
    {
        if (Input.GetKeyDown(_noclipOptions.noclipKey))
        {
            if (_playerCanEnableNoclip)
                StartCoroutine(EnableNoclip());
            else if (_playerCanDisableNoclip)
                StartCoroutine(DisableNoclip());
            else if (!_noclipEnabled)
                EventManager.TriggerEvent("DisplayHint", "NOCLIP ZONE NOT FOUND. PRESSING P HAS NO EFFECT"); 
            else if (_noclipEnabled)
                EventManager.TriggerEvent("DisplayHint", "RETURN TO YOUR BODY TO DISABLE NOCLIP"); 

            //switch materials
            GameObject environment = GameObject.Find("Environment");
            try{
                //find RealityObjectsHolder among the children of Environment
                GameObject realityObjectsHolder = environment.transform.Find("RealityObjectsHolder").gameObject;
                //find AutomaticNoclipMaterial
                AutomaticNoclipMaterial automaticNoclipMaterial = realityObjectsHolder.GetComponent<AutomaticNoclipMaterial>();
                //call SwitchMaterials
                automaticNoclipMaterial.SwitchMaterials(_noclipEnabled);
            }
            catch{
                Debug.LogError("Problem with " + environment.name);
            }
            //gameobject.find puzzles
            GameObject puzzles = GameObject.Find("Puzzles");
            //for each endabled child of puzzles find RealityObjectsHolder
            foreach (Transform puzzle in puzzles.transform)
            {
                try{
                    //find RealityObjectsHolder
                    GameObject realityObjectsHolder = puzzle.Find("RealityObjectsHolder").gameObject;
                    //find AutomaticNoclipMaterial
                    AutomaticNoclipMaterial automaticNoclipMaterial = realityObjectsHolder.GetComponent<AutomaticNoclipMaterial>();
                    //call SwitchMaterials
                    automaticNoclipMaterial.SwitchMaterials(_noclipEnabled);
                }
                catch{
                    Debug.LogError("Problem with " + puzzle.name);
                }
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