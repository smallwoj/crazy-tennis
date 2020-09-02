/*
Alex Kitt (first time doing Unity development alone!)
July 11, 2018
NPCBehaviour: This one is like bork.cs, except instead of borking it dialogues instead

It also handles the animation for the thing that pops up to show that an NPC has dialogue (the "indicator"). It
pops up (deccelerating due to drag) when the player comes close 
and accelerates downwards before disappearing when the player walks away.
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCBehaviour : MonoBehaviour
{
    public GameObject dialoguePrefab;   // Prefab of the dialogue window
    private GameObject dialogue;

    public GameObject indicatorPrefab;    // Prefab of the indicator
    private GameObject indicator;   // Instance of the above prefab
    private bool playerInRange = false; // Tells whether the player is in speaking range to the NPC

    void OnTriggerEnter2D(Collider2D other)
    {
        // If the player is near an NPC who they can talk to, create a little speech bubble to indicate that
        if (other.CompareTag("Player"))
        {
            playerInRange = true;

            // Instantiate the indicator
            Transform transform = GetComponent<Transform>();
            CapsuleCollider2D trigger = GetComponent<CapsuleCollider2D>();
            indicator = (GameObject)Instantiate(indicatorPrefab, 
                transform.position + new Vector3(trigger.offset.x, trigger.offset.y + trigger.size.y, 0),
                Quaternion.identity
                );
            
            // Set the indicator's initial velocity to make it pop up
            Rigidbody2D indicatorBody = indicator.GetComponent<Rigidbody2D>();
            indicatorBody.velocity = new Vector2(0, 5);
        }
    }

    void OnTriggerStay2D(Collider2D other)
    {
        if (other.CompareTag("Player") && Input.GetButtonDown("Submit") && dialogue == null)
        {
            // talking time >:)
            dialogue = (GameObject)Instantiate(dialoguePrefab,
                Camera.main.ViewportToWorldPoint(new Vector3(0,1,0)), Quaternion.identity);
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        // If the player leaves the NPC's trigger area, move the speech bubble downwards and destory it
        if (other.CompareTag("Player"))
        {
            playerInRange = false;
            if (indicator != null)    
            {
                // The magic numbers below were found through very scientific trial and error. Don't touch them!!!
                GameObject.Destroy(indicator, 0.19f);
            }

            // Also, if the NPC is currently speaking, destroy their dialogue box too 
            // (it's easier than disabling movement during dialogue, plus it's funny)
            if (dialogue != null)
            {
                GameObject.Destroy(dialogue);
            }
        }
    }

    void FixedUpdate() 
    {
        // Animate the indicator disappearing when the player walks away
        if (!playerInRange && indicator != null)
        {
            indicator.GetComponent<Rigidbody2D>().velocity += new Vector2(0, -1);
        }
    }

}
