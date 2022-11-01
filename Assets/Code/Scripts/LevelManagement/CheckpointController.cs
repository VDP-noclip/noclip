using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// CheckpointController checks if the player has collided with a checkpoint.
/// If a new checkpoint is hit, then it'll be overwritten.
/// </summary>
public class CheckpointController : MonoBehaviour
{
    private GameManager _gameManager;
    [SerializeField] private GameObject _gameObject;
    private bool _checkpointEnabled = false;
    void Awake()
    {
        _gameManager = GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameManager>();
    }
    
    private void OnTriggerEnter(Collider otherObject)
    {
        if(_checkpointEnabled){
            if (otherObject.CompareTag("RealityPlayer"))
            {
                _checkpointEnabled = false;
                Debug.Log("you entered checkpoint: " + gameObject.name);
                _gameManager.SetLastCheckpointPos(transform.position);
                _gameManager.ActivateNextCheckpoint();
                Physics.SyncTransforms();
                CheckpointReached();
            }
        }
    }

    private void CheckpointReached(){
        //change color of _gameObject
        _gameObject.GetComponent<Renderer>().material.color = Color.green;
    }

    public void ActivateCheckpoint(){
        _checkpointEnabled = true;
        _gameObject.GetComponent<Renderer>().material.color = Color.red;
    }
}
