using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckpointController : MonoBehaviour
{
    private GameManager _gameManager;
    /*
     * CheckpointController checks if the player has collided with a checkpoint.
     * If a new checkpoint is hit, then it'll be overwritten.
     */
    void Start()
    {
        _gameManager = GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameManager>();
    }
    private void OnTriggerEnter(Collider otherObject)
    {
        if (otherObject.gameObject.name == "RealityPlayer")
        {
            _gameManager.lastCheckPointPos = transform.position;
            Physics.SyncTransforms();
        }
    }
}
