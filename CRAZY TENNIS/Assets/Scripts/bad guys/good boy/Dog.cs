using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System;
using UnityEngine;

public class Dog : GoodBoy
{
    private Vector2 to, from;
    private float t;
    private float inc;
    private int phase;
    private Ball ball;
    private Rigidbody2D rb;
    private Vector2 prev;
    private Animator anim;
    new private SpriteRenderer renderer;
    private Collider2D JumpOver;
    private DateTime startTime;
    private bool hasBall;
    private bool waiting;
    public String nextEnemy;
    // Start is called before the first frame update
    new void Start()
    {
        base.Start();
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        renderer = GetComponent<SpriteRenderer>();
        FindObjectOfType<PlayerBehaviour>().Breakout = true;
        phase = 0;
        hasBall = true;
        waiting = false;
        NextPhase();
        FindObjectOfType<PlayerBehaviour>().PlayerHurt += ReplaceBall;
        FindObjectOfType<PlayerBehaviour>().PlayerGameOver -= SpawnRecoveryEnemy;
    }

    // Update is called once per frame
    void Update()
    {
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
        if (t < 1)
        {
            if(phase != 3)
            {
                inc = 0.05f / Vector2.Distance(to, from);
                t += inc;
                if(t >= 1)
                    t = 1f;
                float T = (float)(1f - System.Math.Cos(t * System.Math.PI))/2f;
                rb.position = Vector2.Lerp(from, to, T);
            }
            else if(phase == 3)
            {
                inc =  0.2f / Vector2.Distance(to, from);
                t += inc;
                if(t >= 1)
                    t = 1f;
                float T = (float)(1f - System.Math.Cos(t * System.Math.PI/2.0));
                Vector2 anchor = new Vector2(from.x, to.y);
                rb.position = (1f-T) * (1f-T) * from + 2f * (1f-T) * T * anchor + T*T*to;
            }
        }
        else
        {
            if(phase == 1)
            {
                NextPhase();
            }
            else if(phase == 2)
                if(!hasBall && !waiting)
                {
                    from = rb.position;
                    to = rb.position + new Vector2(0.1f, 0f);
                    t = 0;
                    waiting = true;
                }

        }
        DetermineDirection();
    }

    public override void Ouch(Ball ball)
    {
        anim.SetTrigger("catch ball");
        ((GenericHittable) ball).OnHit -= GoFetch;
        DestroyAllBalls();
        hasBall = true;
        FindObjectOfType<CrowdBehaviour>().Cheer(2f);
        NextPhase();
    }

    public void GoFetch()
    {
        NextPhase();
    }

    void OnCollisionEnter2D(Collision2D other)
    {
        if (other.collider.name == "Net")
        {
            JumpOver = other.collider;
            Physics2D.IgnoreCollision(GetComponent<Collider2D>(), JumpOver, true);
            anim.SetTrigger("jump");
        }
    }

    public void CheerForJump()
    {
        FindObjectOfType<CrowdBehaviour>().Cheer(0.75f);
    }

    public void ActuallyPutDownBall()
    {
        hasBall = false;
        ball = SpawnBall(
            typeof(GenericHittable),
            rb.position + new Vector2(-0.328f, 0.007f),
            Vector2.zero,
            0f
        );
        ((GenericHittable) ball).OnHit += GoFetch;
        ball.transform.localScale = new Vector3(0.65f, 0.65f, 1);
        // remove collision between the new ball and this
        Physics2D.IgnoreCollision(ball.GetComponent<Collider2D>(), this.GetComponent<Collider2D>(), true);
        from = rb.position;
        to = rb.position + new Vector2(-2f, -1f);
        t = 0;
    }

    public void Land()
    {
        Physics2D.IgnoreCollision(GetComponent<Collider2D>(), JumpOver, false);
        JumpOver = null;
    }

    public void ReplaceBall()
    {
        anim.ResetTrigger("drop ball");
        anim.SetTrigger("catch ball");
        hasBall = true;
        waiting = false;
        phase = 0;
        NextPhase();
    }

    private void DetermineDirection()
    {
        List<string> names = new List<string>(new string[] {"up", "right", "down", "left"});
        foreach (string name in names)
            anim.SetBool(name, false);
        Vector2 dir = (rb.position - prev).normalized;
        if (dir.magnitude != 0)
        {
            List<Vector2> directions = new List<Vector2>(new Vector2[] {Vector2.up, Vector2.right, Vector2.down, Vector2.left});
            List<float> dots = directions.Select(x => Vector2.Dot(dir, x)).ToList();
            string name = names[dots.IndexOf(dots.Max())];
            anim.SetBool(name, true);
            renderer.flipX = name == "right";
        }
    }

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
