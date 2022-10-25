using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// The simplest form of level managing. It just moves onto the next level after hitting an object.
/// This script should only belong to a GoalPlatform
/// </summary>
public class FinishLevel : MonoBehaviour
{
    private void OnTriggerEnter(Collider otherObject)
    {
        if (otherObject.CompareTag("RealityPlayer"))
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
        }
    }
}
