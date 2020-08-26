/// <summary>
/// DOG
/// </summary>
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System;
using UnityEngine;

public class Dog : GoodBoy
{
    /// <summary>
    /// Positions that dog started from, and is going to
    /// </summary>
    private Vector2 to, from;

    /// <summary>
    /// t
    /// between 0 and 1, used for movin
    /// </summary>
    private float t;

    /// <summary>
    /// How much to increment t by every frame
    /// </summary>
    private float inc;

    /// <summary>
    /// the phases
    /// 
    /// 1 - dog run up to you
    /// 2 - drop the ball and wait patiently. if youre silly and hurt yourself, dog put ball back
    /// 3 - oh god hes so FAST HE HAS TO GET THE BALL
    /// 4 - he sit triumphantly while the crowd goes nuts
    /// 5 - go back to whoever spawned this guy
    /// </summary>
    private int phase;

    /// <summary>
    /// dogie has the ballie : )
    /// </summary>
    private Ball ball;

    /// <summary>
    /// reference to dog body
    /// </summary>
    private Rigidbody2D rb;

    /// <summary>
    /// position of dog last frame
    /// </summary>
    private Vector2 prev;

    /// <summary>
    /// reference to dog animator
    /// </summary>
    private Animator anim;

    /// <summary>
    /// reference to the thing that displays dog
    /// </summary>
    new private SpriteRenderer renderer;

    /// <summary>
    /// The collider that the dog is jumping over
    /// </summary>
    private Collider2D JumpOver;

    /// <summary>
    /// time
    /// </summary>
    private DateTime startTime;

    /// <summary>
    /// if dogy have ballie
    /// </summary>
    private bool hasBall;

    /// <summary>
    /// if dogy is waiting for the time to strike.
    /// </summary>
    private bool waiting;

    /// <summary>
    /// The prefab string of the enemy to spawn next. Often, it will be the enemy the player lost to
    /// </summary>
    public String nextEnemy;

    // Start is called before the first frame update
    new void Start()
    {
        // set references
        base.Start();
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        renderer = GetComponent<SpriteRenderer>();
        PlayerBehaviour pb = FindObjectOfType<PlayerBehaviour>();
        pb.Breakout = true;
        phase = 0;
        hasBall = true;
        waiting = false;
        // start the next phase
        NextPhase();
        pb.PlayerHurt += ReplaceBall;
        pb.PlayerGameOver -= SpawnRecoveryEnemy;
    }

    // Update is called once per frame
    void Update()
    {
        // if in phase 2, determine whether we should drop the ball
        switch (phase)
        {
            case 2:
            if((DateTime.Now - startTime).Seconds >= 1)
            {
                if (hasBall)
                    anim.SetTrigger("drop ball");
            }
            break;
        }
    }

    void FixedUpdate()
    {
        prev = rb.position;
        // do t nonsense
        if (t < 1)
        {
            // if phase is not 3, move like line
            if(phase != 3)
            {
                inc = 0.05f / Vector2.Distance(to, from);
                t += inc;
                if(t >= 1)
                    t = 1f;
                // T2, i never remembered why we needed this
                float T = (float)(1f - System.Math.Cos(t * System.Math.PI))/2f;
                // linear interpolation be like
                rb.position = Vector2.Lerp(from, to, T);
            }
            else if(phase == 3) //bezier curve
            {
                inc =  0.2f / Vector2.Distance(to, from);
                t += inc;
                if(t >= 1)
                    t = 1f;
                //T22
                float T = (float)(1f - System.Math.Cos(t * System.Math.PI/2.0));
                //set the anchor point
                Vector2 anchor = new Vector2(from.x, to.y);
                //bezier curve be like
                rb.position = (1f-T) * (1f-T) * from + 2f * (1f-T) * T * anchor + T*T*to;
            }
        }
        else //we're done 
        {
            if(phase == 1)
            {
                //move on
                NextPhase();
            }
            else if(phase == 2)
                //set ball down and go a bit away
                if(!hasBall && !waiting)
                {
                    from = rb.position;
                    to = rb.position + new Vector2(0.1f, 0f);
                    t = 0;
                    waiting = true;
                }
        }
        //determine the direction
        DetermineDirection();
    }

    /// <summary>
    /// The dog doesn't actually get hit, he catches it!
    /// </summary>
    /// <param name="ball">The ball that the dog caught</param>
    public override void Ouch(Ball ball)
    {
        //animator nonsense
        anim.SetTrigger("catch ball");
        // remove the GoFetch method 
        ((GenericHittable) ball).OnHit -= GoFetch;
        // remove all balls
        DestroyAllBalls();
        //dogy have ball
        hasBall = true;
        //the crowd goes fucking bonkers
        FindObjectOfType<CrowdBehaviour>().Cheer(2f);
        NextPhase();
    }

    /// <summary>
    /// dog go fetch!
    /// </summary>
    public void GoFetch()
    {
        NextPhase();
    }

    /// <summary>
    /// if the dog collides with the net, jump over it
    /// </summary>
    /// <param name="other"></param>
    void OnCollisionEnter2D(Collision2D other)
    {
        if (other.collider.name == "Net")
        {
            // jump over it
            JumpOver = other.collider;
            Physics2D.IgnoreCollision(GetComponent<Collider2D>(), JumpOver, true);
            anim.SetTrigger("jump");
        }
    }

    /// <summary>
    /// When the dog jumps, the crowd just simply has to cheer for him, wouldn't you?
    /// </summary>
    public void CheerForJump()
    {
        FindObjectOfType<CrowdBehaviour>().Cheer(0.75f);
    }

    /// <summary>
    /// This time the dog actually puts down the ball (spawning it)
    /// </summary>
    public void ActuallyPutDownBall()
    {
        hasBall = false; //dog don't have ball :(
        ball = SpawnBall(
            typeof(GenericHittable),
            rb.position + new Vector2(-0.328f, 0.007f),
            Vector2.zero,
            0f
        );
        // add the go fetch method to run when the ball is hit
        ((GenericHittable) ball).OnHit += GoFetch;
        // scale the ball to match the sprite
        ball.transform.localScale = new Vector3(0.65f, 0.65f, 1);
        // remove collision between the new ball and this
        Physics2D.IgnoreCollision(ball.GetComponent<Collider2D>(), this.GetComponent<Collider2D>(), true);
        // go down a bit
        from = rb.position;
        to = rb.position + new Vector2(-2f, -1f);
        t = 0;
    }

    /// <summary>
    /// Re-adds collision between the net and dog
    /// </summary>
    public void Land()
    {
        Physics2D.IgnoreCollision(GetComponent<Collider2D>(), JumpOver, false);
        JumpOver = null;
    }

    /// <summary>
    /// Replace the ball when the player damages themselves
    /// </summary>
    public void ReplaceBall()
    {
        //the dog just kinda generates the ball
        anim.ResetTrigger("drop ball");
        anim.SetTrigger("catch ball");
        hasBall = true;
        waiting = false;
        //go back a phase to perform phase 1 stuff
        phase = 0;
        NextPhase();
    }

    /// <summary>
    /// Determines what direction the dog is facing using MATH 1300 - Vector Geometry and Linear Algebra
    /// </summary>
    private void DetermineDirection()
    {
        // list of the variables in the animator
        List<string> names = new List<string>(new string[] {"up", "right", "down", "left"});
        // set all to false
        foreach (string name in names)
            anim.SetBool(name, false);
        //determine doggy direction
        Vector2 dir = (rb.position - prev).normalized;
        //if there is a direction
        if (dir.magnitude != 0)
        {
            //list of directions in vector form
            List<Vector2> directions = new List<Vector2>(new Vector2[] {Vector2.up, Vector2.right, Vector2.down, Vector2.left});
            //take the dog product of the cardinal directions (functionally!)
            List<float> dots = directions.Select(x => Vector2.Dot(dir, x)).ToList();
            //determine which direction is closest to where the dog is facing
            string name = names[dots.IndexOf(dots.Max())];
            anim.SetBool(name, true);
            //flip the sprite if the dog going right lmao
            renderer.flipX = name == "right";
        }
    }

    /// <summary>
    /// Goes to the next phase
    /// </summary>
    public override void NextPhase()
    {
        phase++;
        switch (phase)
        {
            case 1: // dogy bring ball to you!
                from = rb.position;
                to = new Vector2(-0.16f, -2.5f);
                t = 0;
            break;
            case 2: // dogy put the ball down and wait.
                startTime = DateTime.Now;
            break;
            case 3: // go fetch!!
                from = rb.position;
                to = ball.body.position + ball.body.velocity * 0.75f;
                t = 0;
                waiting = false;
                anim.ResetTrigger("drop ball");
            break;
            case 4: // win!
                StartCoroutine("GoToNextEnemy");
            break;
        }
    }

    /// <summary>
    /// Coroutine that goes to the next enemy after 2.5 seconds
    /// </summary>
    /// <returns>i dont even know</returns>
    private IEnumerator GoToNextEnemy()
    {
        yield return new WaitForSeconds(2.5f);
        FindObjectOfType<PlayerBehaviour>().PlayerHurt -= ReplaceBall;
        FindObjectOfType<PlayerBehaviour>().inRecovery = false;
        SpawnNextEnemy(nextEnemy);
    }

    /// <summary>
    /// String representing the enemies prefab
    /// </summary>
    /// <returns>See: summary</returns>
    public override string PrefabString() 
    {
        return "dogy";
    }
}
