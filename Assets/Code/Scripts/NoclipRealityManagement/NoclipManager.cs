using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/// <summary>
/// This class manages the switch between noclip and normal mode. It calls Noclip() of all the noclip objects and
/// changes the camera from the noclip to the normal one, only when certain conditions are met.
/// </summary>
public class NoclipManager : MonoBehaviour
{
    [SerializeField] private KeyCode noclipKey = KeyCode.P;

    private List<NoclipObjController> _noclipObjControllers;
    private CameraManager _cameraManager;

    private bool _playerCanEnableNoclip;
    private bool _playerCanDisableNoclip;

    void Awake()
    {
        GameObject[] noclipObjects = GameObject.FindGameObjectsWithTag("NoclipObject");
        _noclipObjControllers = noclipObjects.Select(
            obj => obj.GetComponent<NoclipObjController>()).ToList();
        
        _cameraManager = FindObjectOfType<CameraManager>();
    }

    /// <summary>
    /// Set this to true when the player is in the noclipEnabler with its body.
    /// </summary>
    public void SetPlayerCanEnableNoClip(bool playerCanEnableNoclip)
    {
        _playerCanEnableNoclip = playerCanEnableNoclip;
    }

    /// <summary>
    /// Set this to true when the player moves the noclip camera close enough to the real body.
    /// </summary>
    public void SetPlayerCanDisableNoclip(bool playerCanDisableNoclip)
    {
        _playerCanDisableNoclip = playerCanDisableNoclip;
    }

    /// <summary>
    /// Activate the noclip mode to all the objects and switch camera to the noclip one.
    /// </summary>
    private void EnableNoclip()
    {
        _noclipObjControllers.ForEach(obj => obj.Noclip());
        _cameraManager.SwitchCamera();
    }

    /// <summary>
    /// Deactivate the noclip mode to all the objects and switch camera to the normal one.
    /// </summary>
    private void DisableNoclip()
    {
        _noclipObjControllers.ForEach(obj => obj.Noclip());
        _cameraManager.SwitchCamera();
    }

    /// <summary>
    /// Allows the player to enable/disable noclip mode.
    /// </summary>
    private void Update()
    {
        if (Input.GetKeyDown(noclipKey) && _playerCanEnableNoclip)
        {
            EnableNoclip();
            _playerCanEnableNoclip = false;
        }

        if (Input.GetKeyDown(noclipKey) && _playerCanDisableNoclip)
        {
            DisableNoclip();
            _playerCanDisableNoclip = false;
            // The camera/bodies are still in the correct area, this should be set to false when the player exits
            // the platform
            _playerCanEnableNoclip = true;
        }
    }
}