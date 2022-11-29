using System;
using Code.ScriptableObjects;
using POLIMIGameCollective;
using UnityEngine;

namespace Code.Scripts.Audio
{
    public class AreaAudioSourceController : MonoBehaviour
    {
        [SerializeField] private AudioClip _customAreaSoundtrack;
        [SerializeField] private AudioTracks _audioTracks;
        [SerializeField] private AudioSource _audioSource;

        private AudioClip _areaSoundtrack;

        private void Awake()
        {
            if (_customAreaSoundtrack == null)
                _areaSoundtrack = _audioTracks.DefaultAreaSoundtrack;
            else
                _areaSoundtrack = _customAreaSoundtrack;
            
            EventManager.StartListening("ApplyNoclipEffects", ApplyNoclipEffects);
        }

        private void OnEnable()
        {
            _audioSource.PlayOneShot(_areaSoundtrack);
        }

        private void ApplyNoclipEffects()
        {
            
        }
    }
}