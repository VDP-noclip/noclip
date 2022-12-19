using System;
using System.Collections;
using System.Collections.Generic;
using POLIMIGameCollective;
using UnityEngine;

public class DialogueBoxController : MonoBehaviour
{
    [SerializeField] private string boxDialogue;
    private BoxCollider collider;

    private void Start()
    {
        collider = gameObject.GetComponent<BoxCollider>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("RealityPlayer"))
        {
            EventManager.TriggerEvent("DisplayDialogue", boxDialogue);  // We need to pass also the time
            collider.enabled = false;
        }
    }
    
}
