using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SurferDude : BadThing
{
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
	private Ball _ball;
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
    private Vector3 prev;

    // Start is called before the first frame update
    new void Start()
    {
        base.Start();
        anim = GetComponent<Animator>();
        phase = 0;
        NextPhase();
        pb = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerBehaviour>();
        pb.Breakout = false;
		pb.PlayerHurt += PlayerGotOuchSad;
        Physics2D.IgnoreCollision(GameObject.FindGameObjectWithTag("bounds false").GetComponent<Collider2D>(), this.GetComponent<Collider2D>(), true);
		anim.SetBool("NeedBall", true);
	}

	void Update()
	{
		if(ball != null && ball.OutsideCourt())
		{
			Destroy(ball.gameObject);
			ball = null;
		}
	}

	public override void Ouch(Ball ball)
	{
		base.Ouch(ball);
		this.ball = null;
	}

	// Update is called once per frame
	void FixedUpdate()
    {
		if(!anim.GetCurrentAnimatorStateInfo(0).IsName("swing"))
		{
			prev = transform.position;
			t += 0.05f / Vector2.Distance(to, from);
			float T = (float)(1f - System.Math.Cos(t * System.Math.PI))/2f;
			if(t >= 1f)
			{
				if (anim.GetBool("NeedBall"))
					anim.SetTrigger("swing");
			    t = 1f;
			    transform.position = Vector3.Lerp(from, to, T);
			    t = 0;
			    from = to;
			    target = (target + 1) % currPath.Count;
			    to = currPath[target];
			}
			else
			    transform.position = Vector3.Lerp(from, to, T);
			anim.SetFloat("xVel", transform.position.x - prev.x);
		}
    }

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
            SpawnNextEnemy("redCharacter");
        }
        target = 0;
		maxhits = 3;
        from = transform.position;
        to = currPath[target];
    }

	public void PlayerGotOuchSad()
	{
		ball = null;
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
		ball = SpawnTheBall();
		anim.SetBool("NeedBall", false);
    }

	void OnDestroy()
	{
		pb.PlayerHurt -= PlayerGotOuchSad;
	}
}
