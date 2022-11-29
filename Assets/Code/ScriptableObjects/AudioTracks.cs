using UnityEngine;

namespace Code.ScriptableObjects
{
 
    [CreateAssetMenu(menuName = "Noclip/Audio Tracks")]
    public class AudioTracks : ScriptableObject
    {
        public AudioClip FinishPuzzle;
        public AudioClip EnableNoclip;
        public AudioClip DisableNoclip;
    }
}