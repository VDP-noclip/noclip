using System.Collections;
using JetBrains.Annotations;
using POLIMIGameCollective;
using UnityEngine;

namespace Code.Scripts.PlayerManagement
{
    public class TimeConstraints : MonoBehaviour
    {
        private RespawningManager _respawningManager;

        // If True, the player needs to finish the puzzle before _maxTimeToFinishPuzzle
        private bool _timeLimitForPuzzleEnabled;
        private float _maxTimeToFinishPuzzle;

        private float _realityTimeLeftInThisPuzzle;
        private bool _isRunning;

        private void Awake()
        {
            _respawningManager = GetComponentInParent<RespawningManager>();
            ResetTimeLimitConstraints();

            EventManager.StartListening("SetNewTimeLimitConstraint", SetNewTimeLimitConstraint);
            EventManager.StartListening("ResetTimeLimitConstraints", ResetTimeLimitConstraints);
            EventManager.StartListening("RestartTimeConstraintsTimer", RestartTimeConstraintsTimer);
            EventManager.StartListening("ResumeTimeConstraintsTimer", ResumeTimeConstraintsTimer);
            EventManager.StartListening("PauseTimeConstraintsTimer", PauseTimeConstraintsTimer);
        }

        void Update()
        {
            if (!_timeLimitForPuzzleEnabled || !_isRunning)
                return;

            _realityTimeLeftInThisPuzzle -= Time.deltaTime;
            if (_realityTimeLeftInThisPuzzle <= 0)
                StartCoroutine(GameLostCoroutine());
        }

        /// <summary>
        /// Set the time that the player needs to finish this puzzle.
        /// IMPORTANT: if maxTimeToFinishPuzzle is '0', the puzzle does not have a time limit.
        /// </summary>
        private void SetNewTimeLimitConstraint(string maxTimeToFinishPuzzleStr)
        {
            var maxTimeToFinishPuzzle = float.Parse(maxTimeToFinishPuzzleStr);
            if (maxTimeToFinishPuzzle == 0)
            {
                Debug.Log("TimeConstraints: No time limits for this puzzle!");
                _timeLimitForPuzzleEnabled = false;
            }
            else
            {
                Debug.Log($"Setting time constraint for this puzzle to {maxTimeToFinishPuzzle}");
                _timeLimitForPuzzleEnabled = true;
            }

            _maxTimeToFinishPuzzle = maxTimeToFinishPuzzle;
            ResetTimeLimitConstraints();
        }

        private void ResetTimeLimitConstraints()
        {
            Debug.Log($"Resetting time limit constraints. Time to finish is {_maxTimeToFinishPuzzle}");
            _realityTimeLeftInThisPuzzle = _maxTimeToFinishPuzzle;
            _isRunning = false;
            EventManager.TriggerEvent("GuiResetTimer", _maxTimeToFinishPuzzle.ToString());
        }

        private IEnumerator GameLostCoroutine()
        {
            EventManager.TriggerEvent("DisplayHint", "you ran out of time (right click to skip animation)");
            _respawningManager.RespawnAllTransforms();
            ResetTimeLimitConstraints();
            yield return null;
        }

        /// <summary>
        /// Start the internal timer and triggers the GUI
        /// </summary>
        private void RestartTimeConstraintsTimer()
        {
            ResetTimeLimitConstraints();

            if (!_timeLimitForPuzzleEnabled)
                return;

            _isRunning = true;
            EventManager.TriggerEvent("GuiResumeTimer");
        }
        
        private void ResumeTimeConstraintsTimer()
        {
            if (!_timeLimitForPuzzleEnabled)
                return;

            _isRunning = true;
            EventManager.TriggerEvent("GuiResumeTimer");
        }
        
        private void PauseTimeConstraintsTimer()
        {
            if (!_timeLimitForPuzzleEnabled)
                return;
            _isRunning = false;
            EventManager.TriggerEvent("GuiPauseTimer");
        }
    }
}