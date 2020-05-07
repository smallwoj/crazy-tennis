// Script for instantiating and animating a single spectator in the crowd

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpectatorBehaviour : MonoBehaviour
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
    /// Makes this spectator jump up and down wildly (or mildly, or even not at all, depending on the hype value)
    /// </summary>
    /// <param name="hype"> A number from 0 to 1 measuring how excited the crowd gets. 
    /// More specifically, it's the probability that any given spectator will cheer, 
    /// and it plays a (as of yet undetermined) role in deciding the intensity and length of a spectator's cheer</param>
    public void Cheer(float hype)
    {
        print(hype);
    }
}
