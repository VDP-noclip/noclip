using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public class RealityPlayerCollisions : MonoBehaviour
{

    private RespawningManager _respawningManager;
    private NoclipManager _noclipManager;
    private bool _touchingNoclipEnabler;
    
    private void Awake()
    {
        _noclipManager = GetComponent<NoclipManager>();
        _respawningManager = GetComponentInParent<RespawningManager>();
    }

    private void Update()
    {
        if (CanCallNoclip() && Input.GetKeyDown(KeyCode.E))
        {
            if (_noclipManager.NoclipEnabled)
            {
                _noclipManager.DisableNoclip();
            }
            else
            {
                _noclipManager.EnableNoclip();
            }
        }
    }

    private void OnTriggerEnter(Collider other) {
        if (other.CompareTag("NoclipEnabler"))
        {
            _touchingNoclipEnabler = true;
        }
        else if (other.CompareTag("Checkpoint"))
        {
            _respawningManager.UpdateCheckpointValues();
        }
        else if (other.CompareTag("OutOfBounds"))
        {
            _respawningManager.RespawnAllTransforms();
        }
        else if (other.CompareTag("ProgressSaver"))
        {
            Debug.Log("I'm saving this scene: " + SceneManager.GetActiveScene().name);
            PlayerPrefs.SetString("SavedLevel", SceneManager.GetActiveScene().name);
        }
    }
    
    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("NoclipEnabler"))
        {
            _touchingNoclipEnabler = false;
        }
    }

    /// <summary>
    /// Check if we can call noclip method. If we are on the platform, we can call the method to enable/disable the
    /// noclip mode!
    /// </summary>
    private bool CanCallNoclip()
    {
        return _touchingNoclipEnabler;
    }
}
