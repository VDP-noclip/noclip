using POLIMIGameCollective;
using UnityEngine;

namespace Code.Scripts.GuiManagement
{
    public class ResetStats : MonoBehaviour
    {
        public static void ResetGameStatistics()
        {
            PlayerPrefs.SetInt("outOfBoundsCounter", 0);
            PlayerPrefs.SetInt("outOfTimeCounter", 0);
            PlayerPrefs.SetInt("noclipActivationsCounter", 0);
            PlayerPrefs.SetInt("skippedPuzzlesCounter", 0);
            PlayerPrefs.SetInt("completedPuzzlesCounter", 0);
            EventManager.TriggerEvent("RequestGuiUpdateScore");
        }
    }
}