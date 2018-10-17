using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletScript : MonoBehaviour {

    public float lifetime = 3f;
    public float speed = 3f;

    public Vector2 fireDirection;

    float existenceTimer = 0f;
    


	// Use this for initialization
	void Start () {
        //targetDirection = new Vector2(gameObject.transform.rotation.x, gameObject.transform.rotation.y);
        //targetDirection.Normalize();
        fireDirection.Normalize();
        float direction = Mathf.Atan2(fireDirection.y, fireDirection.x);
        direction = Mathf.Rad2Deg * direction;
        gameObject.transform.SetPositionAndRotation(gameObject.transform.position, Quaternion.Euler(0 ,0, direction));
    }
	
	// Update is called once per frame
	void Update () {
        existenceTimer += Time.deltaTime;
        if(existenceTimer >= lifetime)
        {
            Destroy(gameObject);
        }
        
        gameObject.transform.position = new Vector3(gameObject.transform.position.x + (fireDirection.x/speed), gameObject.transform.position.y + (fireDirection.y/speed), gameObject.transform.position.z);
    }
    void OnCollisionEnter2D(Collision2D collision)
    {
        if(collision.gameObject.tag == "Player")
        {
            //Take Damage
            collision.gameObject.GetComponent<PlayerMovement>().playerHit();
            Destroy(gameObject);
        }
        if(LayerMask.LayerToName(collision.gameObject.layer) == "Trashcan")
        {
            Physics2D.IgnoreCollision(collision.gameObject.GetComponent<Collider2D>(), gameObject.GetComponent<Collider2D>());
        }
        if(LayerMask.LayerToName(collision.gameObject.layer) == "Wall")
        {
            Destroy(gameObject);
        }
    }
}
