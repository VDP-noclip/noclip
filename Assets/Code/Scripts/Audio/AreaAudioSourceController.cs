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
                
            
            EventManager.StartListening("StartNoclipAudioEffects", StartNoclipAudioEffects);
            EventManager.StartListening("StopNocliAudioEffects", StopNoclipSoundEffect);
            EventManager.StartListening("ChangeAreaSoundTrack", ChangeAreaSoundtrack);

            EventManager.StartListening("ChangeVolumeInNoclip", ChangeVolumeInNoclip);
            
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
        
        private void ChangeVolumeInNoclip(string mode)
        {
            float originalVolume;
            _mixer.GetFloat("globalVolume", out originalVolume);
            Debug.Log("orginalvolume "+originalVolume);
            //if mode is "noclip" then decrease volume, else if mode is "reality" decrease volume
            float modifiedVolume = mode == "noclip" ? originalVolume - 10: originalVolume + 10;
            Debug.Log("newvolume "+modifiedVolume);
            _mixer.SetFloat("globalVolume", modifiedVolume);
        }

        private void StartNoclipAudioEffects()
        {
            EventManager.StopListening("StartNoclipAudioEffects", StartNoclipAudioEffects);
            
            Debug.Log("abbassssssaaaa");
            float originalVolume;
            _mixer.GetFloat("globalVolume", out originalVolume);
            Debug.Log(originalVolume);
            float modifiedVolume =  originalVolume - Mathf.Log(_noclipDecreaseVolumeValue) * 20 ;
            Debug.Log(modifiedVolume);
            _mixer.SetFloat("globalVolume", modifiedVolume);
            EventManager.StartListening("StartNoclipAudioEffects", StartNoclipAudioEffects);
        }

        private void StopNoclipSoundEffect()
        {
            EventManager.StopListening("StopNocliAudioEffects", StopNoclipSoundEffect);
            Debug.Log("alllzzzzaaaa");
            float modifiedVolume;
            _mixer.GetFloat("globalVolume", out modifiedVolume);
            float originalVolume = modifiedVolume + Mathf.Log(_noclipDecreaseVolumeValue) * 20;
            _mixer.SetFloat("globalVolume", originalVolume);
            EventManager.StartListening("StopNocliAudioEffects", StopNoclipSoundEffect);
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