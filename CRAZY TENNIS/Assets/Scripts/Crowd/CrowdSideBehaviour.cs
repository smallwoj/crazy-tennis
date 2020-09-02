// Script for managing the people in one side of the crowd - namely, making them jump up and down when the player does something good

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class CrowdSideBehaviour : MonoBehaviour
{
    // Constan🅱s
    /// <summary>
    /// How many spectators are (generally) in the crowd
    /// </summary>
    private static readonly int CROWD_SIZE = 8;
    /// <summary>
    /// The actual value for the number of spectators in the crowd is CROWD_SIZE +/- CROWD_VARIATION
    /// </summary>
    private static readonly int CROWD_VARIATION = 1;
    /// <summary>
    /// Bounds for the spectator's position relative to the centre of the crowd
    /// </summary>
    private static readonly float X_MIN = -0.19f, X_MAX = 0.2f, Y_MIN = -3.375f, Y_MAX = 3.14f;
    /// <summary>
    /// Bound for a small, random displacement from a spectator's calculated y position
    /// </summary>
    private static readonly float Y_OFFSET = 0.2f;
    /// <summary>
    /// This is taken away from the current value of xOffset to get the next row's x position
    /// </summary>
    private static readonly float X_SUBTRAHEND = 1;

    // Actual variables I think
    /// <summary>
    /// How far away the current row of the crowd is from the starting position
    /// </summary>
    private float xOffset = 0;
    /// <summary>
    /// A trigger defined by the area of the screen
    /// </summary>
    public Collider2D screenTrigger;

    // Start is called before the first frame update
    void Start()
    {
        // All-new in this update: Keep making rows of the crowd until we go off-screen
        while (xOnScreen(xOffset))
        {
            // Decide how many spectators there will be
            int chosenCrowdSize = CROWD_SIZE + (int)(System.Math.Round(Random.value * (2 * CROWD_VARIATION) - CROWD_VARIATION));  // CROWD_SIZE +/- CROWD_VARIATION

            // Create the spectators in the crowd
            for (int i = 0 + (int)Mathf.Sign(xOffset); i < chosenCrowdSize; i++)    // That weird i definition makes it so all rows after the first include (at least) one more spectator
            {
                // Instantiate a spectator
                string prefabName;  // The name of the spectator prefab 
                switch ((int)(Random.value * 100))
                {
                    case 0:
                    case 1:
                    case 2:
                    case 3:
                    case 4: prefabName = "oh yeah woo yeah"; break;
                    default: prefabName = "Generic crowd person"; break;
                }
                GameObject spectator = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Prefabs/" + prefabName + ".prefab");
                GameObject spectatorInstance = PrefabUtility.InstantiatePrefab(spectator) as GameObject;

                // Make it a child of the crowd object (and then fix up the transform so it displays properly)
                spectatorInstance.transform.parent = gameObject.transform;

                // The spectator's y position is pretty involved, so it's calculated here before doing anything else with the transform
                float yPos = ((float)i / (chosenCrowdSize - 1)) * (Y_MAX - Y_MIN) + Y_MIN + Random.Range(0, Y_OFFSET);
                // Basically, repeating this distributes the spectators evenly along the side of the court, but with a random offset to make it look more natural

                // aaaand fix up the transform, as promised
                spectatorInstance.transform.localScale = Vector3.one;
                spectatorInstance.transform.localPosition = new Vector3(Random.Range(X_MIN, X_MAX) + xOffset, yPos, 0);
                spectatorInstance.GetComponent<Spectator>().setSortingOrder(chosenCrowdSize - i);
            }

            // Update the offset for the next row
            xOffset -= X_SUBTRAHEND;
        }
    }

    // Update is called once per frame
    void Update()
    {

    }

    /// <summary>
    /// And the crowd goes wild!!!
    /// </summary>
    /// <param name="hype"> A number from 0 to 1 measuring how excited the crowd gets (more detail in SpectatorBehaviour.Cheer) </param>
    public void Cheer(float hype)
    {
        // Where are my children
        foreach (Transform spectator in transform)
        {
            spectator.GetComponent<Spectator>().Cheer(hype);
        }
    }

    /// <summary>
    /// Tells if a a value of xOffset corresponds to a position on-screen. Used in generating a crowd of the correct size
    /// </summary>
    /// <param name="point"> A point that may or may not be on-screen </param>
    /// <returns> Whether that point is on-screen </returns>
    private bool xOnScreen(float xOffset)
    {
        Vector2 point = new Vector2(transform.localScale.x * transform.position.x + xOffset, 0);
        Vector2 closest = screenTrigger.ClosestPoint(point);
        // Fun fact from answers.unity.com: If the given point is inside the collider, ClosestPoint will return that point
        return closest == point;
    }
}
