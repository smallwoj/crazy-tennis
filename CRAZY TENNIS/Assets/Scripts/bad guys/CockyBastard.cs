using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CockyBastard : BadThing
{
    /// <summary>
    /// da ball
    /// </summary>
    private Ball ball;

    /// <summary>
    /// Current phase
    /// 1 -> he serves it straight down
    /// 2 -> he serves it in a random direction
    /// 3 -> he serves it right at you
    /// 4 -> fuckin deadd
    /// </summary>
    private int phase;

    /// <summary>
    /// However many rallies he still needs before he takes a hit
    /// </summary>
    private int rallyCount;

    /// <summary>
    /// Shortcut to the player
    /// </summary>
    private PlayerBehaviour pb;

    /// <summary>
    /// Shortcut to the animator
    /// </summary>
    private Animator anim;
    /// <summary>
    /// How many degrees off the center the direction for the second phase is.
    /// </summary>
    private float angleOffset = 135;
    // Start is called before the first frame update
    new void Start()
    {
        // call the BadThing start method
        // you need this. if youre encountering null pointers when taking damage thats prolly it
        base.Start();
        anim = GetComponent<Animator>();
        phase = 0;
        NextPhase();
        pb = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerBehaviour>();
        pb.Breakout = false;
        // add rally function to player hurt event
        pb.PlayerHurt += Rally;
    }

    // Update is called once per frame
    void Update()
    {
        //respawn the ball if its outside the court
        if(ball != null && ball.OutsideCourt)
        {
            Destroy(ball.gameObject);
            ball = null;
            anim.SetTrigger("rally");
        }
    }

    /// <summary>
    /// Goes to the next phase, as described in the 'phase' summary
    /// </summary>
    public override void NextPhase()
    {
        DestroyAllBalls();
        ball = null;
        phase++;
        maxhits = 0;
        if(phase == 4)
        {
            SpawnNextEnemy("Surfer dude");
        }
        else
        {
            // Award points (the if is there to avoid awarding points when the guy spawns)
            if (phase > 1)
            {
                base.NextPhase();
            }
            anim.SetTrigger("rally");
        }
    }

    /// <summary>
    /// Spawns a ball based on the current phase
    /// </summary>
    /// <param name="phase">current phase</param>
    /// <returns>Ball component with the properties for the phase</returns>
    private Ball SpawnBallPhase(int phase)
    {
        if(phase == 1)
        {
            return SpawnBall(
                typeof(GenericHittable), 
                transform.position + new Vector3(-0.7f, -0.2f, 0),
                (pb.CenterPos - ((Vector2)transform.position + new Vector2(-0.7f, -0.2f))).normalized * 2,
                Random.Range(6f, 10f)
            );
        }
        else if(phase == 2)
        {
            return SpawnBall(
                typeof(GenericHittable), 
                transform.position + new Vector3(-0.7f, -0.2f, 0),
                Quaternion.AngleAxis(Random.Range(-angleOffset, angleOffset), Vector3.up) * (pb.CenterPos - ((Vector2)transform.position + new Vector2(-0.7f, -0.2f))).normalized * 4,
                Random.Range(6f, 10f)
            );
        }
        else if(phase == 3)
        {
            return SpawnBall(
                typeof(GenericHittable), 
                transform.position + new Vector3(-0.7f, -0.2f, 0),
                new Vector2(Random.Range(-3f, 3f), Random.Range(-3f, -4f)).normalized * 5,
                Random.Range(6f, 10f)
            );
        }
        else
            return null;
    }

    /// <summary>
    /// Overridden method that is fired when the enemy takes damage.
    /// If we still have some rallies to do, make the rally, else do the regular ouch.
    /// </summary>
    /// <param name="ball"></param>
    public override void Ouch(Ball ball)
    {
        if(rallyCount <= 0)
        {
            anim.SetTrigger("hurt");
            base.Ouch(ball);
        }    
        else
        {
            anim.SetTrigger("rally");
            rallyCount--;
            // the ball is not hit anymore
            ball.hit = false;
            // ignore collision between the enemy and the ball again
            Physics2D.IgnoreCollision(ball.GetComponent<Collider2D>(), this.GetComponent<Collider2D>(), true);
        }
    }

    /// <summary>
    /// This method is fired when the bastard is at a certain frame of his swing animation.
    /// It spawns a ball if there isnt one, or resets the velocity to go to places.
    /// </summary>
    public void HitTime()
    {
        if(ball == null)
        {
            if(maxhits == 0)
                maxhits = 3;
            ball = SpawnBallPhase(phase);
            FindObjectOfType<CameraBehaviour>().Impact(0.2f, Vector2.down);
            rallyCount = phase - 1;
        }
        else
        {
            FindObjectOfType<CameraBehaviour>().Impact(0.2f, Vector2.down);
            if(phase == 2)
                ball.Velocity = Quaternion.AngleAxis(Random.Range(-angleOffset, angleOffset), Vector3.up) * (pb.CenterPos - ((Vector2)transform.position + new Vector2(-0.7f, -0.2f))).normalized * 4;
            else if(phase == 3)
                ball.Velocity = new Vector2(Random.Range(-3f, 3f), Random.Range(-3f, -4f)).normalized * 5;
        }
    }

    /// <summary>
    /// Method to trigger the rally animation
    /// </summary>
    public void Rally()
    {
        anim.SetTrigger("rally");
    }

    /// <summary>
    /// aw fuc i missed tyhis
    /// 
    /// when the game object die remove the rally from the delegate palyer hurt ouch
    /// </summary>
    new void OnDestroy()
    {
        base.OnDestroy();
        // remove this
        pb.PlayerHurt -= Rally;
    }

    /// <summary>
    /// String representing the enemies prefab
    /// </summary>
    /// <returns>See: summary</returns>
    public override string PrefabString()
    {
        return "CockyBastard";
    }
}
