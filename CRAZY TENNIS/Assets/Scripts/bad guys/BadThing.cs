using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

/// <summary>
/// This is the base enemy class. All enemies draw from this.
/// </summary>
public abstract class BadThing : MonoBehaviour
{
    /// <summary> List of tips that appear on the pause menu for this enemy </summary>
    public TipList TipList;

    /// <summary>
    /// Radius in 'unity units' of the health circle
    /// </summary>
    public float HealthRadius;

    /// <summary>
    /// How many hits the enemy has left. Hit 0 and uh oh
    /// </summary>
    protected int hits
    {
        get
        {
            return _hits;
        }
        set
        {
            _hits = value;
            if(value != 0 && maxhits != 0)
            {
                List<Vector3> vertices = new List<Vector3>();
                double limit = ((double)hits/maxhits)*2*System.Math.PI;
                double inc = limit/100;
                for(double t = 0; t <= limit; t+=inc)
                {
                    vertices.Add(new Vector3((float)System.Math.Cos(t+0.5*System.Math.PI)*HealthRadius, (float)System.Math.Sin(t+0.5*System.Math.PI)*HealthRadius, 0f));
                }
                HealthCircle.positionCount = vertices.Count;
                HealthCircle.SetPositions(vertices.ToArray());
            }
            else
            {
                HealthCircle.positionCount = 0;
            }
        }
    }

    private int _hits = 0;
    /// <summary>
    /// Sets the maximum health (and also the current health)
    /// Used to calculate how much of the health ring to draw
    /// </summary>
    public int maxhits
    {
        get
        {
            return _maxhits;
        }
        set
        {
            _maxhits = value;
            hits = value;
        }
    }
    private int _maxhits = 0;

    /// <summary>
    /// LineRenderer that draws the ring of health around the enemy.
    /// </summary>
    private LineRenderer HealthCircle;

    // Start isn't called before the first frame update this time...
    // nvm
    protected void Start()
    {
        HealthCircle = GetComponent<LineRenderer>();
        FindObjectOfType<PlayerBehaviour>().PlayerGameOver += SpawnRecoveryEnemy;
    }

    /// <summary>
    /// Spawns a ball at the position with the given type.
    /// </summary>
    /// <param name="type">The type of the ball to spawn (use typeof([desired type]))</param>
    /// <param name="initPos">The initial position that the ball will start at.</param>
    /// <param name="initVel">The initial velocity of the ball</param>
    /// <param name="initRotVel">The initial rotational velocity of the ball (IN DEGREES)</param>
    /// <returns>The ball created</returns>
    public Ball SpawnBall(System.Type type, Vector2 initPos, Vector2 initVel, float initRotVel)
    {
        GameObject ball = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Prefabs/Balls/"+type.Name+".prefab");
        //instantiate the prefab
		ball = (GameObject)PrefabUtility.InstantiatePrefab(ball);
        Ball comp = (Ball)ball.AddComponent(type);
        comp.body = ball.GetComponent<Rigidbody2D>();
        comp.Velocity = initVel;
        comp.RotationalVelocity = initRotVel;
        comp.Parent = this;
        comp.transform.position = initPos;
        Physics2D.IgnoreCollision(ball.GetComponent<Collider2D>(), this.GetComponent<Collider2D>(), true);
        return comp;
    }

    /// <summary>
    /// Transitions to the next enemy, 
    /// denoted by the name of a prefab in Assets/Prefabs/Enemies
    /// </summary>
    /// <param name="nextEnemy">prefab of the next enemy</param>
    public BadThing SpawnNextEnemy(string nextEnemy)
    {
        //this is self documenting code
        if(this is Dog)
        {
            //it's a dog!
        }
        else
        {
            // Yay they did it
            GameObject.FindGameObjectWithTag("Score").GetComponent<ScoringSystem>().OpponentBeat();
        }

        DestroyAllBalls();
        enabled = false;

        if(this != null) //??????
            Destroy(this.gameObject);
        if(GameObject.FindGameObjectsWithTag("Enemy").Length == 1)
        {
            GameObject enemy = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Prefabs/Enemies/"+nextEnemy+".prefab");
            enemy = PrefabUtility.InstantiatePrefab(enemy) as GameObject;
            return enemy.GetComponent<BadThing>();
        }
        return null;
    }

    /// <summary>
    /// Brings up a prompt to make the layer press space to transition to the next enemy, 
    /// denoted by the name of a prefab in Assets/Prefabs/Enemies
    /// </summary>
    /// <param name="nextEnemy">prefab of the next enemy</param>
    public void TransitionToNextEnemy(string nextEnemy)
    {
        // (note that this will eventually call SpawnNextEnemy)
        GameObject.FindGameObjectWithTag("Enemy transition").GetComponent<EnemyTransitionControl>().StartTransition(this, nextEnemy);
    }

    /// <summary>
    /// This method spawns funny dog.
    /// </summary>
    public void SpawnRecoveryEnemy()
    {
        //TODO: make this be a lookup table for when we have different recovery enemies and not just Dog
        BadThing enemy = SpawnNextEnemy("dogy");
        if(enemy is Dog dog) //that a funny line
        {
            dog.nextEnemy = this.PrefabString();
        }
    }

    /// <summary>
    /// Deletes all current balls in the scene
    /// </summary>
    public static void DestroyAllBalls()
    {
        foreach(GameObject ball in GameObject.FindGameObjectsWithTag("Ball"))
        {
            Destroy(ball);
        }
    }

    /// <summary>
    /// It hurts so much
    /// </summary>
    /// <param name="ball">Thats the fucker who did it right there thats him</param>
    public virtual void Ouch(Ball ball)
    {
        Destroy(ball.gameObject);
        hits--;
        if(hits <= 0)
        {
            NextPhase();
            FindObjectOfType<CameraBehaviour>().ShakeScreen(0.6f);
        }
        else
            FindObjectOfType<CameraBehaviour>().ShakeScreen(0.4f);

        // Award some points to the player
        GameObject.FindGameObjectWithTag("Score").GetComponent<ScoringSystem>().OpponentHit();
    }

    /// <summary>
    /// This method will go to the next phase, whatever that may be
    /// </summary>
    public virtual void NextPhase()
    {
        // Give the player some epic points
        GameObject.FindGameObjectWithTag("Score").GetComponent<ScoringSystem>().PhaseClear();
        // Any derived class of BadThing should probably refine this method with more functionality, such as going to the next phase
    }

    /// <summary>
    /// String representing the enemies prefab
    /// </summary>
    /// <returns>See: summary</returns>
    public abstract string PrefabString();

    protected void OnDestroy()
    {
        FindObjectOfType<PlayerBehaviour>().PlayerGameOver -= SpawnRecoveryEnemy;
    }
}
