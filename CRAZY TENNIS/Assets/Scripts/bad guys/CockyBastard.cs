﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CockyBastard : BadThing
{
    private Ball ball;
    private int phase;
    private int rallyCount;
    // Start is called before the first frame update
    void Start()
    {
        phase = 0;
        NextPhase();
        PlayerBehaviour pb = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerBehaviour>();
        pb.Breakout = true;
    }

    // Update is called once per frame
    void Update()
    {
        if(ball != null && ball.OutsideCourt())
        {
            Destroy(ball);
            ball = null;
        }
        if(ball == null)
        {
            ball = SpawnBallPhase(phase);
            rallyCount = phase - 1;
        }
    }

    public override void NextPhase()
    {
        DestroyAllBalls();
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
                transform.position, 
                new Vector2(0, -3), 
                Random.Range(6f, 15f)
            );
        }
        else if(phase == 2)
        {
            return SpawnBall(
                typeof(GenericHittable), 
                transform.position, 
                new Vector2(Random.Range(-4f, 4f), Random.Range(-3f, -4f)),
                Random.Range(6f, 15f)
            );
        }
        else if(phase == 3)
        {
            return SpawnBall(
                typeof(GenericHittable), 
                transform.position, 
                new Vector2(0, -5), 
                Random.Range(6f, 15f)
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
            rallyCount--;
            print("rally");
            ball.hit = false;
            Physics2D.IgnoreCollision(ball.GetComponent<Collider2D>(), this.GetComponent<Collider2D>(), true);
            if(phase == 2)
                ball.Velocity = new Vector2(Random.Range(-4f, 4f), Random.Range(-3f, -4f));
        }
    }

}
