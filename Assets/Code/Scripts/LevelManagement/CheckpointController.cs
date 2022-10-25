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

    void Awake()
    {
        _gameManager = GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameManager>();
    }
    
    private void OnTriggerEnter(Collider otherObject)
    {
        if (otherObject.CompareTag("RealityPlayer"))
        {
            _gameManager.SetLastCheckpointPos(transform.position);
            Physics.SyncTransforms();
        }
    }
}
