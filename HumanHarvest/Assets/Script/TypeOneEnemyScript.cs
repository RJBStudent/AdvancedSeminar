using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TypeOneEnemyScript : MonoBehaviour {
    
    //movement variables
    public float speed;
    Vector2 targetLocation;

    //Tests for movement
    bool canHear = false;
    bool canSee = false;
    bool shouldChase = false;
    bool moveToSound = false;

    //Outside effectors variables
    public GameObject thePlayer;
    public LayerMask lMask;


    //Prefabs
    public GameObject bulletPrefab;


	// Use this for initialization
	void Start ()
    {
		
	}
	
	// Update is called once per frame
	void Update ()
    {
       canHear = GetComponent<NoiseListenerScript>().ListenForSound();
       canSee = GetComponent<VisionScript>().Vision(targetLocation.normalized);
        if(canSee)
        {
            shouldChase = true;
        }
        else if(canHear)
        {
            moveToSound = true;
        }

	}
}
