/*
 * Joshua Smallwood
 * May 22, 2019 (idk when i started this)
 * Project Tribute
 * 
 * This script is used as like a door to start a scene when a player enters the trigger.
 */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ChangeScenes : MonoBehaviour
{
    public string sceneName;
    public float xPosition;
    public float yPosition;

    void OnTriggerEnter2D(Collider2D other)
    {
        if(other.CompareTag("Player"))
        {
			//activate the scene loader to load the given level
			SceneLoader.instance.LoadLevel(sceneName, new Vector3(xPosition, yPosition, 0));
		}
    }
}
