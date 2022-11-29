using UnityEngine;
using UnityEngine.Serialization;

namespace Code.ScriptableObjects
{
 
    [CreateAssetMenu(menuName = "Noclip/Audio Tracks")]
    public class AudioTracks : ScriptableObject
    {
        public AudioClip finishPuzzle;
        
        public AudioClip enableNoclip;
        public AudioClip disableNoclip;

        public AudioClip noclipZoneSound;
        
        public AudioClip defaultAreaSoundtrack;
    }
}