using System.Collections;
using POLIMIGameCollective;
using TMPro;
using UnityEngine;

namespace Code.Scripts.TutorialManagement
{
    public class TutorialController : MonoBehaviour
    {
        [SerializeField] private GameObject _controlsContainer;
        [SerializeField] private TMP_Text _tutorialText;
        [SerializeField] private float _hintDuration = 4f;
        private float _endHintTime;

        private void Awake()
        {
            EventManager.StartListening ("ClearHints", ClearHints);
            EventManager.StartListening ("DisplayHint", DisplayHint);
            _tutorialText.text = "";
        }


        private void ClearHints()
        {
            EventManager.StopListening("ClearHints", ClearHints);
            _tutorialText.text = "";
            EventManager.StartListening("ClearHints", ClearHints);
        }

        private void DisplayHint(string hint)
        {
        
            EventManager.StopListening("DisplayHint", DisplayHint);
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

    }
}
