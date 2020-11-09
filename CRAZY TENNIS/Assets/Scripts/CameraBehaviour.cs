using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraBehaviour : MonoBehaviour
{
    /// <summary>
    /// Amount of frames the random shaking happens
    /// </summary>
    private int length;

    /// <summary>
    /// The base magnitude of the moving screen
    /// </summary>
    private float baseMagnitude;

    /// <summary>
    /// Base is 20 frames
    /// </summary>
    public int BASE_LENGTH = 20;

    /// <summary>
    /// Initial position of the camera
    /// </summary>
    private Vector3 INIT_POS;

    /// <summary>
    /// Direction of movement
    /// </summary>
    private Vector2 dir;

    /// <summary>
    /// Tells if there will be a single, directed impact or not
    /// </summary>
    private bool isImpact;

    /// <summary>
    /// Sets the initial position
    /// </summary>
    public void Start()
    {
        INIT_POS = transform.position;
    }
    
    public void Update()
    {
        // Save reference to the current position
        Vector3 pos = transform.position;

        // If we're in the middle of shaking
        if(length > 0)
        {
            // If the current operation is not an impact, set a random direction, else untick the impact bool
            if(!isImpact)
                dir = Random.insideUnitCircle;
            else
                isImpact = false;
            // Move the screen
            transform.position += (Vector3) dir * baseMagnitude * Random.Range(0.95f, 1.05f);
            length--;
        }
        else if(pos != INIT_POS)
        {
            // Move back to the initial position
            transform.position = Vector3.Lerp(pos, INIT_POS, 0.1f);
        }
    }

    /// <summary>
    /// Single, directed camera movement
    /// </summary>
    /// <param name="intensity">Float to determine the intensity, affects the magnitude</param>
    /// <param name="dir">Direction of movement</param>
    public void Impact(float intensity, Vector2 dir) 
    {
        length = 1;
        baseMagnitude = intensity;
        this.dir = dir;
        isImpact = true;
    }

    /// <summary>
    /// Shake the screen with an amount of intensity
    /// </summary>
    /// <param name="intensity">Float to determine the intensity. Affects length and magnitude</param>
    public void ShakeScreen(float intensity) 
    {
        length = (int) (BASE_LENGTH * intensity);
        baseMagnitude = (float) System.Math.Log10(intensity + 1);
    }
}
