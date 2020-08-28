// Script for instantiating and animating a single spectator in the crowd

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpectatorBehaviour : MonoBehaviour, Spectator
{
    /// <summary> File names for spectators' skin colours </summary>
    private static readonly string[] SKIN_COLOURS = { "Pale", "Fair", "Tan", "Dark" };
    /// <summary> How many times the spectator may jump during a cheer </summary>
    private static readonly float MAX_JUMPS = 16;
    /// <summary> How many (world space?) distance units high the spectator can jump </summary>
    private static readonly float MAX_HEIGHT = 0.5f;
    /// <summary> The least amount of times the spectator can jump per second </summary>
    private static readonly float MIN_SPEED = 3;
    /// <summary> How many times the spectator can jump per second </summary>
    private static readonly float MAX_SPEED = 6;
    /// <summary> How many of the sprites in the list up above are fans of a 
    /// particular enemy (this value is used to make sure that the cameo sprites spawn at most once) </summary>
    private static readonly int FANS = 6;
    /// <summary> File names for spectators' sprites </summary>
    private static List<string> sprites = new List<string> { "Generic crowd person", "Cocky bastard fan", "Surfer dude fan", "Mechanical ball thrower fan", "Spider fan", "Huma N. fan", "Skyla", "Yar", "Alex", "Josh" };
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
        SpriteRenderer mainSprite = transform.GetChild(0).gameObject.GetComponent<SpriteRenderer>();    // Sprite renderer for the main sprite
        SpriteRenderer skinColour = transform.GetChild(1).gameObject.GetComponent<SpriteRenderer>();    // Sprite renderer for the skin colour sprite
        int spriteIndex = (int)(Random.value * sprites.Count);  // Index of the chosen sprite
        string spriteName = sprites[spriteIndex];   // Name of the chosen sprite (necessary since cameos are removed from the list once they're chosen)
        string path = "Assets/images/Crowd/";   // Where to find the sprites

        // It's a fan, so we randomly choose the skin colour too
        if (spriteIndex < FANS)
        {
            path += "Outfits/";
            skinColour.sprite = UnityEditor.AssetDatabase.LoadAssetAtPath<Sprite>("Assets/images/Crowd/Skin colours/" + SKIN_COLOURS[(int)(Random.value * SKIN_COLOURS.Length)] + ".png");
        }
        // It's a cameo, so we also make sure that it doesn't appear a second time
        else
        {
            path += "Cameos/";
            sprites.RemoveAt(spriteIndex);
        }

        // Choose the sprite!
        mainSprite.sprite = UnityEditor.AssetDatabase.LoadAssetAtPath<Sprite>(path + spriteName + ".png");
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
