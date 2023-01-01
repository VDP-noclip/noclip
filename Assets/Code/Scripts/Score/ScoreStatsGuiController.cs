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
            float[] puzzleStats = ScoreManager.GetStats();
            float? bestscore = ScoreManager.GetBestScore();
            _scoreStats.text = $"Current score: {currentScore}\n" +
                               $"Best score: {bestscore}\n \n \n" +
                               $"N. of deaths: {puzzleStats[0]}\n" +
                               $"N. ran out of time: {puzzleStats[1]}\n" +
                               $"N. of noclips: {puzzleStats[2]}\n" +
                               $"N. of skipped puzzles: {puzzleStats[3]}\n" +
                               $"N. of completed puzzles: {puzzleStats[3]}";
        }
    }
}