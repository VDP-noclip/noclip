using System.Collections;
using JetBrains.Annotations;
using POLIMIGameCollective;
using UnityEngine;

namespace Code.Scripts.PlayerManagement
{
    public class TimeConstraints : MonoBehaviour
    {
        private NoclipManager _noclipManager;
        private RespawningManager _respawningManager;

        private bool _timeLimitForPuzzleEnabled;
        private float _maxTimeToFinishPuzzle;
        private float _realityTimeLeftInThisPuzzle;

        private bool _justSwitchedMode = true;
        private bool prevModeWasNoclip = false;

        private void Awake()
        {
            _noclipManager = GetComponent<NoclipManager>();
            _respawningManager = GetComponentInParent<RespawningManager>();
            ResetTimeLimitConstraints();
            EventManager.StartListening("SetNewTimeLimitConstraint", SetNewTimeLimitConstraint);
        }

        // Update is called once per frame
        void Update()
        {
            if (!_timeLimitForPuzzleEnabled || _noclipManager.IsNoclipEnabled())
                return;
            
            _realityTimeLeftInThisPuzzle -= Time.deltaTime;
            if (_realityTimeLeftInThisPuzzle <= 0)
                StartCoroutine(GameLostCoroutine("GAME LOST! TO MUCH TIME TO FINISH THE PUZZLE"));
            
        }
        
        private void SetNewTimeLimitConstraint([CanBeNull] string maxTimeToFinishPuzzle)
        {
            if (maxTimeToFinishPuzzle == null)
            {
                Debug.Log("No time limits for this puzzle");
                _timeLimitForPuzzleEnabled = false;
            }
            else
            {
                Debug.Log("Setting time constraint for this puzzle to " + maxTimeToFinishPuzzle);
                _timeLimitForPuzzleEnabled = true;
                _maxTimeToFinishPuzzle = float.Parse(maxTimeToFinishPuzzle);
                ResetTimeLimitConstraints();
            }
        }

        private void ResetTimeLimitConstraints()
        {
            Debug.Log("Resetting time limit constraints. Time limits enabled = " + _timeLimitForPuzzleEnabled);
            Debug.Log("Time value:" + _maxTimeToFinishPuzzle);
            _realityTimeLeftInThisPuzzle = _maxTimeToFinishPuzzle;
        }

        private IEnumerator GameLostCoroutine(string gameLostMessage)
        {
            Debug.Log(gameLostMessage);
            EventManager.TriggerEvent("DisplayHint", gameLostMessage);
            _respawningManager.RespawnAllTransforms();
            ResetTimeLimitConstraints();
            yield return null;
        }
    }
}