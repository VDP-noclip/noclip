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

        private float _fadeTime = 1.0f;
        private void Awake()
        {
            _noclipManager = GetComponent<NoclipManager>();
            _respawningManager = GetComponentInParent<RespawningManager>();
            ResetTimeLimitConstraints();
            _timerWasActive = TimerShouldBeActive();
            
            EventManager.StartListening("SetNewTimeLimitConstraint", SetNewTimeLimitConstraint);
            EventManager.StartListening("ResetTimeLimitConstraints", ResetTimeLimitConstraints);
            EventManager.StartListening("StartTimeConstraintsTimer", StartTimeConstraintsTimer);
        }
        
        private void Start()
        {
            //find BlackFadein gameobject
            GameObject blackFadein = GameObject.Find("BlackFadein");
            //get BlackFadein script from BlackFadein
            BlackFadein blackFadeinScript = blackFadein.GetComponent<BlackFadein>();
            //get fadeTime from BlackFadein
            _fadeTime = blackFadeinScript.GetFadeTime();
        }

        void Update()
        {
            if (!_timeLimitForPuzzleEnabled || !_isRunning)
                return;

            ResumeOrPauseGuiTimer();
            
            if (!TimerShouldBeActive())
                return;
            
            _realityTimeLeftInThisPuzzle -= Time.deltaTime;
            //if (_realityTimeLeftInThisPuzzle <= 0)
            //    StartCoroutine(GameLostCoroutine());
            if (_realityTimeLeftInThisPuzzle <= _fadeTime)
                EventManager.TriggerEvent("FadeOutRespawn");
            
        }
        
        /// <summary>
        /// This function is ONLY to trigger the start/stop of the GUI
        /// </summary>
        private void ResumeOrPauseGuiTimer()
        {
            if (TimerShouldBeActive() && !_timerWasActive)
                EventManager.TriggerEvent("GuiPauseTimer");
            else if (!TimerShouldBeActive() && _timerWasActive)
                EventManager.TriggerEvent("GuiResumeTimer");

            _timerWasActive = TimerShouldBeActive();
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
            _timerWasActive = TimerShouldBeActive();
            EventManager.TriggerEvent("GuiResetTimer", _maxTimeToFinishPuzzle.ToString());
            _isRunning = false;
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
        private void StartTimeConstraintsTimer()
        {
            ResetTimeLimitConstraints();
            
            if (!_timeLimitForPuzzleEnabled)
                return;
            
            if (TimerShouldBeActive())
                EventManager.TriggerEvent("GuiResumeTimer");
            _isRunning = true;
        }

        private bool TimerShouldBeActive()
        {
            return _noclipManager.RealityPlayerCanMove();
        }
    }
}