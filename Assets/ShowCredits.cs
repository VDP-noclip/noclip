using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using POLIMIGameCollective;
using TMPro;


public class ShowCredits : MonoBehaviour
{
    private bool _started = false;
    private UnityEngine.UI.Image _image;
    private TMPro.TextMeshProUGUI _creditsThanksTextMesh;
    private TMPro.TextMeshProUGUI _creditsNamesTextMesh;
    private TMPro.TextMeshProUGUI _creditsExitTextMesh;
    private bool _thanksCompleted = false;
    private bool _namesCompleted = false;
    private bool _exitCompleted = false;
    [SerializeField] private float _maxAlpha = 1f;
    [SerializeField] private float _fadeTime = 1.0f;
    [SerializeField] private float _timeBetween = 1.0f;
    private float _waitingTime = 0f;
    // Start is called before the first frame update
    void Start()
    {
        EventManager.StartListening("StartCredits", StartCredits);
        //find textmeshpro in children
        GameObject creditsThanks = transform.Find("CreditsThanks").gameObject;
        GameObject creditsNames = transform.transform.Find("CreditsNames").gameObject;
        GameObject creditsExit = transform.transform.Find("CreditsExit").gameObject;

        //get textmeshpro component
        _creditsThanksTextMesh = creditsThanks.GetComponent<TMPro.TextMeshProUGUI>();
        _creditsNamesTextMesh = creditsNames.GetComponent<TMPro.TextMeshProUGUI>();
        _creditsExitTextMesh = creditsExit.GetComponent<TMPro.TextMeshProUGUI>();
        _image = GetComponent<UnityEngine.UI.Image>();
    }

    // Update is called once per frame
    void Update()
    {
        //fade in image, then thanks, then names, then exit
        if (_started)
        {
            if (_image.color.a < _maxAlpha)
            {
                _image.color = new Color(_image.color.r, _image.color.g, _image.color.b, _image.color.a + Time.deltaTime / _fadeTime);
            }
            else if (!_thanksCompleted)
            {
                if (_creditsThanksTextMesh.color.a < _maxAlpha)
                {
                    _creditsThanksTextMesh.color = new Color(_creditsThanksTextMesh.color.r, _creditsThanksTextMesh.color.g, _creditsThanksTextMesh.color.b, _creditsThanksTextMesh.color.a + Time.deltaTime / _fadeTime);
                }
                else
                {
                    _thanksCompleted = true;
                    _waitingTime = Time.time;
                }
            }
            else if (!_namesCompleted)
            {
                if (Time.time - _waitingTime > _timeBetween)
                {
                    if (_creditsNamesTextMesh.color.a < _maxAlpha)
                    {
                        _creditsNamesTextMesh.color = new Color(_creditsNamesTextMesh.color.r, _creditsNamesTextMesh.color.g, _creditsNamesTextMesh.color.b, _creditsNamesTextMesh.color.a + Time.deltaTime / _fadeTime);
                    }
                    else
                    {
                        _namesCompleted = true;
                        _waitingTime = Time.time;
                    }
                }
            }
            else if (!_exitCompleted)
            {
                if (Time.time - _waitingTime > _timeBetween)
                {
                    if (_creditsExitTextMesh.color.a < _maxAlpha)
                    {
                        _creditsExitTextMesh.color = new Color(_creditsExitTextMesh.color.r, _creditsExitTextMesh.color.g, _creditsExitTextMesh.color.b, _creditsExitTextMesh.color.a + Time.deltaTime / _fadeTime);
                    }
                    else
                    {
                        _exitCompleted = true;
                        _waitingTime = Time.time;
                    }
                }
            }
            else
            {
                if (Time.time - _waitingTime > _timeBetween)
                {
                    EventManager.TriggerEvent("EndCredits");
                }
            }
        }
    }

    void StartCredits()
    {
        _started = true;
        //log credits
        Debug.Log("Start Credits (inside ShowCredits)");
    }
}
