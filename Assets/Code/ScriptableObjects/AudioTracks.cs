using UnityEngine;
using UnityEngine.Serialization;

namespace Code.ScriptableObjects
{
 
    [CreateAssetMenu(menuName = "Noclip/Audio Tracks")]
    public class AudioTracks : ScriptableObject
    {
        [Header("Array of sounds for the footseps, chosen randomly at each step")]
        public AudioClip[] footsteps;
        
        [Header("Sound the player makes when landing")]
        public AudioClip landSound;

        [Header("Soundtrack that the user hears when he is inside noclip enabler. Added to the area soundtrack")]
        public AudioClip noclipZoneSound;
        public float noClipSoundVolumeMultiplier;

        [Header("Default area soundtrack, if we do no specify a custom one in its audio source")]
        public AudioClip defaultAreaSoundtrack;

        [Header("List of soundtrack that can be played by an event")]
        public AudioClip[] availableSoundtracks;
        
        [Header("Miscellanea")]
        public AudioClip finishPuzzle;
        public AudioClip enableNoclip;
        public AudioClip disableNoclip;
        
        [Header("The player farts when landing")]
        public AudioClip easterEggFart;
    }
}