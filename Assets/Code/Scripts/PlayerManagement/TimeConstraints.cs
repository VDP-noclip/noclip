using System.Collections;
using System.Security.Cryptography;
using JetBrains.Annotations;
using POLIMIGameCollective;
using UnityEngine;

namespace Code.Scripts.PlayerManagement
{
    public class TimeConstraints : MonoBehaviour
    {
        private NoclipManager _noclipManager;
        private RespawningManager _respawningManager;

        private bool _timeLimitForPuzzleEnabled;
        private float _maxTimeToFinishPuzzle;
        private float _realityTimeLeftInThisPuzzle;
        private float? _lastRealityModeActivationTimestamp;
        private bool _realityModeIsActive;

        private void Awake()
        {
            _noclipManager = GetComponent<NoclipManager>();
            _respawningManager = GetComponentInParent<RespawningManager>();
            ResetTimeLimitConstraints();
            EventManager.StartListening("SetNewTimeLimitConstraint", SetNewTimeLimitConstraint);
        }

        // Update is called once per frame
        void Update()
        {
            if (!_noclipManager.IsNoclipEnabled() && !_realityModeIsActive)
            {
                _lastRealityModeActivationTimestamp = Time.time;
                _realityModeIsActive = true;
            }

            if (_noclipManager.IsNoclipEnabled())
                _realityModeIsActive = false;
            else
                StartCoroutine(CheckTimeConstraints());
        }
    
        private IEnumerator CheckTimeConstraints()
        {
            if (_timeLimitForPuzzleEnabled &&
                !_noclipManager.IsNoclipEnabled() &&
                GetRealityTimeLeftInThisPuzzle() <= 0)
            {
                GameLost( "GAME LOST! TO MUCH TIME TO FINISH THE PUZZLE");
            }

            yield return null;
        }
    
        private float GetRealityTimeLeftInThisPuzzle()
        {
            if (!_realityModeIsActive || _lastRealityModeActivationTimestamp == null)
                return _realityTimeLeftInThisPuzzle;
        
            var currentTimeInRealityMode = Time.time - (float)_lastRealityModeActivationTimestamp;
            return _realityTimeLeftInThisPuzzle - currentTimeInRealityMode;
        }

        private void SetNewTimeLimitConstraint([CanBeNull] string maxTimeToFinishPuzzle)
        {
            if (maxTimeToFinishPuzzle == null)
            {
                Debug.Log("No time limits for this puzzle");
                _timeLimitForPuzzleEnabled = false;
            }
            else
            {
                Debug.Log("Setting time constraint for this puzzle to " + maxTimeToFinishPuzzle);
                _timeLimitForPuzzleEnabled = true;
                _maxTimeToFinishPuzzle = float.Parse(maxTimeToFinishPuzzle);
                ResetTimeLimitConstraints();
            }
        }
    
        private void ResetTimeLimitConstraints()
        {
            Debug.Log("Resetting time limit constraints. Time limits enabled = " + _timeLimitForPuzzleEnabled);
            _realityTimeLeftInThisPuzzle = _maxTimeToFinishPuzzle;
            if (_realityModeIsActive)
                _lastRealityModeActivationTimestamp = Time.time; // We start in reality mode!
            else
                _lastRealityModeActivationTimestamp = null;
        }

        private void GameLost(string specialMessage)
        {
            Debug.Log(specialMessage);
            EventManager.TriggerEvent("DisplayHint", specialMessage);
            _respawningManager.RespawnAllTransforms();
            ResetTimeLimitConstraints();
        }
    }
}
