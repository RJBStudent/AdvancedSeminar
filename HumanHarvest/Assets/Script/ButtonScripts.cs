using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ButtonScripts : MonoBehaviour
{
       Animator anim;

    void Start()
    {
        anim = GetComponent<Animator>();
    }

    public void StartGame()
    {
        SceneManager.LoadScene("TutorialScene");
    }

    public void ExitGame()
    {
        Application.Quit();
    }

    public void SetAnim()
    {
        anim.SetBool("shouldAnimate", !anim.GetBool("shouldAnimate"));
    }
}
