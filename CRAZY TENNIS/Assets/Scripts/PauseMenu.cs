using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PauseMenu : MonoBehaviour
{
    private Animation textAnim;

    // Start is called before the first frame update
    void Start()
    {
        textAnim = GetComponentInChildren<Animation>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    /// <summary>
    /// Called when the game is paused
    /// </summary>
    private void OnEnable() {
        print("win");
    }
}
