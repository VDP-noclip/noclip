using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using Debug = UnityEngine.Debug;

public class MenuManager : Singleton<MenuManager>
{
    public enum Menu
    {
        Main,
        Feedback,
    };
    
    public GameObject MainMenu;
    public GameObject FeedbackMenu;

    private void SetMenu(Menu menu)
    {
        MainMenu.SetActive(false);
        FeedbackMenu.SetActive(false);

        switch (menu)
        {
            case Menu.Main:
                MainMenu.SetActive(true);
                break;
            case Menu.Feedback:
                FeedbackMenu.SetActive(true);
                break;
        }
    }

    public void Play()
    {
        
    }
    // Start is called before the first frame update
    void Start()
    {    
        SetMenu(Menu.Main);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            SetMenu(Menu.Main);
        }
    }
    
    public void OpenMainMenu()
    {
        SetMenu(Menu.Main);
    }

    public void OpenFeedbackMenu()
    {
        SetMenu(Menu.Feedback);
    }

    public void Submit()
    {
        Debug.Log("Submitting Feedback");
        SetMenu(Menu.Main);
    }
}
