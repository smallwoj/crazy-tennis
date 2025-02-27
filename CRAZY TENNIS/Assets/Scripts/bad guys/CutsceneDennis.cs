using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// An 'enemy' who delivers the highly-anticipated plot twist cutscene
/// </summary>
public class CutsceneDennis : BadThing
{
    /// <summary> The displacement between the enemy's position and the bullet's initial position </summary>
    private static readonly Vector3 BULLET_OFFSET = new Vector3(0.28f, 0, 0);
    /// <summary> How fast the bullet travels </summary>
    private static readonly Vector2 BULLET_VELOCITY = new Vector2(0, -15);
    /// <summary> Game objects that we mess with in order to make the cutscene work as expected </summary>
    private static GameObject player, bottomBound;
    /// <summary> For messing with the player even more </summary>
    private static PlayerBehaviour pb;
    /// <summary> The component that makes Crazy Dennis go whoosh </summary>
    private Animator anim;
    /// <summary> The dialogue box that tells you some straight facts </summary>
    private GameObject dialogue;
    /// <summary> Whether the dialogue box is currently doin its thing </summary>
    private bool talking = false;
    /// <summary> If I remember correctly, we started development calling balls 
    /// bullets, so this bullet categorized as a ball brings us full circle </summary>
    private Ball bullet;
    /// <summary> The "BANG!" sound that plays when he shoots you </summary>
    private AudioClip gunshot;

    // Start is called before the first frame update
    new void Start()
    {
        base.Start();
        anim = GetComponent<Animator>();

        // Get the dialogue
        dialogue = transform.GetChild(0).gameObject;

        // Disable player movement and swinging
        player = GameObject.FindGameObjectWithTag("Player");
        if (player)
        {
            player.GetComponent<Move2D>().enabled = false;
            player.GetComponent<Rigidbody2D>().velocity = Vector2.zero;
            Animator anim = player.GetComponent<Animator>();
            anim.SetFloat("xVel", 0);
            anim.SetFloat("yVel", 0);
            pb = player.GetComponent<PlayerBehaviour>();
            player.transform.position = pb.defaultPosition;
            pb.enabled = false;
            // Also give them an extra life, since we're about so so meanly shoot them
            pb.lives++;
        }
        // (and collision with the bottom of the screen too!)
        bottomBound = GameObject.Find("Bottom court bound");
        if (bottomBound)
        {
            bottomBound.GetComponent<BoxCollider2D>().enabled = false;
        }
        FindObjectOfType<PlayerBehaviour>().DeathCoroutine = "CutsceneDennisDie";

        gunshot = Resources.Load<AudioClip>("Audio/Sound Effects/Cutscene Dennis/Shotgun");
    }

    // Update is called once per frame
    void Update()
    {
        if (!dialogue.activeSelf && talking)
        {
            talking = false;
            gun();
        }
    }

    /// <summary>
    /// Stops the walking animation. This should be called on frame 256ish of the animation
    /// </summary>
    public void stopWalking()
    {
        // An entire function for this? Yeah events kinda funny tho
        anim.SetTrigger("Stop walking");
    }

    /// <summary>
    /// Spawns a dialogue box
    /// </summary>
    public void beginDialogue()
    {
        // Spawn a dialogue box (TODOne)
        dialogue.SetActive(true);
        talking = true;
    }

    /// <summary>
    /// 🔫
    /// </summary>
    public void gun()
    {
        anim.SetTrigger("Gun");
    }

    /// <summary>
    /// 💥🔫
    /// </summary>
    public void fire()
    {
        // The bullet is actually a GenericUnhittable with a different image. 
        // Don't tell anyone!
        bullet = SpawnBall(typeof(GenericUnhittable), transform.position + BULLET_OFFSET, BULLET_VELOCITY, 0);
        bullet.gameObject.GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("Literal bullet");
        FindObjectOfType<CameraBehaviour>().Impact(0.1f, Vector2.down);
        audioSource.clip = gunshot;
        audioSource.Play();
    }

    /// <summary>
    /// Since the player has been shot, the battle is, by definition, over.
    /// </summary>
    public override void NextPhase()
    {
        // 🚮 Garbage 
        DestroyAllBalls();
        SpawnNextEnemy("Crazy Dennis");
    }

    /// <summary>
    /// He'll be back...
    /// </summary>
    private new void OnDestroy() 
    {
        base.OnDestroy();
        // Lift the restrictions we put on the player and the bounds
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player)
        {
            player.GetComponent<Move2D>().enabled = true;
            pb.enabled = true;
            player.transform.position = pb.defaultPosition;
            player.GetComponent<PlayerBehaviour>().DeathCoroutine = "WaitAndRespawn";
            player.GetComponent<Animator>().SetBool("Dead x_x", false);
        }
        if (bottomBound)
        {
            bottomBound.GetComponent<BoxCollider2D>().enabled = true;
        }
        
        // Set up for the final battle
        GameObject.Find("Net").SetActive(false);
        // GameObject.Find("Crowd").SetActive(false);
        GameObject.Find("Court cropped").GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("Blood court");
        GameObject.Find("Main Camera").GetComponent<Camera>().backgroundColor = new Color((float)106/255, 0, 0);
    }

    /// <summary>
    /// String representing the enemy's prefab
    /// </summary>
    /// <returns>See: summary</returns>
    public override string PrefabString()
    {
        return "Cutscene Dennis";
    }
}