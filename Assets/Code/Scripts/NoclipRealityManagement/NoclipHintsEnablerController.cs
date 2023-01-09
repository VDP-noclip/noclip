using UnityEngine;

namespace Code.Scripts.NoclipRealityManagement
{
    public class NoclipHintsEnablerController: MonoBehaviour
    {
        [SerializeField] private bool noclipHintsEnabled = true;

        private void Start()
        {
            NoclipManager noclipManager = FindObjectOfType<NoclipManager>();
            if (noclipManager != null)
            {
                Debug.Log("Noclipmanager has hints: " + noclipHintsEnabled);
                noclipManager.SetHintsAreEnabled(noclipHintsEnabled);
                return;
            }

            Debug.Log("noclipManager not found in the scene ...");
        }
    }
}