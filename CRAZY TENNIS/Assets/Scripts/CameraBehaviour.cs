using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraBehaviour : MonoBehaviour
{
    public Camera cam;

    private int length;

    private float baseMagnitude;

    public int BASE_LENGTH = 20;

    private Vector3 INIT_POS;

    private Vector2 dir;

    private bool isImpact;

    public void Start()
    {
        INIT_POS = transform.position;
    }
    
    public void Update()
    {
        Vector3 pos = transform.position;
        if(length > 0)
        {
            if(!isImpact)
                dir = Random.insideUnitCircle;
            else
                isImpact = false;
            transform.position += (Vector3) dir * baseMagnitude * Random.Range(0.95f, 1.05f);
            length--;
        }
        else if(pos != INIT_POS)
        {
            transform.position = Vector3.Lerp(pos, INIT_POS, 0.1f);
        }
    }

    public void Impact(float intensity, Vector2 dir) 
    {
        length = 1;
        baseMagnitude = intensity;
        this.dir = dir;
        isImpact = true;
    }

    public void ShakeScreen(float intensity) 
    {
        length = (int) (BASE_LENGTH * intensity);
        baseMagnitude = (float) System.Math.Log10(intensity + 1);
    }
}
