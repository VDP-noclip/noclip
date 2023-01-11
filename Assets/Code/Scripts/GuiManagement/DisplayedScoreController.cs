using Code.Scripts.Score;
using POLIMIGameCollective;
using TMPro;
using UnityEngine;

namespace Code.Scripts.GuiManagement
{
    /// <summary>
    /// This is a simple example of how the ScoreManager can be used. This class reacts to the 'RequestGuiUpdateScore',
    /// an event triggered by the ScoreManager everytime there is a score update to be displayed.
    /// </summary>
    public class DisplayedScoreController: MonoBehaviour
    {
        [SerializeField] private TMP_Text _scoreText;
        [SerializeField] private TMP_Text _scoreInfo;
        
        private void Start()
        {
            EventManager.StartListening("RequestGuiUpdateScore", RequestGuiUpdateScore);
            RequestGuiUpdateScore();
        }
    
        private void RequestGuiUpdateScore()
        {
            if (_scoreText != null)
            {
                float currentScore = ScoreManager.GetTotalScore();
                _scoreText.text = $"SCORE: {Mathf.Round(currentScore)} \n";
            }
            
            if (_scoreInfo != null)
            {
                float[] puzzleStats = ScoreManager.GetStats();
                float? bestscore = ScoreManager.GetBestScore();
                _scoreInfo.text = $"N. of deaths: {puzzleStats[0]} \n" +
                                  $"N. ran out of time: {puzzleStats[1]} \n" +
                                  $"N. of noclips: {puzzleStats[2]} \n" +
                                  $"N. of skipped puzzles: {puzzleStats[3]} \n" +
                                  $"N. of completed puzzles: {puzzleStats[3]} \n" +
                                  $"Best score: {bestscore}";
            }
        }
    }
}

