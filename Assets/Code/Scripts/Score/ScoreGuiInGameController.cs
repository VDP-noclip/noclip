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
        private bool _isHidden;

        private void Start()
        {
            EventManager.StartListening("RequestGuiUpdateScore", UpdateScoreText);
            EventManager.StartListening("HideScoreGui", HideScoreGui);
            EventManager.StartListening("ShowScoreGui", ShowScoreGui);
            UpdateScoreText();
        }

        private void OnEnable()
        {
            UpdateScoreText();
        }

        private void UpdateScoreText()
        {
            if (_isHidden)
                return;
            
            float currentScore = ScoreManager.GetTotalScore();
            _scoreText.text = $"SCORE: {Mathf.Round(currentScore)} \n";
        }

        private void HideScoreGui()
        {
            _scoreText.text = "";
            _isHidden = true;
        }
        
        private void ShowScoreGui()
        {
            _isHidden = false;
            UpdateScoreText();
        }
    }
}