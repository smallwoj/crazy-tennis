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

    // Start is called before the first frame update
    new void Start()
    {
        base.Start();
        anim = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    /// <summary>
    /// Stops the walking animation and spawns a dialogue box
    /// </summary>
    public void beginDialogue()
    {
        // Stop the walking animation
        anim.SetTrigger("Stop walking");

        // Spawn a dialogue box (TODO)

    }
}
