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
            if(Input.GetButtonDown("Submit"))
            {
                ActuallySpawnNextEnemy();
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
        this.currentEnemy = currentEnemy;
        this.nextEnemy = nextEnemy;
    }

    /// <summary>
    /// Actually spawns the next enemy (I still miss the autocommenter)
    /// </summary>
    private BadThing ActuallySpawnNextEnemy()
    {        
        if(currentEnemy != null) //??????
            Destroy(currentEnemy.gameObject);
        if(GameObject.FindGameObjectsWithTag("Enemy").Length == 1)
        {
            GameObject enemy = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Prefabs/Enemies/"+nextEnemy+".prefab");
            enemy = PrefabUtility.InstantiatePrefab(enemy) as GameObject;
            return enemy.GetComponent<BadThing>();
        }
        return null;
    }
}
