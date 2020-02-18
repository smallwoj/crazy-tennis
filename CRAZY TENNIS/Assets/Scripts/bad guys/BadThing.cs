using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public abstract class BadThing : MonoBehaviour
{
    public float HealthRadius;
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
                    vertices.Add(new Vector3((float)System.Math.Cos(t+0.5*System.Math.PI)*HealthRadius, (float)System.Math.Sin(t+0.5*System.Math.PI)*HealthRadius, -1));
                }
                lines.positionCount = vertices.Count;
                lines.SetPositions(vertices.ToArray());
            }
        }
    }
    private int _hits = 0;
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
    private LineRenderer lines;

    // Start isn't called before the first frame update this time...
    // nvm
    protected void Start()
    {
        lines = GetComponent<LineRenderer>();
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
        comp.Velocity = initVel;
        comp.RotationalVelocity = initRotVel;
        comp.Parent = this;
        comp.transform.position = initPos;
        Physics2D.IgnoreCollision(ball.GetComponent<Collider2D>(), this.GetComponent<Collider2D>(), true);
        return comp;
    }

    /// <summary>
    /// Spawns the next enemy, denoted by the name of a prefab in Assets/Prefabs/Enemies
    /// </summary>
    /// <param name="nextEnemy">prefab of the next enemy</param>
    public void SpawnNextEnemy(string nextEnemy)
    {
        Destroy(this.gameObject);
        if(GameObject.FindGameObjectsWithTag("Enemy").Length == 1)
        {
            DestroyAllBalls();
            GameObject enemy = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Prefabs/Enemies/"+nextEnemy+".prefab");
            PrefabUtility.InstantiatePrefab(enemy);
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
            NextPhase();
    }

    public abstract void NextPhase();
}
