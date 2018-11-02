using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ButtonScripts : MonoBehaviour
{

    public void StartGame()
    {
        SceneManager.LoadScene("TutorialScene");
    }

    public void ExitGame()
    {
        Application.Quit();
    }
}
