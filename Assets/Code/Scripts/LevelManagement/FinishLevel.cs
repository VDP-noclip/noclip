using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class FinishLevel : MonoBehaviour
{
    /*
     * The simplest form of level managing. It just moves onto the next level after hitting an object.
     * This script should only belong to a GoalPlatform
     */

    private void OnTriggerEnter(Collider otherObject)
    {
        if (otherObject.gameObject.name == "RealityPlayer")
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
        }
    }
}
