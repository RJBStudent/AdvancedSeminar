using System.Collections;
using System.Collections.Generic;
using UnityEngine;


//Stationary Enemy
public class TypeTwoEnemyScript : MonoBehaviour {

    //test for aggresion
    bool canSee = false;
    bool didSeeLastFrame = false;
    bool lockedOn = false;

    //Viewing direction variables
    Vector2 targetDirection;
    public float[] directionNodes;
    public float waitAtDirection;
    public float minWaitAtDirection;
    public float rotateSpeed;
    public float maxRotateSpeed;

    //shooting variables
    public float timeToNotice;
    public float minTimeToNotice;
    float currentTimeNoticed  = 0;
    

    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void FixedUpdate ()
    {
        if (!lockedOn)
        {
            canSee = GetComponent<VisionScript>().Vision(targetDirection.normalized);
            if (didSeeLastFrame)
            {
                currentTimeNoticed += Time.deltaTime;
                //addRedToViewingCone
            }
            else
            {
                currentTimeNoticed = 0;
                //ViewingConeIsWhite
            }
            didSeeLastFrame = canSee;
            if (currentTimeNoticed >= timeToNotice)
            {
                lockedOn = true;
            }
        }
        
    }
}
