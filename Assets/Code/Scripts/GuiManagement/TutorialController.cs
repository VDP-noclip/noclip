using System;
using System.Collections;
using System.Collections.Generic;
using Code.POLIMIgameCollective.EventManager;
using POLIMIGameCollective;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Code.Scripts.TutorialManagement
{
    public class TutorialController : MonoBehaviour
    {
        [Header("Gameobjects")]
        [SerializeField] private GameObject _controlsContainer;
        [SerializeField] private GameObject _dialogueContainer;
        [SerializeField] private GameObject _dialogueTextObject;
        [SerializeField] private GameObject _tutorialTextObject;
        [SerializeField] private GameObject _skipButtonTextObject;
        [SerializeField] private GameObject _imageObject;
        
        [SerializeField] private Image _image;
        
        [Space]
        [SerializeField] private TMP_Text _tutorialText;
        [SerializeField] private TMP_Text _dialogueText;
        [SerializeField] private KeyCode _skipDialogueKey; // TODO put a name and use it in the InputManager
        
        [Space]
        [Header("Timers")]
        [SerializeField] private float _fadeDuration = 0.5f;
        [SerializeField] private float _hintDuration = 4f;
        [SerializeField] private float _bufferTimeAfterTutorialText = 4f;
        
        private float _endDialogueTime;
        private float _endHintTime;

        private Coroutine _displayHintCoroutine;
        private Coroutine _displayDialogueCoroutine;
        private bool _displayDialogueCoroutineIsRunning;
        private bool _displayHintCoroutineIsRunning;

        private RealityMovementCalibration _realityMovement;


        #region Unity Methods

        private void Awake()
        {
            _realityMovement = FindObjectOfType<RealityMovementCalibration>();
            
            EventManager.StartListening ("ClearHints", ClearHints);
            EventManager.StartListening ("DisplayHint", DisplayHint);
            EventManager.StartListening("DisplayDialogue", DisplayDialogue);
            _tutorialText.text = "";
            _dialogueText.text = "";
            _dialogueContainer.SetActive(false);
            _controlsContainer.SetActive(false);
        }

        private void Update()
        {
            if (_displayDialogueCoroutineIsRunning && Input.GetKeyDown(_skipDialogueKey))
            {
                StopCurrentDialogue();
            }
                
        }

        #endregion

        #region Private Methods

        private void ClearHints()
        {
            EventManager.StopListening("ClearHints", ClearHints);
            StopCurrentHintCoroutine();
            StartCoroutine(FadeInAndOutCoroutine(_controlsContainer, false, _fadeDuration));
            StartCoroutine(FadeInAndOutCoroutine(_tutorialTextObject, false, _fadeDuration));
            EventManager.StartListening("ClearHints", ClearHints);
        }

        private void DisplayHint(string hint)
        {
            EventManager.StopListening("DisplayHint", DisplayHint);
            StopCurrentHintCoroutine();
            StartCoroutine(FadeInAndOutCoroutine(_controlsContainer, true, _fadeDuration));
            StartCoroutine(FadeInAndOutCoroutine(_tutorialTextObject, true, _fadeDuration));
            _displayHintCoroutine = StartCoroutine(DisplayHintCoroutine(hint));
            EventManager.StartListening("DisplayHint", DisplayHint);
        }

        private void StopCurrentDialogue()
        {
            if (_displayDialogueCoroutineIsRunning)
            {
                StopCoroutine(_displayDialogueCoroutine);
                _dialogueContainer.SetActive(false);
                _realityMovement.SetSlowMode(false);
                _displayDialogueCoroutineIsRunning = false;
            }
        }
        
        private void StopCurrentHintCoroutine()
        {
            if (_displayHintCoroutineIsRunning)
            {
                StopCoroutine(_displayHintCoroutine);
                _displayHintCoroutineIsRunning = false;
            }
        }
        
        private void DisplayDialogue(TutorialDialogObject dialogueObject) // We need to pass also the timer
        {
            EventManager.StopListening("DisplayDialogue", DisplayDialogue);
            StopCurrentDialogue();
            StartCoroutine(FadeInAndOutCoroutine(_dialogueContainer, true, _fadeDuration));
            StartCoroutine(FadeInAndOutCoroutine(_dialogueTextObject, true, _fadeDuration));
            StartCoroutine(FadeInAndOutCoroutine(_skipButtonTextObject, true, _fadeDuration));
            _displayDialogueCoroutine = StartCoroutine(DisplayDialogueCoroutine(dialogueObject));
            if (dialogueObject.GetImage() != null)
            {
                _image.sprite = dialogueObject.GetImage().GetComponent<Image>().sprite;
                StartCoroutine(FadeInAndOutCoroutine(_imageObject, true, _fadeDuration));
            }

            EventManager.StartListening("DisplayDialogue", DisplayDialogue);
        }

        #endregion

        #region Coroutines

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
        
        private IEnumerator DisplayHintCoroutine(string hint)
        {
            _tutorialText.text = hint;
            _controlsContainer.SetActive(true);
            _tutorialTextObject.SetActive(true);
            
            _displayHintCoroutineIsRunning = true;

            _endHintTime = Time.time + _hintDuration;

            while (Time.time < _endHintTime)
            {
                yield return new WaitForSecondsRealtime(0.5f);
            }
            
            StartCoroutine(FadeInAndOutCoroutine(_tutorialTextObject, false, _fadeDuration));
            StartCoroutine(FadeInAndOutCoroutine(_controlsContainer, false, _fadeDuration));
            
            yield return new WaitForSecondsRealtime(_fadeDuration);
            
            _controlsContainer.SetActive(false);
        }
        
        

        private IEnumerator DisplayDialogueCoroutine(TutorialDialogObject dialogueObject)
        {
            _dialogueText.text = "";   //reset the text
            _displayDialogueCoroutineIsRunning = true;
            
            _dialogueContainer.SetActive(true);
            _realityMovement.SetSlowMode(dialogueObject.IsSlowDown());

            _endDialogueTime = Time.realtimeSinceStartup + dialogueObject.GetTotalTime() + _bufferTimeAfterTutorialText;

            for (int i = 0; i < dialogueObject.GetDialog().Length; i++)  // Write like a typer
            {
                _dialogueText.text += dialogueObject.GetDialog()[i];
                yield return new WaitForSecondsRealtime(dialogueObject.GetTimePerLetter());
            }
            
            while (Time.realtimeSinceStartup < _endDialogueTime)
            {
                yield return new WaitForSecondsRealtime(0.05f);
            }
            //FinalDialogueCoroutineOperations();
            
            // These coroutines are synchronized!!!!
            StartCoroutine(FadeInAndOutCoroutine(_dialogueContainer, false, _fadeDuration));
            StartCoroutine(FadeInAndOutCoroutine(_dialogueTextObject, false, _fadeDuration));
            StartCoroutine(FadeInAndOutCoroutine(_skipButtonTextObject, false, _fadeDuration));
            if (dialogueObject.GetImage() != null)
            {
                StartCoroutine(FadeInAndOutCoroutine(_imageObject, false, _fadeDuration));
                _image.sprite = null;
            }
            yield return new WaitForSecondsRealtime(_fadeDuration);
            _realityMovement.SetSlowMode(false);
            _displayDialogueCoroutineIsRunning = false;
        }

        #endregion
        
    }
    
    
}
