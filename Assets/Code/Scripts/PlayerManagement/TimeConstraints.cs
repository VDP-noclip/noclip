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
        
        // If True, the player needs to finish the puzzle before _maxTimeToFinishPuzzle
        private bool _timeLimitForPuzzleEnabled;
        private float _maxTimeToFinishPuzzle;
        
        private float _realityTimeLeftInThisPuzzle;
        private bool _noclipModeWasActive;

        private void Awake()
        {
            _noclipManager = GetComponent<NoclipManager>();
            _respawningManager = GetComponentInParent<RespawningManager>();
            ResetTimeLimitConstraints();
            _noclipModeWasActive = _noclipManager.IsNoclipEnabled();
            
            EventManager.StartListening("SetNewTimeLimitConstraint", SetNewTimeLimitConstraint);
        }
        
        void Update()
        {
            if (!_timeLimitForPuzzleEnabled)
                return;
            
            ResumeOrPauseTimer();
            
            if (_noclipManager.IsNoclipEnabled())
                return;
            
            _realityTimeLeftInThisPuzzle -= Time.deltaTime;
            if (_realityTimeLeftInThisPuzzle <= 0)
                StartCoroutine(GameLostCoroutine("GAME LOST! TO MUCH TIME TO FINISH THE PUZZLE"));
            
        }

        private void ResumeOrPauseTimer()
        {
            if (_noclipManager.IsNoclipEnabled() && !_noclipModeWasActive)
                EventManager.TriggerEvent("PauseTimer");
            else if (!_noclipManager.IsNoclipEnabled() && _noclipModeWasActive)
                EventManager.TriggerEvent("ResumeTimer");

            _noclipModeWasActive = _noclipManager.IsNoclipEnabled();
        }
        
        /// <summary>
        /// Set the time that the player needs to finish this puzzle.
        /// IMPORTANT: if maxTimeToFinishPuzzle is null, the puzzle does not have a time limit.
        /// </summary>
        private void SetNewTimeLimitConstraint([CanBeNull] string maxTimeToFinishPuzzle)
        {
            if (maxTimeToFinishPuzzle == null)
            {
                Debug.Log("No time limits for this puzzle");
                _timeLimitForPuzzleEnabled = false;
            }
            else
            {
                Debug.Log($"Setting time constraint for this puzzle to {maxTimeToFinishPuzzle}");
                _timeLimitForPuzzleEnabled = true;
                _maxTimeToFinishPuzzle = float.Parse(maxTimeToFinishPuzzle);
                ResetTimeLimitConstraints();
            }
        }

        private void ResetTimeLimitConstraints()
        {
            Debug.Log($"Resetting time limit constraints. Time to finish is {_maxTimeToFinishPuzzle}");

            _realityTimeLeftInThisPuzzle = _maxTimeToFinishPuzzle;
            _noclipModeWasActive = _noclipManager.IsNoclipEnabled();
            
            EventManager.TriggerEvent("ResetTimer", _maxTimeToFinishPuzzle.ToString());
            if (!_noclipManager.IsNoclipEnabled())
                EventManager.TriggerEvent("ResumeTimer");
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