using System;
using Code.Scripts.Score;
using POLIMIGameCollective;
using TMPro;
using UnityEngine;

namespace Code.Scripts.GuiManagement
{
    public class ScoreStatsGuiController : MonoBehaviour
    {
        [SerializeField] private TMP_Text _scoreStats;

        private void Start()
        {
            EventManager.StartListening("RequestGuiUpdateScore", UpdateScoreStatsText);
            UpdateScoreStatsText();
        }

        private void OnEnable()
        {
            UpdateScoreStatsText();
        }

        private void UpdateScoreStatsText()
        {
            int bestScore = Mathf.RoundToInt(PlayerPrefs.GetFloat("bestScore"));
            _scoreStats.text = $"PERSONAL BEST: {bestScore}\n \n" +
                               $"TIMEOUTS: {PlayerPrefs.GetInt("outOfTimeCounter")}\n" +
                               $"DEATHS: {PlayerPrefs.GetInt("outOfBoundsCounter")}\n" +
                               $"NOCLIPS: {PlayerPrefs.GetInt("noclipActivationsCounter")}\n" +
                               $"PUZZLES SKIPPED: {PlayerPrefs.GetInt("skippedPuzzlesCounter")}\n" +
                               $"PUZZLES COMPLETED: {PlayerPrefs.GetInt("completedPuzzlesCounter")}";
        }
    }
}