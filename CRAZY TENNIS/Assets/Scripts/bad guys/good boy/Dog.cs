using System.Collections;
using System.Collections.Generic;
using System.Linq;
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
    // Start is called before the first frame update
    new void Start()
    {
        base.Start();
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        renderer = GetComponent<SpriteRenderer>();
        phase = 0;
        NextPhase();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        prev = rb.position;
        if (t < 1)
        {
            inc = 0.05f / Vector2.Distance(to, from);
            t += inc;
            if(t >= 1)
                t = 1f;
            float T = (float)(1f - System.Math.Cos(t * System.Math.PI))/2f;
            rb.position = Vector2.Lerp(from, to, T);
        }
        else
        {

        }
        DetermineDirection();
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

    public void Land()
    {
        Physics2D.IgnoreCollision(GetComponent<Collider2D>(), JumpOver, false);
        JumpOver = null;
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
                to = new Vector2(-0.16f, -1.95f);
                t = 0;
            break;
        }
    }
}
