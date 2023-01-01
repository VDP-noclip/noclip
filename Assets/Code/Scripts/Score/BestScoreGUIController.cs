using System;
using TMPro;
using UnityEngine;

namespace Code.Scripts.Score
{
    public class BestScoreGUIController : MonoBehaviour
    {
        [SerializeField] private TMP_Text _bestScore;

        private void Start()
        {
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