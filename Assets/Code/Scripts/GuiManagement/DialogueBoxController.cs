using System;
using System.Collections;
using System.Collections.Generic;
using POLIMIGameCollective;
using UnityEngine;

public class DialogueBoxController : MonoBehaviour
{
    [SerializeField] private string _dialog = "put a dialog";
    [SerializeField] private float _timeDialog = 1f;
    [SerializeField] private bool _slowDown = false;
    private BoxCollider collider;

    private void Start()
    {
        collider = gameObject.GetComponent<BoxCollider>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("RealityPlayer"))
        {
            EventManager.TriggerEvent("DisplayDialogue", new TutorialDialogObject(_dialog, _timeDialog, _slowDown));  // We need to pass also the time
            collider.enabled = false;
        }
    }
    
}
