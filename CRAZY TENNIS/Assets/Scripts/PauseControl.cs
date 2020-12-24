using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Pauses the game when the player presses a specific button
/// </summary>
public class PauseControl : MonoBehaviour
{
    /// <summary> Whether the game is currently paused </summary>
    public static bool Paused { get; private set; } = false;

    /// <summary> The pause menu that this script enables </summary>
    private GameObject menu;

    /// <summary> The pause/unpause sound </summary>
    private AudioClip sound;

    /// <summary> Plays some cool sound </summary>
    private AudioSource player;

    // Start is called before the first frame update
    void Start()
    {
        // Menu! Where are you???
        menu = transform.GetChild(0).gameObject;
        // pLAYER! wHERE ARE YOU???
        player = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        // Pause and unpause when the cancel button (right now, Esc) is pressed
        if (Input.GetButtonDown("Cancel"))
        {
            Paused = !Paused;
            menu.SetActive(Paused);
            // If this were C(++) I'd be able to say Time.timeScale = !paused 😤
            if (Paused)
            {
                Time.timeScale = 0;
                sound = Resources.Load<AudioClip>("Audio/Sound Effects/UI/pause");
            }
            else
            {
                Time.timeScale = 1;
                sound = Resources.Load<AudioClip>("Audio/Sound Effects/UI/unpause");
            }
            player.clip = sound;
            player.Play();

            // This block is part of the more complicated pausing algorithm, 
            // ranted about below
            /*
            if (!pausing)
            {
                pausing = true;
            }
            */
        }

        /* More complicated pausing algorithm that visibly slows down time 
        instead of instantly stopping gameplay. 
        It's commented out because if we go with this, I'd want it to slow down 
        the music and sound effects too, but it's not possible to test that 
        kinda thing right now, without music nor sound effects :P

        All variables are private instance variables, with timeAcceleration 
        having default value -0.025 and all the bools having default value false
        */
        /*
        if (pausing)
        {
            // Get the new time scale
            newTimeScale = Time.timeScale + timeAcceleration;

            // Once time is either stopped or returned to normal, reset 
            // variables
            if (newTimeScale < 0 || newTimeScale > 1)
            {
                pausing = !pausing;
                timeAcceleration = -timeAcceleration;

                if (newTimeScale < 0)
                {
                    newTimeScale = 0;
                }
                else    // newTimeScale > 1
                {
                    newTimeScale = 1;
                }
            }

            // Gradually slow down time
            Time.timeScale = newTimeScale;
        }
        */
    }
}
