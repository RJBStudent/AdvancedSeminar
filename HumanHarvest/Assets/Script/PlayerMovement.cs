using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class PlayerMovement : MonoBehaviour
{

    public float playerSpeed;

    public float noiseLevel;

    public GameObject InteractButton;
    public GameObject humanPrefab;
    public Image interactTimeSprite;

    public int health = 3;
    public float iFrameTime = 1f;
    public Text healthText;
    public Image hasHumanImage;

    float xDirection, yDirection;
    float lastX, lastY;
    bool hasHuman = false;
    bool inUFOArea = false;
    bool canInteract = false;
    bool stillPenalty = false;
    bool searching = false;
    

    GameObject InteractableObject;
    Animator anim;
    ParticleSystem runEffect;
    ParticleSystem.ShapeModule particleShape;
    ParticleSystem.EmissionModule particleEmission;
    public Animator gleamAnimator;
    SpriteRenderer gleamSprite;

    public bool cantGetHit;

    float interactTime = 0f;
    public float requiredInteractTime = 2f;
    [SerializeField]
   private AudioSource WalkAudio;
    public AudioSource HitAudio;
    public AudioSource HumanGetAudio;
    public AudioSource HumanInUFOAudio;
    public AudioSource NoHumanAudio;

    public GameObject TrashNoisePrefab;
    public RectTransform NoiseVisualizer;

    // Use this for initialization
    void Start()
    {
        InteractButton.SetActive(false);
        anim = GetComponent<Animator>();

        healthText.text = "x" + health;
        runEffect = GetComponent<ParticleSystem>();
        particleShape = runEffect.shape;
        particleEmission = runEffect.emission;
        runEffect.Play();
        particleEmission.enabled = false;
        interactTimeSprite.enabled = false;
        gleamSprite = gleamAnimator.gameObject.GetComponent<SpriteRenderer>();
        gleamSprite.enabled = false;
        hasHumanImage.enabled = false;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (health <= 0)
        {
            //Smoothly Send Coroutine to UI manager that says game over
            WalkAudio.loop = false; 
            WalkAudio.Stop();
            RoundManagerScript.code.GameEndTransition();
        }

        RecieveInput();
        Interact();
        PlayerMove();

        if (stillPenalty)
        {
            noiseLevel = 2f;
        }
        VisualizeNoise();
    }

    void VisualizeNoise()
    {
        float noiseVisualValue = noiseLevel;
        if(noiseVisualValue > 1f)
        {
            noiseVisualValue = 1f;
        }

        if(noiseVisualValue < 0.05f)
        {
            noiseVisualValue = 0.05f;
        }


        NoiseVisualizer.localScale = new Vector3(1, noiseVisualValue, 1);
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
            particleEmission.enabled = false;
            WalkAudio.Pause();

        }
        else
        {
            lastX = xDirection;
            lastY = yDirection;
            anim.SetBool("Move", true);
            particleEmission.enabled = true;
            if (!WalkAudio.isPlaying)
            {
                WalkAudio.Play();
            }
        }
        anim.SetFloat("VelocityY", yDirection);
        anim.SetFloat("VelocityX", xDirection);


        float currentAngle = Mathf.Atan2(yDirection, xDirection);
        currentAngle *= Mathf.Rad2Deg;

        particleShape.rotation = new Vector3(currentAngle, -90, 0);

        noiseLevel = (Mathf.Abs(xDirection) + Mathf.Abs(yDirection)) / 3;
        float pitchAndVolumeNoiseLevel = (Mathf.Abs(xDirection) >= Mathf.Abs(yDirection)) ? Mathf.Abs(xDirection) : Mathf.Abs(yDirection);
        runEffect.startSpeed = (pitchAndVolumeNoiseLevel)*4;
        WalkAudio.pitch = pitchAndVolumeNoiseLevel;
        WalkAudio.volume = pitchAndVolumeNoiseLevel;

    }

    void PlayerMove()
    {
        gameObject.GetComponent<Rigidbody2D>().velocity = new Vector2(xDirection * playerSpeed, yDirection * playerSpeed);
    }

    void Interact()
    {
        if (canInteract == true)
        {
            if (Input.GetButton("Interact"))
            {
                if (InteractableObject == null)
                {
                    return;
                }
                else if (InteractableObject.tag == "Trashcan" && !hasHuman)
                {
                    if (searching)
                    {
                        interactTime += Time.deltaTime;
                        interactTimeSprite.enabled = true;
                        interactTimeSprite.fillAmount = (interactTime / requiredInteractTime);
                    }
                    else
                    {
                        interactTime = 0;
                    }
                    searching = true;

                    if (interactTime >= requiredInteractTime)
                    {
                        if (!InteractableObject.GetComponent<TrashcanScript>().HasHuman)
                        {
                           // gameObject.GetComponent<SpriteRenderer>().color = new Color(0, 255, 0);
                            interactTimeSprite.enabled = false;
                            interactTime = 0;

                            StartCoroutine(TrashcanMiss());       //uncomment if it doenst work
                        }
                        else
                        {
                            hasHuman = true;
                            canInteract = false;
                            InteractableObject.GetComponent<TrashcanScript>().HasHuman = false;
                            InteractButton.SetActive(false);

                            anim.SetBool("Has Human", true);
                            hasHumanImage.enabled = true;
                            gleamSprite.enabled = true;
                            gleamAnimator.SetTrigger("PLAY");
                            HumanGetAudio.Play();
                            interactTime = 0;
                        }
                    }
                }
                else if (hasHuman)
                {
                    if (searching)
                    {
                        interactTime += Time.deltaTime;
                        interactTimeSprite.enabled = true;
                        interactTimeSprite.fillAmount = (interactTime /requiredInteractTime);
                    }
                    else
                    {
                        interactTime = 0;
                        interactTimeSprite.enabled = false;
                    }
                    searching = true;

                    if (interactTime >= requiredInteractTime)
                    {
                        if (inUFOArea)
                        {
                            //Destroy(InteractableObject);
                            hasHuman = false;
                            anim.SetBool("Has Human", false);
                            hasHumanImage.enabled = false;
                            InteractButton.SetActive(false);              
                            RoundManagerScript.code.RemoveHuman();
                            HumanInUFOAudio.Play();
                            RoundManagerScript.code.AddScore(RoundManagerScript.ScoreType.HUMAN);
                            canInteract = false;
                            interactTime = 0;
                        }
                    }
                }
            }
            else
            {
                interactTime = 0;
                searching = false;
                interactTimeSprite.enabled = false;
            }
        }
        else
        {
            interactTime = 0;
            searching = false;
            interactTimeSprite.enabled = false;
        }
    }

    public void playerHit()
    {
        if (!cantGetHit)
        {
            if (hasHuman)
            {
                GameObject newHuman = (GameObject)Instantiate(humanPrefab, transform.position, Quaternion.identity, GameObject.Find("RoundManager").transform);
                Physics2D.IgnoreCollision(GetComponent<Collider2D>(), newHuman.GetComponent<Collider2D>());
                hasHuman = false;
                anim.SetBool("Has Human", false);
                hasHumanImage.enabled = false;
            }
            cantGetHit = true;
            StartCoroutine(IFrames());
        }
    }

    IEnumerator IFrames()
    {
        gameObject.GetComponent<SpriteRenderer>().color = new Color(gameObject.GetComponent<SpriteRenderer>().color.r, gameObject.GetComponent<SpriteRenderer>().color.g, gameObject.GetComponent<SpriteRenderer>().color.b, 0.5f);
        health--;
        healthText.text = "x" + health;
        HitAudio.Play();
        yield return new WaitForSeconds(iFrameTime);
        cantGetHit = false;
        gameObject.GetComponent<SpriteRenderer>().color = new Color(gameObject.GetComponent<SpriteRenderer>().color.r, gameObject.GetComponent<SpriteRenderer>().color.g, gameObject.GetComponent<SpriteRenderer>().color.b, 1);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {

        if (LayerMask.LayerToName(collision.gameObject.layer) == "Trashcan")
        {
            Physics2D.IgnoreCollision(collision.gameObject.GetComponent<Collider2D>(), gameObject.GetComponent<Collider2D>());
        }

        if (collision.gameObject.tag == "Collectable")
        {
            Destroy(collision.gameObject);
            RoundManagerScript.code.collectableOnScreen = false;
            RoundManagerScript.code.AddScore(RoundManagerScript.ScoreType.COLLECTABLE);
            HumanGetAudio.Play();
        }
    }

    void OnTriggerEnter2D(Collider2D collision)
    {

        if (collision.gameObject.tag == "Trashcan")
        {
            if (!hasHuman)
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
        if (collision.gameObject.tag == "UFO_Area")
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
        canInteract = false;
        gameObject.GetComponent<SpriteRenderer>().color = new Color(1, 0, 0);
        GameObject tempNoise = (GameObject)Instantiate(TrashNoisePrefab, InteractableObject.transform);
        NoHumanAudio.Play();
        yield return new WaitForSeconds(.5f);
        stillPenalty = false;
        canInteract = false;
        InteractableObject = null;
        gameObject.GetComponent<SpriteRenderer>().color = new Color(1, 1, 1);
      

    }
}
