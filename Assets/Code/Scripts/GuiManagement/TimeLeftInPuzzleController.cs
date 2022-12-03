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
        //[SerializeField] private TMP_Text _timerText;
        [SerializeField] private Image _timerImage;
        
        private bool _isActive;
        private bool _isRunning;
        private float _timeLeftInPuzzle;
        private float _totalTimeForPuzzle;

        private void Awake()
        {
            //_timerText.text = "";
            _timerImage.fillAmount = 0;
            
            EventManager.StartListening("GuiResetTimer", ResetTimer);
            EventManager.StartListening("GuiResumeTimer", ResumeTimer);
            EventManager.StartListening("GuiPauseTimer", PauseTimer);
        }
        
        private void ResumeTimer()
        {
            EventManager.StopListening("GuiResumeTimer", ResumeTimer);
            _isActive = true;
            _isRunning = true;
            Debug.Log("Started timer"+ _timeLeftInPuzzle);
            EventManager.StartListening("GuiResumeTimer", ResumeTimer);
        }

        private void PauseTimer()
        {
            EventManager.StopListening("GuiPauseTimer", PauseTimer);
            _isRunning = false;
            Debug.Log("Timer paused!" + _timeLeftInPuzzle);
            EventManager.StartListening("GuiPauseTimer", PauseTimer);
        }
        
        private void ResetTimer(string totalTimeForPuzzleStr)
        {
            EventManager.StopListening("GuiResetTimer", ResetTimer);
            var totalTimeForPuzzle = float.Parse(totalTimeForPuzzleStr);
            
            _isRunning = false;
            
            if (totalTimeForPuzzle == 0)
            {
                Debug.Log("Reset timer: no time limit for this puzzle!");
                //_timerText.text = "NO TIME ZONE";
                _timerImage.fillAmount = 0;
                _isActive = false;
            }
            else if (totalTimeForPuzzle > 0)
            {
                Debug.Log($"Reset timer: time limit set {totalTimeForPuzzle} for this puzzle!");
                _isActive = true;
                _timeLeftInPuzzle = totalTimeForPuzzle;
                _totalTimeForPuzzle = totalTimeForPuzzle;
                UpdateRenderValues();
            }
            else
            {
                throw new Exception("The timer must have a value bigger than 0! Got + " + totalTimeForPuzzle);
            }
            EventManager.StartListening("GuiResetTimer", ResetTimer);
        }

        private void Update()
        {
            if (!_isActive)
                return; // Make sure we do not display anything
                
            if (_isRunning)
            {
                _timeLeftInPuzzle -= Time.deltaTime;
                UpdateRenderValues();
            }
        }

        private void UpdateRenderValues()
        {
            //_timerText.text = Mathf.RoundToInt(_timeLeftInPuzzle).ToString();
            _timerImage.fillAmount = _timeLeftInPuzzle / _totalTimeForPuzzle;
        }
    }
}