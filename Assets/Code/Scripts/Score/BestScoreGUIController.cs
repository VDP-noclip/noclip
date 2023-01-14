using System;
using POLIMIGameCollective;
using TMPro;
using UnityEngine;

namespace Code.Scripts.Score
{
    public class BestScoreGUIController : MonoBehaviour
    {
        [SerializeField] private TMP_Text _bestScore;

        private void Start()
        {
            EventManager.StartListening("RequestGuiUpdateScore", DisplayBestScore);
            DisplayBestScore();
        }

        private void OnEnable()
        {
            DisplayBestScore();
        }

        private void DisplayBestScore()
        {
            _bestScore.text = "BEST SCORE: " +
                              Math.Round(PlayerPrefs.GetFloat("bestScore"));
        }
    }
}