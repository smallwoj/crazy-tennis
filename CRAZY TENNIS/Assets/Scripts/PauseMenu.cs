using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Pauses the game when the player presses a specific button
/// </summary>
public class PauseMenu : MonoBehaviour
{
    /// <summary> Whether the game is currently paused </summary>
    private bool paused = false;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        // Pause and unpause when the cancel button (right now, Esc) is pressed
        if (Input.GetButtonDown("Cancel"))
        {
            if (paused)
            {
                Time.timeScale = 0;
            }
            else
            {
                Time.timeScale = 1;
            }
            paused = !paused;
        }
    }
}
