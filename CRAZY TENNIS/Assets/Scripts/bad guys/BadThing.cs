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

    /// <summary>
    /// this where doot
    /// </summary>
    public AudioClip music;
    
    /// <summary>
    /// Plays cool sounds
    /// </summary>
    internal AudioSource audioSource;
    
    /// <summary>
    /// Sound effects for actions that all bad things do.  
    /// Any inheriting class can override these by setting them, or silence 
    /// them by setting them to null.
    /// 
    /// Note that ouch and nextPhase are played by this class, whereas hitBall 
    /// and dead need to be played in a derived class.
    /// </summary>
    internal AudioClip hitBall, ouch, nextPhase, dead;

    // Start isn't called before the first frame update this time...
    // nvm
    protected void Start()
    {
        HealthCircle = GetComponent<LineRenderer>();
        FindObjectOfType<PlayerBehaviour>().PlayerGameOver += SpawnRecoveryEnemy;
        audioSource = GetComponent<AudioSource>();

        // Load default sound effects
        hitBall = Resources.Load<AudioClip>("Audio/Sound Effects/BadThing defaults/Axe swing");
        ouch = Resources.Load<AudioClip>("Audio/Sound Effects/BadThing defaults/Realistic punch");
        nextPhase = Resources.Load<AudioClip>("Audio/Sound Effects/BadThing defaults/Jab");
        dead = Resources.Load<AudioClip>("Audio/Sound Effects/BadThing defaults/Uppercut");
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
    /// Brings up a prompt to make the player press space to transition to the next enemy, 
    /// denoted by the name of a prefab in Assets/Prefabs/Enemies
    /// </summary>
    /// <param name="nextEnemy">prefab of the next enemy</param>
    public void TransitionToNextEnemy(string nextEnemy)
    {
        DestroyAllBalls();
        // (note that this will eventually call SpawnNextEnemy)
        GameObject.FindObjectOfType<MusicManager>().Stop();
        GameObject.FindGameObjectWithTag("Enemy transition").GetComponent<EnemyTransitionControl>().StartTransition(this, nextEnemy);

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
    }

    /// <summary>
    /// This method spawns funny dog.
    /// </summary>
    public void SpawnRecoveryEnemy()
    {
        bool transitionActive = FindObjectOfType<EnemyTransitionControl>().transition.activeSelf;
        bool dennisDie = false;
        if (FindObjectOfType<CrazyDennis>())
        {
            dennisDie = FindObjectOfType<CrazyDennis>().anim.GetBool("Dead");
        }
        if (!transitionActive && !dennisDie)
        {
            //TODO: make this be a lookup table for when we have different recovery enemies and not just Dog
            BadThing enemy = SpawnNextEnemy("dogy");
            if(enemy is Dog dog) //that a funny line
            {
                dog.nextEnemy = this.PrefabString();
            }
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
        // Play the sound effect
        // (putting this before the NextPhase call, which also plays a sound 
        // effect, makes it so the nextPhase sound effect overrides the ouch 
        // sound effect if the player just cleared a phase)
        audioSource.clip = ouch;
        audioSource.Play();

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
        // Also play the sound effect
        audioSource.clip = nextPhase;
        audioSource.Play();
        // Any derived class of BadThing should probably refine this method with more functionality, such as going to the next phase
    }

    /// <summary>
    /// String representing the enemies prefab
    /// </summary>
    /// <returns>See: summary</returns>
    public abstract string PrefabString();

    protected void OnDestroy()
    {
        PlayerBehaviour pb = FindObjectOfType<PlayerBehaviour>();
        if(pb != null)
            FindObjectOfType<PlayerBehaviour>().PlayerGameOver -= SpawnRecoveryEnemy;
    }
}
