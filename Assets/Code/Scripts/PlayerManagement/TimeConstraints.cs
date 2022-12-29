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
        private bool _timerWasActive;

        private bool _isRunning;

        private void Awake()
        {
            _noclipManager = GetComponent<NoclipManager>();
            _respawningManager = GetComponentInParent<RespawningManager>();
            ResetTimeLimitConstraints();
            _timerWasActive = _noclipManager.RealityPlayerCanMove();
            
            EventManager.StartListening("SetNewTimeLimitConstraint", SetNewTimeLimitConstraint);
            EventManager.StartListening("ResetTimeLimitConstraints", ResetTimeLimitConstraints);
            EventManager.StartListening("StartTimeConstraintsTimer", StartTimeConstraintsTimer);
        }
        
        void Update()
        {
            if (!_timeLimitForPuzzleEnabled || !_isRunning)
                return;

            ResumeOrPauseGuiTimer();
            
            if (_noclipManager.IsNoclipEnabled())
                return;
            
            _realityTimeLeftInThisPuzzle -= Time.deltaTime;
            if (_realityTimeLeftInThisPuzzle <= 0)
                StartCoroutine(GameLostCoroutine("GAME LOST! TO MUCH TIME TO FINISH THE PUZZLE"));
            
        }
        
        /// <summary>
        /// This function is ONLY to trigger the start/stop of the GUI
        /// </summary>
        private void ResumeOrPauseGuiTimer()
        {
            if (!_noclipManager.RealityPlayerCanMove() && !_timerWasActive)
                EventManager.TriggerEvent("GuiPauseTimer");
            else if (_noclipManager.RealityPlayerCanMove() && _timerWasActive)
                EventManager.TriggerEvent("GuiResumeTimer");

            _timerWasActive = _noclipManager.RealityPlayerCanMove();
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
            _timerWasActive = _noclipManager.RealityPlayerCanMove();
            EventManager.TriggerEvent("GuiResetTimer", _maxTimeToFinishPuzzle.ToString());
            _isRunning = false;
        }

        private IEnumerator GameLostCoroutine(string gameLostMessage)
        {
            Debug.Log(gameLostMessage);
            EventManager.TriggerEvent("DisplayHint", gameLostMessage);
            _respawningManager.RespawnAllTransforms();
            ResetTimeLimitConstraints();
            yield return null;
        }
        
        /// <summary>
        /// Start the internal timer and triggers the GUI
        /// </summary>
        private void StartTimeConstraintsTimer()
        {
            ResetTimeLimitConstraints();
            
            if (!_timeLimitForPuzzleEnabled)
                return;
            
            if (!_noclipManager.RealityPlayerCanMove())
                EventManager.TriggerEvent("GuiResumeTimer");
            _isRunning = true;
        }
    }
}