using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Displays a helpful hint [citation needed] for the player on the pause screen
/// </summary>
public class TipText : MonoBehaviour
{
    /// <summary> The text for the tip. It needs to be public because it's 
    /// manipulated in OnEnable, which is miraculously called before 
    /// Start 
    /// Imagine having a constructor lol </summary>
    public Text Tip;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    /// <summary>
    /// Called when the game is paused
    /// </summary>
    private void OnEnable() {
/* TODO: Find out how the character-specific tip selection 
should work. Should there be a centralized tip database (less stuff to keep 
track of while making the tip display), or should each enemy store their own 
tips (less stuff to keep track of while making characters)?
I'm leaning towards the second option. It's more extensible.
*/

        Tip.text = "Oppa gangnam style";
    }
}
