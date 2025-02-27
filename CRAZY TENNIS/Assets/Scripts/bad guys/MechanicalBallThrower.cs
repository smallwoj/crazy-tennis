﻿// my baby

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MechanicalBallThrower : BadThing
{
    // Constants
    /// <summary> The displacement between the enemy's position and the ball's initial position </summary>
    private static readonly Vector3 BALL_OFFSET = new Vector3(0, -0.35f, 0);
    /// <summary> How long (in seconds) it takes to go from one charging animation to the other. Indexed by phase  </summary>
    private static readonly float[] CHARGE_ANIM_TIMES = {0, 1/8, 1, 2};
    /// <summary> How fast the ball goes in each phase </summary>
    private static readonly float[] BALL_SPEEDS = {0, 7, 15, 4};
    private static readonly int[] MAX_HITS = {0, 2, 1, 15};
    /// <summary> How many balls are sent out during the last phase </summary>
    private static readonly int NUM_BALLS = 25;

    // Instance variables
    /// <summary> Reference to the ball that this thing throws </summary>
    private Ball ball;
    /// <summary> References to the many balls that this thing throws during its third phase </summary>
    private Ball[] lastPhaseBalls = new Ball[NUM_BALLS];
    /// <summary>
    /// Current phase
    /// 1 -> Counts down (extremely) quickly and then shoots a ball 
    /// 2 -> Counts down slowly and then shoots a ball at high speed
    /// 3 -> Maximum Overdrive!
    /// </summary>
    private int phase = 0;
    /// <summary> Shortcut to the animator </summary>
    private Animator anim;
    /// <summary> How many seconds remain in the current charging animation.
    /// Starts at chargeAnimTime and goes down to 0 </summary>
    private float chargeTimeRemaining = 0;
    /// <summary>
    /// Sound effects that play when it charges
    /// </summary>
    private AudioClip charge, chargeComplete;

    // Start is called before the first frame update
    new void Start()
    {
        base.Start();
        FindObjectOfType<MusicManager>().Play(base.music);
        
        anim = GetComponent<Animator>();
        NextPhase();
        
        // Set the player's ball rules to breakoutn't
        GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerBehaviour>().Breakout = false;

        // Override sounds
        hitBall = Resources.Load<AudioClip>("Audio/Sound Effects/Mechanical ball thrower/Laser cannon short");
        dead = Resources.Load<AudioClip>("Audio/Sound Effects/Mechanical ball thrower/Computer error");
        charge = Resources.Load<AudioClip>("Audio/Sound Effects/Mechanical ball thrower/Airplane ding");
        chargeComplete = Resources.Load<AudioClip>("Audio/Sound Effects/Mechanical ball thrower/Electronic chime");
    }

    // Update is called once per frame
    void Update()
    {
        // Keep time and (maybe, just maybe) move to the next animation
        if (phase < 4)
        {
            chargeTimeRemaining -= Time.deltaTime;
            if (chargeTimeRemaining < 0)
            {
                anim.SetTrigger("Next charge");
                chargeTimeRemaining = CHARGE_ANIM_TIMES[phase];

                if (phase >= 2)
                {
                    audioSource.clip = charge;
                    audioSource.Play();
                }
            }
            else
            {
                anim.ResetTrigger("Next charge");
            }
        }
    }

    /// <summary>
    /// Goes to the next phase, as described in the 'phase' summary
    /// </summary>
    public override void NextPhase()
    {
        DestroyAllBalls();
        phase++;
        if (phase == 4)
        {
            audioSource.clip = dead;
            audioSource.Play();
            anim.SetTrigger("Dead");
            TransitionToNextEnemy("Spider/Spider");
        }
        else if (phase < 4)
        {
            // Award points (the if is there to avoid awarding points when the guy spawns)
            if (phase > 1)
            {
                base.NextPhase();
            }

            // Get ready for the next phase
            maxhits = MAX_HITS[phase];
            chargeTimeRemaining = CHARGE_ANIM_TIMES[phase];
            anim.SetTrigger("Reset charge");
        }
    }

    /// <summary>
    /// Overridden method that is fired when the enemy takes damage.
    /// Activates the hurt effect
    /// </summary>
    /// <param name="ball"> The causer of the ouch </param>
    public override void Ouch(Ball ball)
    {
        base.Ouch(ball);
        // Coroutines are so great :)
        StartCoroutine("HurtEffect");
    }
    /// <summary>
    /// Briefly shows an effect for getting hurt
    /// </summary>
    /// <returns> An IEnumerator, obviously </returns>
    private IEnumerator HurtEffect()
    {
        GameObject hitEffect = transform.GetChild(0).gameObject;
        hitEffect.SetActive(true);
        yield return new WaitForSeconds(0.5f);
        hitEffect.SetActive(false);
    }

    /// <summary>
    /// Shoots a very fast ball at incredible hihg speed 
    /// </summary>
    public void Fire()
    {
        // Destroy an existing ball to prevent memory leaks
        if (ball != null && ball.OutsideCourt)
        {
            Destroy(ball.gameObject);
            ball = null;
        }

        // Based on the phase, fire either one ball or quite a few balls
        if (phase < 3)
        {
            // Fire one ball
            ball = SpawnBall(
                typeof(GenericHittable),
                transform.position + BALL_OFFSET,
                new Vector2(0, -BALL_SPEEDS[phase]),
                Random.Range(6f, 10f)
                );
            FindObjectOfType<CameraBehaviour>().Impact(0.4f, Vector2.down);
        }
        else
        {
            // Fire several balls. This is like TestBadGuy's entire gimmick, 
            // except the balls are only fired once
            for (int i = 0; i < NUM_BALLS; i++)
            {
                // Clean up this ball from the previous call to this method
                if (lastPhaseBalls[i] != null && lastPhaseBalls[i].OutsideCourt)
                {
                    GameObject.Destroy(lastPhaseBalls[i].gameObject);
                    lastPhaseBalls[i] = null;
                }

                // Fire a new ball!
                if (lastPhaseBalls[i] == null)
                {
                    // Prepare a velocity vector that uses a random angle
                    Vector2 velocity = new Vector2(Random.Range(-1/2f, 1/2f), Random.Range(-1f, -1/2f));
                    velocity.Normalize();
                    velocity *= Random.value * BALL_SPEEDS[phase];

                    // Fire this ball at a random angle
                    lastPhaseBalls[i] = SpawnBall(
                        typeof(GenericHittable), 
                        transform.position + BALL_OFFSET,
                        velocity,
                        velocity.magnitude * 2
                        );
                        FindObjectOfType<CameraBehaviour>().Impact(0.01f, Vector2.down);
                }
            }
            FindObjectOfType<CameraBehaviour>().Impact(0.4f, Vector2.down);
        }
        // Also play the sound effect(s)
        audioSource.clip = chargeComplete;
        audioSource.Play();
        audioSource.PlayOneShot(hitBall);
    }

    public void Shake()
    {
        FindObjectOfType<CameraBehaviour>().ShakeScreen(0.1f);
    }

    /// <summary>
    /// String representing the enemies prefab
    /// </summary>
    /// <returns>See: summary</returns>
    public override string PrefabString()
    {
        return "Mechanical ball thrower";
    }
}