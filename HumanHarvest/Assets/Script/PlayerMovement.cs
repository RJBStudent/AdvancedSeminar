﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerMovement : MonoBehaviour {

    public float playerSpeed;

    public float noiseLevel;

    public GameObject InteractButton;
    public GameObject humanPrefab;

    public int health = 3;
    public float iFrameTime = 1f;

    float xDirection, yDirection;
    float lastX, lastY;
    bool hasHuman = false;
    bool inUFOArea = false;
    bool canInteract = false;
    bool stillPenalty = false;
    
    GameObject InteractableObject;
    Animator anim;

    public bool cantGetHit;
    
    

    // Use this for initialization
    void Start () {
        InteractButton.SetActive(false);
        anim = GetComponent<Animator>();
	}
	
	// Update is called once per frame
	void Update ()
    {
        if(health <= 0)
        {
            //Smoothly Send Coroutine to UI manager that says game over
            //SceneManager.LoadScene("GameEndScene");
            RoundManagerScript.code.GameEndTransition();
        }
        if (stillPenalty)
        {
            gameObject.GetComponent<Rigidbody2D>().velocity = new Vector2(0, 0);
            noiseLevel = 2;

            return;
        }
            
        RecieveInput();
        Interact();
        PlayerMove();
       
    }

    void RecieveInput()
    {  
        xDirection = Input.GetAxis("Horizontal");
        yDirection = Input.GetAxis("Vertical");

        if (xDirection == 0f && yDirection == 0f)
        {
            anim.SetFloat("LastY", lastY);
            anim.SetFloat("LastX", lastX);
            anim.SetBool("Move", false);

        }
        else
        {
                lastX = xDirection;
                lastY = yDirection;
                anim.SetBool("Move", true);
        }
        anim.SetFloat("VelocityY", yDirection);
        anim.SetFloat("VelocityX", xDirection);

        noiseLevel = (Mathf.Abs(xDirection) + Mathf.Abs(yDirection)) / 2;
        
    }

    void PlayerMove()
    {
        gameObject.GetComponent<Rigidbody2D>().velocity = new Vector2(xDirection * playerSpeed, yDirection * playerSpeed);
    }

    void Interact()
    {
        if(canInteract == true)
        {
            if (Input.GetButtonDown("Interact"))
            {
                Debug.Log("Interact");
                if(InteractableObject == null)
                {
                    Debug.Log("NOTHING");
                    return;
                }
                else if (InteractableObject.tag == "Trashcan" && !hasHuman)
                {
                    if (!InteractableObject.GetComponent<TrashcanScript>().HasHuman)
                    {
                        gameObject.GetComponent<SpriteRenderer>().color = new Color(0, 255, 0);
                        //canInteract = false;
                        //InteractButton.SetActive(false);
                        StartCoroutine(TrashcanMiss());
                    }
                    else
                    { 
                        hasHuman = true;
                        canInteract = false;
                        InteractableObject.GetComponent<TrashcanScript>().HasHuman = false;
                        InteractButton.SetActive(false);
                        
                        anim.SetBool("Has Human", true);
                    }
                }
                else if (hasHuman)
                {
                    Debug.Log("HasHuman");
                    if (inUFOArea)
                    {
                        //Destroy(InteractableObject);
                        hasHuman = false;
                        anim.SetBool("Has Human", false);
                        InteractButton.SetActive(false);
                        gameObject.GetComponent<SpriteRenderer>().color = new Color(1, 1, 1);
                        RoundManagerScript.code.RemoveHuman();
                        RoundManagerScript.code.AddScore(RoundManagerScript.ScoreType.HUMAN);
                    }

                }
            }
        }
    }

    public void playerHit()
    {
        if(!cantGetHit)
        {
            if(hasHuman)
            {
                GameObject newHuman = (GameObject)Instantiate(humanPrefab, transform.position, Quaternion.identity, GameObject.Find("RoundManager").transform);
                Physics2D.IgnoreCollision(GetComponent<Collider2D>(), newHuman.GetComponent<Collider2D>());
                hasHuman = false;
                anim.SetBool("Has Human", false);
                gameObject.GetComponent<SpriteRenderer>().color = new Color(1, 1, 1);
            }
            cantGetHit = true;
            StartCoroutine(IFrames());
        }
    }

    IEnumerator IFrames()
    {
        gameObject.GetComponent<SpriteRenderer>().color = new Color(gameObject.GetComponent<SpriteRenderer>().color.r, gameObject.GetComponent<SpriteRenderer>().color.g, gameObject.GetComponent<SpriteRenderer>().color.b, 0.5f);
        health--;
        yield return new WaitForSeconds(iFrameTime);
        cantGetHit = false;
        gameObject.GetComponent<SpriteRenderer>().color = new Color(gameObject.GetComponent<SpriteRenderer>().color.r, gameObject.GetComponent<SpriteRenderer>().color.g, gameObject.GetComponent<SpriteRenderer>().color.b, 1);
    }

    void OnTriggerEnter2D(Collider2D collision)
    {

        if (collision.gameObject.tag == "Trashcan")
        {
            if(!hasHuman)
            {
                canInteract = true;
                InteractableObject = collision.gameObject;
                InteractButton.SetActive(true);
            }
               
           
        }
        if (collision.gameObject.tag == "UFO_Area")
        {
            inUFOArea = true;
            canInteract = true;
            if (hasHuman)
            {
                InteractableObject = collision.gameObject;
                InteractButton.SetActive(true);
            }
        }
    }

    void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Trashcan")
        {            
                canInteract = false;
                InteractableObject = null;
                InteractButton.SetActive(false);
        }
        if(collision.gameObject.tag == "UFO_Area")
        {
            inUFOArea = false;
            canInteract = false;
            if (hasHuman)
            {

                InteractableObject = null;
                InteractButton.SetActive(false);
            }
        }
    }

    IEnumerator TrashcanMiss()
    {
        stillPenalty = true;
        yield return new WaitForSeconds(3f);
        stillPenalty = false;
        canInteract = false;
        InteractableObject = null;
        if (!hasHuman)
        {
            gameObject.GetComponent<SpriteRenderer>().color = new Color(1, 1, 1);
        }
        else
        {
            gameObject.GetComponent<SpriteRenderer>().color = new Color(1, 0, 0);
        }
    }
}
