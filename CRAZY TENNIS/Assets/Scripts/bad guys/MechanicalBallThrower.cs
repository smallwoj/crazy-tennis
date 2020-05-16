// my baby

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MechanicalBallThrower : BadThing
{
    // Constants
    /// <summary> How long (in seconds) it takes to go from one charging animation to the other. Indexed by phase  </summary>
    private static readonly float[] CHARGE_ANIM_TIMES = {0, 1/8, 1, 3};
    /// <summary> How fast the ball goes in each phase </summary>
    private static readonly float[] BALL_SPEEDS = {0, 7, 15, 5};
    private static readonly int[] MAX_HITS = {0, 2, 1, 15};
    /// <summary> The probability that a newly-spawned ball will be unhittable </summary>
    private static readonly float UNHITTABLE_PROBABILITY = 0.5f;

    // Instance variables
    /// <summary> Reference to the ball that this thing throws </summary>
    private Ball ball;
    /// <summary>
    /// Current phase
    /// 1 -> Counts down quickly and then shoots a ball 
    /// 2 -> Counts down slowly and then shoots a ball at speed 30
    /// 3 -> Maximum Overdrive!
    /// </summary>
    private int phase = 0;
    /// <summary> Shortcut to the animator </summary>
    private Animator anim;
    /// <summary> How many seconds remain in the current charging animation.
    /// Starts at chargeAnimTime and goes down to 0 </summary>
    private float chargeTimeRemaining = 0;

    // Start is called before the first frame update
    new void Start()
    {
        base.Start();
        anim = GetComponent<Animator>();
        NextPhase();
        
        // Set the player's ball rules to breakoutn't
        GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerBehaviour>().Breakout = false;
    }

    // Update is called once per frame
    void Update()
    {
        // Keep time and (maybe, just maybe) move to the next animation
        chargeTimeRemaining -= Time.deltaTime;
        if (chargeTimeRemaining <= 0)
        {
            anim.SetTrigger("Next charge");
            chargeTimeRemaining = CHARGE_ANIM_TIMES[phase];
        }
        else
        {
            anim.ResetTrigger("Next charge");
        }
    }

    /// <summary>
    /// Goes to the next phase, as described in the 'phase' summary
    /// </summary>
    public override void NextPhase()
    {
        DestroyAllBalls();
        phase++;
        maxhits = MAX_HITS[phase];
        if (phase == 4)
        {
            SpawnNextEnemy("redCharacter");
        }
        else
        {
            // Award points (the if is there to avoid awarding points when the guy spawns)
            if (phase > 1)
            {
                base.NextPhase();
            }
        }

        chargeTimeRemaining = CHARGE_ANIM_TIMES[phase];
    }

    /// <summary>
    /// Shoots a very fast ball at incredible hihg speed 
    /// </summary>
    public void Fire()
    {
        // Randomly decide which type of ball to fire
        // System.Type ballType;
        // 
        // if (Random.value < UNHITTABLE_PROBABILITY)
        // {
        //     ballType = typeof(GenericUnhittable);
        // }
        // else
        // {
        //     ballType = typeof(GenericHittable);
        // }

        // Destroy an existing ball to prevent memory leaks
        if (ball != null)
        {
            Destroy(ball.gameObject);
            ball = null;
        }

        ball = SpawnBall(
            typeof(GenericHittable),
            transform.position + new Vector3(0, -0.35f, 0),
            new Vector2(0, -BALL_SPEEDS[phase]),
            Random.Range(6f, 10f)
            );
    }
}