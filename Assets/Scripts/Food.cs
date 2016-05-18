using UnityEngine;
using System.Collections;

public class Food : MonoBehaviour {

    private enum State
    {
        Idle,
        Blink,
        Hit,
        Splatting
    }
    private State currentState = State.Idle;

    private SpriteRenderer spriteRenderer;
    public Sprite idleSprite;
    public Sprite blinkSprite;

    public Sprite hitSprite;
    public Sprite scaredSprite;

    public GameObject splatAnimation; // animation object to instantiate
    public Sprite idleSplatSprite;
    public Sprite blinkSplatSprite;

    private GameObject splatAnimatorObject; // instatiated animation object


    public bool splatted = false;

    private float updateTimer = 0.0f;

    private float blinkTime = 4f; // seconds

    private float blinkLength = 0.1f; // seconds
    private float splatLength = 1f; // seconds
    private float hitLength = 0.5f; // seconds



    // Use this for initialization
    void Start ()
    {
        blinkTime = RandomBlinkTime();

        currentState = State.Idle;
        spriteRenderer = GetComponent<SpriteRenderer>();
        spriteRenderer.sprite = idleSprite;
    }
	
	// Update is called once per frame
	void Update ()
    {
        updateTimer += Time.deltaTime;

        switch (currentState)
        {
            case State.Idle:
                if (updateTimer >= blinkTime) // blink if needed
                {
                    currentState = State.Blink;
                    spriteRenderer.sprite = (splatted) ? blinkSplatSprite : blinkSprite;
                    updateTimer = 0;
                }
                break;
            case State.Blink:
                if (updateTimer >= blinkLength) // stop blinking if needed
                {
                    currentState = State.Idle;
                    spriteRenderer.sprite = (splatted) ? idleSplatSprite : idleSprite;
                    updateTimer = 0;
                }
                break;
            case State.Hit:
                if (updateTimer >= hitLength) // stop hit if needed
                {
                    currentState = State.Idle;
                    spriteRenderer.sprite = (splatted) ? idleSplatSprite : idleSprite;
                    updateTimer = 0;
                }
                break;
            case State.Splatting:
                if (updateTimer >= splatLength) // end splat animation if needed
                {
                    splatted = true;
                    currentState = State.Idle;

                    Destroy(splatAnimatorObject);
                    spriteRenderer.sprite = idleSplatSprite;
                    GetComponent<Transform>().Rotate(new Vector3(0f, 0f, 0f));
                    GetComponent<Transform>().rotation = Quaternion.identity;

                    updateTimer = 0;
                }
                break;
        }

        /*
        splatAnimator.GetInteger("");
        splatAnimator.SetInteger()
        */
    }



    private void OnCollisionEnter2D(Collision2D coll)
    {
        if (coll.gameObject.tag == "Floor" && IsAlive())
        {
            PlaySplatAnimation();
        }
        else if (coll.gameObject.tag == "Food" && (!IsAlive()))
        {
            Physics2D.IgnoreCollision(GetComponent<PolygonCollider2D>(), coll.collider);
        }
    }


    
    public void PlaySplatAnimation()
    {
        currentState = State.Splatting;
        splatAnimatorObject = (GameObject) Instantiate(splatAnimation, transform.position, Quaternion.identity);
        spriteRenderer.sprite = null;
        //GetComponent<Rigidbody2D>().Sleep();
        Destroy(GetComponent<Rigidbody2D>());
        updateTimer = 0;
    }

    public void Hit()
    {
        if (IsAlive())
        {
            currentState = State.Hit;
            spriteRenderer.sprite = hitSprite;
            updateTimer = 0;
        }
    }


    public void FadeOut()
    {
        LeanTween.alpha(this.gameObject, 0f, 1f);
    }


    public void DestroyEverything()
    {
        Destroy(splatAnimatorObject);
    }


    private float RandomBlinkTime()
    {
        return Random.Range(5f, 10f);
    }


    public bool IsAlive()
    {
        return !(currentState == State.Splatting || splatted);
    }

}
