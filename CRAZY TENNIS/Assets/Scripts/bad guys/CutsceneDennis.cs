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
    }

    // Update is called once per frame
    void Update()
    {
        if (dialogue == null && talking)
        {
            talking = false;
            anim.SetTrigger("Gun");
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
        //anim.SetTrigger("Gun");
    }
}
