using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScoreManagerScript : MonoBehaviour
{
    public static ScoreManagerScript code;

    int longestTime;
    int scoreWithLongestTime;

    int bestScore;
    int timeWithBestScore;

    void Start()
    {
        code = this;
        longestTime = PlayerPrefs.GetInt("LongestTime", 0);
        scoreWithLongestTime = PlayerPrefs.GetInt("ScoreWithLongestTime", 0);
        bestScore = PlayerPrefs.GetInt("BestScore", 0);
        timeWithBestScore = PlayerPrefs.GetInt("TimeWithBestScore", 0);
    }

    public void UpdateHighScores(int currentScore, int currentTime)
    {
        if(currentScore > bestScore)
        {
            PlayerPrefs.SetInt("BestScore", currentScore);
            PlayerPrefs.SetInt("TimeWithBestScore", currentTime);
        }

        if(currentTime > longestTime)
        {
            PlayerPrefs.SetInt("LongestTime", currentTime);
            PlayerPrefs.SetInt("ScoreWithLongestTime", currentScore);
        }
        PlayerPrefs.SetInt("MostRecentTime", currentTime);
        PlayerPrefs.SetInt("MostRecentScore", currentScore);
    }

}
