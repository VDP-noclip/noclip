using System.Collections.Generic;
using POLIMIGameCollective;
using UnityEngine;

namespace Code.Scripts.Score
{
    /// <summary>
    /// This is a singleton that manages the score. Make sure there is at least one object with this class to be able
    /// to use the score! Put it, for instance, in the GameManager.
    /// </summary>
    public class ScoreManager: MonoBehaviour
    {
        private static ScoreManager scoreManager;
        
        [Tooltip("Initial score, after each puzzle we sum its score to this value")]
        [SerializeField] private float _initialScore = 1000f;
        
        [Tooltip("After each out of bound, we remove this value from the puzzle score")]
        [SerializeField] private float _outOfBoundsPenalty = 10f;
        
        [Tooltip("After each out of time, we remove this value from the puzzle score")]
        [SerializeField] private float _outOfTimePenalty = 10f;
        
        [Tooltip("After each noclip activation, we remove this value from the puzzle score")]
        [SerializeField] private float _noclipActivationPenalty = 10f;
               
        [Tooltip("When the player skips the puzzle, we remove points")]
        [SerializeField] private float _puzzleSkippedPenalty = 250f;
        
        [Tooltip("When we finish a puzzle, we multiply this by the amount of time left to compute a positive score")]
        [SerializeField] private float _timeLeftMultiplier = 5f;
        
        [Tooltip("When we finish a puzzle, we give this fixed quantity to the user (if it was not skipped)")]
        [SerializeField] private float _bonusForPuzzleCompletion = 10f;

        private float _totalScore;
        private float _currentPuzzleScore;
        
        private bool _currentPuzzleWasSkipped;
        
        private int _currentPuzzleIndex;
        private Dictionary<int, float> _puzzleScores = new ();

        public static ScoreManager instance
        {
            get
            {
                if (!scoreManager)
                {
                    scoreManager = FindObjectOfType (typeof (ScoreManager)) as ScoreManager;

                    if (!scoreManager) 
                    {
                        Debug.LogError ("There needs to be one active ScoreManager script on a GameObject in your scene.");
                    }
                    else
                    {
                        scoreManager.Init(); 
                    }
                }

                return scoreManager;
            }
        }

        private void Init()
        {
            _totalScore = _initialScore;
            if (!PlayerPrefs.HasKey("bestScore"))
            {
                PlayerPrefs.SetFloat("bestScore", _totalScore);
            }
        }

        public static float GetTotalScore()
        {
            return instance._totalScore;
        }

        public static float GetCurrentPuzzleScore()
        {
            return instance._currentPuzzleScore;
        }

        public static void UpdateScoreAfterOutOfBounds()
        {
            IncrementByOnePlayerPrefs("outOfBoundsCounter");
            UpdateScore(-instance._outOfBoundsPenalty);
        }

        public static void UpdateScoreAfterOutOfTime()
        {
            IncrementByOnePlayerPrefs("outOfTimeCounter");
            UpdateScore(-instance._outOfTimePenalty);
        }
        
        public static void UpdateScoreAfterNoclipActivation()
        {
            IncrementByOnePlayerPrefs("noclipActivationsCounter");
            UpdateScore(-instance._noclipActivationPenalty);
        }
        
        public static void UpdateScoreAfterPuzzleSkipped()
        {
            instance._currentPuzzleWasSkipped = true;
            IncrementByOnePlayerPrefs("skippedPuzzlesCounter");
            UpdateScore(-instance._puzzleSkippedPenalty);
        }

        public static void UpdateScoreWhenPuzzleIsCompleted(float timeLeftWhenPuzzleIsCompleted)
        {
            if (!instance._currentPuzzleWasSkipped)
            {
                var deltaScoreAfterPuzzleCompleted = instance._bonusForPuzzleCompletion +
                                                     timeLeftWhenPuzzleIsCompleted * instance._timeLeftMultiplier;
                UpdateScore(deltaScoreAfterPuzzleCompleted);
            }
            instance._puzzleScores.Add(instance._currentPuzzleIndex, instance._currentPuzzleScore);
            instance._currentPuzzleIndex += 1;
            instance._currentPuzzleScore = 0;
            instance._currentPuzzleWasSkipped = false;
            IncrementByOnePlayerPrefs("completedPuzzlesCounter");
            EventManager.TriggerEvent("RequestGuiUpdateScore");
        }

        public static void SaveBestScore()
        {
            if (instance._totalScore > PlayerPrefs.GetFloat("bestScore"))
                PlayerPrefs.SetFloat("bestScore", instance._totalScore);
            
        }

        private static void UpdateScore(float deltaScore)
        {
            instance._totalScore += deltaScore;
            instance._currentPuzzleScore += deltaScore;
            SaveBestScore();
            Debug.Log($"Puzzle score: {instance._currentPuzzleIndex} : {GetCurrentPuzzleScore()}. Total score: {GetTotalScore()}");
            EventManager.TriggerEvent("RequestGuiUpdateScore");
        }
        
        
        public static void IncrementByOnePlayerPrefs(string key)
        {
            PlayerPrefs.SetInt(key, PlayerPrefs.GetInt(key) + 1);
        }
    }
}