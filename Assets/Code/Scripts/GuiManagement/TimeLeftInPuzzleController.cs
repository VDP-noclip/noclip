using System;
using System.Collections;
using POLIMIGameCollective;
using UnityEngine;
using UnityEngine.Events;

namespace Code.Scripts.GuiManagement
{
    public class TimeLeftInPuzzleController : MonoBehaviour
    {
        private bool _isActive;
        private bool _isRunning;
        private float _timeLeftInPuzzle;
        
        private void Awake()
        {
            EventManager.StartListening("ResetTimer", ResetTimer);
            EventManager.StartListening("ResumeTimer", ResumeTimer);
            EventManager.StartListening("PauseTimer", PauseTimer);
        }
        
        private void ResumeTimer()
        {
            EventManager.StopListening("ResumeTimer", ResumeTimer);
            _isActive = true;
            _isRunning = true;
            Debug.Log("Started timer"+ _timeLeftInPuzzle);
            EventManager.StartListening("ResumeTimer", ResumeTimer);
        }

        private void PauseTimer()
        {
            EventManager.StopListening("PauseTimer", PauseTimer);
            _isRunning = false;
            Debug.Log("Timer paused!" + _timeLeftInPuzzle);
            EventManager.StartListening("PauseTimer", PauseTimer);
        }
        
        private void ResetTimer(string timerTimeStr)
        {
            EventManager.StopListening("ResetTimer", ResetTimer);
            var timerTime = float.Parse(timerTimeStr);
            
            _isRunning = false;
            
            if (timerTime == 0)
            {
                Debug.Log("Reset timer: no time limit for this puzzle!");
                _isActive = false;
            }
            else if (timerTime > 0)
            {
                Debug.Log($"Reset timer: time limit set {timerTime} for this puzzle!");
                _isActive = true;
                _timeLeftInPuzzle = timerTime;
            }
            else
            {
                throw new Exception("The timer must have a value bigger than 0! Got + " + timerTime);
            }
            EventManager.StartListening("ResetTimer", ResetTimer);
        }

        private void Update()
        {
            if (!_isActive)
                return; // Make sure we do not display anything
                
            if (_isRunning)
            {
                _timeLeftInPuzzle -= Time.deltaTime;
                // Todo: update text...
            }
        }
    }
}