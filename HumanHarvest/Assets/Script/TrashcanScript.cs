using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrashcanScript : MonoBehaviour
{


    public bool HasHuman = false;
    PlayerMovement thePlayerValues;
    NoiseListenerScript playerListener;
    Transform thePlayer;

    bool dontLookOut = false;

    Animator anim;

    float listenDistance;
    public float defaultLookOutTime = 5;
    public float shouldLookOutRangeMin = 1, shouldLookOutRangeMax = 3;
    float shouldLookOutTimer;
    float currentLookOutTime;
    

    // Use this for initialization
    void Start()
    {
        thePlayer = GameObject.Find("Player").transform;
        thePlayerValues = thePlayer.gameObject.GetComponent<PlayerMovement>();
        anim = GetComponent<Animator>();
        playerListener = GetComponent<NoiseListenerScript>();
        listenDistance = playerListener.listenDistance;
        currentLookOutTime = defaultLookOutTime * Random.Range(shouldLookOutRangeMin, shouldLookOutRangeMax);
        playerListener.thePlayer = thePlayer.gameObject;        
    }

    // Update is called once per frame
    void Update()
    {
        
        if(!HasHuman)
        {
            return;
        }
        if(IsInListenRadius())
        {
            if(playerListener.ListenForSound())
            {
                Debug.Log("HIDING");
                shouldLookOutTimer = 0;
                return;
            }
        }
        shouldLookOutTimer += Time.deltaTime;
        if(shouldLookOutTimer >=currentLookOutTime)
        {
            anim.SetTrigger("LookOut");
            shouldLookOutTimer = 0;
            currentLookOutTime = defaultLookOutTime * Random.Range(shouldLookOutRangeMin, shouldLookOutRangeMax);
        }
    }

    bool IsInListenRadius()
    {
        Vector2 distance = thePlayer.transform.position - transform.position;
        if(distance.magnitude < listenDistance)
        {
            return true;
        }
        return false;
    }

}
