﻿// Script for instantiating and animating a single spectator in the crowd

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpectatorBehaviour : MonoBehaviour, Spectator
{
    private static readonly string[] OUTFITS = { "Generic crowd person", "Cocky bastard fan", "Surfer dude fan" };
    private static readonly string[] SKIN_COLOURS = { "Pale", "Fair", "Tan", "Dark" };
    /// <summary> How many times the spectator may jump during a cheer </summary>
    private static readonly float MAX_JUMPS = 16; // (note: if the variable hype stuff doesn't work out and we decide to just have constant hype, set this to 7. Makes the crowd look less... crazy)
    /// <summary> How many (world space?) distance units high the spectator can jump </summary>
    private static readonly float MAX_HEIGHT = 0.5f;
    /// <summary> The least amount of times the spectator can jump per second </summary>
    private static readonly float MIN_SPEED = 3;
    /// <summary> How many times the spectator can jump per second </summary>
    private static readonly float MAX_SPEED = 6;
    /// <summary> Whether the spectator is currently cheering </summary>
    private bool hyped = false;
    /// <summary> How many seconds are left in the cheer </summary>
    private float timeLeft = 0;
    /// <summary> How many (world space?) distance units high the spectator jumps </summary>
    private float jumpHeight = 0;
    /// <summary> How many times the spectator jumps per second </summary>
    private float speed = 1;
    /// <summary> The spectator's position before they did the whole cheering thing </summary>
    private Vector3 originalPos = Vector3.zero;

    // Start is called before the first frame update
    void Start()
    {
        // Local variables
        SpriteRenderer outfit = transform.GetChild(0).gameObject.GetComponent<SpriteRenderer>();    // Sprite renderer for the outfit sprite
        SpriteRenderer skinColour = transform.GetChild(1).gameObject.GetComponent<SpriteRenderer>();    // Sprite renderer for the skin colour sprite

        // Randomly choose an outfit and skin colour
        outfit.sprite = UnityEditor.AssetDatabase.LoadAssetAtPath<Sprite>("Assets/images/Crowd/Outfits/" + OUTFITS[(int)(Random.value * OUTFITS.Length)] + ".png");
        skinColour.sprite = UnityEditor.AssetDatabase.LoadAssetAtPath<Sprite>("Assets/images/Crowd/Skin colours/" + SKIN_COLOURS[(int)(Random.value * SKIN_COLOURS.Length)] + ".png");
    }

    // Update is called once per frame
    void Update()
    {
        // Play the hype animation (jumping up and down) if applicable
        if (hyped)
        {
            transform.position = new Vector3(
                originalPos.x, 
                originalPos.y + jumpHeight * Mathf.Abs(Mathf.Sin(Mathf.PI * speed * timeLeft)), 
                originalPos.z);

            timeLeft -= Time.deltaTime;

            if (timeLeft <= 0)
            {
                hyped = false;
                transform.position = originalPos;
            }
        }
    }

    /// <summary>
    /// Makes this spectator start to jump up and down wildly (or mildly, or even not at all, depending on the hype value)
    /// </summary>
    /// <param name="hype"> A number from 0 to 1 measuring how excited the crowd gets. 
    /// More specifically, it's the probability that any given spectator will cheer, 
    /// and it provides a coefficient for the intensity and length of a spectator's cheer</param>
    public void Cheer(float hype)
    {
        // Randomly decide whether or not to cheer, based on the hype. A hype of 1 guarantees a cheer
        if (!hyped && Random.value <= hype)
        {
            hyped = true;

            // Randomly decide the jump's properties
            jumpHeight = Random.value * MAX_HEIGHT * hype;
            speed = Random.value * (MAX_SPEED - MIN_SPEED) + MIN_SPEED * hype;
            // The calcuation for the duration of the cheer ensures that the spectator will start their cheer on the ground 
            timeLeft = (int)(Random.value * MAX_JUMPS * hype) / speed;

            // Record the position before cheering
            originalPos = transform.position;
        }
    }

    /// <summary>
    /// Sets the spectator's order in their drawing layer
    /// </summary>
    /// <param name="sortOrder"> The intended value for their order in layer </param>
    public void setSortingOrder(int sortOrder)
    {
        foreach (Transform child in transform)
        {
            child.GetComponent<SpriteRenderer>().sortingOrder = sortOrder;
        }
    }
}
