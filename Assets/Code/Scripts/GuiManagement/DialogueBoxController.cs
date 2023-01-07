using System;
using System.Collections;
using System.Collections.Generic;
using Code.POLIMIgameCollective.EventManager;
using POLIMIGameCollective;
using UnityEngine;
using UnityEngine.UI;

public class DialogueBoxController : MonoBehaviour
{
    [SerializeField] private string _dialog = "Dialog placeholder";
    [SerializeField] private float _timePerLetter = 0.05f;
    [SerializeField] private bool _slowDown = false;
    [SerializeField] private bool _crosshairTutorial = false;
    [SerializeField] private bool _persistent;
    private BoxCollider collider;

    [SerializeField] private GameObject _image;
    [SerializeField] private AudioClip _audioTutorial;

    private void Start()
    {
        collider = gameObject.GetComponent<BoxCollider>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("RealityPlayer"))
        {
            var tutorialDialogObject = new TutorialDialogObject(_dialog, _timePerLetter, _slowDown, _crosshairTutorial, _image, _audioTutorial);
            EventManager.TriggerEvent("DisplayDialogue", tutorialDialogObject);  // We need to pass also the time
            if (!_persistent)
                collider.enabled = false;
        }
    }
    
}
