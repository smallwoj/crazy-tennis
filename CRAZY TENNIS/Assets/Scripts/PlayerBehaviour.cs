using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerBehaviour : MonoBehaviour
{
    public int lives = 5;

    public int bombs = 5;

    public int score = 0;

    public Vector2 defaultPosition = new Vector2(-1.87f, -2.55f);

    private int defaultBombs;
    private Animator anim;
    private Collider2D swang;
    public bool Breakout = false;
    // Start is called before the first frame update
    void Start()
    {
        anim = GetComponent<Animator>();
        GetComponent<Rigidbody2D>().position = defaultPosition;
        defaultBombs = bombs;
        swang = GameObject.Find("Player/swang").GetComponent<PolygonCollider2D>();
        swang.enabled = false;
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetButtonDown("Submit"))
        {
            anim.ResetTrigger("space released");
            anim.SetTrigger("space pressed");
        }
        else if(Input.GetButtonUp("Submit"))
        {
            anim.ResetTrigger("space pressed");
            anim.SetTrigger("space released");
        }
    }

    /// <summary>
    /// Die
    /// </summary>
    public void Die()
    {
        lives--;
        if(lives <= 0)
        {
            print("uh oh i am dead X_X");
        }
        else
        {
            bombs = defaultBombs;
            GetComponent<Rigidbody2D>().position = defaultPosition;
            print("lives: "+lives);
        }
        BadThing.DestroyAllBalls();
    }

    public void ToggleSwang()
    {
        swang.enabled = !swang.enabled;
    }
}
