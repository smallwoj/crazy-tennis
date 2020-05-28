// Another script for the UFO. This one contains methods called by the UFO's 
// ship's animator, which mainly exist to pass messages back to the UFO's script

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UFOShip : MonoBehaviour
{
    // Instance variables
    private UFO mainScript; // The script for the UFO (this gameobject's parent)

    // Start is called before the first frame update
    void Start()
    {
        // Yo where's the UFO
        mainScript = transform.parent.gameObject.GetComponent<UFO>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    /// <summary>
    /// Tells the main script to hit a ball
    /// </summary>
    public void Hit()
    {
        mainScript.Hit();
    }

    /// <summary>
    /// Tells the main script to remove the ball and make a new path
    /// </summary>
    public void ResetUFO()
    {
        mainScript.ResetUFO();
    }
}
