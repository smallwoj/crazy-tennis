// Simple script that transitions scenes after a game object becomes inactive. 
// This was made with dialogue in mind, but can be used with any game object.

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NewSceneOnDestroy : MonoBehaviour
{
    /// <summary> The object that this class is waiting for the death of </summary>
    public GameObject gameObj;
    /// <summary> Name of the string to transition to </summary>
    public string sceneName;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (!gameObj.activeSelf)
        {
            SceneLoader.instance.LoadLevel(sceneName, null);
            // Without this, many copies of the scene would be loaded over and over. Whoops!
            enabled = false;
        }
    }
}
