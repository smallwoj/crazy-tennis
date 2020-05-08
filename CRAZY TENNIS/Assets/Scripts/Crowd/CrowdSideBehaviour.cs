// Script for managing the people in one side of the crowd - namely, making them jump up and down when the player does something good

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class CrowdSideBehaviour : MonoBehaviour
{
    // Constan🅱s
    /// <summary>
    /// How many spectators are in the crowd
    /// </summary>
    private static readonly int CROWD_SIZE = 8;
    /// <summary>
    /// Bounds for the spectator's position relative to the centre of the crowd
    /// </summary>
    private static readonly float X_MIN = -0.19f, X_MAX = 0.2f, Y_MIN = -3f, Y_MAX = 3.14f;
    /// <summary>
    /// Bound for a small, random displacement from a spectator's calculated y position
    /// </summary>
    private static readonly float Y_OFFSET = 0.2f;


    // Start is called before the first frame update
    void Start()
    {
        // Create the spectators in the crowd
        for (int i = 0; i < CROWD_SIZE; i++)
        {
            // Instantiate a spectator
            string prefabName;  // The name of the spectator prefab 
            switch ((int)(Random.value * 10))
            {
                case 0: prefabName = "oh yeah woo yeah";    break;
                default: prefabName = "Generic crowd person";   break;
            }
            GameObject spectator = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Prefabs/" + prefabName + ".prefab");
            GameObject spectatorInstance = PrefabUtility.InstantiatePrefab(spectator) as GameObject;
        
            // Make it a child of the crowd object (and then fix up the transform so it displays properly)
            spectatorInstance.transform.parent = gameObject.transform;

            // The spectator's y position is pretty involved, so it's calculated here before doing anything else with the transform
            float yPos = ((float)i / (CROWD_SIZE - 1)) * (Y_MAX - Y_MIN) + Y_MIN + Random.Range(0, Y_OFFSET);
            // Basically, repeating this distributes the spectators evenly along the side of the court, but with a random offset to make it look more natural

            // aaaand fix up the transform, as promised
            spectatorInstance.transform.localScale = Vector3.one;
            spectatorInstance.transform.localPosition = new Vector3(Random.Range(X_MIN, X_MAX), yPos, 0);
            spectatorInstance.GetComponent<SpriteRenderer>().sortingOrder = CROWD_SIZE - i;
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
}
