using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour {

    public float playerSpeed;
    public float xMax, xMin, yMax, yMin;

    public float noiseLevel;

    float xDirection, yDirection;
    bool hasHuman = false;

    

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update ()
    {
        RecieveInput();
        PlayerMove();
        PlayerLimit();
       
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

    void PlayerLimit()
    {
        if(gameObject.transform.position.x < xMin)
        {
            gameObject.transform.position = new Vector2(xMin, transform.position.y);

        }
        else if (gameObject.transform.position.x > xMax)
        {
            gameObject.transform.position = new Vector2(xMax, transform.position.y);
        }


        if (gameObject.transform.position.y > yMax)
        {
            gameObject.transform.position = new Vector2(transform.position.x, yMax);
        }
        else if (gameObject.transform.position.y < yMin)
        {
            gameObject.transform.position = new Vector2(transform.position.x, yMin);
        }
    }

}
