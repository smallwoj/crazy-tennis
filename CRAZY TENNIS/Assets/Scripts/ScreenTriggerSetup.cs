// Creates a trigger that takes up the whole screen, so we can detect balls exiting the screen

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScreenTriggerSetup : MonoBehaviour
{
    public Camera mainCamera;       // The scene's camera 
    private BoxCollider2D trigger;  // The trigger that this script creates
    private Rect cameraRect;        // The camera's visible area

    // Start is called before the first frame update
    void Start()
    {
        trigger = gameObject.GetComponent<BoxCollider2D>();
        Vector2 bottomLeft = mainCamera.ScreenToWorldPoint(Vector3.zero);
        Vector2 topRight = mainCamera.ScreenToWorldPoint(new Vector3(mainCamera.pixelWidth, mainCamera.pixelHeight));
        trigger.size = topRight - bottomLeft;

        cameraRect = new Rect(
            bottomLeft.x,
            bottomLeft.y,
            topRight.x - bottomLeft.x,
            topRight.y - bottomLeft.y);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    virtual protected void OnTriggerExit2D(Collider2D other)
    {
        if (other.gameObject.tag == "Ball")
        {
            // Idea for later: To avoid balls going under the UI, reuse this code in UIBoundSetup.OnCollisionorwhatever
            print(other.gameObject.tag + " is offscreen");
        }
    }
}
