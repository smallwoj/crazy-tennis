// Reacts to an enemy being defeated and prompts the player to move on to the next enemy

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class EnemyTransitionControl : MonoBehaviour
{
    /// <summary> The object containing the enemy transition canvas </summary>
    GameObject transition;
    /// <summary> The current enemy's script </summary>
    BadThing currentEnemy;
    /// <summary> The enemy to transition to </summary>
    string nextEnemy;
    /// <summary> If we are waiting before transitioning </summary>
    bool waiting;

    // Start is called before the first frame update
    void Start()
    {
        transition = transform.GetChild(0).gameObject;
    }

    // Update is called once per frame
    void Update()
    {
        if (transition.activeSelf)
        {
            // Spawn the next enemy when the player presses the button
            if(Input.GetButtonDown("Submit") && !waiting)
            {
                FinishTransition();
                transition.SetActive(false);
            }
        }
    }

    /// <summary>
    /// Starts the transition to a new enemy
    /// </summary>
    /// <param name="currentEnemy"> The script of the enemy who was just defeated </param>
    /// <param name="nextEnemy"> The name of the next enemy's prefab </param>
    public void StartTransition(BadThing currentEnemy, string nextEnemy)
    {
        transition.SetActive(true);
        transition.transform.GetChild(0).gameObject.SetActive(false);
        this.currentEnemy = currentEnemy;
        this.nextEnemy = nextEnemy;
        StartCoroutine("WaitToContinue");
    }

    /// <summary>
    /// Sets the waiting and active stuff yeah
    /// </summary>
    /// <returns> It sure does </returns>
    private IEnumerator WaitToContinue()
    {
        waiting = true;
        yield return new WaitForSeconds(2);
        
        transition.transform.GetChild(0).gameObject.SetActive(true);
        waiting = false;
    }

    /// <summary>
    /// Actually spawns the next enemy (I still miss the autocommenter)
    /// </summary>
    private void FinishTransition()
    {        
        // TODO: PUT NEW MUSIC PLAY HERE
        currentEnemy.SpawnNextEnemy(nextEnemy);
    }
}
