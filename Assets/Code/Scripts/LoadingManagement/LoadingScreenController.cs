using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class LoadingScreenController : MonoBehaviour
{
    [SerializeField] private LoadingSceneOption _loadingSceneOption;
    [SerializeField] private TMP_Text loadingText = null;
    [SerializeField] private TMP_Text loadingDots = null;
    [SerializeField] private GameObject pressButtons;
    [SerializeField] private Image loadingCircle;
    float _firstTime = 0;
    private float _multiplier;

    private bool _isLoaded;

    //private string _currentDialog;

    private void Start()
    {
        loadingCircle.fillAmount = 0;
        _multiplier = Random.Range(3f, 5f);
        string[] dialogs = _loadingSceneOption._dialogs;
        loadingText.text = dialogs[_loadingSceneOption.GetSceneNumber()];

    }

    private void Update()
    {
        if (_firstTime < _multiplier)
        {
            _firstTime += Time.deltaTime;
            loadingCircle.fillAmount = _firstTime / _multiplier;
            
            Debug.Log(_firstTime);
        }
        else
        {
            pressButtons.SetActive(true);
            _isLoaded = true;
        }

        if (_isLoaded)
        {
            if (Input.anyKey)
            {
                _loadingSceneOption.ChangeSceneNumber();
                GameObject.Find("GameManager").GetComponent<GameManager>().SetAreaFinished();
            }
        }
    }
}
