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
            _scoreStats.text = $"Best score: {bestScore}\n \n \n" +
                               $"N. of deaths: {PlayerPrefs.GetInt("outOfBoundsCounter")}\n" +
                               $"N. ran out of time: {PlayerPrefs.GetInt("outOfTimeCounter")}\n" +
                               $"N. of noclips: {PlayerPrefs.GetInt("noclipActivationsCounter")}\n" +
                               $"N. of skipped puzzles: {PlayerPrefs.GetInt("skippedPuzzlesCounter")}\n" +
                               $"N. of completed puzzles: {PlayerPrefs.GetInt("completedPuzzlesCounter")}";
        }
    }
}