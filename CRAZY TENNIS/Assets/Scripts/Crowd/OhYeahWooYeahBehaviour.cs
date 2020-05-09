// This guy works just like the generic spectator, except instead of cheering he grooves

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class OhYeahWooYeahBehaviour : MonoBehaviour, Spectator
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    /// <summary>
    /// he groovin!!!
    /// </summary>
    /// <param name="hype"> How much groove he is in </param>
    public void Cheer(float hype)
    {
        if (/* not groovin && */ Random.value <= hype)
        {
            print("oh yeah woo yeah baybey!!");
            // TODO: start the frame-by-frame animation :)
        }
    }

    /// <summary>
    /// Sets the spectator's order in their drawing layer
    /// </summary>
    /// <param name="sortOrder"> The intended value for their order in layer </param>
    public void setSortingOrder(int sortOrder)
    {
        GetComponent<SpriteRenderer>().sortingOrder = sortOrder;
    }
}
