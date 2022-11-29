using UnityEngine;

namespace Code.ScriptableObjects
{
 
    [CreateAssetMenu(menuName = "Noclip/Noclip Options")]
    public class NoclipOptions : ScriptableObject
    {
        public KeyCode noclipKey = KeyCode.Mouse1;
        
        [Header("Control the hints we give to the player")]
        public string howToActivateNoclip = "PRESS&HOLD RIGHT CLICK TO NOCLIP";
        public string howToDeactivateNoclip = "MOVE AROUND, OR RELEASE RIGHT CLICK TO RETURN TO YOUR BODY";
        public string tryToActivateNoclipOutside = "NOCLIP ZONE NOT FOUND. RIGHT CLICK HAS NO EFFECT";
        public string tryToDeactivateNoclipOutside = "RETURN TO YOUR BODY TO DISABLE NOCLIP";

        [Header("Skybox change")]
        public Material realitySkyboxMaterial;
        public Material noClipSkyboxMaterial;
        
        [Header("Default materials for objects when we are in noclip mode")] 
        public Material[] noClipMaterialsForRealityObjects;
        public Material[] noClipMaterialsForBackgroundObjects;
    }
}