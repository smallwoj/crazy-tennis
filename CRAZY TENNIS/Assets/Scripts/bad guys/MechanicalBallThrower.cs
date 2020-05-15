// my baby

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MechanicalBallThrower : BadThing
{
    // Instance variables
    /// <summary>
    /// Current phase
    /// 1 -> Counts down from 3 and then shoots a ball at high speed
    /// 2 -> dunno
    /// </summary>
    private int phase = 0;

    /// <summary>
    /// Shortcut to the animator
    /// </summary>
    private Animator anim;

    // Start is called before the first frame update
    new void Start()
    {
        base.Start();
        anim = GetComponent<Animator>();
        NextPhase();
        maxhits = 4;
        
        // Set the player's ball rules to breakoutn't
        GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerBehaviour>().Breakout = false;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    /// <summary>
    /// Goes to the next phase, as described in the 'phase' summary
    /// </summary>
    public override void NextPhase()
    {
        DestroyAllBalls();
        phase++;
        maxhits = 0;
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
    }
}
