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

                }
                else if (InteractableObject.tag == "Trashcan" && !hasHuman)
                {
                    hasHuman = true;
                    canInteract = false;
                    InteractableObject.GetComponent<TrashcanScript>().HasHuman = false;
                    InteractButton.SetActive(false);
                    gameObject.GetComponent<SpriteRenderer>().color = new Color(255, 0, 0);
                }
                else if (hasHuman)
                {
                    if (inUFOArea)
                    {
                        //Destroy(InteractableObject);
                        hasHuman = false;
                        InteractButton.SetActive(false);
                        gameObject.GetComponent<SpriteRenderer>().color = new Color(0, 0, 0);
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
            if (collision.gameObject.GetComponent<TrashcanScript>().HasHuman)
            {
                if (!hasHuman)
                {
                    canInteract = true;
                    InteractableObject = collision.gameObject;
                    InteractButton.SetActive(true);
                }
            }
        }
        if (collision.gameObject.tag == "UFO_Area")
        {
            inUFOArea = true;
            canInteract = true;
            if (hasHuman)
            {
                InteractButton.SetActive(true);
            }
        }
    }

    void OnTriggerExit2D(Collider2D collision)
    {
        Debug.Log("Exit");
        if (collision.gameObject.tag == "Trashcan")
        {
            if (collision.gameObject.GetComponent<TrashcanScript>().HasHuman)
            {

                if (!hasHuman)
                {
                    canInteract = false;
                    InteractableObject = null;
                    InteractButton.SetActive(false);
                }
            }
        }
        if(collision.gameObject.tag == "UFO_Area")
        {
            inUFOArea = false;
            canInteract = false;
            if (hasHuman)
            {
                InteractButton.SetActive(false);
            }
        }
    }


}
