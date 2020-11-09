// A single racket used by the spider

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpiderRacket : MonoBehaviour
{
    // Constants
    /// <summary> The displacement from this object's position and the head 
    /// (the part that hits the ball) of the racket </summary>
    private static readonly Vector3 HEAD_OFFSET = new Vector3(0.023f, -0.748f, 0);

    // Instance variables
    /// <summary> The spider holding this racket </summary>
    public Spider Spider { private get; set; }
    /// <summary> Shortcut to the animator </summary>
    public Animator Anim { get; private set; }
    /// <summary> The type of ball spawned by this racket </summary>
    public System.Type BallType = null;
    /// <summary> The ball spawned by this racket </summary>
    private Ball ball;

    // Start is called before the first frame update
    void Start()
    {
        // Get the animator
        Anim = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    /// <summary>
    /// Despawns the ball, but only if it's offscreen
    /// </summary>
    /// <returns> Whether the ball is null at the end of the method </returns>
    public bool DespawnBallIfOffscreen()
    {
        bool toReturn;  // Whether the ball is null at the end of the method

        if (ball == null)
        {
            // In this case, the ball is kinda despawned by default
            toReturn = true;
        }
        else
        {
            if (ball.OutsideCourt)
            {
                // The only good path! Despawn the ball and report a success
                Destroy(ball.gameObject);
                ball = null;
                toReturn = true;
            }
            else
            {
                // If it's non-null and inside the court, it's sad time
                toReturn =  false;
            }
        }
        return toReturn;
    }

    /// <summary>
    /// Throws a ball at the player. 
    /// Called at the end of the swinging animation
    /// </summary>
    public void SpawnBall()
    {
        // Only swing if the ball has an established type
        // (this condition allows the swinging animation to be reused as a 
        // reveal animation without spawning a ball)
        if (BallType != null)
        {
            ball = Spider.SpawnBall(
                BallType,
                transform.position + 
                    Quaternion.AngleAxis(transform.eulerAngles.z, transform.forward) * HEAD_OFFSET,
                new Vector2(0, -5),
                Random.Range(6f, 10f)
            ); 

            BallType = null;

            FindObjectOfType<CameraBehaviour>().Impact(0.2f, Vector2.down);
        }

        // Also, reset the trigger so there aren't redundant swings
        Anim.ResetTrigger("Swing time");
    }
}
