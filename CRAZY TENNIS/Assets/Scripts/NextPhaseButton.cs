// Code for an on-screen button that automatically advances to the current enemy's next phase.
// Used for ease of debugging specific enemeies

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NextPhaseButton : MonoBehaviour
{
    // ButtonPress is called whenever the button is pressed
    public void ButtonPress()
    {
        // One-liner time baybee! It's necessary to load the enemy each time, in order to account for NextEnemy calls
        GameObject.FindGameObjectWithTag("Enemy").GetComponent<BadThing>().NextPhase();
    }
}
