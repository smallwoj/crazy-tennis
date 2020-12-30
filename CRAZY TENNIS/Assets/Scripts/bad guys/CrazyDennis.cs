// It's the final boss baybee!

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CrazyDennis : BadThing
{
    /// <summary> How many balls can be on-screen at once </summary>
    private static readonly int MAX_BALLS = 128;
    /// <summary> How many times you gotta rally with him in phase 4 </summary>
    private static readonly int INITIAL_RALLY_COUNT = 5;
    /// <summary> Initial value for ballSpeed </summary>
    private static readonly float INITIAL_BALL_SPEED = 1;
    /// <summary> How fast the ball speed increases every hit in phase 4 </summary>
    private static readonly float BALL_SPEED_INCREASE = 0.5f;
    /// <summary> The displacement Between the enemy's position and a new ball's position </summary>
    private static readonly Vector3 BALL_OFFSET = Vector3.zero;
    /// <summary> How many unhittable balls to spawn in phase 4 </summary>
    private static readonly int PHASE_4_BALLS = MAX_BALLS / 2;

    /// <summary> Current phase of the battle </summary>
    private int phase;
    /// <summary> Makes the sprites go whee (we're planning to add whoosh functionality in the next version) </summary>
    private Animator anim;
    /// <summary> Lets us communicate with the player </summary>
    private PlayerBehaviour pb;
    /// <summary> Object pool for spawning and despawning a whole bunch of balls </summary>
    private Ball[] ballPool = new Ball[MAX_BALLS];
    /// <summary> The ball he rallies during phase 4 </summary>
    private Ball rallyBall = null;
    /// <summary> How fast the ball goes in phase 4 </summary>
    private float ballSpeed = INITIAL_BALL_SPEED;
    /// <summary> Current horizontal ball spawn displacement during the first phase </summary>
    private float amplitude = 0;
    /// <summary> Starting angle for a hit in the first phase </summary>
    private float startingAngle = 0;
    /// <summary> Whether he's currently firing balls during the second phase </summary>
    private bool phase2Active = false;
    /// <summary> The dialogue box that conveniently stalls in phase 3 while the phase 2 movement finishes </summary>
    private GameObject dialogue;
    /// <summary> Whether the aforementioned dialogue box is active </summary>
    private bool talking = false;
    /// <summary> How many times he can rally before taking a hit </summary>
    private int rallyCount = INITIAL_RALLY_COUNT;
    /// <summary> Whether it's time to spawn a bunch of balls in phase 4 </summary>
    private bool phase4Ready = false;

    // Start is called before the first frame update
    new void Start()
    {
        // Good ol' badthing
        base.Start();
        anim = GetComponent<Animator>();
        phase = 0;
        pb = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerBehaviour>();
        pb.Breakout = false;
        
        // Dummy value for maxhits, so the health circle can display right away
        maxhits = 1;
        
        // Get the dialogue
        dialogue = transform.GetChild(0).gameObject;
    }

    // Update is called once per frame
    void Update() 
    {
        switch (phase)
        {
            // Swing the racket as soon as the previous swing animation ends
            case 1:
            {
                anim.SetTrigger("Swing");
                break;
            }
            // Check for the dialogue ending
            case 3:
            {
                // That last condition is to make sure that he finishes up the 
                // phase 2 movement before transitioning to phase 4
                if (!dialogue.activeSelf && talking && anim.GetCurrentAnimatorStateInfo(0).IsName("None"))
                {   
                    talking = false;
                    NextPhase();
                }
                break;
            }
            // Check for the rallying ball going out of the court 
            case 4:
            {
                if(rallyBall != null && rallyBall.OutsideCourt)
                {
                    Destroy(rallyBall.gameObject);
                    rallyBall = null;
                    Serve();
                }
                break;
            }
            case 5:
            {
                break;
            }
        }
    }

    // FixedUpdate is probably called once per fixed frame
    void FixedUpdate()
    {
        switch (phase)
        {
            // Constantly spawn balls in a sine wave pattern
            case 2:
            {
                // (but only if the phase is active! Doing this continuously would be pretty much unbeatable)
                if (phase2Active)
                {
                    // Local variables? Wacky
                    // Index in the ball pool to spawn a ball at
                    int index;

                    // Adjust the amplitude
                    amplitude = Mathf.Sin(Mathf.PI * Time.fixedTime);

                    // Spawn two balls, one hittable and one not, on opposite sides
                    // Hittable
                    index = findBallIndex();
                    if (index >= 0)
                    {
                        ballPool[index] = SpawnBall(typeof(GenericHittable), transform.position + BALL_OFFSET + new Vector3(amplitude, 0, 0), new Vector2(0, -7), Random.Range(6f, 10f));
                    }
                    // Non-hittable
                    index = findBallIndex();
                    if (index >= 0)
                    {
                        // Very similar to the above spawn, except it's unhittable and the amplitude is subtracted from the position
                        ballPool[index] = SpawnBall(typeof(GenericUnhittable), transform.position + BALL_OFFSET + new Vector3(-amplitude, 0, 0), new Vector2(0, -7), Random.Range(6f, 10f));
                    }
                    FindObjectOfType<CameraBehaviour>().Impact(0.05f, Random.insideUnitCircle.normalized);
                }
                break;
            }
            // After a short delay, use red character's behaviour
            case 4:
            {
                if (phase4Ready)
                {
                    for (int i = 0; i < PHASE_4_BALLS; i++)
                    {
                        if (ballPool[i] != null && ballPool[i].OutsideCourt)
                        {
                            Destroy(ballPool[i].gameObject);
                            ballPool[i] = null;
                        }

                        if (ballPool[i] == null)
                        {
                            Vector2 velocity = new Vector2(Random.Range(-1f, 1f), Random.Range(-1f, 1f)).normalized * 3;
                            ballPool[i] = SpawnBall(
                                typeof(GenericUnhittable), 
                                transform.position + BALL_OFFSET, 
                                velocity,
                                Random.Range(6f, 10f)
                            );
                            FindObjectOfType<CameraBehaviour>().Impact(0.05f, velocity.normalized);
                        }
                    }
                }
                break;
            }
        }
    }

    /// <summary>
    /// Goes to the next phase
    /// </summary>
    public override void NextPhase()
    {
        phase++;
        anim.SetInteger("Phase", phase);
        DestroyAllBalls();

        switch (phase)
        {
            case 1:
            {
                maxhits = 20;
                break;
            }
            case 2:
            {
                maxhits = 50;
                togglePhase2Active();
                break;
            }
            case 3:
            {
                if (phase2Active)
                    togglePhase2Active();

                dialogue.SetActive(true);
                talking = true;
                maxhits = 999;
                break;
            }
            case 4:
            {
                maxhits = 2;
                Serve();
                pb.PlayerHurt += Serve;
                break;
            }
            case 5:
            {
                DestroyAllBalls();
                FindObjectOfType<CameraBehaviour>().ShakeScreen(1f);
                anim.SetTrigger("Dead");
                break;
            }
        }
    }

    /// <summary>
    /// Reacts to getting hit by a ball
    /// </summary>
    /// <param name="hitBall"> The ball that hit the enemy </param>
    public override void Ouch(Ball hitBall)
    {
        // No rallying until phase 4
        if (phase < 4 || rallyCount <= 0)
        {
            anim.SetTrigger("Hurt");
            if (phase == 4)
            {
                Serve();
            }
            base.Ouch(hitBall);
        }
        else
        {
            // Rallying time!
            anim.SetTrigger("Swing");
            rallyCount--;
            hitBall.hit = false;
            Physics2D.IgnoreCollision(hitBall.GetComponent<Collider2D>(), this.GetComponent<Collider2D>(), true);
        }
    }

    /// <summary>
    /// Hits a ball!!!
    /// </summary>
    public void HitBall()
    {
        switch (phase)
        {
            // circle 😳
            case 1:
            {
                // To be more specific, he fires a whole bunch of balls in a segmented circular formation, with the 
                // balls varing in type between segments and varying in speed within segments
                int segments = 4;

                for (int segment = 0; segment < segments; segment++)
                {
                    // Randomly decide type
                    System.Type ballType;
                    if (Random.value > 0.5)
                        ballType = typeof(GenericHittable);
                    else
                        ballType = typeof(GenericUnhittable);

                    for (float angleInSegment = 0; angleInSegment < 2*Mathf.PI/segments; angleInSegment += 2*Mathf.PI/(segments*7))
                    {
                        int index = findBallIndex();
                        if (index >= 0)
                        {
                            // Make a vector for the ball's angle 
                            float θ = startingAngle + angleInSegment + segment * 2*Mathf.PI/segments; 
                            Vector2 angleVector = new Vector2(Mathf.Cos(θ), Mathf.Sin(θ));

                            // Fire the ball!
                            ballPool[index] = SpawnBall(
                                ballType, 
                                transform.position + (Vector3)angleVector * (2*Mathf.PI/segments - angleInSegment), 
                                angleVector * (1 + 5*(angleInSegment/2*Mathf.PI/segments)), 
                                Random.Range(6f, 10f));
                        }
                    }
                }

                // Adjust the starting angle for the next hit
                startingAngle += Mathf.PI/12;
                if (startingAngle >= 2*Mathf.PI) startingAngle = 0;

                FindObjectOfType<CameraBehaviour>().Impact(0.2f, Vector2.left);
                break;
            }
            // Just a good ol plain simple serve
            case 4:
            {
                // If there isn't already a ball, spawn a new one
                if (rallyBall == null)
                {
                    ballSpeed = INITIAL_BALL_SPEED;
                    rallyBall = base.SpawnBall(
                        typeof(GenericHittable),
                        transform.position + BALL_OFFSET,
                        new Vector2(Random.Range(-3f, 3f), Random.Range(-3f, -4f)).normalized * ballSpeed,
                        Random.Range(6f, 10f));
                }
                // Otherwise, send the existing one back
                else
                {
                    ballSpeed += BALL_SPEED_INCREASE;
                    rallyBall.Velocity = new Vector2(Random.Range(-2.5f, 2.5f), Random.Range(-3f, -4f)).normalized * ballSpeed;
                }
                // Make this one hit a little harder to further emphasize the Final Phase
                FindObjectOfType<CameraBehaviour>().Impact(0.3f, Vector2.left);
                break;
            }
        }
    }

    /// <summary>
    /// Finds an index in the ball pool for a new ball and does garbage collection if necessary
    /// </summary>
    /// <returns> A free index in the ball pool if one exists, or -1 otherwise </returns>
    private int findBallIndex()
    {
        for (int i = 0; i < MAX_BALLS; i++)
        {
            // Garbage-collect if necessary
            if (ballPool[i] != null && ballPool[i].OutsideCourt)
            {
                GameObject.Destroy(ballPool[i].gameObject);
                ballPool[i] = null;
            }

            // We found one!
            if (ballPool[i] == null)
            {
                return i;
            }
        }

        // If we got to the end of the loop, then there are no free spaces :(
        return -1;
    }

    /// <summary>
    /// Toggles phase2Active.
    /// Remember the autocommenter? That woulda been perfect for this :(
    /// </summary>
    private void togglePhase2Active()
    {
        phase2Active = !phase2Active;
        anim.SetBool("Phase 2 active", phase2Active);
    }

    /// <summary>
    /// Toggles phase4Ready
    /// </summary>
    private void togglePhase4Ready()
    {
        phase4Ready = !phase4Ready;
        FindObjectOfType<CameraBehaviour>().Impact(0.3f, Random.insideUnitCircle.normalized);
    }

    /// <summary>
    /// Serves a ball in phase 4
    /// </summary>
    public void Serve()
    {
        phase4Ready = false;

        // I never destroyed the animator I swear
        if (!anim) anim = GetComponent<Animator>();
        
        anim.SetTrigger("Phase 4 delay");
        anim.SetTrigger("Swing");
        rallyCount = INITIAL_RALLY_COUNT;
    }

    /// <summary>
    /// Called at the end of the death explosion animation
    /// (obligatory "gamers don't die, they-")
    /// </summary>
    void Die() 
    {
        // Instead of dying the usual way (i.e. using SpawnNextEnemy), transition to the outro scene
        Destroy(this.gameObject);
        SceneLoader.instance.LoadLevel("Outro", null);
    }

    /// <summary>
    /// Called when he dies :(
    /// </summary>
    new void OnDestroy() {
        base.OnDestroy();
        pb.PlayerHurt -= Serve;
    }

    /// <summary>
    /// String representing the enemy's prefab
    /// </summary>
    /// <returns>See: summary</returns>
    public override string PrefabString()
    {
        return "Crazy Dennis";
    }

    /// <summary>
    /// Shakes the screen for the length of the aura reveal animation
    /// </summary>
    public void AuraShake()
    {
        FindObjectOfType<CameraBehaviour>().ShakeScreen(0.2f, 105);
    }
}
