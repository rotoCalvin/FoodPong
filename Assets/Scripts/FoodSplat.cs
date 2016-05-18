using UnityEngine;
using System.Collections;

public class FoodSplat : MonoBehaviour {

    private SpriteRenderer spriteRenderer;
    public Sprite idleSprite;
    public Sprite blinkSprite;

    private bool isBlinking = false;

    private float updateTimer = 0.0f;
    private float blinkTime = 4f; // seconds
    private float blinkLength = 0.1f; // seconds

    // Use this for initialization
    void Start ()
    {
        blinkTime = RandomBlinkTime();

        spriteRenderer = GetComponent<SpriteRenderer>();
        spriteRenderer.sprite = idleSprite;
    }
	
	// Update is called once per frame
	void Update ()
    {

        updateTimer += Time.deltaTime;

        if (isBlinking)
        {
            if (updateTimer >= blinkLength)
            {

                updateTimer = 0;
                isBlinking = false;
                blinkTime = RandomBlinkTime();
                spriteRenderer.sprite = idleSprite;

            }
        }
        else
        {
            if (updateTimer >= blinkTime)
            {
                updateTimer = 0;
                isBlinking = true;
                spriteRenderer.sprite = blinkSprite;
            }
        }
    }


    private void OnCollisionEnter2D(Collision2D coll)
    {
        if (coll.gameObject.tag == "Food")
        {
            Physics2D.IgnoreCollision(GetComponent<Collider2D>(), coll.collider);
        }
    }


    private float RandomBlinkTime()
    {
        return Random.Range(5f, 10f);
    }
}
