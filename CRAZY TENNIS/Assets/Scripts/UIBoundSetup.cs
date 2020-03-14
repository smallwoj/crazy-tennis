// Adjusts the collision bound for the UI (based on screen bounds, which are in turn based on the aspect ratio)

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIBoundSetup : MonoBehaviour
{
    public Camera mainCamera;   // Used to get the edges of the screen
    public float sillyOffset = 0.115f;   // Used to adjust the bound so it lines up with the UI juuuust right

    // Start is called before the first frame update
    void Start()
    {
        // Get the important bound(s)
        BoxCollider2D bound = gameObject.GetComponent<BoxCollider2D>();

        bound.offset = new Vector2(mainCamera.ViewportToWorldPoint(new Vector3(1 - sillyOffset, 0, 0)).x, 0);
    }

    // Update is called once per frame
    void Update()
    {

    }
}
