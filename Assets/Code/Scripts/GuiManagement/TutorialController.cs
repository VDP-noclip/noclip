using System.Collections;
using POLIMIGameCollective;
using TMPro;
using UnityEngine;

namespace Code.Scripts.TutorialManagement
{
    public class TutorialController : MonoBehaviour
    {
        [SerializeField] private GameObject _controlsContainer;
        [SerializeField] private GameObject _dialogueContainer;
        [SerializeField] private TMP_Text _tutorialText;
        [SerializeField] private TMP_Text _dialogueText;
        [SerializeField] private float _hintDuration = 4f;
        [SerializeField] private float _dialogueDuration = 3f;   // NOT NEEDED
        private float _endDialogueTime;
        private float _endHintTime;

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
            _controlsContainer.SetActive(true);
            StartCoroutine(DisplayHintCoroutine(hint));
            EventManager.StartListening("DisplayHint", DisplayHint);
        
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
        
        private void DisplayDialogue(TutorialDialogObject dialogueObject) //We need to pass also the timer
        {
            EventManager.StopListening("DisplayDialogue", DisplayDialogue);
            _dialogueContainer.SetActive(true);
            StartCoroutine(DisplayDialogueCoroutine(dialogueObject));
            EventManager.StartListening("DisplayDialogue", DisplayDialogue);
        }
        private IEnumerator DisplayDialogueCoroutine(TutorialDialogObject dialogueObject)
        {
            
            _dialogueContainer.SetActive(true);
            if (dialogueObject.IsSlowDown())
            {
                _realityMovement.SetSlowMode(true);
            }
            
            for (int i = 0; i < dialogueObject.GetDialog().Length; i++)  // Write like a typer
            {
                _dialogueText.text += dialogueObject.GetDialog()[i];
                yield return new WaitForSecondsRealtime(0.05f);
            }
            //Debug.Log("Finish write");

            _endDialogueTime = Time.time + dialogueObject.GetTime();

            
            while (Time.time < _endDialogueTime)
            {
                yield return new WaitForSecondsRealtime(0.05f);
            }
            
            _dialogueContainer.SetActive(false);
            if (dialogueObject.IsSlowDown())
            {
                _realityMovement.SetSlowMode(false);
            }
            
            _dialogueText.text = "";  //Reset the text

            yield return null;
        }
    }
}
