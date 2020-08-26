using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Displays a helpful hint [citation needed] for the player on the pause screen
/// </summary>
public class PauseMenuTip : MonoBehaviour
{
    /// <summary> The text for the tip. It needs to be public because it's 
    /// manipulated in OnEnable, which is miraculously called before 
    /// Start 
    /// Imagine having a constructor lol </summary>
    public Text Tip;

    /// <summary> Tips that can appear for any enemy </summary>
    public TipList genericTipList;

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
        // Get the enemy's tips
        TipList enemyTipList = 
            GameObject.FindGameObjectWithTag("Enemy").GetComponent<BadThing>()
            .TipList;

        // Choose a tip randomly from the generic tips and the enemy's tip list
        int numTips = genericTipList.Tips.Count + enemyTipList.Tips.Count;
        int tipNum = (int)(Random.value * numTips);
        // This selection algorithm sorta acts as if the enemy-specific tips 
        // were all appended to the end of the generic tip list
        if (tipNum < genericTipList.Tips.Count)
        {
            Tip.text = genericTipList.Tips[tipNum];
        }
        else
        {
            Tip.text = enemyTipList.Tips[tipNum - genericTipList.Tips.Count];
        }
    }
}
