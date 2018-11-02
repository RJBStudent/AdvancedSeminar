using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TutorialScript : MonoBehaviour {	
	// Update is called once per frame
	void Update () {
        if (Input.GetButtonDown("Interact"))
        {
            SceneManager.LoadScene("GameScene");
        }
    }
}
