﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HumanScript : MonoBehaviour
{

    public float speed = 1f;

    public GameObject[] trashcanList;
    public GameObject currentTrashTarget;    

    Vector2 targetDirection;
    Rigidbody2D rb;
    public LayerMask lMask;

    // Use this for initialization
    void Start()
    {
        trashcanList = GameObject.FindGameObjectsWithTag("Trashcan");
        UpdateClosest();
    }

    void UpdateClosest()
    {
        float shortestDistance = 200f;
        foreach (GameObject trashcan in trashcanList)
        {
            if (!trashcan.GetComponent<TrashcanScript>().HasHuman)
            {
                float tempDistance = Vector2.Distance(trashcan.transform.position, gameObject.transform.position);
                tempDistance = Mathf.Abs(tempDistance);
                if (tempDistance < shortestDistance)
                {
                    shortestDistance = tempDistance;
                    currentTrashTarget = trashcan;
                }
            }
        }
        targetDirection = currentTrashTarget.transform.position - transform.position;
        targetDirection.Normalize();

    }


    // Update is called once per frame
    void Update()
    {
        
        UpdateClosest();
        CheckWall();        
        Move();
       
    }

    void CheckWall()
    {
        Vector2 newTarget;
        for(int i = -1; i < 2; i++)
        {
            float currentAngle = Mathf.Atan2(targetDirection.y, targetDirection.x);
            currentAngle *= Mathf.Rad2Deg;
            currentAngle += i * 25;
            currentAngle *= Mathf.Deg2Rad;
            newTarget = new Vector2(Mathf.Cos(currentAngle), Mathf.Sin(currentAngle));

            Debug.DrawLine(gameObject.transform.position, gameObject.transform.position + (new Vector3(newTarget.x, newTarget.y, 0)* 2), new Color(255, 0, 0), 0f);

            RaycastHit2D hit = Physics2D.Raycast(gameObject.transform.position, newTarget, 2, lMask.value);
            if (hit)
            {
                Vector3 hitPerp = hit.normal;
                //hitPerp = Vector3.Normalize(hitPerp);
                
                Vector3 targetPosLocal = currentTrashTarget.transform.InverseTransformPoint(transform.position);
                if(targetPosLocal.x<0)
                {
                    targetDirection = new Vector2(Mathf.Abs(hitPerp.y), hitPerp.x);
                }
                else if(targetPosLocal.x> 0)
                {
                    targetDirection = new Vector2(-Mathf.Abs(hitPerp.y), hitPerp.x);
                }

                if (targetPosLocal.y < 0)
                {
                    targetDirection = new Vector2(hitPerp.y, Mathf.Abs(hitPerp.x));
                }
                else if (targetPosLocal.y > 0)
                {
                    targetDirection = new Vector2(hitPerp.y, -Mathf.Abs(hitPerp.x));
                }
               // Debug.Log(currentAngle*Mathf.Rad2Deg);
              
            }

            // Vector2 currentRaycastDirection = currentAngle + (i * 30);

            // RaycastHit2D hit = Physics2D.Raycast(gameObject.transform.position, targetDirection, 4, 8);    
        }

        

    }

    void OnCollisionEnter2D(Collision2D col)
    {
        if (col.gameObject.tag == "Trashcan")
        {
           
            if (!col.gameObject.GetComponent<TrashcanScript>().HasHuman)
            {
                Debug.Log("EnteredTrash");
                col.gameObject.GetComponent<TrashcanScript>().HasHuman = true;
                Destroy(gameObject);
            }
        }
    }

    void Move()
    {

        targetDirection /= speed;
        gameObject.transform.position = new Vector3(gameObject.transform.position.x+ targetDirection.x, gameObject.transform.position.y + targetDirection.y, gameObject.transform.position.z);
    }

}
