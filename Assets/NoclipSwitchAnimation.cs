using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using POLIMIGameCollective;
//import application
using UnityEngine.UI;

public class NoclipSwitchAnimation : MonoBehaviour
{
    private int _fadeIn = -1;
    private bool _fading = false;
    private bool _holding = false;
    private float _holdingSince = 0f;
    [SerializeField] private float _fadeTime = 1.0f;
    [SerializeField] private float _holdTime = 0.3f;
    //serialize max alpha
    [SerializeField] private float _maxAlpha = 0.5f;
    
    private RespawningManager _respawningManager;
    private UnityEngine.UI.Image _image;
    // Start is called before the first frame update
    void Start()
    {
        _image = GetComponent<UnityEngine.UI.Image>();
        //find AllPlayer gameobject
        GameObject allPlayer = GameObject.Find("AllPlayer");
        //get RespawningManager script from AllPlayer
        _respawningManager = allPlayer.GetComponent<RespawningManager>();
        EventManager.StartListening("StartNoclipAnimation", FadingTrue);
    }

    // Update is called once per frame
    void Update()
    {
        if (_fading && !_holding)
        {
            Color color = _image.color;
            color.a += _fadeIn * Time.deltaTime / _fadeTime;
            if (color.a >= _maxAlpha)
            {
                color.a = _maxAlpha;
                _fading = true;
                _fadeIn = -1;
                Hold();
            }
            else if (color.a <= 0.0f)
            {
                color.a = 0.0f;
                _fading = false;
            }
            _image.color = color;
        }

        if (_holding)
        {
            _holdingSince += Time.deltaTime;
            if (_holdingSince >= _holdTime)
            {
                _holding = false;
                _holdingSince = 0f;
            }
        }
    }

    private void FadingTrue()
    {
        _fading = true;
        _fadeIn = 1;
    }

    public float GetFadeTime()
    {
        return _fadeTime;
    }

    private void Hold()
    {
        _holding = true;
        //TODO DISABLE PLAYER INPUT (LIKE PAUSE)
    }
}
