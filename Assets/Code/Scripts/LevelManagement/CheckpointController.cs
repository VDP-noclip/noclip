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

    /// <summary>
    /// Call this function if the player collides with the checkpoint.
    /// </summary>
    public void ReactToPlayerCollision()
    {
        if (_checkpointEnabled)
        {
            _checkpointEnabled = false;
            Debug.Log("You entered checkpoint: " + gameObject.name);
            _gameManager.ActivateNextCheckpoint();
            Physics.SyncTransforms();
            CheckpointReached();
            //disable the collider of this object
            GetComponent<Collider>().enabled = false;
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
