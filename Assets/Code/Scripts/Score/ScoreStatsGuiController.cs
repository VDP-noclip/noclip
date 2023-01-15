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
        [SerializeField] private bool _showCurrentScore;

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
            int currentScore = Mathf.RoundToInt(PlayerPrefs.GetFloat("currentScore"));
            if (_showCurrentScore)
                _scoreStats.text = $"CURRENT SCORE: {currentScore}\n";
            else
                _scoreStats.text = "";
            _scoreStats.text += $"PERSONAL BEST: {bestScore}\n \n" +
                                "STATS \n" +
                                $"- TIMEOUTS: {PlayerPrefs.GetInt("outOfTimeCounter")}\n" +
                                $"- DEATHS: {PlayerPrefs.GetInt("outOfBoundsCounter")}\n" +
                                $"- NOCLIPS: {PlayerPrefs.GetInt("noclipActivationsCounter")}\n" +
                                $"- PUZZLES SKIPPED: {PlayerPrefs.GetInt("skippedPuzzlesCounter")}\n" +
                                $"- PUZZLES COMPLETED: {PlayerPrefs.GetInt("completedPuzzlesCounter")}";
        }
    }
}