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

    void Awake()
    {
        _gameManager = GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameManager>();
    }
    
    private void OnTriggerEnter(Collider otherObject)
    {
        if (otherObject.CompareTag("RealityPlayer"))
        {
            Debug.Log("you entered checkpoint: " + gameObject.name);
            _gameManager.SetLastCheckpointPos(transform.position);
            Physics.SyncTransforms();
            ChangeBlockColor();
        }
    }

    private void ChangeBlockColor(){
        //change color of _gameObject
        _gameObject.GetComponent<Renderer>().material.color = Color.green;
    }

}
