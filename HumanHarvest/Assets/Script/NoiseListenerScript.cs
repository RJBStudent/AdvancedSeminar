using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NoiseListenerScript : MonoBehaviour {

    public float listenDistance;
    public GameObject thePlayer;
    public float listenVolume = 1f; //needs to be from 0.0f to 1.0f

   

    public bool ListenForSound()
    {
        Vector2 playerPos = thePlayer.transform.position;
        Vector2 curPos = gameObject.transform.position;
        float distance = Vector2.Distance(curPos, playerPos);
        float listenMultiplier = distance / listenDistance;
        float playerNoiseLevel = thePlayer.GetComponent<PlayerMovement>().noiseLevel;

        if (listenMultiplier > 1)
        {
            return false;
        }
        listenMultiplier -= 1;
        listenMultiplier = Mathf.Abs(listenMultiplier);
        playerNoiseLevel *= listenMultiplier;
        if(playerNoiseLevel < listenVolume)
        {
            Debug.Log(listenMultiplier);
            return false;
        }
        return true;
    }
}
