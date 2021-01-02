using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GenericHittable : GenericUnhittable
{
    public delegate void HitEvent();
    public event HitEvent OnHit;
    void OnTriggerEnter2D(Collider2D other)
    {
        if(other.tag == "player child")
        {
            if(!hit)
            {
                hit = true;
                // print("i just got hit by the player wow");
                if(GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerBehaviour>().Breakout)
                {
                    //this one is kinda like breakout
                    Velocity = (transform.position - other.transform.position).normalized*8;
                    if(Velocity.y < 0)
                        Velocity = new Vector2(Velocity.x, 0f).normalized*8;
                }
                else
                {
                    //this one just go 
                    Velocity = (Parent.transform.position - transform.position).normalized*8;
                }
                Physics2D.IgnoreCollision(this.GetComponent<Collider2D>(), Parent.GetComponent<Collider2D>(), false);
                // You hit the ball!!!!!! You get a point
                GameObject.FindGameObjectWithTag("Score").GetComponent<ScoringSystem>().BallHit();
                // Fire the on hit event
                if(OnHit != null)
                    OnHit();
                // And finally, the sound effect
                GetComponent<AudioSource>().Play();
            }
        }
    }
}
