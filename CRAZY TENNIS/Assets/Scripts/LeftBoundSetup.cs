// Pins a collider to the left edge of the screen. 
// Useful for narrow aspect ratios, where the left court bounds are outside the screen

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LeftBoundSetup : MonoBehaviour
{
    public Camera mainCamera;   // Used to get the edges of the screen

    // Start is called before the first frame update
    void Start()
    {
        // Move the left bound to the left edge of the screen
        BoxCollider2D bound = gameObject.GetComponent<BoxCollider2D>();
        bound.offset = new Vector2(mainCamera.ViewportToWorldPoint(new Vector3(0, 0, 0)).x, 0);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
