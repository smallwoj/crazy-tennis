﻿// Script that exists because each side of the crowd is a separate object and I want a convenient way to call methods for both sides

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CrowdBehaviour : MonoBehaviour
{
    // A reference to each side
    private CrowdSideBehaviour left, right;

    // Tells whether a point is on screen, used to place spectators (I think)
    public Collider2D screenTrigger;

    // Plays the cool sound effect(s)
    private AudioSource audioSource;
    
    // The cool sound effects
    private AudioClip cheer, bigCheer;

    // Start is called before the first frame update
    void Start()
    {
        left = transform.GetChild(0).gameObject.GetComponent<CrowdSideBehaviour>();
        right = transform.GetChild(1).gameObject.GetComponent<CrowdSideBehaviour>();
        audioSource = GetComponent<AudioSource>();
        cheer = Resources.Load<AudioClip>("Audio/Sound Effects/Cheering no toot");
        bigCheer = Resources.Load<AudioClip>("Audio/Sound Effects/Cheering");
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    /// <summary>
    /// This method tells both crowds to cheer, which in turn tell all of their contained spectators to cheer.
    /// Isn't delegation great?
    /// </summary>
    /// <param name="hype"> A number from 0 to 1 measuring how excited the crowd gets (more detail in SpectatorBehaviour.Cheer) </param>
    public void Cheer(float hype)
    {
        left.Cheer(hype);
        right.Cheer(hype);

        // Also, cheer based on the hype
        audioSource.volume = hype;
        if (hype >= 1)
            audioSource.clip = bigCheer;
        else
            audioSource.clip = cheer;
        audioSource.Play();
    }

    private bool isOnScreen(Vector2 point)
    {
        Vector2 closest = screenTrigger.ClosestPoint(point);
        // Fun fact from answers.unity.com: If the given point is inside the collider, ClosestPoint will return that point
        return closest == point;
    }
}
