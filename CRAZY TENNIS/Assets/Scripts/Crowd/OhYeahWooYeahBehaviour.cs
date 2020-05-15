// This guy works just like the generic spectator, except instead of jumping he grooves

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class OhYeahWooYeahBehaviour : MonoBehaviour, Spectator
{
    /// <summary> Component that allows for frame-by-frame animation! </summary>
    private Animator anim;
    /// <summary> How many times the guy can groove per cheer </summary>
    private static readonly int MAX_GROOVES = 10;

    // Start is called before the first frame update
    void Start()
    {
        // Store the animator
        anim = GetComponent<Animator>();
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
        // Randomly decide whether or not to cheer, based on the hype. A hype of 1 guarantees a cheer
        if (Random.value <= hype)
        {
            anim.SetInteger("Loop count", (int)(hype * MAX_GROOVES));   
            // "Loop count" is an animator variable that dictates how many times the guy should groove before returning to an idle state
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

    /// <summary>
    /// Decreases the animator's "Loop count" variable by 1
    /// </summary>
    void DecrementLoopCount()
    {
        anim.SetInteger("Loop count", anim.GetInteger("Loop count") - 1);
        // btw "Loop count" is an animator variable that dictates how many times the guy should groove before returning to an idle state
    }
}
