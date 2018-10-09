using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class EndSceneScript : MonoBehaviour {

    public Text currentScore;
    public Text currentTime;
    public Text highScore;
    public Text highScoreTime;
    public Text longestTime;
    public Text longestTimeScore;

    // Use this for initialization
    void Start ()
    {
        currentScore.text = PlayerPrefs.GetInt("MostRecentScore", 0).ToString();
        currentTime.text = PlayerPrefs.GetInt("MostRecentTime", 0).ToString();
        highScore.text = PlayerPrefs.GetInt("BestScore", 0).ToString();
        highScoreTime.text = PlayerPrefs.GetInt("TimeWithBestScore", 0).ToString();
        longestTime.text = PlayerPrefs.GetInt("LongestTime", 0).ToString(); 
        longestTimeScore.text = PlayerPrefs.GetInt("ScoreWithLongestTime", 0).ToString();
    }

    void Update()
    {
        if(Input.GetButtonDown("Interact"))
        {
            SceneManager.LoadScene("MainMenu");
        }
    }
}
