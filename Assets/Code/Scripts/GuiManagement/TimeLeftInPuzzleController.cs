using System;
using System.Collections;
using System.Net.Mime;
using POLIMIGameCollective;
using TMPro;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Code.Scripts.GuiManagement
{
    public class TimeLeftInPuzzleController : MonoBehaviour
    {
        [SerializeField] private TMP_Text _timerText;
        [SerializeField] private Image _timerImage;
        
        private bool _isActive;
        private bool _isRunning;
        private float _timeLeftInPuzzle;
        private float _totalTimeForPuzzle;

        private void Awake()
        {
            _timerText.text = "";
            _timerImage.fillAmount = 0;
            
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
        
        private void ResetTimer(string totalTimeForPuzzleStr)
        {
            EventManager.StopListening("ResetTimer", ResetTimer);
            var totalTimeForPuzzle = float.Parse(totalTimeForPuzzleStr);
            
            _isRunning = false;
            
            if (totalTimeForPuzzle == 0)
            {
                Debug.Log("Reset timer: no time limit for this puzzle!");
                _timerText.text = "NO TIME ZONE";
                _isActive = false;
            }
            else if (totalTimeForPuzzle > 0)
            {
                Debug.Log($"Reset timer: time limit set {totalTimeForPuzzle} for this puzzle!");
                _isActive = true;
                _timeLeftInPuzzle = totalTimeForPuzzle;
                _totalTimeForPuzzle = totalTimeForPuzzle;
            }
            else
            {
                throw new Exception("The timer must have a value bigger than 0! Got + " + totalTimeForPuzzle);
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
                _timerText.text = Mathf.RoundToInt(_timeLeftInPuzzle).ToString();
                _timerImage.fillAmount = _timeLeftInPuzzle / _totalTimeForPuzzle;
            }
        }
    }
}