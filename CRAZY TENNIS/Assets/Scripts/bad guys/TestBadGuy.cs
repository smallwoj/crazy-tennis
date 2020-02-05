using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using Unity.Jobs;
using Unity.Collections;

public class TestBadGuy : BadThing
{
    private Ball[] balls;
    private int phase = 0;
    private PlayerBehaviour pb;
    void Start()
    {
        balls = new Ball[25];
        pb = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerBehaviour>();
        pb.Breakout= false;
        GameObject bounds = GameObject.Find("bounds false");
        if(bounds != null)
            bounds.SetActive(false);
        NextPhase();
    }

    void Update()
    {
        for(int i = 0; i < balls.Length; i++)
        {
            if(balls[i] != null && balls[i].OutsideCourt())
            {
                Destroy(balls[i].gameObject);
                balls[i] = null;
            }
            if(balls[i] == null)
            {
                System.Type ballType;
                if(Random.Range(0, 1f) < 0.1f)
                    ballType = typeof(GenericUnhittable);
                else
                    ballType = typeof(GenericHittable);
                Vector2 v = new Vector2(Random.Range(-1f, 1f), Random.Range(-1f, 1f));
                v *= 3;
                if(v.magnitude < 0.1)
                    v*=3;

                balls[i] = (Ball)SpawnBall(ballType, transform.position+new Vector3(0.619f, 0.207f, 0), v, 3f);
            }
        }
        if(Input.GetKeyUp("tab"))
        {
            SpawnNextEnemy("redCharacter");
        }
    }


    public override void NextPhase()
    {
        if(phase == 3)
            SpawnNextEnemy("redCharacter");
        else
        {
            phase++;
            hits = 5*phase;
            DestroyAllBalls();
            balls = new Ball[balls.Length*2];
        }
    }
}
