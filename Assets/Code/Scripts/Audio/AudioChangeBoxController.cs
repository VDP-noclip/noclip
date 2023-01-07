using System;
using System.Collections;
using System.Collections.Generic;
using POLIMIGameCollective;
using UnityEngine;

public class AudioChangeBoxController : MonoBehaviour
{
    [SerializeField] private string _newAudioSoundtrackName;
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("RealityPlayer"))
        {
            EventManager.TriggerEvent("ChangeAreaSoundTrack", _newAudioSoundtrackName);
        }
    }
}
