﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Main class for the player.
/// </summary>
public class PlayerBehaviour : MonoBehaviour
{
    /// <summary>
    /// How many lives the player has, hit 0 and uh oh
    /// </summary>
    public int lives = 5;

    /// <summary>
    /// Lamp oil? Rope? Bombs? You want it?
    /// </summary>
    public int bombs;

    /// <summary>
    /// This is the good number, if it goes up youre kinda epic
    /// </summary>
    public int score = 0;

    /// <summary>
    /// Where the player starts in the scene/fight
    /// </summary>
    public Vector2 defaultPosition = new Vector2(-1.87f, -2.55f);

    /// <summary>
    /// However many bombs you start with
    /// </summary>
    public int defaultBombs = 5;

    /// <summary>
    /// Shortcut to animator object
    /// </summary>
    private Animator anim;

    /// <summary>
    /// Shortcut to collision for the swing
    /// </summary>
    private Collider2D swang;

    /// <summary>
    /// Breakout mode or not
    /// </summary>
    public bool Breakout = false;

    /// <summary>
    /// Signature of a method with no return type or parameters
    /// </summary>
    public delegate void ouchohgod();

    /// <summary>
    /// Fires when the player takes a hit
    /// </summary>
    public event ouchohgod PlayerHurt;
    // Start is called before the first frame update
    void Start()
    {
        anim = GetComponent<Animator>();
        GetComponent<Rigidbody2D>().position = defaultPosition;
        defaultBombs = bombs;
        swang = GameObject.Find("Player/swang").GetComponent<PolygonCollider2D>();
        swang.enabled = false;
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetButtonDown("Submit"))
        {
            anim.ResetTrigger("space released");
            anim.SetTrigger("space pressed");
        }
        else if(Input.GetButtonUp("Submit"))
        {
            anim.ResetTrigger("space pressed");
            anim.SetTrigger("space released");
        }
    }

    /// <summary>
    /// Die
    /// </summary>
    public void Die()
    {
        lives--;
        if(lives <= 0)
        {
            print("uh oh i am dead X_X");
        }
        else
        {
            bombs = defaultBombs;
            GetComponent<Rigidbody2D>().position = defaultPosition;
            print("lives: "+lives);
        }
        BadThing.DestroyAllBalls();
        if(PlayerHurt != null)
            PlayerHurt();
    }

    /// <summary>
    /// Enables the swing component hitbox yes
    /// </summary>
    public void ToggleSwang()
    {
        swang.enabled = !swang.enabled;
    }
}
