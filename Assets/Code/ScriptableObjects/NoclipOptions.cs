using UnityEngine;

namespace Code.ScriptableObjects
{
 
    [CreateAssetMenu(menuName = "Noclip/Noclip Options")]
    public class NoclipOptions : ScriptableObject
    {
        public KeyCode noclipKey = KeyCode.Mouse1;
        
        [Header("Control the hints we give to the player")]
        public string howToActivateNoclip = "hold right click to noclip";
        public string howToDeactivateNoclip = "release to return";
        public string tryToActivateNoclipOutside = "you cannot noclip here";
        public string tryToDeactivateNoclipOutside = "release to return";

        [Header("Skybox change")]
        public Material realitySkyboxMaterial;
        public Material noClipSkyboxMaterial;
        
        [Header("Default materials for objects when we are in noclip mode")] 
        public Material[] noClipMaterialsForRealityObjects;
        public Material[] noClipMaterialsForBackgroundObjects;
    }
}