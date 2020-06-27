using System.Collections;
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
    /// The real center of the player
    /// </summary>
    public Vector2 CenterPos
    {
        get
        {
            return rb.position + new Vector2(0, -0.75f);
        }
    }

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

    /// <summary>
    /// Reference to the rigidbody component
    /// </summary>
    public Rigidbody2D rb;
    // Start is called before the first frame update
    void Start()
    {
        anim = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
        rb.position = defaultPosition;
        defaultBombs = bombs;
        swang = GameObject.Find("Player/swang").GetComponent<PolygonCollider2D>();
        swang.enabled = false;
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetButtonDown("Submit") && !PauseControl.Paused)
        {
            anim.SetTrigger("space pressed");
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        // Award points for almost hitting the ball
        if (other.gameObject.CompareTag("Ball"))
        {
            GameObject.FindGameObjectWithTag("Score").GetComponent<ScoringSystem>().BallNearHit();
        }
    }

    /// <summary>
    /// Die
    /// </summary>
    public void Die()
    {
        lives--;
        if(lives > 0)
        {
            bombs = defaultBombs;
            GetComponent<Rigidbody2D>().position = defaultPosition;
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
