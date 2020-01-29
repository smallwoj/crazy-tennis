/*
 * Joshua Smallwood
 * May 22, 2019
 * Project Tribute
 * 
 * This script is never actually used by any game objects! it acts as an always active static class to load levels but more nicely
 */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEditor;

public class SceneLoader : MonoBehaviour
{
	private Vector3? pos;	//it has a ? because Vector3 cant be null and the ? makes it so it can be null
							//it is also the initial position of the player in the new scene
	private static SceneLoader _instance;	//the instance of this so we can use the load level
	public static SceneLoader instance		//the instance of the scene loader used to access the load level method
	{
		get
		{
			if(_instance == null)
			{
				//make a dummy game object to hold the instance of the scene loader
				GameObject loader = new GameObject("[SceneLoader]");
				_instance = loader.AddComponent<SceneLoader>();
				//make it so it becomes god
				DontDestroyOnLoad(loader);
			}
			return _instance;
		}
	}

	private static GameObject _overlay;	//instance of the overlay used for the cool effect 
	private static GameObject overlay
	{
		get
		{	
			//if it doesn't exist create it
			if(_overlay == null)
			{
				//get the data from the prefab
				_overlay = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Prefabs/SceneTransition.prefab");
				//instantiate the prefab
				_overlay = (GameObject)PrefabUtility.InstantiatePrefab(_overlay);
				//make it so it becomes god
				DontDestroyOnLoad(_overlay);
			}
			return _overlay;
		}
	}

	/// <summary>
	/// adds the delegate method WhenLevelLoads when this script is enabled
	/// </summary>
	void OnEnable()
	{
		//it basically runs the given method when the scene is loaded
		SceneManager.sceneLoaded += WhenLevelLoads;
	}

	/// <summary>
	/// removes the delegate method WhenLevelLoads when this script is disabled
	/// </summary>
	void OnDisable()
	{
		SceneManager.sceneLoaded -= WhenLevelLoads;
	}

	/// <summary>
	/// Loads the scene with the given name and closes the scene
	/// </summary>
	/// <param name="sceneName">Name of the scene to load</param>
	private IEnumerator _LoadLevel(string sceneName)
	{
		overlay.GetComponentInChildren<Animator>().SetBool("open", false);
		yield return new WaitForSeconds(1.25f);
		SceneManager.LoadSceneAsync(sceneName);
	}

	/// <summary>
	/// Loads the scene with player starting position
	/// </summary>
	/// <param name="sceneName">Name of the scene to load</param>
	/// <param name="initPos">Initial position of the player</param>
	public void LoadLevel(string sceneName, Vector3? initPos)
	{
		StartCoroutine(_LoadLevel(sceneName));
		pos = initPos;
	}

	/// <summary>
	/// When the level loads, check if there is a player and a valid position and set the player's position to the provided position.
	/// </summary>
	/// <param name="scene">The Scene that just loaded.</param>
	/// <param name="mode">gamer mode</param>
	void WhenLevelLoads(Scene scene, LoadSceneMode mode)
	{
		overlay.GetComponentInChildren<Animator>().SetBool("open", true);
		GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
		if (playerObj != null && pos != null)
			playerObj.transform.position = (Vector3)pos;
	}
}
