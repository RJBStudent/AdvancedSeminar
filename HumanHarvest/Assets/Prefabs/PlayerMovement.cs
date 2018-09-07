using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour {

    public float playerSpeed;

    public float noiseLevel;

    float xDirection, yDirection;
    bool hasHuman = false;
    bool inUFOArea = false;
    bool canInteract = false;
    
    GameObject InteractableObject;

    // Use this for initialization
    void Start () {
		
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
                else if (InteractableObject.tag == "Human" && !hasHuman)
                {
                    hasHuman = true;
                    canInteract = false;
                    InteractableObject.SetActive(false);
                }
                else if (hasHuman)
                {
                    if (inUFOArea)
                    {
                        Destroy(InteractableObject);
                        hasHuman = false;
                    }

                }
            }
        }
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        Debug.Log("Enter");
        if (collision.gameObject.tag == "Human")
        {
            canInteract = true;
            InteractableObject = collision.gameObject;
        }
        if (collision.gameObject.tag == "UFO_Area")
        {
            inUFOArea = true;
            canInteract = true;
        }
    }

    void OnTriggerExit2D(Collider2D collision)
    {
        Debug.Log("Exit");
        if (collision.gameObject.tag == "Human")
        {
            canInteract = false;
            InteractableObject = null;
        }
        if(collision.gameObject.tag == "UFO_Area")
        {
            inUFOArea = false;
            canInteract = false;
        }
    }
}
