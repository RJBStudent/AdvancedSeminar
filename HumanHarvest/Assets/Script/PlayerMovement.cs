using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour {

    public float playerSpeed;

    public float noiseLevel;

    public GameObject InteractButton;

    float xDirection, yDirection;
    float lastX, lastY;
    bool hasHuman = false;
    bool inUFOArea = false;
    bool canInteract = false;
    bool stillPenalty = false;
    
    GameObject InteractableObject;
    Animator anim;
   
    

    // Use this for initialization
    void Start () {
        InteractButton.SetActive(false);
        anim = GetComponent<Animator>();
	}
	
	// Update is called once per frame
	void Update ()
    {
        if (stillPenalty)
        {
            gameObject.GetComponent<Rigidbody2D>().velocity = new Vector2(0, 0);

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
                        gameObject.GetComponent<SpriteRenderer>().color = new Color(255, 0, 0);
                    }
                }
                else if (hasHuman)
                {
                    Debug.Log("HasHuman");
                    if (inUFOArea)
                    {
                        //Destroy(InteractableObject);
                        hasHuman = false;
                        InteractButton.SetActive(false);
                        gameObject.GetComponent<SpriteRenderer>().color = new Color(1, 1, 1);
                        RoundManagerScript.code.RemoveHuman();
                        RoundManagerScript.code.AddScore(RoundManagerScript.ScoreType.HUMAN);
                    }

                }
            }
        }
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        Debug.Log("Enter");
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
                Debug.Log("Has HUman");
                InteractButton.SetActive(true);
            }
        }
    }

    void OnTriggerExit2D(Collider2D collision)
    {
        Debug.Log("Exit");
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
