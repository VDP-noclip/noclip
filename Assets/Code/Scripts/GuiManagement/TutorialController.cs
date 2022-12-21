using System;
using System.Collections;
using Code.POLIMIgameCollective.EventManager;
using POLIMIGameCollective;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Code.Scripts.TutorialManagement
{
    public class TutorialController : MonoBehaviour
    {
        [SerializeField] private GameObject _controlsContainer;
        [SerializeField] private GameObject _dialogueContainer;
        [SerializeField] private GameObject _tutorialCrosshair;
        [SerializeField] private TMP_Text _tutorialText;
        [SerializeField] private TMP_Text _dialogueText;
        [SerializeField] private KeyCode _skipDialogueKey; 
        [SerializeField] private float _hintDuration = 4f;
        [SerializeField] private float _bufferTimeAfterTutorialText = 4f;
        private float _endDialogueTime;
        private float _endHintTime;
        private Coroutine _displayDialogueCoroutine;
        private bool _displayDialogueCoroutineIsRunning;

        private RealityMovementCalibration _realityMovement;



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
                StopCurrentDialogueCoroutine();
            }
                
        }

        private void ClearHints()
        {
            EventManager.StopListening("ClearHints", ClearHints);
            _tutorialText.text = "";
            _controlsContainer.SetActive(false);
            EventManager.StartListening("ClearHints", ClearHints);
        }

        private void DisplayHint(string hint)
        {
            EventManager.StopListening("DisplayHint", DisplayHint);
            StartCoroutine(fadeInAndOut(_controlsContainer, true, 1f));
            StartCoroutine(DisplayHintCoroutine(hint));
            EventManager.StartListening("DisplayHint", DisplayHint);
        }
        
        IEnumerator fadeInAndOut(GameObject objectToFade, bool fadeIn, float duration)
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
                            tempSPRenderer.color = new Color(currentColor.r, currentColor.g, currentColor.b, alpha/4);
                            break;
                        case 1:
                            tempImage.color = new Color(currentColor.r, currentColor.g, currentColor.b, alpha/4);
                            break;
                        case 2:
                            tempRawImage.color = new Color(currentColor.r, currentColor.g, currentColor.b, alpha/4);
                            break;
                        case 3:
                            tempText.color = new Color(currentColor.r, currentColor.g, currentColor.b, alpha/4);
                            break;
                        case 4:
                            tempRenderer.material.color = new Color(currentColor.r, currentColor.g, currentColor.b, alpha/4);
                            break;
                    }
                    
                    
                    yield return null;
                }
            }
        
        private IEnumerator DisplayHintCoroutine(string hint)
        {
            _controlsContainer.SetActive(true);
            _tutorialText.text = hint;

            _endHintTime = Time.time + _hintDuration;

            while (Time.time < _endHintTime)
            {
                yield return new WaitForSecondsRealtime(0.5f);
            }
            
            _controlsContainer.SetActive(false);

            yield return null;
        }
        
        private void DisplayDialogue(TutorialDialogObject dialogueObject) // We need to pass also the timer
        {
            EventManager.StopListening("DisplayDialogue", DisplayDialogue);
            StopCurrentDialogueCoroutine();
            StartCoroutine(fadeInAndOut(_dialogueContainer, true, 1f));
            _displayDialogueCoroutine = StartCoroutine(DisplayDialogueCoroutine(dialogueObject));
            
            
            EventManager.StartListening("DisplayDialogue", DisplayDialogue);
        }

        private IEnumerator DisplayDialogueCoroutine(TutorialDialogObject dialogueObject)
        {
            _displayDialogueCoroutineIsRunning = true;
            
            _dialogueContainer.SetActive(true);
            _realityMovement.SetSlowMode(dialogueObject.IsSlowDown());

            if (dialogueObject.IsCrossHairHighlighted())
            {
                _tutorialCrosshair.SetActive(true);
            }

            _dialogueText.text = "";
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
            yield return fadeInAndOut(_dialogueContainer, false, 1f);
            FinalDialogueCoroutineOperations();


        }
        
        private void FinalDialogueCoroutineOperations()
        {
            _dialogueContainer.SetActive(false);
            _dialogueText.text = "";  //Reset the text
            _realityMovement.SetSlowMode(false);
            _tutorialCrosshair.SetActive(false);
            _displayDialogueCoroutineIsRunning = false;
            
        }

        private void StopCurrentDialogueCoroutine()
        {
            if (_displayDialogueCoroutineIsRunning)
            {
                StopCoroutine(_displayDialogueCoroutine);
                FinalDialogueCoroutineOperations();
            }
        }
    }
    
    
}
