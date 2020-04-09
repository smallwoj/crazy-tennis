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
    private Rigidbody2D rb;

    // Start is called before the first frame update
    new void Start()
    {
        base.Start();
        anim = GetComponent<Animator>();
        phase = 0;
        pb = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerBehaviour>();
        pb.Breakout = false;
		pb.PlayerHurt += PlayerGotOuchSad;
        Physics2D.IgnoreCollision(GameObject.Find("Net").GetComponent<Collider2D>(), this.GetComponent<Collider2D>(), true);
        Physics2D.IgnoreCollision(GameObject.Find("Wacky collision stuff").transform.Find("Court bounds").GetComponent<Collider2D>(), this.GetComponent<Collider2D>(), true);
		anim.SetBool("NeedBall", true);
        rb = GetComponent<Rigidbody2D>();
        NextPhase();
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
        anim.SetTrigger("ouch");
		base.Ouch(ball);
		this.ball = null;
	}

	// Update is called once per frame
	void FixedUpdate()
    {
		if(!anim.GetCurrentAnimatorStateInfo(0).IsName("swing") && !anim.GetCurrentAnimatorStateInfo(0).IsName("Ouch!"))
		{
			prev = rb.position;
			t += 0.05f / Vector2.Distance(to, from);
			float T = (float)(1f - System.Math.Cos(t * System.Math.PI))/2f;
			if(t >= 1f)
			{
				if (anim.GetBool("NeedBall"))
					anim.SetTrigger("swing");
			    t = 1f;
			    rb.position = Vector3.Lerp(from, to, T);
			    t = 0;
			    from = to;
			    target = (target + 1) % currPath.Count;
			    to = currPath[target];
			}
			else
			    rb.position = Vector3.Lerp(from, to, T);
			anim.SetFloat("xVel", rb.position.x - prev.x);
		}
        else
        	anim.SetFloat("xVel", 0f);
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
        from = rb.position;
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
            rb.position + new Vector2(0, 0),
            (pb.GetComponent<Rigidbody2D>().position - (rb.position + new Vector2(-0.7f, -0.2f))).normalized * 4,
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
