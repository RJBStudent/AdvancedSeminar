using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TypeOneEnemyScript : MonoBehaviour
{
    //Difficulty Variables
    public float secondsToIncreaseDifficulty;
    int currentTime = 0;


    //movement variables
    public int startNode;
    public float speed;
    public float maxSpeed;
    public Transform[] nodeLocations;
    public float nodeTargetRadius;
    public float waitAtNode;
    public float minWaitAtNode;
    int targetNode;
    Vector2 targetLocation;
    Vector2 targetDirection;
    bool isPatrolingOnNode;
    Vector2 lastHeardNoisePos;
    bool returningToNodePatrol = false;


    //Shooting Variables
    public float fireRadius;
    public float cooldownTime;
    public float minCoolDownTime;
    public float closestDistanceToPlayer;
    public float shouldNotChaseDistance;
    public float predictionRate;
    public float predictionMin;

    //Tests for movement
    bool canHear = false;
    bool canSee = false;
    bool shouldChase = false;
    bool moveToSound = false;
    bool shootingAtPlayer = false;

    //Outside effectors variables
    public GameObject thePlayer;
    public LayerMask playerAndWallLayerMask;
    public LayerMask wallMask;
    Rigidbody2D playerRigidbody;

    public float xBounds;
    public float yBounds;



    //Prefabs
    public GameObject bulletPrefab;
    GameObject exclamationPrefab;
    GameObject questionPrefab;

    SpriteRenderer exclamationSpriteRenderer;
    SpriteRenderer questionSpriteRenderer;


    // Use this for initialization
    void Start()
    {
        targetNode = startNode;
        gameObject.transform.position = nodeLocations[targetNode].transform.position;
        targetLocation = nodeLocations[targetNode].transform.position;
        targetDirection = targetLocation - new Vector2(gameObject.transform.position.x, gameObject.transform.position.y);
        exclamationPrefab = transform.GetChild(1).gameObject;
        questionPrefab = transform.GetChild(0).gameObject;
        exclamationSpriteRenderer = exclamationPrefab.GetComponent<SpriteRenderer>();
        questionSpriteRenderer = questionPrefab.GetComponent<SpriteRenderer>();
        exclamationSpriteRenderer.enabled = false;
        questionSpriteRenderer.enabled = false;
        playerRigidbody = thePlayer.gameObject.GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        IncreaseDifficulty();


        exclamationPrefab.transform.position = transform.position;
        questionPrefab.transform.position = transform.position;
        if (shootingAtPlayer)
        {
            exclamationSpriteRenderer.enabled = false;
            ExclamationRender();
            return;
        }
        canHear = GetComponent<NoiseListenerScript>().ListenForSound();
        canSee = GetComponent<VisionScript>().Vision(targetDirection.normalized);
        if (canSee)
        {
            shouldChase = true;
            ExclamationRender();
        }
        else if (canHear)
        {

            moveToSound = true;
            lastHeardNoisePos = thePlayer.transform.position;
            SeekingRender();
        }

        if (shouldChase)
        {
            questionSpriteRenderer.enabled = false;
            ExclamationRender();
            ChaseTarget();
        }
        else if (!moveToSound)
        {
            MoveToNode();
            exclamationSpriteRenderer.enabled = false;
            questionSpriteRenderer.enabled = false;
        }
        else
        {
            exclamationSpriteRenderer.enabled = false;
            SeekingRender();
            MoveToNoise();
        }
    }

    void IncreaseDifficulty()
    {
        if (currentTime == (int)RoundManagerScript.code.currentTime)
            return;
        if((int)RoundManagerScript.code.currentTime % secondsToIncreaseDifficulty == 0)
        {
            currentTime = (int)RoundManagerScript.code.currentTime;
            if (speed > maxSpeed)
            {
                speed -= 0.5f;
            }

            if(cooldownTime > minCoolDownTime)
            {
                cooldownTime -= .05f;
            }
            
            if(waitAtNode > minWaitAtNode)
            {
                waitAtNode -= 0.2f;
            }
            if(predictionRate > predictionMin)
            {
                predictionRate -= 0.2f;
            }
        }
    }

    void ChaseTarget()
    {
        //stay away from target a distance but follow them until out of shooting range. 
        //if within range, check if theres a wall that would be shot through, if there isnt shoot, stop movement before shooting for a few seconds
        //then catch up if still seeing        
        //if outside of chase range stop chaseing
        // Debug.Log("IM CHASING YOU");
        moveToSound = false;

        targetLocation = thePlayer.transform.position;
        targetDirection = targetLocation - new Vector2(gameObject.transform.position.x, gameObject.transform.position.y);

        if (targetDirection.magnitude > shouldNotChaseDistance)
        {
            shouldChase = false;
            return;
        }
        if (targetDirection.magnitude <= closestDistanceToPlayer)
        {
            //moveAway

            targetDirection = new Vector2(gameObject.transform.position.x, gameObject.transform.position.y) - targetLocation;
            targetDirection.Normalize();
            CheckWall();
            targetDirection /= speed;
            gameObject.transform.position = new Vector3(gameObject.transform.position.x + targetDirection.x, gameObject.transform.position.y + targetDirection.y, gameObject.transform.position.z);

            return;
        }
        if (targetDirection.magnitude < fireRadius)
        {
            //SHOOT
            if(!WallInShootRange())
            {
                StartCoroutine(FireAtPlayer());
                shouldChase = false;
                return;
            }            
        }

        targetDirection.Normalize();
        CheckWall();
        targetDirection /= speed;
        gameObject.transform.position = new Vector3(gameObject.transform.position.x + targetDirection.x, gameObject.transform.position.y + targetDirection.y, gameObject.transform.position.z);

    }

    bool WallInShootRange()
    {
        RaycastHit2D hit = Physics2D.Raycast(gameObject.transform.position, targetDirection, fireRadius, wallMask.value);
        if(hit)
        {
            return true;
        }

        return false;
    }

    void MoveToNode()
    {
        if (returningToNodePatrol)
        {
            float lowestMag = Mathf.Infinity;
            int arrayLoc = 0;
            for (int i = 0; i < nodeLocations.Length; i++)
            {
                Vector2 nodePos = nodeLocations[i].transform.position;
                Vector2 distance = nodePos - new Vector2(gameObject.transform.position.x, gameObject.transform.position.y);
                if (distance.magnitude < lowestMag)
                {
                    lowestMag = distance.magnitude;
                    arrayLoc = i;
                }
            }
            targetLocation = nodeLocations[arrayLoc].transform.position;
            returningToNodePatrol = false;
        }
        //if within certain distance of node switch to next node //nodeTarget++
        //move in direction of patrol node
        //check if theres a wall
        targetDirection = targetLocation - new Vector2(gameObject.transform.position.x, gameObject.transform.position.y);
        if (targetDirection.magnitude < nodeTargetRadius && !isPatrolingOnNode)
        {
            StartCoroutine(patrolOnNode());
        }
        if (isPatrolingOnNode)
        {
            //look left then right in time;
        }
        else
        {
            targetDirection.Normalize();

            CheckWall();
            targetDirection /= speed;
            gameObject.transform.position = new Vector3(gameObject.transform.position.x + targetDirection.x, gameObject.transform.position.y + targetDirection.y, gameObject.transform.position.z);
        }

    }
    void MoveToNoise()
    {
        //get noise position then move towards that
        //if seen change to move to noise
        //if nothing seen movetosound is false and pick closest patrol path.
        //Debug.Log("I HEARD SOMETHING");
        targetLocation = lastHeardNoisePos;
        targetDirection = targetLocation - new Vector2(gameObject.transform.position.x, gameObject.transform.position.y);
        if (targetDirection.magnitude < nodeTargetRadius)
        {
            //LookBothWays
            StartCoroutine(patrolOnNode());
            //Return to node path
            Debug.Log("Nothing's here");
            moveToSound = false;
            returningToNodePatrol = true;
            return;
        }
        targetDirection.Normalize();

        CheckWall();
        targetDirection /= speed;
        gameObject.transform.position = new Vector3(gameObject.transform.position.x + targetDirection.x, gameObject.transform.position.y + targetDirection.y, gameObject.transform.position.z);

    }


    IEnumerator patrolOnNode()
    {
        isPatrolingOnNode = true;
        yield return new WaitForSeconds(waitAtNode);
        targetNode++;
        targetNode %= nodeLocations.Length;
        targetLocation = nodeLocations[targetNode].transform.position;
        targetDirection = targetLocation - new Vector2(gameObject.transform.position.x, gameObject.transform.position.y);
        isPatrolingOnNode = false;

    }

    IEnumerator FireAtPlayer()
    {
        //Fire
        shootingAtPlayer = true;
        yield return new WaitForSeconds(cooldownTime);
        Vector2 target = new Vector2(targetDirection.x + (playerRigidbody.velocity.x/ predictionRate), targetDirection.y + (playerRigidbody.velocity.y/ predictionRate));
        GameObject bullet = (GameObject)Instantiate(bulletPrefab, gameObject.transform.position, Quaternion.identity);
        bullet.GetComponent<BulletScript>().fireDirection = target ;
        targetLocation = thePlayer.transform.position;
        targetDirection = targetLocation - new Vector2(gameObject.transform.position.x, gameObject.transform.position.y);

        if (targetDirection.magnitude < shouldNotChaseDistance)
        {
            shouldChase = true;
        }
        else if (targetDirection.magnitude > shouldNotChaseDistance)
        {
            shouldChase = false;
        }
        shootingAtPlayer = false;
    }

    void CheckWall()
    {
        Vector2 newTarget;
        for (int i = -1; i < 2; i++)
        {
            float currentAngle = Mathf.Atan2(targetDirection.y, targetDirection.x);
            currentAngle *= Mathf.Rad2Deg;
            currentAngle += i * 25;
            currentAngle *= Mathf.Deg2Rad;
            newTarget = new Vector2(Mathf.Cos(currentAngle), Mathf.Sin(currentAngle));

            Debug.DrawLine(gameObject.transform.position, gameObject.transform.position + (new Vector3(newTarget.x, newTarget.y, 0) * 2), new Color(0, 0, 255), 0f);

            RaycastHit2D hit = Physics2D.Raycast(gameObject.transform.position, newTarget, 2, playerAndWallLayerMask.value);
            if (hit)
            {
                Vector3 hitPerp = hit.normal;

                //hitPerp = Vector3.Normalize(hitPerp);
                //targetLocation.
                Vector3 targetPosLocal = transform.InverseTransformPoint(new Vector3(targetLocation.x, targetLocation.y, 0));

                if (targetPosLocal.x <= 0)
                {
                    targetDirection = new Vector2(-Mathf.Abs(hitPerp.y), hitPerp.x);

                }
                else if (targetPosLocal.x >= 0)
                {
                    targetDirection = new Vector2(Mathf.Abs(hitPerp.y), hitPerp.x);

                }

                if (targetPosLocal.y <= 0)
                {
                    targetDirection = new Vector2(targetDirection.x, -Mathf.Abs(hitPerp.x));
                }
                else if (targetPosLocal.y >= 0)
                {
                    targetDirection = new Vector2(targetDirection.x, Mathf.Abs(hitPerp.x));
                }

                // Debug.Log(currentAngle*Mathf.Rad2Deg);              
            }
        }
    }

    void ExclamationRender()
    {
        exclamationPrefab.transform.position = transform.position;
        //Vector3 localPosition = thePlayer.transform.InverseTransformPoint(gameObject.transform.position);

        exclamationSpriteRenderer.enabled = true;
        exclamationPrefab.transform.position = new Vector3(Mathf.Clamp(exclamationPrefab.transform.position.x, thePlayer.transform.position.x - xBounds, thePlayer.transform.position.x + xBounds), Mathf.Clamp(exclamationPrefab.transform.position.y, thePlayer.transform.position.y - yBounds, thePlayer.transform.position.y + yBounds), 0);
    }

    void SeekingRender()
    {
        questionPrefab.transform.position = transform.position;
        //Vector3 localPosition = thePlayer.transform.InverseTransformPoint(gameObject.transform.position);

        questionSpriteRenderer.enabled = true;
        questionSpriteRenderer.transform.position = new Vector3(Mathf.Clamp(exclamationPrefab.transform.position.x, thePlayer.transform.position.x - xBounds, thePlayer.transform.position.x + xBounds), Mathf.Clamp(exclamationPrefab.transform.position.y, thePlayer.transform.position.y - yBounds, thePlayer.transform.position.y + yBounds), 0);
       
    }
}