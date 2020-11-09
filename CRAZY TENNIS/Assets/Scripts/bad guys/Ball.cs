using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ball : MonoBehaviour
{
    public bool hit = false;
    public float RotationalVelocity;
    public BadThing Parent;
    public Vector2 Velocity;
    public Rigidbody2D body;
    private static bool start = false;
    public bool OutsideCourt = false;
    // Start is called before the first frame update
    void Start()
    {
        if(!start)
        {
            Physics2D.IgnoreLayerCollision(8,8,true);
            Physics2D.IgnoreLayerCollision(8,11,true);
        }
        body = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    /// <summary>
    /// Like Update but it stops being called when the game is paused 😤
    /// </summary>
    private void FixedUpdate()
    {
        transform.eulerAngles = new Vector3(0, 0, transform.eulerAngles.z + RotationalVelocity);
        body.velocity = (Vector3)Velocity;
    }

    virtual protected void OnCollisionEnter2D(Collision2D other)
    {
        if(other.gameObject.CompareTag("Player"))
        {
            if(!hit)
            {
                other.gameObject.GetComponent<PlayerBehaviour>().Die();
                Destroy(gameObject);
            }
        }
        else if(other.gameObject.CompareTag("Enemy"))
        {
            if(hit)
            {
                other.gameObject.GetComponent<BadThing>().Ouch(this);
            }
        }

    }
}
