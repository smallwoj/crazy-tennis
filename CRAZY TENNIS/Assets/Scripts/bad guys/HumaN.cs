// A completely normal, non-suspicious human...

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HumaN : BadThing
{
    // cONSTANTS
    /// <summary> The intial value for the rallyCount variable </summary>
    private static readonly int INITIAL_RALLY_COUNT = 10;
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
    /// 3 - He stands still while the UFOs pull a Space Invaders on ya
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
        pb.PlayerHurt += Rally;
        pb.Breakout = false;

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
        DestroyAllBalls();
        ball = null;
        maxhits = THREE * THREE;
        phase++;

        // Destroy all current UFOs
        foreach (UFO ufo in ufoFleet)
        {
            Destroy(ufo.gameObject);
        }
        ufoFleet.Clear();

        // Spawn some UFOs based on the phase
        switch (phase)
        {
            case 1: 
            {
                GameObject ufoPrefab = UnityEditor.AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Prefabs/Enemies/UFO.prefab");
                
                GameObject ufo1 = (GameObject)UnityEditor.PrefabUtility.InstantiatePrefab(ufoPrefab);
                ufo1.GetComponent<UFO>().Commander = this;
                ufo1.transform.position = new Vector3(2.38f, 3.26f, 0);
                ufoFleet.Add(ufo1.GetComponent<UFO>());

                GameObject ufo2 = (GameObject)UnityEditor.PrefabUtility.InstantiatePrefab(ufoPrefab);
                // It would be so cool if I could pass parameters to a new object in order to ensure that it's properly initialized once it's created. I would call this feature a "constructor"
                ufo2.GetComponent<UFO>().Commander = this;
                ufo2.transform.position = new Vector3(-2.4f, 3.26f, 0);
                ufoFleet.Add(ufo2.GetComponent<UFO>());

                break;
            }
        }

        if(phase == 4)
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
            
            // Start the rally
            Serve();
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
                    ufo.Reset();
                }
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
        // If there isn't already a ball, spawn a new one
        if (ball == null)
        {
            ball = SpawnBall();
        }
        // Otherwise, send the existing one back
        else
        {
            ball.Velocity = new Vector2(Random.Range(-3f, 3f), Random.Range(-3f, -4f)).normalized * 5;
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
                typeof(GenericHittable),
                transform.position + HEAD_OFFSET,
                new Vector2(Random.Range(-3f, 3f), Random.Range(-3f, -4f)).normalized * 5,
                Random.Range(6f, 10f)
            );
        default: return null;
        }
    }

    /// <summary>
    /// Cleans up when this object is destroyed
    /// </summary>
    void OnDestroy()
    {
        // remove this
        pb.PlayerHurt -= Rally;
    }
}
