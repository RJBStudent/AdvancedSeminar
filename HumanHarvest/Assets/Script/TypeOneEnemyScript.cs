using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TypeOneEnemyScript : MonoBehaviour {
    
    //movement variables
    public float speed;
    public Transform[] nodeLocations;
    public float nodeTargetRadius;
    int targetNode;
    Vector2 targetLocation;
    Vector2 targetDirection;

    //Shooting Variables
    public float fireRadius;
    public float cooldownTime;
    public float closestDistanceToPlayer;

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
        targetNode = 0;
        gameObject.transform.position = nodeLocations[targetNode].transform.position;
        targetLocation = nodeLocations[targetNode].transform.position;
        targetDirection = targetLocation - new Vector2(gameObject.transform.position.x, gameObject.transform.position.y);

    }
	
	// Update is called once per frame
	void Update ()
    {

        canHear = GetComponent<NoiseListenerScript>().ListenForSound();
        canSee = GetComponent<VisionScript>().Vision(targetDirection.normalized);
        if (canSee)
        {
            shouldChase = true;
        }
        else if (canHear)
        {
            moveToSound = true;
        }

        if (shouldChase)
        {
            ChaseTarget();
        }
        else if (!moveToSound)
        {
            MoveToNode();
        }
        else
        {
            MoveToNoise();
        }
        //MoveToNode();
    }

    void ChaseTarget()
    {
        //stay away from target a distance but follow them until out of shooting range. 
        //if within range, check if theres a wall that would be shot through, if there isnt shoot, stop movement before shooting for a few seconds
        //then catch up if still seeing        
        //if outside of chase range stop chaseing
       // Debug.Log("IM CHASING YOU");
    }

    void MoveToNode()
    {
        //if within certain distance of node switch to next node //nodeTarget++
        //move in direction of patrol node
        //check if theres a wall
        targetDirection = targetLocation -new Vector2(gameObject.transform.position.x, gameObject.transform.position.y);
        if(targetDirection.magnitude < nodeTargetRadius)
        {
            targetNode++;
            targetNode %= nodeLocations.Length;
           // Debug.Log(targetNode);
            targetLocation = nodeLocations[targetNode].transform.position;
            targetDirection = targetLocation - new Vector2(gameObject.transform.position.x, gameObject.transform.position.y);
        }
        targetDirection.Normalize();

        targetDirection /= speed;
        gameObject.transform.position = new Vector3(gameObject.transform.position.x + targetDirection.x, gameObject.transform.position.y + targetDirection.y, gameObject.transform.position.z);
    }
    void MoveToNoise()
    {
        //get noise position then move towards that
        //if seen change to move to noise
        //if nothing seen movetosound is false and pick closest patrol path.
        Debug.Log("I HEARD SOMETHING");
    }

    void DetectWall()
    {
        //same as human script except check in every search
    }
}
