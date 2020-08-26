// A completely normal, non-suspicious human...

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HumaN : BadThing
{
    // cONSTANTS
    /// <summary> The intial value for the rallyCount variable </summary>
    private static readonly int INITIAL_RALLY_COUNT = 2;
    /// <summary> The displacement from this object's position and the head 
    /// (the part that hits the ball) of the racket </summary>
    private static readonly Vector3 HEAD_OFFSET = new Vector3(-0.399f, -0.322f, 0);
    /// <summary> The third natural number </summary>
    private static readonly int THREE = 2 + 1;

    // Instance variables
    /// <summary> The current ball in the scene </summary>
    private Ball ball;
    /// <summary>
    /// The current phase
    /// 1 - Identical to Cocky Bastard's 3rd phase, except a couple of UFOs 
    ///     are also shooting at you
    /// 2 - He moves around the court erratically while some UFOs are further 
    ///     harrassing the player
    /// 3 - He stands still while the UFOs go sicko mode
    /// </summary>
    private int phase;
    /// <summary> How many times it can rally before taking a hit </summary>
    private int rallyCount = INITIAL_RALLY_COUNT;
    /// <summary> Reference to the player's script </summary>
    private PlayerBehaviour pb;
    /// <summary> Reference to the animator </summary>
    private Animator anim;
    /// <summary> A bunch of UFOs that support Huma N. in battle </summary>
    private List<UFO> ufoFleet = new List<UFO>();

    // Start is called before the first frame update
    new void Start()
    {
        // Standard badthing stuff
        base.Start();
        anim = GetComponent<Animator>();
        phase = 0;
        
        // Tell the player about this cool thing called Rally (and !Breakout)
        pb = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerBehaviour>();
        pb.PlayerHurt += Serve;
        pb.Breakout = false;

        pb.PlayerGameOver -= base.SpawnRecoveryEnemy;
        pb.PlayerGameOver += SpawnRecoveryEnemy;
        NextPhase();
    }

    // Update is called once per frame
    void Update()
    {
        // bring me the ball
		if(ball != null && ball.OutsideCourt)
		{
            // we must destroy it 
            // for it is a creature of darkness
			Destroy(ball.gameObject);
            ball = null;
            Serve();
		}
    }

    /// <summary>
    /// Moves on to the next phase, as described in the "phase" summary
    /// </summary>
    public override void NextPhase()
    {
        ball = null;
        phase++;

        // Spawn some UFOs and set the health
        switch (phase)
        {
            case 1: 
            {   
                GameObject ufoPrefab = UnityEditor.AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Prefabs/Enemies/Huma N/UFO.prefab");
                
                GameObject ufo1 = (GameObject)UnityEditor.PrefabUtility.InstantiatePrefab(ufoPrefab);
                ufo1.GetComponent<UFO>().Commander = this;
                ufo1.transform.position = new Vector3(2.38f, 3.26f, 0);
                ufoFleet.Add(ufo1.GetComponent<UFO>());

                GameObject ufo2 = (GameObject)UnityEditor.PrefabUtility.InstantiatePrefab(ufoPrefab);
                // It would be so cool if I could pass parameters to a new object in order to ensure that it's properly initialized once it's created. I would call this feature a "constructor"
                ufo2.GetComponent<UFO>().Commander = this;
                ufo2.transform.position = new Vector3(-2.4f, 3.26f, 0);
                ufoFleet.Add(ufo2.GetComponent<UFO>());

                maxhits = THREE + THREE;
                UFO.BallProbability = 1;
                break;
            }
            case 2:
            {   
                UFO.BallProbability = 0;
                maxhits = THREE;
                break;
            }
            case 3:
            {
                UFO.BallProbability = 0.75f;

                ufoFleet[0].GetComponent<UFO>().Spin();
                ufoFleet[0].GetComponent<UFO>().MoveTo(new Vector3(-2.4f, 4.5f, 0));

                ufoFleet[1].GetComponent<UFO>().Spin();
                ufoFleet[1].GetComponent<UFO>().MoveTo(new Vector3(2.4f, 4.5f, 0));

                maxhits = THREE * THREE  + THREE + THREE + (THREE / THREE);
                
                pb.PlayerHurt -= Serve;

                break;
            }
        }

        if(phase == 4)
        {
            DestroyAllUFOs();
            SpawnNextEnemy("Cutscene Dennis");
        }
        else
        {
            // Award points (the if is there to avoid awarding points when the guy spawns)
            if (phase > 1)
            {
                base.NextPhase();
            }
            
            // Start the rally
            if (phase < 3)
            {
                Serve();
            }
        }
    }

    /// <summary>
    /// Called when hit by a ball.
    /// Either rallies or gets hurt, depending on the ball's origin and the 
    /// current value of RallyCount.
    /// A ball from a UFO will hurt Huma N. instantly, skipping the rally, 
    /// because it turns out getting a BadThing to rally another BadThing's 
    /// ball is very glitchy and difficult :(
    /// </summary>
    /// <param name="hitBall"> The ball that hit the enemy </param>
    public override void Ouch(Ball hitBall)
    {
        // The enemy's time has come... execute oof
        if (rallyCount <= 0 || hitBall != this.ball)
        {
            anim.SetTrigger("Hurt");
            base.Ouch(hitBall);
            
            // If the ouch was caused by a UFO, reset that UFO
            foreach (UFO ufo in ufoFleet)
            {
                if (hitBall == ufo.Ball)
                {
                    ufo.ResetUFO();
                }
            }

            if (phase < 3)
            {
                Serve();
            }
        }
        else
        {
            // Rallying time!
            Rally();
            rallyCount--;
            hitBall.hit = false;
            // Ignore collision between the enemy and the ball
            Physics2D.IgnoreCollision(hitBall.GetComponent<Collider2D>(), this.GetComponent<Collider2D>(), true);
        }
    }

    /// <summary>
    /// Once a certain frame of his swing animation is reached, this method is 
    /// called to either spawn a new ball or send the existing ball back
    /// </summary>
    private void HitTime()
    {
        /* 
        There's a bug in which he swings the racket while his ball is 
        currently somewhere else on the court, and I have no idea what causes 
        it.
        All I really know is that this method is being called at the wrong time 
        sometimes.

        Oh, and sometimes he stops serving until the player gets hurt. I think that glitch is caused by him getting 
        hurt after transitioning to the serve animation but before spawning the 
        ball.
        I think it can be worked around by appending "&& ball != null" to the 
        condition in the first line of Ouch. This would make the enemy 
        invincible while serving, which would probably be more frustrating to 
        the player than the occasional pause.

        Basically, remind me not to let an enemy get hurt by another enemy's 
        ball ever again 😤
        */

        // If there isn't already a ball, spawn a new one
        if (ball == null)
        {
            ball = SpawnBall();
        }
        // Otherwise, send the existing one back
        else
        {
            ball.body.velocity = new Vector2(Random.Range(-3f, 3f), Random.Range(-3f, -4f)).normalized * 5;
        }
    }

    /// <summary>
    /// Triggers the serve animation
    /// </summary>
    public void Serve()
    {
        anim.SetTrigger("Serve");
        rallyCount = INITIAL_RALLY_COUNT;
    }

    /// <summary>
    /// Triggers the rally animation
    /// </summary>
    public void Rally()
    {
        anim.SetTrigger("Rally");
    }

    /// <summary>
    /// Throws a new ball toward the player
    /// </summary>
    /// <returns> The ball that was spawned </returns>
    private Ball SpawnBall()
    {
        switch (phase)
        {
        case 1: 
            // Identical to Cocky Bastard phase 3, but the ball is unhittable 
            // because this guy's a jerk.
            return base.SpawnBall(
                typeof(GenericUnhittable),
                transform.position + HEAD_OFFSET,
                new Vector2(Random.Range(-3f, 3f), Random.Range(-3f, -4f)).normalized * 5,
                Random.Range(6f, 10f)
            );
        case 2: 
            // Very similar to the previous phase, but the ball is hittable and 
            // a bit faster
            return base.SpawnBall(
                typeof(GenericHittable),
                transform.position + HEAD_OFFSET,
                new Vector2(Random.Range(-3f, 3f), Random.Range(-3f, -4f)).normalized * 6,
                Random.Range(6f, 10f)
            );
        // In phase 3, the UFOs do all the spawning
        default: return null;
        }
    }

    /// <summary>
    /// Destroys all current UFOs
    /// </summary>
    private void DestroyAllUFOs()
    {     
        foreach (UFO ufo in ufoFleet)
        {
            Destroy(ufo.gameObject);
        }
        ufoFleet.Clear();
    }

    /// <summary>
    /// Clears any ufos, then spawns the recovery enemy
    /// </summary>
    public new void SpawnRecoveryEnemy()
    {
        DestroyAllUFOs();
        base.SpawnRecoveryEnemy();
    }

    /// <summary>
    /// Removes the Serve method from the PlayerHurt event when the gameobject is destroyed
    /// </summary>
    private new void OnDestroy()
    {
        base.OnDestroy();
        pb.PlayerHurt -= Serve;
    }

    /// <summary>
    /// String representing the enemies prefab
    /// </summary>
    /// <returns>See: summary</returns>
    public override string PrefabString() 
    {
        return "Huma N/Huma N.";
    }
}
