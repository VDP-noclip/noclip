using System;
using System.Collections;
using System.Collections.Generic;
using POLIMIGameCollective;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TutorialController : MonoBehaviour
{
    [SerializeField] private GameObject _controlsContainer;
    [SerializeField] private TMP_Text _tutorialText;
    [SerializeField] private float _hintDuration;
    private float _endHintTime;

    private void Awake()
    {
        EventManager.StartListening ("PressP", PressP);
        EventManager.StartListening ("DisplayHint", DisplayHint);
    }


    private void PressP()
    {
        EventManager.StopListening("PressP", PressP);
        StartCoroutine(DisplayHintCoroutine("PRESS P"));
        EventManager.StartListening("PressP", PressP);
    }

    private void DisplayHint(string hint)
    {
        
        EventManager.StopListening("DisplayHint", DisplayHint);
        StartCoroutine(DisplayHintCoroutine(hint));
        EventManager.StartListening("DisplayHint", DisplayHint);
        
    }

    private IEnumerator DisplayHintCoroutine(string hint)
    {
        _controlsContainer.SetActive(true);
        _tutorialText.text = hint;

        _endHintTime = Time.time + 4;

        while (Time.time < _endHintTime)
        {
            yield return new WaitForSecondsRealtime(0.5f);
        }

        _controlsContainer.SetActive(false);

        yield return null;
    }

}
