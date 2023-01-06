using UnityEngine;

namespace Code.ScriptableObjects
{
 
    [CreateAssetMenu(menuName = "Noclip/Noclip Options")]
    public class NoclipOptions : ScriptableObject
    {
        public float cooldownSeconds = 0;
        public float backToBodyAnimationDuration = 0.5f;
        
        [Header("Control the hints we give to the player")]
        public string howToActivateNoclip = "hold right click to noclip";
        public string tryToActivateNoclipOutside = "you cannot noclip here";
        
        [Header("Default materials for objects when we are in noclip mode")] 
        public Material[] noClipMaterialsForRealityObjects;
        public Material[] noClipMaterialsForBackgroundObjects;
    }
}