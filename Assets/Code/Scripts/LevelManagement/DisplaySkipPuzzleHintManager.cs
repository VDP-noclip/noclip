using System.Collections;
using Code.POLIMIgameCollective.EventManager;
using POLIMIGameCollective;
using UnityEngine;

namespace Code.Scripts.LevelManagement
{
    public class DisplaySkipPuzzleHintManager : MonoBehaviour
    {
        [SerializeField] private int _numberOfDeathsToTriggerHint = 7;

        [SerializeField] private string _skipDialogueHintText =
            "I see that you are struggling! You can skip this level using TAB / JOYSTIC BUTTON. " +
            "Or you can change difficulty from the menu if you need more time.";

        [Tooltip(
            "How long to wait before showing the hint. Make sure this is higher than the fade respawn time defined in black fadein.")]
        [SerializeField]
        private float _showHintDelay = 3f;

        [SerializeField] private GameObject _skipButtonImage;

        private int _numberOfDeathsInPuzzle;

        private void Start()
        {
            EventManager.StartListening("OneDeathInPuzzle", IncrementDeathsInPuzzle);
            EventManager.StartListening("ResetDeathsInPuzzle", ResetDeathsInPuzzle);
        }

        private void IncrementDeathsInPuzzle()
        {
            _numberOfDeathsInPuzzle += 1;
            if (_numberOfDeathsInPuzzle == _numberOfDeathsToTriggerHint)
            {
                StartCoroutine(ShowSkipPuzzleCoroutine());
                _numberOfDeathsInPuzzle = 0;
            }
        }

        private IEnumerator ShowSkipPuzzleCoroutine()
        {
            yield return new WaitForSeconds(_showHintDelay);
            var tutorialDialogObject = new TutorialDialogObject(
                _skipDialogueHintText, 0.02f, false, false, _skipButtonImage, null);
            EventManager.TriggerEvent("DisplayDialogue", tutorialDialogObject); // We need to pass also the time
        }

        private void ResetDeathsInPuzzle()
        {
            _numberOfDeathsInPuzzle = 0;
        }
    }
}