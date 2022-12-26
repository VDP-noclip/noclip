using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEditor.Rendering;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class LoadingScreenController : MonoBehaviour
{
    [SerializeField] private LoadingSceneOption _loadingSceneOption;
    [SerializeField] private TMP_Text loadingText = null;
    [SerializeField] private TMP_Text loadingDots = null;
    [SerializeField] private GameObject DialogueBox;
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

        StartCoroutine(FadeInAndOutCoroutine(DialogueBox, true, 3f));

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
    
        private IEnumerator FadeInAndOutCoroutine(GameObject objectToFade, bool fadeIn, float duration)
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

                int mode = 0;
                Color currentColor = Color.clear;

                SpriteRenderer tempSPRenderer = objectToFade.GetComponent<SpriteRenderer>();
                Image tempImage = objectToFade.GetComponent<Image>();
                RawImage tempRawImage = objectToFade.GetComponent<RawImage>();
                MeshRenderer tempRenderer = objectToFade.GetComponent<MeshRenderer>();
                Text tempText = objectToFade.GetComponent<Text>();

                //Check if this is a Sprite
                if (tempSPRenderer != null)
                {
                    currentColor = tempSPRenderer.color;
                    mode = 0;
                }
                //Check if Image
                else if (tempImage != null)
                {
                    currentColor = tempImage.color;
                    mode = 1;
                }
                //Check if RawImage
                else if (tempRawImage != null)
                {
                    currentColor = tempRawImage.color;
                    mode = 2;
                }
                //Check if Text 
                else if (tempText != null)
                {
                    currentColor = tempText.color;
                    mode = 3;
                }

                //Check if 3D Object
                else if (tempRenderer != null)
                {
                    currentColor = tempRenderer.material.color;
                    mode = 4;

                    //ENABLE FADE Mode on the material if not done already
                    tempRenderer.material.SetFloat("_Mode", 2);
                    tempRenderer.material.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
                    tempRenderer.material.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
                    tempRenderer.material.SetInt("_ZWrite", 0);
                    tempRenderer.material.DisableKeyword("_ALPHATEST_ON");
                    tempRenderer.material.EnableKeyword("_ALPHABLEND_ON");
                    tempRenderer.material.DisableKeyword("_ALPHAPREMULTIPLY_ON");
                    tempRenderer.material.renderQueue = 3000;
                }
                else
                {
                    yield break;
                }

                while (counter < duration)
                {
                    counter += Time.deltaTime;
                    float alpha = Mathf.Lerp(a, b, counter / duration);

                    switch (mode)
                    {
                        case 0:
                            tempSPRenderer.color = new Color(currentColor.r, currentColor.g, currentColor.b, alpha);
                            break;
                        case 1:
                            tempImage.color = new Color(currentColor.r, currentColor.g, currentColor.b, alpha);
                            break;
                        case 2:
                            tempRawImage.color = new Color(currentColor.r, currentColor.g, currentColor.b, alpha);
                            break;
                        case 3:
                            tempText.color = new Color(currentColor.r, currentColor.g, currentColor.b, alpha);
                            break;
                        case 4:
                            tempRenderer.material.color = new Color(currentColor.r, currentColor.g, currentColor.b, alpha);
                            break;
                    }
                    
                    yield return null;
                }
            }
        
}
