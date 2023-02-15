using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class AnimationsController : MonoBehaviour
{
    [Header("Objects to fade")] 
    [SerializeField] private GameObject mainCanvas;
    [SerializeField] private GameObject mainGradient;
    [SerializeField] private GameObject bottomGradient;
    [SerializeField] private GameObject noclipLogo;
    [SerializeField] private GameObject logoBlur;
    [SerializeField] private GameObject enterButton;
    [SerializeField] private GameObject settingsButton;
    [SerializeField] private GameObject exitButton;
    [SerializeField] private GameObject feedbackButton;
    [SerializeField] private GameObject controlsButton;
    [SerializeField] private GameObject statisticsButton;
    [SerializeField] private GameObject creditsButton;
    
    [Header("Audio To Play")] 
    [SerializeField] private AudioSource noclipEcho;
    private void OnEnable()
    {
        StartCoroutine(FadeUI());
    }

    private IEnumerator FadeUI()
            {
                StartCoroutine(FadeInAndOutCoroutine(mainCanvas, true, 0.1f));
                yield return new WaitForSecondsRealtime(1f);
                StartCoroutine(FadeInAndOutCoroutine(mainGradient, true, 0.1f));
                StartCoroutine(FadeInAndOutCoroutine(bottomGradient, true, 0.1f));
                yield return new WaitForSecondsRealtime(1f);
                StartCoroutine(FadeInAndOutCoroutine(enterButton, true, 0.1f));
                yield return new WaitForSecondsRealtime(0.1f);
                StartCoroutine(FadeInAndOutCoroutine(settingsButton, true, 0.1f));
                yield return new WaitForSecondsRealtime(0.1f);
                StartCoroutine(FadeInAndOutCoroutine(exitButton, true, 0.1f));
                yield return new WaitForSecondsRealtime(0.2f);
                StartCoroutine(FadeInAndOutCoroutine(feedbackButton, true, 0.1f));
                yield return new WaitForSecondsRealtime(0.2f);
                StartCoroutine(FadeInAndOutCoroutine(controlsButton, true, 0.1f));
                yield return new WaitForSecondsRealtime(0.2f);
                StartCoroutine(FadeInAndOutCoroutine(statisticsButton, true, 0.1f));
                yield return new WaitForSecondsRealtime(0.2f);
                StartCoroutine(FadeInAndOutCoroutine(creditsButton, true, 0.1f));
                yield return new WaitForSecondsRealtime(1f);
                StartCoroutine(FadeInAndOutCoroutine(logoBlur, true, 0.1f));
                noclipEcho.Play();
                yield return new WaitForSecondsRealtime(0.2f);
                StartCoroutine(FadeInAndOutCoroutine(noclipLogo, true, 0.1f));
            }
        
        private IEnumerator FadeOutUI()
        {
            StartCoroutine(FadeInAndOutCoroutine(mainCanvas, false, 0.5f));
            yield return new WaitForSecondsRealtime(1f);
            StartCoroutine(FadeInAndOutCoroutine(mainGradient, false, 0.5f));
            StartCoroutine(FadeInAndOutCoroutine(bottomGradient, false, 0.5f));
            yield return new WaitForSecondsRealtime(1f);
            StartCoroutine(FadeInAndOutCoroutine(enterButton, false, 0.5f));
            yield return new WaitForSecondsRealtime(0.2f);
            StartCoroutine(FadeInAndOutCoroutine(settingsButton, false, 0.5f));
            yield return new WaitForSecondsRealtime(0.2f);
            StartCoroutine(FadeInAndOutCoroutine(exitButton, false, 0.5f));
            yield return new WaitForSecondsRealtime(0.5f);
            StartCoroutine(FadeInAndOutCoroutine(feedbackButton, false, 0.5f));
            yield return new WaitForSecondsRealtime(0.5f);
            StartCoroutine(FadeInAndOutCoroutine(controlsButton, false, 0.5f));
            yield return new WaitForSecondsRealtime(0.5f);
            StartCoroutine(FadeInAndOutCoroutine(statisticsButton, false, 0.5f));
            yield return new WaitForSecondsRealtime(0.5f);
            StartCoroutine(FadeInAndOutCoroutine(creditsButton, false, 0.5f));
            yield return new WaitForSecondsRealtime(1f);
            StartCoroutine(FadeInAndOutCoroutine(logoBlur, false, 0.5f));
            noclipEcho.Play();
            yield return new WaitForSecondsRealtime(0.5f);
            StartCoroutine(FadeInAndOutCoroutine(noclipLogo, false, 0.5f));
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
                TMP_Text tempText = objectToFade.GetComponent<TMP_Text>();

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
