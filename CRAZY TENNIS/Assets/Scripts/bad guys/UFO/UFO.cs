// A UFO that provides support for Huma N.
// It's capable of moving around (mostly copied from the surfer dude script), 
// swinging its little racket to spawn a ball, and continuously spinning its 
// racket

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UFO : BadThing
{
    // Constants
    /// <summary> The inclusive upper bound of points in a path </summary>
    private static readonly int MAX_PATH_LENGTH = 4;
    /// <summary> Bounds for the points in the path </summary>
    private static readonly float MIN_X = -5.23F, MIN_Y = -3.73F, MAX_X = 4.57F, MAX_Y = 3.8F;
    /// <summary> Constant used in calculating new t values </summary>
    private static readonly float T_INCREASE = 0.125f;

    // Instance variables
    /// <summary> A number (from 0 to 1) that helps decide which type of ball 
    /// to spawn. 
    /// When a ball is spawned, a random number from 0 to 1 is generated; if 
    /// the random number is less than ballProbability, the ball will be 
    /// hittable, otherwise, it'll be unhittable. </summary>
    public static float BallProbability = 1;
    /// <summary> The bad thing who created this UFO </summary>
    public HumaN Commander { private get; set; }
    /// <summary> The ball that this object spawns </summary>
    public Ball Ball { get; private set; } = null;
    /// <summary> In later phases, we use this to rapidly fire balls </summary>
    private List<Ball> balls = new List<Ball>();
    /// <summary> The script belonging to the player, so we can throw balls at it </summary>
    private PlayerBehaviour pb;
    /// <summary> The animator that belongs to the ship (which is a child of 
    /// this script's gameobject) </summary>
    private Animator shipAnim;
    /// <summary> A set of points that the UFO goes through before spawning a 
    /// ball </summary>
    private List<Vector2> path = null;
    /// <summary> Index in path of the point the UFO is moving towards </summary>
    private int target;
    /// <summary> Starting point of motion </summary>
    private Vector2 from;
    /// <summary> The point indexed by target </summary>
    private Vector2 to;
    /// <summary> Progress (from 0 to 1) from 'from' to 'to' (thank you SurferDude.cs) </summary>
    private float t;

    // Start is called before the first frame update
    new void Start()
    {
        // Start being a bad thing
        base.Start();
        maxhits = 1;
        Physics2D.IgnoreCollision(GetComponent<Collider2D>(), GameObject.FindGameObjectWithTag("Enemy").GetComponent<Collider2D>());

        // Get the ship's animator and the player's, uh, behaviour
        shipAnim = GetComponentInChildren<Animator>();
        pb = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerBehaviour>();
        pb.PlayerHurt += ResetUFO;
        
        // Generate the first path
        GeneratePath();
        t = 0;
        from = transform.position;
        to = path[0];
    }

    // Update is called once per frame
    void Update()
    {
        // Heckin' garbage-collect the ball
		if(Ball != null && Ball.OutsideCourt)
		{
            // 🦀
			Destroy(Ball.gameObject);
			Ball = null;

            // But wait, there's more! If the ball is gone, that also means 
            // it's time for a new path.
            GeneratePath();
		}

        // Garbage collection part 2: This time it's a loop
        for (int i = balls.Count - 1; i >= 0; i--)
        {
            if (balls[i] != null && balls[i].OutsideCourt)
            {
                Destroy(balls[i].gameObject);
                balls[i] = null;
            }
            // Making this condition separate allows us to garbage-collect 
            // balls that were destroyed as as a result of hitting the enemy
            if (balls[i] == null)
            {
                balls.RemoveAt(i);
            }
        }
    }

    // This function is called every fixed framerate frame, if the MonoBehaviour is enabled. (Unity Code Snippets)
    private void FixedUpdate() {
        // Move to the next spot in the path.
        // In addition to checking if a path exists, only move if the ball is 
        // null. That way, it'll stay still for a little while after swinging
        if (path != null && Ball == null)
        {
            // Set new t value
            t += T_INCREASE / Vector2.Distance(to, from);
            
            // React to completed lerp
            if (t >= 1)
            {
                Vector3.Lerp(from, to, 1);

                // Start to move to the next point in the path, if one exists. 
                if (++target < path.Count)
                {
                    t = 0;
                    from = to;
                    to = path[target];
                }
                // Otherwise, stop moving and serve
                else
                {
                    path = null;
                    shipAnim.SetTrigger("Serve");
                }
            }
            // Or just continue lerping, I dunno
            else
            {
                transform.position = Vector3.Lerp(from, to, t);
            }
        }
    }

    // Sent when an incoming collider makes contact with this object's collider (2D physics only). (Unity Code Snippets)
    private void OnCollisionEnter2D(Collision2D other) {
        // If it's another enemy's ball, just... ignore it and move on.

        // We out here ignoring collisions with everything except the ball
        if (other.gameObject != Ball)
        {
           Physics2D.IgnoreCollision(GetComponent<Collider2D>(), other.collider); 
        }
    }

    /// <summary>
    /// Resets the ball (and the path???crazy)
    /// </summary>
    public void ResetUFO()
    {
        Ball = null;

        foreach(Ball ball in balls)
        {
            Destroy(ball.gameObject);
        }
        balls.Clear();
        
        if (!shipAnim.GetBool("Spinning"))
        {
            GeneratePath();
        }
    }

    /// <summary>
    /// Randomly generates a path for the UFO to follow
    /// </summary>
    private void GeneratePath()
    {
        // Randomly decide the length of the path
        int pathLength = (int)(Random.value * MAX_PATH_LENGTH) + 1;

        // Make the path!
        path = new List<Vector2>(pathLength);
        for (int i = 0; i < pathLength; i++)
        {
            path.Add(new Vector2(Random.Range(MIN_X, MAX_X), Random.Range(MIN_Y, MAX_Y)));
        }

        t = 0;
        target = 0;
        from = transform.position;
        to = path[target];
    }

    
    /// <summary>
    /// Generates a one-step path to move to a specific position
    /// </summary>
    /// <param name="destination"> The position to move to </param>
    public void MoveTo(Vector2 destination)
    {
        path = new List<Vector2>() { destination };

        t = 0;
        target = 0;
        from = transform.position;
        to = path[target];
    }

    /// <summary>
    /// Shoots a ball at the player
    /// </summary>
    /// <returns> A ball aimed directly at the player </returns>
    private Ball Fire()
    {
        // Randomize the type of the ball
        System.Type ballType;
        if (Random.value < BallProbability)
        {
            ballType = typeof(GenericHittable);
        }
        else
        {
            ballType = typeof(GenericUnhittable);
        }

        Ball toReturn = SpawnBall(
            ballType,
            transform.position,
            (pb.CenterPos - new Vector2(transform.position.x, transform.position.y)).normalized * 4,
            Random.Range(6f, 10f)
        );

        toReturn.Parent = Commander;

        return toReturn;
    }

    /// <summary>
    /// Ball go swoosh hehe
    /// </summary>
    public void Hit()
    {
        shipAnim.ResetTrigger("Serve");

        // Depending on the state of the UFO, either fire a standalone ball or 
        // contribute to the onslaught of rapid-fire balls
        Ball newBall = Fire();
        if (shipAnim.GetBool("Spinning"))
        {
            balls.Add(newBall);
        }
        else
        {
            Ball = newBall;
        }

        // Ignore collision between the commander and the ball
        Physics2D.IgnoreCollision(newBall.GetComponent<Collider2D>(), Commander.GetComponent<Collider2D>(), true);
    }

    /// <summary>
    /// Makes the UFO spin its racket around, which makes it hurt the player on 
    /// contact
    /// </summary>
    public void Spin()
    {
        /* Since HumaN.NextPhase instantiates a UFO prefab and then calls this
        method, we can't guarantee that shipAnim exists when this method is 
        called. Thus:
        */
        if (shipAnim == null)
        {
            GetComponentInChildren<Animator>().SetBool("Spinning", true);
        }
        else
        {
            shipAnim.SetBool("Spinning", true);
        }

        // 🦀🦀🦀
        path = null;
        if (Ball != null)
        {
            Destroy(Ball.gameObject);
            Ball = null;
        }
    }

    // This function is called when the MonoBehaviour will be destroyed. (Unity Code Snippets)
    private void OnDestroy() {
        pb.PlayerHurt -= ResetUFO;
    }
}
