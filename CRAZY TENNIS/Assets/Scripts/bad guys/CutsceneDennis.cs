using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// An 'enemy' who delivers the highly-anticipated plot twist cutscene
/// </summary>
public class CutsceneDennis : BadThing
{
    /// <summary> The component that make Crazy Dennis go whoosh </summary>
    private Animator anim;
    /// <summary> The dialogue box that tells you some straight facts </summary>
    private GameObject dialogue;
    /// <summary> Whether the dialogue box is currently doin its thing </summary>
    private bool talking = false;

    // Start is called before the first frame update
    new void Start()
    {
        base.Start();
        anim = GetComponent<Animator>();

        // Get the dialogue
        dialogue = transform.GetChild(0).gameObject;

        // Disable player movement
        GameObject.FindGameObjectWithTag("Player").GetComponent<Move2D>().enabled = false;
        // (and swinging)
        GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerBehaviour>().enabled = false;

    }

    // Update is called once per frame
    void Update()
    {
        if (dialogue == null && talking)
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

        /*
        As mentioned in the onenote, try giving DialogueBehaviour a public 
        field that lets it communicate with other entities (either a list of 
        actors or just a reference to the entity that created it). Also, maybe 
        make an interface for things that the dialogue interacts with, to 
        facilitate making a dialogue element for interacting with said things. 
        Maybe.
        dialogue.GetComponent<DialogueBehaviour>().
        */
    }

    /// <summary>
    /// 🔫
    /// </summary>
    public void gun()
    {
        anim.SetTrigger("Gun");
    }
    
    /// <summary>
    /// He'll be back...
    /// </summary>
    private void OnDestroy() {
        // Lift the restrictions we put on the player        
        GameObject.FindGameObjectWithTag("Player").GetComponent<Move2D>().enabled = true;
        GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerBehaviour>().enabled = true;
    }
}
