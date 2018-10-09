using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class RoundManagerScript : MonoBehaviour {

    public static RoundManagerScript code;

    public enum ScoreType
    {
        INVALID = 0,
        HUMAN = 1
    }

    //human spawning variables
    public GameObject thePlayer;
    public GameObject humanPrefab;
    public int maxHumans = 4;
    public float spawnRadius;
    public GameObject[] humanSpawnPoints;

    int currentHumanOnScreenCount;

    //game objective variables
    public int requiredHumans;
    public int currentHumansHeld;
    public float levelTimer;

    int score = 0;
    float currentTime;

    //UI variables
    public Text scoreText;
    public Text timerText;
    public Text gameOverText;

    // Use this for initialization
    void Start ()
    {
        code = this;
        NewRound();
        scoreText.text = score.ToString();
        gameOverText.color = new Color(gameOverText.color.r, gameOverText.color.b, gameOverText.color.g, 0);        

    }
	
	// Update is called once per frame
	void FixedUpdate () {
        CheckHumansOnScreen();
        UpdateTimer();
	}

    void NewRound()
    {
        currentHumansHeld = 0;
        currentTime = levelTimer;
        CheckHumansOnScreen();
    }

    void CheckHumansOnScreen()
    {
        
            while(currentHumanOnScreenCount <= maxHumans)
            {
                int newRandom = Random.Range(0, humanSpawnPoints.Length);
                Transform tempTransform = humanSpawnPoints[newRandom].transform;
                if(Vector3.Distance(tempTransform.position, thePlayer.transform.position) > spawnRadius)
                {
                    SpawnHuman(tempTransform);
                    currentHumanOnScreenCount++;
                }
            }
    }

    void SpawnHuman(Transform spawnTransform)
    {
        GameObject newHuman = (GameObject)Instantiate(humanPrefab, spawnTransform);
    }

    void UpdateTimer()
    {
        timerText.text = ((int)currentTime).ToString();
        currentTime += Time.deltaTime;
    }

    void ResetField()
    {

    }

    public void RemoveHuman()
    {
        currentHumanOnScreenCount--;
    }

    public void AddScore(ScoreType typeOfScore)
    {
        switch(typeOfScore)
        {
            case ScoreType.HUMAN:
                score += 100;
                scoreText.text = score.ToString();
                break;
            case ScoreType.INVALID:
                Debug.Log("Invalid Score Type; FAILED");
                break;
            default:
                break;
        }
    }

    public void GameEndTransition()
    {
        Time.timeScale = 0;
        ScoreManagerScript.code.UpdateHighScores(score, (int)currentTime);
        StartCoroutine(TextTransition());
    }

    IEnumerator TextTransition()
    {
        for(float i = 0.1f; i < 1; i+=0.1f)
        {
            yield return new WaitForSecondsRealtime(0.1f);
            gameOverText.color = new Color(gameOverText.color.r, gameOverText.color.b, gameOverText.color.g, i);
        }

        yield return new WaitForSecondsRealtime(3f);
        Time.timeScale = 1;
        SceneManager.LoadScene("GameEndScene");
    }

}
