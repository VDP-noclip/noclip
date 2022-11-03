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

    private void OnTriggerEnter(Collider other) {
        if (other.CompareTag("NoclipEnabler"))
        {
            _noclipManager.SetPlayerCanEnableNoClip(true);
        }
        else if (other.CompareTag("Checkpoint"))
        {
            other.GetComponent<CheckpointController>().ReactToPlayerCollision();
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
        else if (other.CompareTag(("GoalPlatform")))
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
        }
    }
    
    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("NoclipEnabler"))
        {
            _noclipManager.SetPlayerCanEnableNoClip(false);
        }
    }
}
