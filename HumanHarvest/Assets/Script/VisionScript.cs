using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VisionScript : MonoBehaviour {

    public GameObject thePlayer;
    public LayerMask lMask;
    public int raycastAmmount;
    public float distanceBetweenRaycasts;
    public float viewDistance;

	public bool Vision(Vector2 direction)
    {
        Vector2 newTarget;
        for (int i = -raycastAmmount; i <= raycastAmmount; i++)
        {
            float currentAngle = Mathf.Atan2(direction.y, direction.x);
            currentAngle *= Mathf.Rad2Deg;
            currentAngle += i * distanceBetweenRaycasts;
            currentAngle *= Mathf.Deg2Rad;
            newTarget = new Vector2(Mathf.Cos(currentAngle), Mathf.Sin(currentAngle));

            Debug.DrawLine(gameObject.transform.position, gameObject.transform.position + (new Vector3(newTarget.x, newTarget.y, 0) * viewDistance), new Color(255, 0, 0), 0f);

            RaycastHit2D hit = Physics2D.Raycast(gameObject.transform.position, newTarget, viewDistance, lMask.value);
            if(hit)
            {
                if(hit.transform.gameObject.layer == LayerMask.NameToLayer("Player"))
                {
                    return true;
                }
            }
        }
            return false;
    }
}
