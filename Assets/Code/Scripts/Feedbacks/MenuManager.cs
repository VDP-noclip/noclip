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
        Feedback0,
        Feedback1,
        Feedback2,
        Feedback3,
        Feedback4,
    };
    
    public GameObject MainMenu;
    public GameObject FeedbackMenu0;
    public GameObject FeedbackMenu1;
    public GameObject FeedbackMenu2;
    public GameObject FeedbackMenu3;
    public GameObject FeedbackMenu4;

    private void SetMenu(Menu menu)
    {
        MainMenu.SetActive(false);
        FeedbackMenu0.SetActive(false);
        FeedbackMenu1.SetActive(false);
        FeedbackMenu2.SetActive(false);
        FeedbackMenu3.SetActive(false);
        FeedbackMenu4.SetActive(false);

        switch (menu)
        {
            case Menu.Main:
                MainMenu.SetActive(true);
                break;
            case Menu.Feedback0:
                FeedbackMenu0.SetActive(true);
                break;
            case Menu.Feedback1:
                FeedbackMenu1.SetActive(true);
                break;
            case Menu.Feedback2:
                FeedbackMenu2.SetActive(true);
                break;
            case Menu.Feedback3:
                FeedbackMenu3.SetActive(true);
                break;
            case Menu.Feedback4:
                FeedbackMenu4.SetActive(true);
                break;
            
        }
    }
    
    // Start is called before the first frame update
    void Start()
    {    
        SetMenu(Menu.Main);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.L))
        {
            SetMenu(Menu.Main);
        }
    }
    
    public void OpenMainMenu()
    {
        SetMenu(Menu.Main);
    }

    public void OpenFeedbackMenu0()
    {
        SetMenu(Menu.Feedback0);
    }
    
    public void OpenFeedbackMenu1()
    {
        SetMenu(Menu.Feedback1);
    }
    
    public void OpenFeedbackMenu2()
    {
        SetMenu(Menu.Feedback2);
    }
    
    public void OpenFeedbackMenu3()
    {
        SetMenu(Menu.Feedback3);
    }
    
    public void OpenFeedbackMenu4()
    {
        SetMenu(Menu.Feedback4);
    }

    public void Submit()
    {
        Debug.Log("Submitting Feedback");
        SetMenu(Menu.Main);
    }
}
