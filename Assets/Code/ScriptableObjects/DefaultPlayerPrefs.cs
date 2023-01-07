using UnityEngine;

namespace Code.ScriptableObjects
{
    [CreateAssetMenu(menuName = "UI/Default Settings")]
    public class DefaultPlayerPrefs: ScriptableObject
    {
        public float defaultFov = 90f;
        public float globalVolumeDecibel = 1f;
        public float effectsVolumeDecibel = 1f;
        public float soundTracksVolumeDecibel = 0.9f;
        [Range(0, 2)] public int masterQuality = 1;
        public bool masterFullScreen = true;
        [Range(0, 7)] public float masterSensitivity = 3;
    }
}