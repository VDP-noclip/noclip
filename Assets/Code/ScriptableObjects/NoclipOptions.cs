using UnityEngine;

namespace Code.ScriptableObjects
{
 
    [CreateAssetMenu(menuName = "Noclip/Noclip Options")]
    public class NoclipOptions : ScriptableObject
    {
        public KeyCode noclipKey = KeyCode.P;
        
        [Header("Automatically return to body when you are close to it")]
        public bool automaticReturnToBody = true;
        
        [Header("Skybox change")] 
        public Material realitySkyboxMaterial;
        public Material noClipSkyboxMaterial;
        
        [Header("Default materials for objects when we are in noclip mode")] 
        public Material[] noClipMaterialsForRealityObjects;
        public Material[] noClipMaterialsForBackgroundObjects;
    }
}