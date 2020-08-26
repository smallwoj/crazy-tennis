// It's the final boss baybee!

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CrazyDennis : BadThing
{
    /// <summary> How many balls can be on-screen at once </summary>
    private static readonly int MAX_BALLS = 128;
    /// <summary> The displacement Between the enemy's position and a new ball's position </summary>
    private static readonly Vector3 BALL_OFFSET = Vector3.zero;

    /// <summary> Current phase of the battle </summary>
    private int phase;
    /// <summary> Makes the sprites go whee (we're planning to add whoosh functionality in the next version) </summary>
    private Animator anim;
    /// <summary> Lets us communicate with the player </summary>
    private PlayerBehaviour pb;
    /// <summary> Object pool for spawning and despawning a whole bunch of balls </summary>
    private Ball[] ballPool = new Ball[MAX_BALLS];
    /// <summary> Current horizontal ball spawn displacement during the first phase </summary>
    private static float amplitude = 0;
    /// <summary> Starting angle for a hit in the first phase </summary>
    private static float startingAngle = 0;
    /// <summary> Whether he's currently firing balls during the second phase </summary>
    private static bool phase2Active = false;

    // Start is called before the first frame update
    new void Start()
    {
        // Good ol' badthing
        base.Start();
        anim = GetComponent<Animator>();
        phase = 0;
        pb = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerBehaviour>();
        pb.Breakout = false;
        anim = GetComponent<Animator>();
        
        // Dummy value for maxhits, so the health circle can display right away
        maxhits = 1;
    }

    // Update is called once per frame
    void Update() 
    {
        switch (phase)
        {
            case 1:
            {
                anim.SetTrigger("Swing");
                break;
            }
        }
    }

    // FixedUpdate is, by deduction, called once per fixed frame
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
                        // Very similar to the above spawn, except it's uhittable and the amplitude is subtracted from the position
                        
                        ballPool[index] = SpawnBall(typeof(GenericUnhittable), transform.position + BALL_OFFSET + new Vector3(-amplitude, 0, 0), new Vector2(0, -7), Random.Range(6f, 10f));
                    }
                }
                break;
            }

            // Move around the court randomly while shooting 'lasers' (lines of balls)
            case 3:
            {
                // Interpolate to a point, and generate a new point when we reach a point

                break;
            }

            // A completely normal round of tennis, with the ball getting faster each rally
            case 4:
            {
                // Feel free to decide against any of these haha
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
                break;
            }
        }
    }

    public void OnTriggerEnter2D(Collider2D other) {
        Ball ball = other.gameObject.GetComponent<Ball>();
        if (ball)
            if (ball.hit)
                print("ouch?");
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
}
