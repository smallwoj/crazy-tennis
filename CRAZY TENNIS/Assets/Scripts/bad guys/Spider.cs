//                (
//                 )
//                (
//          /\  .-"""-.  /\   
//    _    //\\/  ,,,  \//\\    _
//   (#)===|/\| ,;;;;;, |/\|===(#) 
//         //\\\;-"""-;///\\
//  _     //  \/   .   \/  \\     _
// (#)===(| ,-_| \ | / |_-, |)===(#)
//         //`__\.-.-./__`\\
//  _     // /.-(() ())-.\ \\     _
// (#)===(\ |)   '---'   (| /)===(#)
//     _  ` (|           |) `  _
//    (#)===\)           (/===(#)
// (from ascii.co.uk/art/spider and https://ascii.co.uk/art/tennis)

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spider : BadThing
{
    // Instance variables
    /// <summary> Shortcut to the animator </summary>
    private Animator anim;
    /// <summary>
    /// Current phase
    /// n -> Throw 2^n balls, 1 of which is hittable
    /// </summary>
    private int phase = 0;
    /// <summary> The spider's octo-wielded rackets </summary>
    private SpiderRacket[] rackets = new SpiderRacket[8];
    /// <summary> How many of the rackets the spider is using </summary>
    private int activeRackets = 0;

    // Start is called before the first frame update
    new void Start()
    {
        base.Start();
        
        // Set the player's ball rules to breakoutn't
        GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerBehaviour>().Breakout = false;

        // Get all of the rackets (and disable them)
        rackets = GetComponentsInChildren<SpiderRacket>();
        foreach (SpiderRacket racket in rackets)
        {
            racket.Spider = this;
            racket.gameObject.SetActive(false);
        }
        
        // Other initialization stuff
        anim = GetComponent<Animator>();
        NextPhase();
    }

    // Update is called once per frame
    void Update()
    {
        // Local variables
        bool allBallsOffscreen = true; // Whether each racket's ball is offscreen

        // Tell each racket to despawn their ball if applicable
        for (int i = 0; i < activeRackets; i++)
        {
            // Along the way, see if all balls are offscreen
            if (!rackets[i].DespawnBallIfOffscreen())
            {
                allBallsOffscreen = false;
            }
        }

        // If all balls are offscreen, and the spider is not hurt, rally!
        if (allBallsOffscreen 
            && !anim.GetAnimatorTransitionInfo(0).IsName("Hurt -> Idle") 
            && anim.GetCurrentAnimatorStateInfo(0).IsName("Idle"))
        {
            SpawnBalls();
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
            DestroyAllBalls();
            SpawnNextEnemy("redCharacter");
        }
        else
        {
            // Award points (the if is there to avoid awarding points when the guy spawns)
            if (phase > 1)
            {
                base.NextPhase();
            }

            // Get ready for the next phase
            activeRackets = (int)Mathf.Pow(2, phase);
            for (int i = 0; i < activeRackets; i++)
            {
                rackets[i].gameObject.SetActive(true);
            }
            maxhits = activeRackets;
        }
    }

    /// <summary>
    /// Makes each racket throw a ball at the player.
    /// Only one racket will throw a hittable ball!
    /// </summary>
    private void SpawnBalls()
    {
        // Randomly decide which racket gets the priviledge of throwing a 
        // hittable ball
        int hittableIndex = Random.Range(0, activeRackets);

        // For each racket, assign it the appropriate type and then swing!
        for (int i = 0; i < activeRackets; i++)
        {
            if (i == hittableIndex)
            {
                rackets[i].BallType = typeof(GenericHittable);
            }
            else
            {
                rackets[i].BallType = typeof(GenericUnhittable);
            }

            rackets[i].Anim.SetTrigger("Swing time");
        }
    }

    /// <summary>
    /// Reacts to taking damage
    /// </summary>
    /// <param name="ball"> The ball that hit the spider :( </param>
    public override void Ouch(Ball ball)
    {
        anim.SetTrigger("Ouch");
        base.Ouch(ball);
    }
}
