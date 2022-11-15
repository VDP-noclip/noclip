using System.Collections;
using System.Collections.Generic;
using System.Security;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenuController : MonoBehaviour
{
    [SerializeField] private GameObject pauseMenuUI;
    [SerializeField] private bool isPaused;
    [SerializeField] private AudioSource menuPress;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.L))
        {
            if (!isPaused)
            {
                ActivateMenu();
            }
            else
            {
                DeactivateMenu();
            }
        }
    }

    public void ActivateMenu()
    {
        Time.timeScale = 0;
        AudioListener.pause = true;
        
        menuPress.ignoreListenerPause=true;
        menuPress.Play();
        
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        pauseMenuUI.SetActive(true);
        isPaused = true;
    }

    public void DeactivateMenu()
    {
        Time.timeScale = 1;
        AudioListener.pause = false;
        
        menuPress.Play();
        
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        
        pauseMenuUI.SetActive(false);
        isPaused = false;
    }
}
