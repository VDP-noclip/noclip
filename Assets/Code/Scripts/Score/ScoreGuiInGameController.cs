using System;
using Code.Scripts.Score;
using POLIMIGameCollective;
using TMPro;
using UnityEngine;

namespace Code.Scripts.GuiManagement
{
    public class ScoreGuiInGameController : MonoBehaviour
    {
        [SerializeField] private TMP_Text _scoreText;

        private void Start()
        {
            EventManager.StartListening("RequestGuiUpdateScore", RequestGuiUpdateScore);
            RequestGuiUpdateScore();
        }

        private void OnEnable()
        {
            RequestGuiUpdateScore();
        }

        private void RequestGuiUpdateScore()
        {
            float currentScore = ScoreManager.GetTotalScore();
            _scoreText.text = $"SCORE: {Mathf.Round(currentScore)} \n";
        }
    }
}