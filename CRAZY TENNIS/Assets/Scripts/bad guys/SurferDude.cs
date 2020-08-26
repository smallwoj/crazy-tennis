using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SurferDude : BadThing
{
    /// <summary>
    /// The current ball in the scene.
    /// </summary>
    /// <value>haha ball</value>
    public Ball ball {
		get
		{
			return _ball;
		}
		set
		{
			anim.SetBool("NeedBall", value == null);
			_ball = value;
		}
	}
    /// <summary>
    /// the ball but its a secret shhh....
    /// </summary>
	private Ball _ball;
    /// <summary>
    /// The current phase.
    /// 1 - goes back and forth in a straight path
    /// 2 - goes around a triangle
    /// 3 - goes around a star formation
    /// </summary>
    private int phase;
    /// <summary>
    /// Reference to the player
    /// </summary>
    private PlayerBehaviour pb;
    /// <summary>
    /// reference to the animation
    /// </summary>
    private Animator anim;
    /// <summary>
    /// path 1 - a line
    /// </summary>
    public List<Vector2> path1;
    /// <summary>
    /// path 2 - a triangle
    /// </summary>
    public List<Vector2> path2;
    /// <summary>
    /// path 3 - a star
    /// </summary>
    public List<Vector2> path3;
    /// <summary>
    /// reference to the current path
    /// </summary>
    private List<Vector2> currPath;
    /// <summary>
    /// current index of the current path that we are going towards
    /// </summary>
    private int target;
    /// <summary>
    /// reference to the vector indexed by target
    /// </summary>
    private Vector2 to;
    /// <summary>
    /// vector we started at
    /// </summary>
    private Vector2 from;
    /// <summary>
    /// progress from 'from' to 'to' (thank you english language)
    /// 0 to 1, as stated in comp 3490 thabks
    /// </summary>
    private float t;
    /// <summary>
    /// How much the target variable increases (or decreases!) each time it changes.
    /// Its usual values are 1 (clockwise) and -1 (counterclockwise), but 
    /// values with absolute value greater than 1 can be used to skip points, 
    /// making for less predicatble paths (and a value of 0 can be used to stay still)
    /// </summary>
    private int pathStep;
    /// <summary>
    /// the previous position of the dude that is this
    /// </summary>
    private Vector3 prev;
    /// <summary>
    /// reference to the ridid body of surfer dude
    /// </summary>
    private Rigidbody2D rb;

    // Start is called before the first frame update!!!!! im so proud of it
    new void Start()
    {
        // the base class has to start too you know
        base.Start();
        // initialize some good ol starting variables
        anim = GetComponent<Animator>();
        phase = 0;
        // snag the player and set some stuff
        pb = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerBehaviour>();
        pb.Breakout = false;
		pb.PlayerHurt += PlayerGotOuchSad;
        // ignore collision between the net and the surfer dude
        Physics2D.IgnoreCollision(GameObject.Find("Net").GetComponent<Collider2D>(), this.GetComponent<Collider2D>(), true);
        // ignore collision between the top bound and the surfer dude
        Physics2D.IgnoreCollision(GameObject.Find("Wacky collision stuff").transform.Find("Court bounds").transform.Find("Top court bound").GetComponent<Collider2D>(), this.GetComponent<Collider2D>(), true);
        // we NEED  ball im begging you
		anim.SetBool("NeedBall", true);
        rb = GetComponent<Rigidbody2D>();
        NextPhase();
	}

	// Update is called once per frame
	void Update()
	{
        // if ball gone
		if(ball != null && ball.OutsideCourt)
		{
            // bye
			Destroy(ball.gameObject);
			ball = null;
		}
	}

    /// <summary>
    /// Called when the enemy gets hit with a ball, it sets an animator trigger p much
    /// </summary>
    /// <param name="ball">the ball that did the deed</param>
	public override void Ouch(Ball ball)
	{
        anim.SetTrigger("ouch");
		base.Ouch(ball);
		this.ball = null;
	}

	// FixedUpdate is called?? who even knows about its frequency 
	void FixedUpdate()
    {
        // check if the animator is not in the state 'swing' or 'Ouch!'
        // note: This would prolly be better as an extension method 👀
		if(!anim.GetCurrentAnimatorStateInfo(0).IsName("swing") && !anim.GetCurrentAnimatorStateInfo(0).IsName("Ouch!"))
		{
            //set previous position
			prev = rb.position;
            // set new t value
			t += 0.05f / Vector2.Distance(to, from);
            //set real t value??? uh oh
			float T = (float)(1f - System.Math.Cos(t * System.Math.PI))/2f;
			if(t >= 1f)
			{
                // if we need a ball, then simply we swing
				if (anim.GetBool("NeedBall"))
					anim.SetTrigger("swing");
			    T = 1f;
                // haha lerp
			    rb.position = Vector3.Lerp(from, to, T);
                // go towards the next point
			    t = 0;
			    from = to;
			    target = (target + pathStep) % currPath.Count;
                // If we're going counterclockwise, add currPath.Count to ensure proper cycling
                if (pathStep < 0)
                    to = currPath[currPath.Count - 1 + target];
                else
                    to = currPath[target];
			}
			else // just lerp
			    rb.position = Vector3.Lerp(from, to, T);
			anim.SetFloat("xVel", rb.position.x - prev.x);
		}
        else // he aint moving
        	anim.SetFloat("xVel", 0f);
    }

    /// <summary>
    /// Goes to the next phase
    /// 1 - goes back and forth in a straight path
    /// 2 - goes around a triangle
    /// 3 - goes around a star formation
    /// 4 - farewell....
    /// </summary>
    public override void NextPhase()
    {
        phase++;
        if(phase == 1)
        {
            currPath = path1;
        }
        else if(phase == 2)
        {
            currPath = path2;
        }
        else if(phase == 3)
        {
            currPath = path3;
        }
        else if(phase == 4)
        {
            SpawnNextEnemy("Mechanical ball thrower");
        }
        t = 0;
        target = 0;
		maxhits = 3;
        from = rb.position;
        // Randomly decide to go clockwise or counterclockwise
        if (Random.value * 2 > 1)
        {
            pathStep = 1;
            to = currPath[target];
        }
        else
        {
            pathStep = -1;
            to = currPath[currPath.Count - 1 + target];
        }

        // Award points (the if is there to avoid awarding points when the guy spawns)
        if (phase > 1)
        {
            base.NextPhase();
        }
    }

    /// <summary>
    /// runs when the player gets hurt, reset the ball
    /// </summary>
	public void PlayerGotOuchSad()
	{
		ball = null;
	}

    /// <summary>
    /// Spawns the ball specific to what the surfer dude wants
    /// </summary>
    /// <returns>round spherical object tennis</returns>
    private Ball SpawnTheBall()
    {
        Ball b = SpawnBall(
            typeof(GenericHittable),
            rb.position + new Vector2(0, 0),
            (pb.CenterPos - (rb.position/* + new Vector2(-0.7f, -0.2f)*/)).normalized * 4,
            Random.Range(6f, 10f)
        );
        // remove collision between the new ball and this
        Physics2D.IgnoreCollision(b.GetComponent<Collider2D>(), this.GetComponent<Collider2D>(), true);
        return b;
    }

    /// <summary>
    /// when we are ready to hit the ball, spawn it
    /// </summary>
    public void Hit()
    {
		ball = SpawnTheBall();
        // we do NOT need the ball
		anim.SetBool("NeedBall", false);
    }

    /// <summary>
    /// on the destruction of the surfer dude... remove delegate method
    /// </summary>
	new void OnDestroy()
	{
        base.OnDestroy();
		pb.PlayerHurt -= PlayerGotOuchSad;
	}

    /// <summary>
    /// String representing the enemies prefab
    /// </summary>
    /// <returns>See: summary</returns>
    public override string PrefabString()
    {
        return "Surfer dude";
    }
}
