using UnityEngine;

namespace Code.ScriptableObjects
{
 
    [CreateAssetMenu(menuName = "Noclip/Noclip Options")]
    public class NoclipOptions : ScriptableObject
    {
        public KeyCode noclipKey = KeyCode.P;
        
        [Header("Skybox change")] 
        public Material realitySkyboxMaterial;
        public Material noClipSkyboxMaterial;
    }
}