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
        [SerializeField] private float _puzzleSkippedPenalty = 200f;
        
        [Tooltip("When we finish the puzzle, we multiply this by the amount of time left to compute a positive score")]
        [SerializeField] private float _timeLeftMultiplier = 10f;

        private float _totalScore;
        private float _currentPuzzleScore;
        
        private int _outOfBoundsCounter;
        private int _outOfTimeCounter;
        private int _noclipActivationsCounter;
        private int _skippedPuzzlesCounter;
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

        public static float[] GetStats()
        {
            return new float[]
            {
                instance._outOfBoundsCounter, 
                instance._outOfTimeCounter, 
                instance._noclipActivationsCounter,
                instance._skippedPuzzlesCounter,
                instance._currentPuzzleIndex
            };
        }
        
        public static void UpdateScoreAfterOutOfBounds()
        {
            instance._outOfBoundsCounter += 1;
            UpdateScore(-instance._outOfBoundsPenalty);
        }

        public static void UpdateScoreAfterOutOfTime()
        {
            instance._outOfTimeCounter += 1;
            UpdateScore(-instance._outOfTimePenalty);
        }
        
        public static void UpdateScoreAfterNoclipActivation()
        {
            instance._noclipActivationsCounter += 1;
            UpdateScore(-instance._noclipActivationPenalty);
        }
        
        public static void UpdateScoreAfterPuzzleSkipped()
        {
            instance._currentPuzzleWasSkipped = true;
            instance._skippedPuzzlesCounter += 1;
            UpdateScore(-instance._puzzleSkippedPenalty);
        }

        public static void UpdateScoreWhenPuzzleIsCompleted(float timeLeftWhenPuzzleIsCompleted)
        {
            if (!instance._currentPuzzleWasSkipped)
            {
                UpdateScore(timeLeftWhenPuzzleIsCompleted * instance._timeLeftMultiplier);
            }
            StoreAndResetCurrentPuzzleStatistics();
        }

        public static void SaveBestScore()
        {
            if (instance._totalScore > PlayerPrefs.GetFloat("bestScore"))
                PlayerPrefs.SetFloat("bestScore", instance._totalScore);
            
        }

        public static float? GetBestScore()
        {
            return PlayerPrefs.GetFloat("bestScore", instance._totalScore);
        }

        private static void UpdateScore(float deltaScore)
        {
            instance._totalScore += deltaScore;
            instance._currentPuzzleScore += deltaScore;
            SaveBestScore();
            Debug.Log($"Puzzle score: {instance._currentPuzzleIndex} : {GetCurrentPuzzleScore()}. Total score: {GetTotalScore()}");
            EventManager.TriggerEvent("RequestGuiUpdateScore");
        }

        private static void StoreAndResetCurrentPuzzleStatistics()
        {
            instance._puzzleScores.Add(instance._currentPuzzleIndex, instance._currentPuzzleScore);
            instance._currentPuzzleIndex += 1;
            instance._currentPuzzleScore = 0;
            instance._currentPuzzleWasSkipped = false;
            EventManager.TriggerEvent("RequestGuiUpdateScore");
        }
    }
}