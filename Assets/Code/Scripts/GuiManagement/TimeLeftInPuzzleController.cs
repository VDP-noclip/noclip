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
        [SerializeField] private AudioSource _timerAudio;
        [SerializeField] private float _clockTickThreshold = 0.33f;
        [SerializeField] private float _blinkingTimeFrequency = 0.5f;
        
        private bool _isActive;
        private bool _isRunning;
        private float _timeLeftInPuzzle;
        private float _totalTimeForPuzzle;
        private bool _isClockActive;
        
        private Color _crossairColor;
        private Coroutine _blinkingCrossairCoroutine;

        private void Awake()
        {
            //_timerText.text = "";
            _timerImage.fillAmount = 0f;
            _isClockActive = false;
            
            EventManager.StartListening("GuiResetTimer", ResetTimer);
            EventManager.StartListening("GuiResumeTimer", ResumeTimer);
            EventManager.StartListening("GuiPauseTimer", PauseTimer);
        }

        private void ResumeTimer()
        {
            EventManager.StopListening("GuiResumeTimer", ResumeTimer);
            _isActive = true;
            _isRunning = true;

            if (_isClockActive)
            {
                _timerAudio.Play();
                //StartCoroutine(BlinkingCrossairWhiteBlackCoroutine(_timerImage, _blinkingTimeFrequency));
            }
                
            
            Debug.Log("Started timer"+ _timeLeftInPuzzle);
            EventManager.StartListening("GuiResumeTimer", ResumeTimer);
        }

        private void PauseTimer()
        {
            EventManager.StopListening("GuiPauseTimer", PauseTimer);
            _isRunning = false;
            _timerAudio.Pause();
            /*StopCoroutine(_blinkingCrossairCoroutine);
            _timerImage.color = _crossairColor;*/
            Debug.Log("Timer paused!" + _timeLeftInPuzzle);
            EventManager.StartListening("GuiPauseTimer", PauseTimer);
        }
        
        private void ResetTimer(string totalTimeForPuzzleStr)
        {
            EventManager.StopListening("GuiResetTimer", ResetTimer);
            var totalTimeForPuzzle = float.Parse(totalTimeForPuzzleStr);
            
            _isRunning = false;
            _isClockActive = false;
            _timerAudio.Stop();
            /*StopCoroutine(_blinkingCrossairCoroutine);
            _timerImage.color = _crossairColor;*/
            
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

            if ((_timeLeftInPuzzle / _totalTimeForPuzzle) <= _clockTickThreshold && !_isClockActive)
            { 
                _timerAudio.Play();
                //_blinkingCrossairCoroutine = StartCoroutine(BlinkingCrossairWhiteBlackCoroutine(_timerImage, _blinkingTimeFrequency));
                _isClockActive = true;
                
            }
        }
        
        private IEnumerator BlinkingCrossairColorTrasparentCoroutine(Image crossair ,float blinkingTime)
        {
            Debug.Log("startcouroutine blink");
            float counter = 0f;

            Color crossairColor = crossair.color;
            Debug.Log(crossairColor);

            /*while (_timeLeftInPuzzle > 0)
            {*/
                while (counter < blinkingTime)
                {
                    Debug.Log("blink out");
                    counter += Time.deltaTime;

                    float alpha = Mathf.Lerp(crossairColor.a, 0, counter / (blinkingTime));

                    crossair.color = new Color(crossairColor.r, crossairColor.g, crossairColor.b, alpha); 
                    
                    Debug.Log(crossair.color);
                    if (alpha <= 0)
                    {
                        Debug.Log("Crossair trasparent: " + crossair.color.a);
                    }
                }
            
                counter = 0f;

                while (counter < blinkingTime)
                {
                    Debug.Log("blink in");
                    counter += Time.deltaTime;
                
                    float alpha = Mathf.Lerp(0, crossairColor.a, counter / (blinkingTime)); 

                    crossair.color = new Color(crossairColor.r, crossairColor.g, crossairColor.b, alpha);
                    Debug.Log(crossair.color);

                    if (alpha >= crossairColor.a)
                    {
                        Debug.Log("Crossair complete: " + crossair.color.a);
                    }
                }
            /*}*/
            
            Debug.Log("Finish coroutine");
            yield return null;
        }
        
        private IEnumerator BlinkingCrossairWhiteBlackCoroutine(Image crossair ,float blinkingTime)
        {
            Debug.Log("startcouroutine blink");
            float counter = 0f;

            Color crossairColor = crossair.color;
            Debug.Log(crossairColor);

            /*while (_timeLeftInPuzzle > 0)
            {*/
                while (counter < blinkingTime)
                {
                    Debug.Log("blink out");
                    counter += Time.deltaTime;

                    

                    float red = Mathf.Lerp(crossairColor.r, 0, counter / blinkingTime);
                    float blue = Mathf.Lerp(crossairColor.b, 0, counter / blinkingTime);
                    float green = Mathf.Lerp(crossairColor.g, 0, counter / blinkingTime);

                    crossair.color = new Color(red, blue, green, crossairColor.a); 
                    
                    Debug.Log(crossair.color);
                    
                }
            
                counter = 0f;

                while (counter < blinkingTime)
                {
                    Debug.Log("blink in");
                    counter += Time.deltaTime;
                
                    float red = Mathf.Lerp(0,crossairColor.r, counter / blinkingTime);
                    float blue = Mathf.Lerp(0,crossairColor.b, counter / blinkingTime);
                    float green = Mathf.Lerp(0,crossairColor.g,  counter / blinkingTime);

                    crossair.color = new Color(red, blue, green, crossairColor.a);
                    Debug.Log(crossair.color);

                }
            /*}*/
            
            Debug.Log("Finish coroutine");
            yield return null;
        }
    }
}