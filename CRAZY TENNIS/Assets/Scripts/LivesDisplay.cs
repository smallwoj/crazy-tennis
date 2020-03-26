using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LivesDisplay : MonoBehaviour
{
    private int LIVES_DISPLAYED = 3;    // The number of life icons to display before just showing the number of lives
    private int lives;          // How many lives the player has left
    public GameObject player;   // Reference to the player, so this class can read its lives count
    public Image[] lifeIcons;   // The three icons displayed when the player has fewer than LIVES_DISPLAYED lives
    public Text LivesText;      // The text displayed when the player has more than LIVES_DISPLAYED lives

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        lives = player.GetComponent<PlayerBehaviour>().lives;

        // Display up to three life icons, or the life count if the player has more
        if (lives <= 3) // if statement :)
        {
            // Dynamically set each icon's visibility
            for (int i = 0; i < LIVES_DISPLAYED; i++)
            {
                if (i >= lives && lifeIcons[i].enabled)
                {
                    lifeIcons[i].enabled = false;
                }
                else if (i < lives && !lifeIcons[i].enabled)
                {
                    print ("life " + i + " should be displayed");
                    lifeIcons[i].enabled = true;
                }
            }
            LivesText.text = "";
        }
        else
        {
            // Make only the first life icon visible
            for (int i = 1; i < LIVES_DISPLAYED; i++)
            {
                if (lifeIcons[i].enabled)
                {
                    lifeIcons[i].enabled = false;
                }
            }
            LivesText.text = "x" + lives;
        }
            print(lives);

    }
}
