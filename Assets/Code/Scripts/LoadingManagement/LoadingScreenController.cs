
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class LoadingScreenController : MonoBehaviour
{
    [SerializeField] private LoadingSceneOption _loadingSceneOption;
    [SerializeField] private TMP_Text _loadingText = null;
    [SerializeField] private GameObject _pressButtonsText;
    [SerializeField] private Image _loadingCircle;
    [SerializeField] private float _fadeInDuration = 3f;
    float _firstTime = 0;
    private float _loadingTime;

    private bool _isLoaded;

    //private string _currentDialog;

    private void Start()
    {
        _loadingCircle.fillAmount = 0;

        _loadingText.text = _loadingSceneOption._dialogs[_loadingSceneOption.GetSceneNumber()];
        _loadingTime = _loadingSceneOption._timeDialogs[_loadingSceneOption.GetSceneNumber()];
        
        StartCoroutine(FadeInAndOutTextCoroutine(_loadingText, true, _fadeInDuration));

        
    }

    private void Update()
    {
        if (_firstTime < _loadingTime)
        {
            _firstTime += Time.deltaTime;
            _loadingCircle.fillAmount = _firstTime / _loadingTime;
            
            //Debug.Log(_firstTime);
        }
        else
        {
            _pressButtonsText.SetActive(true);
            StartCoroutine(FadeInAndOutTextCoroutine(_pressButtonsText.GetComponent<TMP_Text>(), true, _fadeInDuration));
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
    
        private IEnumerator FadeInAndOutTextCoroutine(TMP_Text textToFade, bool fadeIn, float duration)
            {
                float counter = 0f;

                //Set Values depending on if fadeIn or fadeOut
                float a, b;
                if (fadeIn)
                {
                    a = 0;
                    b = 1;
                }
                else
                {
                    a = 1;
                    b = 0;
                }
                
                Color currentColor = textToFade.color;
                
                while (counter < duration)
                {
                    counter += Time.deltaTime;
                    float alpha = Mathf.Lerp(a, b, counter / duration);
                    textToFade.color = new Color(currentColor.r, currentColor.g, currentColor.b, alpha);
                    yield return null;
                }
            }
        
}
