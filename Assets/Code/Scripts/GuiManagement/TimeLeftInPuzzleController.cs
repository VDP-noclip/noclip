using System;
using System.Collections;
using POLIMIGameCollective;
using UnityEngine;
using UnityEngine.UI;

namespace Code.Scripts.GuiManagement
{
    public class TimeLeftInPuzzleController : MonoBehaviour
    {
        //[SerializeField] private TMP_Text _timerText;
        [SerializeField] private Image _timerImage;
        [SerializeField] private AudioSource _timerAudio;
        [Tooltip("This number indicates the remaining time for the beginning of the audio and the blinking. It is in percentage")]
        [SerializeField] [Range(0, 100)] private int _clockTickThresholdPercentage = 33;
        [Tooltip("The clock effect time, computed with the percentage above, won't last more than _maxClockEffectTime.")]
        [SerializeField] private float _maxClockEffectTime = 4f;
        [SerializeField] private float _blinkingTimeFrequency = 0.5f;
        [SerializeField] private bool _blinkWhiteBlack = false;
        [SerializeField] private float _maxPitch = 1.5f;
        
        private bool _isActive;
        private bool _isRunning;
        private float _timeLeftInPuzzle;
        private float _totalTimeForPuzzle;
        private bool _isClockActive;
        private float _clockTickThreshold;
        
        // For Blinking Crossair Coroutine
        private Color _crossairOriginalColor;
        private Coroutine _blinkingCrossairCoroutine;
        private bool _blinkingCoroutineIsRunning;

        // For Increasing Pitch Coroutine
        private float _originalPitch;
        private bool _increasingPitchCoroutineIsRunning;
        private Coroutine _increasingPitchCoroutine;

        #region Unity Methods

        private void Awake()
        {
            //_timerText.text = "";
            _timerImage.fillAmount = 0f;
            _isClockActive = false;
            _crossairOriginalColor = _timerImage.color;
            _originalPitch = _timerAudio.pitch;
            
            EventManager.StartListening("GuiResetTimer", ResetTimer);
            EventManager.StartListening("GuiResumeTimer", ResumeTimer);
            EventManager.StartListening("GuiPauseTimer", PauseTimer);
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

        #endregion

        #region private Methods

        private void ResumeTimer()
        {
            EventManager.StopListening("GuiResumeTimer", ResumeTimer);
            _isActive = true;
            _isRunning = true;

            if (_isClockActive)
            {
                _timerAudio.Play();
                if (!_blinkingCoroutineIsRunning)
                    if (_blinkWhiteBlack)
                    {
                        _blinkingCrossairCoroutine = StartCoroutine(BlinkingCrossairWhiteBlackCoroutine());
                    }
                    else
                    {
                        _blinkingCrossairCoroutine = StartCoroutine(BlinkingCrossairColorTransparentCoroutine());
                    }

                if (!_increasingPitchCoroutineIsRunning)
                {
                    _increasingPitchCoroutine = StartCoroutine(IncreasingPitchCoroutine());
                }
            }
            
            // Debug.Log("Started timer" + _timeLeftInPuzzle);
            EventManager.StartListening("GuiResumeTimer", ResumeTimer);
        }

        private void PauseTimer()
        {
            EventManager.StopListening("GuiPauseTimer", PauseTimer);
            _isRunning = false;
            _timerAudio.Pause();
            StopBlinkingCoroutine();
            StopIncreasingPitchCoroutine();
            // Debug.Log("Timer paused!" + _timeLeftInPuzzle);
            EventManager.StartListening("GuiPauseTimer", PauseTimer);
        }
        
        private void ResetTimer(string totalTimeForPuzzleStr)
        {
            EventManager.StopListening("GuiResetTimer", ResetTimer);
            var totalTimeForPuzzle = float.Parse(totalTimeForPuzzleStr);
            
            _isRunning = false;
            _isClockActive = false;
            _timerAudio.Stop();
            StopBlinkingCoroutine();
            StopIncreasingPitchCoroutine();
            
            if (totalTimeForPuzzle == 0)
            {
                // Debug.Log("Reset timer: no time limit for this puzzle!");
                _timerImage.fillAmount = 0;
                _isActive = false;
            }
            else if (totalTimeForPuzzle > 0)
            {
                // Debug.Log($"Reset timer: time limit set {totalTimeForPuzzle} for this puzzle!");
                _isActive = true;
                _timeLeftInPuzzle = totalTimeForPuzzle;
                _totalTimeForPuzzle = totalTimeForPuzzle;
                var clockThresholdComputedFromPercentage = _totalTimeForPuzzle * _clockTickThresholdPercentage / 100;
                _clockTickThreshold = Math.Min(_maxClockEffectTime, clockThresholdComputedFromPercentage);
                UpdateRenderValues();
            }
            else
            {
                throw new Exception("The timer must have a value bigger than 0! Got + " + totalTimeForPuzzle);
            }
            EventManager.StartListening("GuiResetTimer", ResetTimer);
        }

        private void UpdateRenderValues()
        {
            _timerImage.fillAmount = _timeLeftInPuzzle / _totalTimeForPuzzle;

            if (_timeLeftInPuzzle <= _clockTickThreshold && !_isClockActive)
            { 
                _timerAudio.Play();
                if (_blinkWhiteBlack)
                {
                    _blinkingCrossairCoroutine = StartCoroutine(BlinkingCrossairWhiteBlackCoroutine());
                }
                else
                {
                    _blinkingCrossairCoroutine = StartCoroutine(BlinkingCrossairColorTransparentCoroutine());
                }

                _increasingPitchCoroutine = StartCoroutine(IncreasingPitchCoroutine());
                
                _isClockActive = true;
                
            }
        }
        
        /// <summary>
        ///  This method is used for stopping the blinking coroutine
        /// </summary>
        private void StopBlinkingCoroutine()
        {
            if (_blinkingCoroutineIsRunning)
            {
                StopCoroutine(_blinkingCrossairCoroutine);
                _timerImage.color = _crossairOriginalColor;
                _blinkingCoroutineIsRunning = false;
            }
        }

        /// <summary>
        ///  This method is used for stopping the increasing audio pitch coroutine
        /// </summary>
        private void StopIncreasingPitchCoroutine()
        {
            if (_increasingPitchCoroutineIsRunning)
            {
                StopCoroutine(_increasingPitchCoroutine);
                _timerAudio.pitch = _originalPitch;
                _increasingPitchCoroutineIsRunning = false;
            }
        }

        #endregion
        

        #region Coroutines

        /// <summary>
        /// This coroutine allows the crossair to blink from white to transparent 
        /// </summary>
        private IEnumerator BlinkingCrossairColorTransparentCoroutine()
        {
            _blinkingCoroutineIsRunning = true;
            //Debug.Log("startcouroutine blink");
            float elapsedTime;

            Color crossairColor = _timerImage.color;
            float blinkingTime = _blinkingTimeFrequency;
            //Debug.Log(crossairColor);

            while (_timeLeftInPuzzle > 0)
            {
                elapsedTime = 0f;
                while (elapsedTime < blinkingTime)
                {
                    // Debug.Log("blink out");
                    elapsedTime += Time.deltaTime;

                    float alpha = Mathf.Lerp(crossairColor.a, 0, elapsedTime / (blinkingTime));
                    _timerImage.color = new Color(crossairColor.r, crossairColor.g, crossairColor.b, alpha);

                    yield return new WaitForEndOfFrame();
                }
            
                
                elapsedTime = 0f;
                while (elapsedTime < blinkingTime)
                {
                    // Debug.Log("blink in");
                    elapsedTime += Time.deltaTime;
                
                    float alpha = Mathf.Lerp(0, crossairColor.a, elapsedTime / (blinkingTime));
                    _timerImage.color = new Color(crossairColor.r, crossairColor.g, crossairColor.b, alpha);
                    
                    yield return new WaitForEndOfFrame();
                }
            }
            
            //Debug.Log("Finish coroutine");
            _blinkingCoroutineIsRunning = false;
            _timerImage.color = _crossairOriginalColor;
            yield return null;
        }

        /// <summary>
        /// This coroutine allows the crossair to blink from white to black
        /// </summary>
        private IEnumerator BlinkingCrossairWhiteBlackCoroutine()
        {
            _blinkingCoroutineIsRunning = true;
            //Debug.Log("startcouroutine blink");
            float elapsedTime;

            Color crossairColor = _timerImage.color;
            float blinkingTime = _blinkingTimeFrequency;
            // Debug.Log(crossairColor);

            while (_timeLeftInPuzzle > 0)
            {
                elapsedTime = 0f;
                while (elapsedTime < blinkingTime)
                {
                    // Debug.Log("blink out");
                    elapsedTime += Time.deltaTime;

                    float red = Mathf.Lerp(crossairColor.r, 0, elapsedTime / blinkingTime);
                    float blue = Mathf.Lerp(crossairColor.b, 0, elapsedTime / blinkingTime);
                    float green = Mathf.Lerp(crossairColor.g, 0, elapsedTime / blinkingTime);
                    
                    _timerImage.color = new Color(red, blue, green, crossairColor.a);

                    yield return new WaitForEndOfFrame();
                }
            
                
                elapsedTime = 0f;
                while (elapsedTime < blinkingTime)
                {
                    // Debug.Log("blink in");
                    elapsedTime += Time.deltaTime;
                
                    
                    float red = Mathf.Lerp(0,crossairColor.r, elapsedTime / blinkingTime);
                    float blue = Mathf.Lerp(0,crossairColor.b, elapsedTime / blinkingTime);
                    float green = Mathf.Lerp(0,crossairColor.g,  elapsedTime / blinkingTime);

                    _timerImage.color = new Color(red, blue, green, crossairColor.a);
                    yield return new WaitForEndOfFrame();
                }
            }
            
            //Debug.Log("Finish coroutine");
            _blinkingCoroutineIsRunning = false;
            _timerImage.color = _crossairOriginalColor;
            yield return null;
        }

        /// <summary>
        /// This coroutine allows the audio to increase the pitch
        /// </summary>
        private IEnumerator IncreasingPitchCoroutine()
        {
            _increasingPitchCoroutineIsRunning = true;
            float elapsedTime = 0f;
            float startingPitch = _timerAudio.pitch;

            float totalTimeLeftInPuzzle = _timeLeftInPuzzle;
            
            while (_timeLeftInPuzzle > 0)
            {
                elapsedTime += Time.deltaTime;
                float pitch = Mathf.Lerp(startingPitch,_maxPitch, elapsedTime / totalTimeLeftInPuzzle);

                _timerAudio.pitch = pitch;
                
                yield return new WaitForEndOfFrame();

            }

            _increasingPitchCoroutineIsRunning = false;
            _timerAudio.pitch = _originalPitch;
            yield return null;
        }

        #endregion
        
    }
}