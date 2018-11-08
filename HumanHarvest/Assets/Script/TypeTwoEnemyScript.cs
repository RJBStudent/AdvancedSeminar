using System.Collections;
using System.Collections.Generic;
using UnityEngine;


//Stationary Enemy
public class TypeTwoEnemyScript : MonoBehaviour
{

    Transform thisTransform;

    //test for aggresion
    bool canSee = false;
    bool didSeeLastFrame = false;
    bool lockedOn = false;

    //Difficulty Variables
    public float secondsToIncreaseDifficulty;
    int currentTime = 0;

    //Viewing direction variables
    float targetDirection;
    Vector2 targetLocation;
    public float[] directionNodes;
    public float waitAtDirection;
    public float minWaitAtDirection;
    public float rotateSpeed;
    public float maxRotateSpeed;
    float currentRotationAngle;
    int currentNodeCount = 0;
    bool nodesIncreasing = true;
    bool isPatrolingOnNode = false;
    bool returningToNodePatrol = false;

    //shooting variables
    public float timeToNotice;
    public float minTimeToNotice;
    float currentTimeNoticed = 0;
    public float cooldownTime;
    public float maxCooldownTime;
    bool canShoot = true;
    public float bulletSpeed;
    public float minBulletSpeed;
    public float predictionRate;
    public float predictionMin;

    //Outside Effectors
    public Transform thePlayerTransform;
    Rigidbody2D playerRigidbody;

    VisionScript visionScript;

    public float xBounds;
    public float yBounds;

    //Prefabs
    public GameObject bulletPrefab;
    GameObject exclamationPrefab;
    GameObject questionPrefab;
    Transform coneTransform;

    SpriteRenderer exclamationSpriteRenderer;
    SpriteRenderer questionSpriteRenderer;
    SpriteRenderer coneSpriteRenderer;

    ParticleSystem shootEffect;

    [SerializeField]
    private AudioSource shootSound;

    // Use this for initialization
    void Start()
    {
        currentRotationAngle = directionNodes[currentNodeCount];
        //thisTransform.rotation = Quaternion.Euler(AngleToVector3(currentRotationAngle));

        // thisTransform = gameObject.transform;
        visionScript = gameObject.GetComponent<VisionScript>();
        exclamationPrefab = transform.GetChild(1).gameObject;
        questionPrefab = transform.GetChild(0).gameObject;
        coneTransform = transform.GetChild(2).gameObject.transform;
        exclamationSpriteRenderer = exclamationPrefab.GetComponent<SpriteRenderer>();
        questionSpriteRenderer = questionPrefab.GetComponent<SpriteRenderer>();
        coneSpriteRenderer = coneTransform.gameObject.GetComponent<SpriteRenderer>();
        exclamationSpriteRenderer.enabled = false;
        questionSpriteRenderer.enabled = false;
        coneTransform.rotation = Quaternion.Euler(AngleToVector3(currentRotationAngle));
        playerRigidbody = thePlayerTransform.gameObject.GetComponent<Rigidbody2D>();
        shootEffect = GetComponent<ParticleSystem>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        canSee = GetComponent<VisionScript>().Vision(AngleToNormal(currentRotationAngle));
        if (!lockedOn)
        {
            // canSee = visionScript.Vision(AngleToNormal(currentRotationAngle));

            if (didSeeLastFrame)
            {
                exclamationSpriteRenderer.enabled = false;
                currentTimeNoticed += Time.deltaTime;
                coneSpriteRenderer.color = new Color(1f, 1f - (currentTimeNoticed / timeToNotice), 1f - (currentTimeNoticed / timeToNotice), .5f);
                SeekingRender();
                //addRedToViewingCone
            }
            else
            {
                exclamationSpriteRenderer.enabled = false;
                questionSpriteRenderer.enabled = false;
                coneSpriteRenderer.color = new Color(1f, 1f, 1f, .5f);
                currentTimeNoticed = 0;
                //ViewingConeIsWhite
            }
            didSeeLastFrame = canSee;
            if (currentTimeNoticed >= timeToNotice)
            {
                questionSpriteRenderer.enabled = false;
                lockedOn = true;
            }
            RotateToNode();
        }
        else
        {

            ExclamationRender();
            FollowPlayer();
            //while firing still follow
            if (!canSee)
            {
                returningToNodePatrol = true;
                lockedOn = false;
            }
        }
        coneTransform.rotation = Quaternion.Euler(AngleToVector3(currentRotationAngle));

    }

    void IncreaseDifficulty()
    {
        if (currentTime == (int)RoundManagerScript.code.currentTime)
            return;
        if ((int)RoundManagerScript.code.currentTime % secondsToIncreaseDifficulty == 0)
        {
            currentTime = (int)RoundManagerScript.code.currentTime;
            if (rotateSpeed > maxRotateSpeed)
            {
                rotateSpeed -= 0.5f;
            }

            if (cooldownTime > maxCooldownTime)
            {
                cooldownTime -= .05f;
            }

            if (waitAtDirection > minWaitAtDirection)
            {
                waitAtDirection -= 0.2f;
            }
            if (bulletSpeed > minBulletSpeed)
            {
                bulletSpeed -= 0.2f;
            }
            if (predictionRate > predictionMin)
            {
                predictionRate -= 0.2f;
            }
        }
    }

    void RotateToNode()
    {
        float nextNodeAngle = 0;


        if (returningToNodePatrol)
        {
            currentRotationAngle = Mathf.Clamp(Mathf.Abs(currentRotationAngle), 0, 360);

            float lowestLength = Mathf.Infinity;
            int arrayLoc = 0;
            for (int i = 0; i < directionNodes.Length; i++)
            {
                float arrayAngle = directionNodes[i];
                float distance = arrayAngle - currentRotationAngle;
                if (Mathf.Abs(distance) < lowestLength)
                {
                    lowestLength = Mathf.Abs(distance);
                    arrayLoc = i;
                }
            }
            nextNodeAngle = directionNodes[arrayLoc];
            currentNodeCount = arrayLoc;

            returningToNodePatrol = false;
        }
        else
        {
            nextNodeAngle = directionNodes[currentNodeCount];
        }
        targetDirection = nextNodeAngle - currentRotationAngle;


        if (Mathf.Abs(targetDirection) < 1 && !isPatrolingOnNode)
        {
            StartCoroutine(patrolOnNode());
        }

        if (!isPatrolingOnNode)
        {
            targetDirection /= rotateSpeed;
            currentRotationAngle += targetDirection;
            /*   thisTransform.rotation */ //gameObject.transform.rotation= Quaternion.Euler(AngleToVector3(currentRotationAngle));

        }

    }

    void FollowPlayer()
    {
        targetLocation = thePlayerTransform.position - transform.position;
        targetLocation.Normalize();
        float newAngle = Mathf.Atan2(targetLocation.y, targetLocation.x);
        newAngle *= Mathf.Rad2Deg;
        currentRotationAngle = Mathf.Clamp(newAngle, 0, 360);
        currentRotationAngle = newAngle;
        if (canShoot)
        {
            StartCoroutine(FireAtPlayer());
        }
    }

    IEnumerator FireAtPlayer()
    {
        canShoot = false;
        yield return new WaitForSeconds(cooldownTime);
        if (lockedOn)
        {
            GameObject bullet = (GameObject)Instantiate(bulletPrefab, gameObject.transform.position, Quaternion.identity);
            Vector2 target = new Vector2(targetLocation.x + (playerRigidbody.velocity.x / predictionRate), targetLocation.y + (playerRigidbody.velocity.y/ predictionRate));
            bullet.GetComponent<BulletScript>().fireDirection = target.normalized;
            bullet.GetComponent<BulletScript>().speed = bulletSpeed;
            shootEffect.Play();
            shootSound.Play();
            canShoot = true;
        }
    }


    IEnumerator patrolOnNode()
    {
        isPatrolingOnNode = true;

        yield return new WaitForSeconds(waitAtDirection);
        if (nodesIncreasing)
        {
            if ((currentNodeCount + 1) % directionNodes.Length == 0)
            {

                currentNodeCount = directionNodes.Length - 1;
                nodesIncreasing = false;
            }
            else
            {
                currentNodeCount++;
            }
        }
        else
        {
            if (currentNodeCount <= 0)
            {
                currentNodeCount = 0;
                nodesIncreasing = true;
            }
            else
            {

                currentNodeCount--;
            }
            // nextNodeAngle = currentNodeCount;
        }
        // targetLocation = nodeLocations[targetNode].transform.position;
        //targetDirection = targetLocation - new Vector2(gameObject.transform.position.x, gameObject.transform.position.y);
        isPatrolingOnNode = false;

    }

    Vector3 AngleToVector3(float angle)
    {
        return new Vector3(0, 0, angle);
    }

    Vector2 AngleToNormal(float angle)
    {
        float radian = angle * Mathf.Deg2Rad;
        Vector2 newVect = new Vector2(Mathf.Cos(radian), Mathf.Sin(radian));
        newVect.Normalize();
        return newVect;
    }


    void ExclamationRender()
    {
        exclamationPrefab.transform.position = transform.position;
        //Vector3 localPosition = thePlayer.transform.InverseTransformPoint(gameObject.transform.position);

        exclamationSpriteRenderer.enabled = true;
        exclamationPrefab.transform.position = new Vector3(Mathf.Clamp(exclamationPrefab.transform.position.x, thePlayerTransform.position.x - xBounds, thePlayerTransform.position.x + xBounds), Mathf.Clamp(exclamationPrefab.transform.position.y, thePlayerTransform.position.y - yBounds, thePlayerTransform.position.y + yBounds), 0);
    }

    void SeekingRender()
    {
        questionPrefab.transform.position = transform.position;
        //Vector3 localPosition = thePlayer.transform.InverseTransformPoint(gameObject.transform.position);

        questionSpriteRenderer.enabled = true;
        questionSpriteRenderer.transform.position = new Vector3(Mathf.Clamp(exclamationPrefab.transform.position.x, thePlayerTransform.position.x - xBounds, thePlayerTransform.position.x + xBounds), Mathf.Clamp(exclamationPrefab.transform.position.y, thePlayerTransform.position.y - yBounds, thePlayerTransform.position.y + yBounds), 0);

    }
}
