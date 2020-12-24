// In the game over screen, this script restarts the game when the player presses space

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TryAgain : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetButtonDown("Submit"))
        {
            // GetComponent<Animator>().speed = 10;
            gameObject.SetActive(false);
            SceneLoader.instance.LoadLevel("main playing field", null);
        }
        
    }
}
