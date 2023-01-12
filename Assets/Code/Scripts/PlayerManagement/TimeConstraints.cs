using POLIMIGameCollective;
using UnityEngine;

namespace Code.Scripts.PlayerManagement
{
    public class TimeConstraints : MonoBehaviour
    {
        [SerializeField] private float _easyModeTimeLeftMultiplier = 2f;
        [SerializeField] private float _normalModeTimeLeftMultiplier = 1.5f;
        [SerializeField] private float _difficultModeTimeLeftMultiplier = 1f;

        // If True, the player needs to finish the puzzle before _realityTimeLeftInThisPuzzle
        private bool _timeLimitForPuzzleEnabled;
        
        // Original time limit set from the checkpoint (before the difficulty factor multiplication)
        private float _originalMaxTimeToFinishPuzzle;
        private float _realityTimeLeftInThisPuzzle;
        private bool _isRunning;

        private float _fadeTime;
        private bool _fading;

        private void Awake()
        {
            ResetTimeLimitConstraints();

            EventManager.StartListening("SetNewTimeLimitConstraint", SetNewTimeLimitConstraint);
            EventManager.StartListening("ResetTimeLimitConstraints", ResetTimeLimitConstraints);
            EventManager.StartListening("RestartTimeConstraintsTimer", RestartTimeConstraintsTimer);
            EventManager.StartListening("ResumeTimeConstraintsTimer", ResumeTimeConstraintsTimer);
            EventManager.StartListening("PauseTimeConstraintsTimer", PauseTimeConstraintsTimer);
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

            _realityTimeLeftInThisPuzzle -= Time.deltaTime;
            
            if (_realityTimeLeftInThisPuzzle <= _fadeTime && !_fading)
            {
                EventManager.TriggerEvent("FadeOutRespawn");
                _fading = true;
            }
        }

        /// <summary>
        /// Set the time that the player needs to finish this puzzle.
        /// IMPORTANT: if maxTimeToFinishPuzzle is '0', the puzzle does not have a time limit.
        /// </summary>
        private void SetNewTimeLimitConstraint(string maxTimeToFinishPuzzleStr)
        {
            _originalMaxTimeToFinishPuzzle = float.Parse(maxTimeToFinishPuzzleStr);
            if (_originalMaxTimeToFinishPuzzle == 0)
                _timeLimitForPuzzleEnabled = false;
            else
                _timeLimitForPuzzleEnabled = true;
            ResetTimeLimitConstraints();
        }

        private float AdjustTimeToFinishPuzzleBasedOnDifficulty(float originalMaxTimetoFinishPuzzle)
        {
            int difficultyLevel = PlayerPrefs.GetInt("difficultyLevel");
            switch (difficultyLevel)
            {
                case 0:
                    return originalMaxTimetoFinishPuzzle * _easyModeTimeLeftMultiplier;
                case 1:
                    return originalMaxTimetoFinishPuzzle * _normalModeTimeLeftMultiplier;
                case 2:
                    return originalMaxTimetoFinishPuzzle * _difficultModeTimeLeftMultiplier;
                default:
                    Debug.LogWarning($"Difficulty level '{difficultyLevel}' not in the known range! Using normal");
                    return originalMaxTimetoFinishPuzzle * _normalModeTimeLeftMultiplier; 
            }
        }

        private void ResetTimeLimitConstraints()
        {
            _realityTimeLeftInThisPuzzle = AdjustTimeToFinishPuzzleBasedOnDifficulty(_originalMaxTimeToFinishPuzzle);
            Debug.Log($"Resetting time limit constraints. Time to finish is {_originalMaxTimeToFinishPuzzle}");
            _isRunning = false;
            EventManager.TriggerEvent("GuiResetTimer", _realityTimeLeftInThisPuzzle.ToString());
            _fading = false;
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