using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CockyBastard : BadThing
{
    private Ball ball;
    private int phase;
    private int rallyCount;
    private PlayerBehaviour pb;
    private Animator anim;
    private bool BallSpawnTriggered;
    // Start is called before the first frame update
    void Start()
    {
        anim = GetComponent<Animator>();
        phase = 0;
        NextPhase();
        pb = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerBehaviour>();
        pb.Breakout = true;
        BallSpawnTriggered = false;
    }

    // Update is called once per frame
    void Update()
    {
        if(ball != null && ball.OutsideCourt())
        {
            Destroy(ball.gameObject);
            ball = null;
        }
        if(ball == null && !BallSpawnTriggered)
        {
            anim.SetTrigger("swing");
            BallSpawnTriggered = true;
            rallyCount = phase - 1;
        }
    }

    public override void NextPhase()
    {
        DestroyAllBalls();
        ball = null;
        phase++;
        hits = 3;
        if(phase == 4)
        {
            SpawnNextEnemy("redCharacter");
        }
    }

    private Ball SpawnBallPhase(int phase)
    {
        if(phase == 1)
        {
            return SpawnBall(
                typeof(GenericHittable),
                transform.position + new Vector3(-0.7f, -0.2f, 0),
                new Vector2(0, -3),
                Random.Range(6f, 10f)
            );
        }
        else if(phase == 2)
        {
            return SpawnBall(
                typeof(GenericHittable), 
                transform.position + new Vector3(-0.7f, -0.2f, 0),
                new Vector2(Random.Range(-3f, 3f), Random.Range(-3f, -4f)),
                Random.Range(6f, 10f)
            );
        }
        else if(phase == 3)
        {
            return SpawnBall(
                typeof(GenericHittable), 
                transform.position + new Vector3(-0.7f, -0.2f, 0),
                (pb.transform.position - transform.position).normalized * 5,
                Random.Range(6f, 10f)
            );
        }
        else
            return null;
    }

    public override void Ouch(Ball ball)
    {
        if(rallyCount <= 0)
            base.Ouch(ball);
        else
        {
            anim.SetTrigger("swing");
            print("rally " + rallyCount);
            rallyCount--;
            ball.hit = false;
            Physics2D.IgnoreCollision(ball.GetComponent<Collider2D>(), this.GetComponent<Collider2D>(), true);
        }
    }

    public void HitTime()
    {
        if(ball == null && BallSpawnTriggered)
        {
            ball = SpawnBallPhase(phase);
            BallSpawnTriggered = false;
        }
        else
        {
            if(phase == 2)
                ball.Velocity = new Vector2(Random.Range(-3f, 3f), Random.Range(-3f, -4f));
            else if(phase == 3)
                ball.Velocity = (pb.transform.position - transform.position).normalized * 5;
        }
    }
}
