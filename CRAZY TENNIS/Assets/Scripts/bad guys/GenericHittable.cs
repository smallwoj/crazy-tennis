using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GenericHittable : GenericUnhittable
{
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
                    Velocity = (transform.position - other.transform.position).normalized*6;
                }
                else
                {
                    //this one just go 
                    Velocity = (Parent.transform.position - transform.position).normalized*6;
                }
                Physics2D.IgnoreCollision(this.GetComponent<Collider2D>(), Parent.GetComponent<Collider2D>(), false);
            }
        }
    }
}
