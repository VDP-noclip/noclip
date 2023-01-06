using System;
using Code.ScriptableObjects;
using POLIMIGameCollective;
using UnityEngine;
using UnityEngine.Audio;

namespace Code.Scripts.Audio
{
    public class AreaAudioSourceController : MonoBehaviour
    {
        [SerializeField] private AudioClip _customAreaSoundtrack;
        [SerializeField] private AudioTracks _audioTracks;
        [SerializeField] private AudioSource _audioSource;
        [SerializeField] private AudioMixer _mixer;

        [SerializeField] private float _noclipDecreaseVolumeValue = 10;

        private AudioClip _areaSoundtrack;

        #region Unity Methods

        private void Start()
        {
            Debug.Log(_audioTracks.defaultAreaSoundtrack.name);
            Debug.Log(_customAreaSoundtrack);
            if (_customAreaSoundtrack == null)
            {
                _areaSoundtrack = _audioTracks.defaultAreaSoundtrack;
            }
            else
            {
                _areaSoundtrack = _customAreaSoundtrack;
            }
                
            
            
            EventManager.StartListening("ChangeAreaSoundTrack", ChangeAreaSoundtrack);

            PlayAudioSoundtrack();
        }

        private void OnEnable()
        {
        }

        #endregion


        #region Private Methods

        private void PlayAudioSoundtrack()
        {
            _audioSource.clip = _areaSoundtrack;
            _audioSource.loop = true;
            _audioSource.Play();
        }
        
        private void ChangeAreaSoundtrack(string newSoundtrackName)
        {
            EventManager.StopListening("ChangeAreaSoundTrack", ChangeAreaSoundtrack);
            foreach (AudioClip audioClip in _audioTracks.availableSoundtracks)
            {
                if (audioClip.name == newSoundtrackName)
                {
                    _audioSource.Stop();
                    _areaSoundtrack = audioClip;
                    PlayAudioSoundtrack();
                }
            }
            
            EventManager.StartListening("ChangeAreaSoundTrack", ChangeAreaSoundtrack);
        }

        #endregion
        
    }
}