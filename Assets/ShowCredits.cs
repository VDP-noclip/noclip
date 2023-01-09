using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using POLIMIGameCollective;
using TMPro;


public class ShowCredits : MonoBehaviour
{
    private bool _started = false;
    private UnityEngine.UI.Image _image;
    private TMPro.TextMeshProUGUI _creditsTextMesh;
    [SerializeField] private float _maxAlpha = 1f;
    [SerializeField] private float _fadeTime = 1.0f;
    // Start is called before the first frame update
    void Start()
    {
        EventManager.StartListening("StartCredits", StartCredits);
        //find textmeshpro in children
        GameObject creditsText = transform.Find("DialogueText").gameObject;
        //get textmeshpro component
        _creditsTextMesh = creditsText.GetComponent<TMPro.TextMeshProUGUI>();
        _image = GetComponent<UnityEngine.UI.Image>();
    }

    // Update is called once per frame
    void Update()
    {
        if (_started)
        {
            //slowly fade in image alpha
            Color color = _image.color;
            color.a += Time.deltaTime / _fadeTime;
            //slowly fade in _creditsTextMesh vertex color alpha
            Color creditsColor = _creditsTextMesh.color;
            creditsColor.a += Time.deltaTime / _fadeTime;
            _creditsTextMesh.color = creditsColor;
            _image.color = color;
            if (color.a >= _maxAlpha)
            {
                _started = false;
            }
        }
    }

    void StartCredits()
    {
        _started = true;
        //log credits
        Debug.Log("Start Credits");
    }
}
