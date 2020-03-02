using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SurferDude : BadThing
{
    private Ball ball;
    private int phase;
    private PlayerBehaviour pb;
    private Animator anim;
    public List<Vector2> path1;
    public List<Vector2> path2;
    public List<Vector2> path3;
    private List<Vector2> currPath;
    private int target;
    private Vector2 to;
    private Vector2 from;
    private float t;

    // Start is called before the first frame update
    new void Start()
    {
        base.Start();
        anim = GetComponent<Animator>();
        phase = 0;
        NextPhase();
        pb = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerBehaviour>();
        pb.Breakout = false;
        Physics2D.IgnoreCollision(GameObject.FindGameObjectWithTag("bounds false").GetComponent<Collider2D>(), this.GetComponent<Collider2D>(), true);
    }

    // Update is called once per frame
    void Update()
    {
        t += 0.005f;
        if(t >= 1f)
        {
            t = 1f;
            transform.position = Vector3.Lerp(from, to, t);
            t = 0;
            from = to;
            target = (target + 1) % currPath.Count;
            to = currPath[target];
        }
        else
            transform.position = Vector3.Lerp(from, to, t);
    }

    public override void NextPhase()
    {
        phase++;
        if(phase == 1)
        {
            currPath = path1;
        }
        target = 0;
        from = transform.position;
        to = currPath[target];
    }

    private Ball SpawnTheBall()
    {
        Ball b = SpawnBall(
            typeof(GenericHittable),
            transform.position + new Vector3(0, 0, 0),
            (pb.transform.position - (transform.position + new Vector3(-0.7f, -0.2f, 0))).normalized * 4,
            Random.Range(6f, 10f)
        );
        Physics2D.IgnoreCollision(b.GetComponent<Collider2D>(), this.GetComponent<Collider2D>(), true);
        return b;
    }

    public void Hit()
    {
        // @TODO set animator trigger
    }
}
