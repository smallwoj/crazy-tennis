using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Main class for the player.
/// </summary>
public class PlayerBehaviour : MonoBehaviour
{
    /// <summary>
    /// How long the player hurt animation takes
    /// </summary>
    private static readonly float HURT_TIME = 0.5f;

    /// <summary>
    /// How long the respawn invincibility lasts 
    /// (note that the invincibility starts at the same time as the hurt 
    /// animation and time accordingly)
    /// </summary>
    private static readonly float RESPAWN_TIME = 2f;

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
    /// Fires when the player experiences the threat known as "game over"
    /// </summary>
    public event ouchohgod PlayerGameOver;

    /// <summary>
    /// Tells if the player is in 'recovery mode'
    /// </summary>
    public bool inRecovery;

    /// <summary>
    /// Reference to the rigidbody component
    /// </summary>
    public Rigidbody2D rb;

    /// <summary>
    /// 🔊
    /// </summary>
    private AudioSource audioSource;

    /// <summary>
    /// Sound effects for swinging the racket and dying, respectively
    /// </summary>
    private AudioClip whoosh, dead;

    public string DeathCoroutine = "WaitAndRespawn";

    // Start is called before the first frame update
    void Start()
    {
        anim = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
        rb.position = defaultPosition;
        defaultBombs = bombs;
        swang = GameObject.Find("Player/swang").GetComponent<PolygonCollider2D>();
        swang.enabled = false;
        audioSource = GetComponent<AudioSource>();
        whoosh = Resources.Load<AudioClip>("Audio/Sound Effects/Player/Whoosh");
        dead = Resources.Load<AudioClip>("Audio/Sound Effects/Player/Losing drums");
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetButtonDown("Submit") && !PauseControl.Paused && !anim.GetBool("Dead x_x"))
        {
            swang.enabled = false;
            anim.SetTrigger("space pressed");
            audioSource.clip = whoosh;
            audioSource.Play();
        }
        anim.SetFloat("xVel", rb.velocity.x);
        anim.SetFloat("yVel", rb.velocity.y);
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
    /// Gamers don't die, they-
    /// </summary>
    public void Die()
    {
        audioSource.clip = dead;
        audioSource.Play();
        anim.SetBool("Dead x_x", true);
        FindObjectOfType<CameraBehaviour>().ShakeScreen(0.3f);
        StartCoroutine(DeathCoroutine);
    }

    /// <summary>
    /// Gamers don't die, they-
    /// </summary>
    /// <returns>All the time you could've spent playing the game if you didn't get hit</returns>
    private IEnumerator WaitAndRespawn()
    {
        if(lives == 1)
            FindObjectOfType<MusicManager>().Stop();
        // Activate respawn invincibility (we do this before respawning because 
        // dying again during the death animation would be, like, the worst)
        Physics2D.IgnoreLayerCollision(10, 8, true);

        // Restrict movement until the death animation ends
        GetComponent<Move2D>().enabled = false;
        rb.velocity = Vector2.zero;

        // Let the death animation loop for a while 
        yield return new WaitForSeconds(HURT_TIME);

        // The "respawn" part of "wait and respawn"
        anim.SetBool("Dead x_x", false);
        GetComponent<Move2D>().enabled = true;
        StartCoroutine("RespawnInvincibility");

        // Stuff that was originally in the Die method
        lives--;
        BadThing.DestroyAllBalls();
        if(PlayerHurt != null)
            PlayerHurt();

        if(lives > 0)
        {
            bombs = defaultBombs;
            GetComponent<Rigidbody2D>().position = defaultPosition;
        }
        else
        {
            bool transitionActive = FindObjectOfType<EnemyTransitionControl>().transition.activeSelf;
            bool dennisDie = false;
            if (FindObjectOfType<CrazyDennis>())
            {
                dennisDie = FindObjectOfType<CrazyDennis>().anim.GetBool("Dead");
            }
            if (!transitionActive && !dennisDie)
            {            
                bombs = defaultBombs;
                GetComponent<Rigidbody2D>().position = defaultPosition;
                if(!inRecovery)
                {
                    inRecovery = true;
                    lives = 5;
                }
                else
                {
                    // TODO: Actual game over screen thabk
                }
                if(PlayerGameOver != null)
                    PlayerGameOver();
            }
            else 
            {
                lives = 1;
            }
        }
    }

    /// <summary>
    /// Gamers don't die, they-
    /// </summary>
    /// <returns>All the time you could've spent playing the game if you didn't get SHOT BY A GUN</returns>
    private IEnumerable CutsceneDennisDie()
    {
        yield return new WaitForSeconds(HURT_TIME);
        anim.SetBool("Dead x_x", false);
    }

    /// <summary>
    /// Makes the player invincible after respawning, and stops the 
    /// invincibility after some time
    /// </summary>
    /// <returns>A brand-new WaitForSeconds object (don't worry about it)</returns>
    private IEnumerator RespawnInvincibility()
    {   
        // Start the animation
        anim.SetBool("Respawn invincibility active", true);
        yield return new WaitForSeconds(RESPAWN_TIME);
        // Descend the player back into the mortal realm
        anim.SetBool("Respawn invincibility active", false);
        Physics2D.IgnoreLayerCollision(10, 8, false);
    }

    /// <summary>
    /// Enables the swing component hitbox yes
    /// </summary>
    public void EnableSwang()
    {
        swang.enabled = true;
    }

    /// <summary>
    /// Disables the swing component hitbox no
    /// </summary>
    public void DisableSwang()
    {
        swang.enabled = false;
    }

    /// <summary>
    /// Toggles the swing component hitbox maybe
    /// </summary>
    public void ToggleSwang()
    {
        swang.enabled = !swang.enabled;
    }
}
